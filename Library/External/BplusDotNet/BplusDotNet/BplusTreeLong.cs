using System;
using System.Collections;

// delete next

namespace BplusDotNet
{
	/// <summary>
	/// Bplustree mapping fixed length strings (byte sequences) to longs (seek positions in file indexed).
	/// "Next leaf pointer" is not used since it increases the chance of file corruption on failure.
	/// All modifications are "shadowed" until a flush of all modifications succeeds.  Modifications are
	/// "hardened" when the header record is rewritten with a new root.  This design trades a few "unneeded"
	/// buffer writes for lower likelihood of file corruption.
	/// </summary>
	public class BplusTreeLong: ITreeIndex
	{
		public System.IO.Stream fromfile;
		// should be read only
		public bool DontUseCulture = false;
		public System.Globalization.CultureInfo cultureContext;
		System.Globalization.CompareInfo cmp = null;
		// should be read only
		public BufferFile buffers;
		// should be read only
		public int buffersize;
		// should be read only
		public int KeyLength;
		public long seekStart = 0;
		public static byte[] HEADERPREFIX = { 98, 112, 78, 98, 112 };
		// header consists of 
		// prefix | version | node size | key size | culture id | buffer number of root | buffer number of free list head
		int headersize = HEADERPREFIX.Length + 1 + BufferFile.INTSTORAGE*3 + BufferFile.LONGSTORAGE*2;
		public const byte VERSION = 0;
		// size of allocated key space in each node (should be a read only property)
		public int NodeSize;
		BplusNode root = null;
		long rootSeek; 
		long freeHeadSeek;
		public Hashtable FreeBuffersOnCommit = new Hashtable();
		public Hashtable FreeBuffersOnAbort = new Hashtable();
		Hashtable IdToTerminalNode = new Hashtable();
		Hashtable TerminalNodeToId = new Hashtable();
		int TerminalNodeCount = 0;
		int LowerTerminalNodeCount = 0;
		int FifoLimit = 100;
		public static int NULLBUFFERNUMBER = -1;
		public static byte NONLEAF = 0, LEAF = 1, FREE = 2;

		public BplusTreeLong(System.IO.Stream fromfile, int NodeSize, int KeyLength, long StartSeek, int CultureId)
		{
			this.cultureContext = new System.Globalization.CultureInfo(CultureId);
			this.cmp = this.cultureContext.CompareInfo;
			this.fromfile = fromfile;
			this.NodeSize = NodeSize;
			this.seekStart = StartSeek;
			// add in key prefix overhead
			this.KeyLength = KeyLength + BufferFile.SHORTSTORAGE;
			this.rootSeek = NULLBUFFERNUMBER;
			this.root = null;
			this.freeHeadSeek = NULLBUFFERNUMBER;
			this.SanityCheck();
		}
		
		public int MaxKeyLength() 
		{
			return this.KeyLength-BufferFile.SHORTSTORAGE;
		}
		public void Shutdown()
		{
			this.fromfile.Flush();
			this.fromfile.Close();
		}
		public int Compare(string left, string right) 
		{
			//System.Globalization.CompareInfo cmp = this.cultureContext.CompareInfo;
			if (this.cultureContext==null || this.DontUseCulture) 
			{
				// no culture context: use miscellaneous total ordering on unicode strings
				int i = 0;
				while (i<left.Length && i<right.Length) 
				{
					int leftOrd = Convert.ToInt32(left[i]);
					int rightOrd = Convert.ToInt32(right[i]);
					if (leftOrd<rightOrd) 
					{
						return -1;
					}
					if (leftOrd>rightOrd)
					{
						return 1;
					}
					i++;
				}
				if (left.Length<right.Length) 
				{
					return -1;
				}
				if (left.Length>right.Length) 
				{
					return 1;
				}
				return 0;
			}
			if (this.cmp==null) 
			{
				this.cmp = this.cultureContext.CompareInfo;
			}
			return this.cmp.Compare(left, right);
		}
		public void SanityCheck(bool strong) 
		{
			this.SanityCheck();
			if (strong) 
			{
				this.Recover(false);
				// look at all deferred deallocations -- they should not be free
				byte[] buffer = new byte[1];
				foreach (DictionaryEntry thing in this.FreeBuffersOnAbort) 
				{
					long buffernumber = (long) thing.Key;
					this.buffers.getBuffer(buffernumber, buffer, 0, 1);
					if (buffer[0]==FREE) 
					{
						throw new BplusTreeException("free on abort buffer already marked free "+buffernumber);
					}
				}
				foreach (DictionaryEntry thing in this.FreeBuffersOnCommit) 
				{
					long buffernumber = (long) thing.Key;
					this.buffers.getBuffer(buffernumber, buffer, 0, 1);
					if (buffer[0]==FREE) 
					{
						throw new BplusTreeException("free on commit buffer already marked free "+buffernumber);
					}
				}
			}
		}
		public void Recover(bool CorrectErrors) 
		{
			Hashtable visited = new Hashtable();
			if (this.root!=null) 
			{
				// find all reachable nodes
				this.root.SanityCheck(visited);
			}
			// traverse the free list
			long freebuffernumber = this.freeHeadSeek;
			while (freebuffernumber!=NULLBUFFERNUMBER) 
			{
				if (visited.ContainsKey(freebuffernumber) ) 
				{
					throw new BplusTreeException("free buffer visited twice "+freebuffernumber);
				}
				visited[freebuffernumber] = FREE;
				freebuffernumber = this.parseFreeBuffer(freebuffernumber);
			}
			// find out what is missing
			Hashtable Missing = new Hashtable();
			long maxbuffer = this.buffers.nextBufferNumber();
			for (long i=0; i<maxbuffer; i++) 
			{
				if (!visited.ContainsKey(i)) 
				{
					Missing[i] = i;
				}
			}
			// remove from missing any free-on-commit blocks
			foreach (DictionaryEntry thing in this.FreeBuffersOnCommit) 
			{
				long tobefreed = (long) thing.Key;
				Missing.Remove(tobefreed);
			}
			// add the missing values to the free list
			if (CorrectErrors) 
			{
				if (Missing.Count>0) 
				{
					System.Diagnostics.Debug.WriteLine("correcting "+Missing.Count+" unreachable buffers");
				}
				ArrayList missingL = new ArrayList();
				foreach (DictionaryEntry d in Missing) 
				{
					missingL.Add(d.Key);
				}
				missingL.Sort();
				missingL.Reverse();
				foreach (object thing in missingL) 
				{
					long buffernumber = (long) thing;
					this.deallocateBuffer(buffernumber);
				}
				//this.ResetBookkeeping();
			} 
			else if (Missing.Count>0)
			{
				string buffers = "";
				foreach (DictionaryEntry thing in Missing) 
				{
					buffers += " "+thing.Key;
				}
				throw new BplusTreeException("found "+Missing.Count+" unreachable buffers."+buffers);
			}
		}
		public void SerializationCheck()  
		{
			if (this.root==null) 
			{
				throw new BplusTreeException("serialization check requires initialized root, sorry");
			}
			this.root.SerializationCheck();
		}
		void SanityCheck() 
		{
			if (this.NodeSize<2) 
			{
				throw new BplusTreeException("node size must be larger than 2");
			}
			if (this.KeyLength<5) 
			{
				throw new BplusTreeException("Key length must be larger than 5");
			}
			if (this.seekStart<0) 
			{
				throw new BplusTreeException("start seek may not be negative");
			}
			// compute the buffer size
			// indicator | seek position | [ key storage | seek position ]*
			int keystorage = this.KeyLength + BufferFile.SHORTSTORAGE;
			this.buffersize = 1 + BufferFile.LONGSTORAGE + (keystorage + BufferFile.LONGSTORAGE)*this.NodeSize;
		}
		public string toHtml() 
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("<h1>BplusTree</h1>\r\n");
			sb.Append("\r\n<br> nodesize="+this.NodeSize);
			sb.Append("\r\n<br> seekstart="+this.seekStart);
			sb.Append("\r\n<br> rootseek="+this.rootSeek);
			sb.Append("\r\n<br> free on commit "+this.FreeBuffersOnCommit.Count+" ::");
			foreach (DictionaryEntry thing in this.FreeBuffersOnCommit) 
			{
				sb.Append(" "+thing.Key);
			}
			sb.Append("\r\n<br> Freebuffers : ");
			Hashtable freevisit = new Hashtable();
			long free = this.freeHeadSeek;
			string allfree = "freehead="+free+" :: ";
			while (free!=NULLBUFFERNUMBER) 
			{
				allfree = allfree+" "+free;
				if (freevisit.ContainsKey(free)) 
				{
					throw new BplusTreeException("cycle in freelist "+free);
				}
				freevisit[free] = free;
				free = this.parseFreeBuffer(free);
			}
			if (allfree.Length==0) 
			{
				sb.Append("empty list");
			} 
			else 
			{
				sb.Append(allfree);
			}
			foreach (DictionaryEntry thing in this.FreeBuffersOnCommit) 
			{
				sb.Append(" "+thing.Key);
			}
			sb.Append("\r\n<br> free on abort "+this.FreeBuffersOnAbort.Count+" ::");
			foreach (DictionaryEntry thing in this.FreeBuffersOnAbort) 
			{
				sb.Append(" "+thing.Key);
			}
			sb.Append("\r\n<br>\r\n");

			//... add more
			if (this.root==null) 
			{
				sb.Append("<br><b>NULL ROOT</b>\r\n");
			} 
			else 
			{
				this.root.AsHtml(sb);
			}
			return sb.ToString();
		}
		public BplusTreeLong(System.IO.Stream fromfile, int KeyLength, int NodeSize, int CultureId):
			this(fromfile, NodeSize, KeyLength, (long)0, CultureId) 
		{
			// just start seek at 0
		}
		public static BplusTreeLong SetupFromExistingStream(System.IO.Stream fromfile) 
		{
			return SetupFromExistingStream(fromfile, (long)0);
		}
		public static BplusTreeLong SetupFromExistingStream(System.IO.Stream fromfile, long StartSeek) 
		{
			int dummyId = System.Globalization.CultureInfo.InvariantCulture.LCID;
			BplusTreeLong result = new BplusTreeLong(fromfile, 7, 100, StartSeek, dummyId); // dummy values for nodesize, keysize
			result.readHeader();
			result.buffers = BufferFile.SetupFromExistingStream(fromfile, StartSeek+result.headersize);
			if (result.buffers.buffersize != result.buffersize) 
			{
				throw new BplusTreeException("inner and outer buffer sizes should match");
			}
			if (result.rootSeek!=NULLBUFFERNUMBER) 
			{
				result.root = new BplusNode(result, null, -1, true);
				result.root.LoadFromBuffer(result.rootSeek);
			}
			return result;
		}
		public static BplusTreeLong InitializeInStream(System.IO.Stream fromfile, int KeyLength, int NodeSize) 
		{
			int dummyId = System.Globalization.CultureInfo.InvariantCulture.LCID;
			return InitializeInStream(fromfile, KeyLength, NodeSize, dummyId);
		}
		public static BplusTreeLong InitializeInStream(System.IO.Stream fromfile, int KeyLength, int NodeSize, int CultureId) 
		{
			return InitializeInStream(fromfile, KeyLength, NodeSize, CultureId, (long)0);
		}
		public static BplusTreeLong InitializeInStream(System.IO.Stream fromfile, int KeyLength, int NodeSize, int CultureId, long StartSeek) 
		{
			if (fromfile.Length>StartSeek) 
			{
				throw new BplusTreeException("can't initialize bplus tree inside written area of stream");
			}
			BplusTreeLong result = new BplusTreeLong(fromfile, NodeSize, KeyLength, StartSeek, CultureId);
			result.setHeader();
			result.buffers = BufferFile.InitializeBufferFileInStream(fromfile, result.buffersize, StartSeek+result.headersize);
			return result;
		}
		public void SetFootPrintLimit(int limit) 
		{
			if (limit<5) 
			{
				throw new BplusTreeException("foot print limit less than 5 is too small");
			}
			this.FifoLimit = limit;
		}
		public void RemoveKey(string key) 
		{
			if (this.root==null) 
			{
				throw new BplusTreeKeyMissing("tree is empty: cannot delete");
			}
			bool MergeMe;
			BplusNode theroot = this.root;
			theroot.Delete(key, out MergeMe);
			// if the root is not a leaf and contains only one child (no key), reroot
			if (MergeMe && !this.root.isLeaf && this.root.SizeInUse()==0) 
			{
				this.root = this.root.FirstChild();
				this.rootSeek = this.root.makeRoot();
				theroot.Free();
			}
		}
		public long this[string key] 
		{
			get 
			{
				long valueFound;
				bool test = this.ContainsKey(key, out valueFound);
				if (!test) 
				{
					throw new BplusTreeKeyMissing("no such key found: "+key);
				}
				return valueFound;
			}
			set 
			{
				if (!BplusNode.KeyOK(key, this)) 
				{
					throw new BplusTreeBadKeyValue("null or too large key cannot be inserted into tree: "+key);
				}
				bool rootinit = false;
				if (this.root==null) 
				{
					// allocate root
					this.root = new BplusNode(this, null, -1, true);
					rootinit = true;
					//this.rootSeek = root.DumpToFreshBuffer();
				}
				// insert into root...
				string splitString;
				BplusNode splitNode;
				root.Insert(key, value, out splitString, out splitNode);
				if (splitNode!=null) 
				{
					// split of root: make a new root.
					rootinit = true;
					BplusNode oldRoot = this.root;
					this.root = BplusNode.BinaryRoot(oldRoot, splitString, splitNode, this);
				}
				if (rootinit) 
				{
					this.rootSeek = root.DumpToFreshBuffer();
				}
				// check size in memory
				this.ShrinkFootprint();
			}
		}
		public string FirstKey() 
		{
			string result = null;
			if (this.root!=null) 
			{
				// empty string is smallest possible tree
				if (this.ContainsKey("")) 
				{
					result = "";
				} 
				else 
				{
					return this.root.FindNextKey("");
				}
				this.ShrinkFootprint();
			}
			return result;
		}
		public string NextKey(string AfterThisKey) 
		{
			if (AfterThisKey==null) 
			{
				throw new BplusTreeBadKeyValue("cannot search for null string");
			}
			string result = this.root.FindNextKey(AfterThisKey);
			this.ShrinkFootprint();
			return result;
		}
		public bool ContainsKey(string key) 
		{
			long valueFound;
			return this.ContainsKey(key, out valueFound);
		} 
		public bool ContainsKey(string key, out long valueFound) 
		{
			if (key==null)
			{
				throw new BplusTreeBadKeyValue("cannot search for null string");
			}
			bool result = false;
			valueFound = (long) 0;
			if (this.root!=null) 
			{
				result = this.root.FindMatch(key, out valueFound);
			}
			this.ShrinkFootprint();
			return result;
		}
		public long Get(string key, long defaultValue) 
		{
			long result = defaultValue;
			long valueFound;
			if (this.ContainsKey(key, out valueFound))
			{
				result = valueFound;
			}
			return result;
		}
		public void Set(string key, object map) 
		{
			if (!(map is long)) 
			{
				throw new BplusTreeBadKeyValue("only longs may be used as values in a BplusTreeLong: "+map);
			}
			this[key] = (long) map;
		}
		public object Get(string key, object defaultValue) 
		{
			long valueFound;
			if (this.ContainsKey(key, out valueFound)) 
			{
				return (object) valueFound;
			}
			return defaultValue;
		}
		/// <summary>
		/// Store off any changed buffers, clear the fifo, free invalid buffers
		/// </summary>
		public void Commit() 
		{
			// store all modifications
			if (this.root!=null) 
			{
				this.rootSeek = this.root.Invalidate(false);
			}
			this.fromfile.Flush();
			// commit the new root
			this.setHeader();
			this.fromfile.Flush();
			// at this point the changes are committed, but some space is unreachable.
			// now free all unfreed buffers no longer in use
			ArrayList toFree = new ArrayList();
			foreach (DictionaryEntry d in this.FreeBuffersOnCommit) 
			{
				toFree.Add(d.Key);
			}
			toFree.Sort();
			toFree.Reverse();
			foreach (object thing in toFree) 
			{
				long buffernumber = (long) thing;
				this.deallocateBuffer(buffernumber);
			}
			// store the free list head
			this.setHeader();
			this.fromfile.Flush();
			this.ResetBookkeeping();
		}
		/// <summary>
		/// Forget all changes since last commit
		/// </summary>
		public void Abort() 
		{
			// deallocate allocated blocks
			ArrayList toFree = new ArrayList();
			foreach (DictionaryEntry d in this.FreeBuffersOnAbort) 
			{
				toFree.Add(d.Key);
			}
			toFree.Sort();
			toFree.Reverse();
			foreach (object thing in toFree) 
			{
				long buffernumber = (long) thing;
				this.deallocateBuffer(buffernumber);
			}
			long freehead = this.freeHeadSeek;
			// reread the header (except for freelist head)
			this.readHeader();
			// restore the root
			if (this.rootSeek==NULLBUFFERNUMBER) 
			{
				this.root = null; // nothing was committed
			} 
			else 
			{
				this.root.LoadFromBuffer(this.rootSeek);
			}
			this.ResetBookkeeping();
			this.freeHeadSeek = freehead;
			this.setHeader(); // store new freelist head
			this.fromfile.Flush();
		}
		void ResetBookkeeping() 
		{
			this.FreeBuffersOnCommit.Clear();
			this.FreeBuffersOnAbort.Clear();
			this.IdToTerminalNode.Clear();
			this.TerminalNodeToId.Clear();
		}
		public long allocateBuffer() 
		{
			long allocated = -1;
			if (this.freeHeadSeek==NULLBUFFERNUMBER) 
			{
				// should be written immediately after allocation
				allocated = this.buffers.nextBufferNumber();
				//System.Diagnostics.Debug.WriteLine("<br> allocating fresh buffer "+allocated);
				return allocated;
			}
			// get the free head data
			allocated = this.freeHeadSeek;
			this.freeHeadSeek = this.parseFreeBuffer(allocated);
			//System.Diagnostics.Debug.WriteLine("<br> recycling free buffer "+allocated);
			return allocated;
		}
		long parseFreeBuffer(long buffernumber) 
		{
			int freesize = 1+BufferFile.LONGSTORAGE;
			byte[] buffer = new byte[freesize];
			this.buffers.getBuffer(buffernumber, buffer, 0, freesize);
			if (buffer[0]!=FREE) 
			{
				throw new BplusTreeException("free buffer not marked free");
			}
			long result = BufferFile.RetrieveLong(buffer, 1);
			return result;
		}
		public void deallocateBuffer(long buffernumber) 
		{
			//System.Diagnostics.Debug.WriteLine("<br> deallocating "+buffernumber);
			int freesize = 1+BufferFile.LONGSTORAGE;
			byte[] buffer = new byte[freesize];
			// it better not already be marked free
			this.buffers.getBuffer(buffernumber, buffer, 0, 1);
			if (buffer[0]==FREE) 
			{
				throw new BplusTreeException("attempt to re-free free buffer not allowed");
			}
			buffer[0] = FREE;
			BufferFile.Store(this.freeHeadSeek, buffer, 1);
			this.buffers.setBuffer(buffernumber, buffer, 0, freesize);
			this.freeHeadSeek = buffernumber;
		}
		void setHeader() 
		{
			byte[] header = this.makeHeader();
			this.fromfile.Seek(this.seekStart, System.IO.SeekOrigin.Begin);
			this.fromfile.Write(header, 0, header.Length);
		}
		public void RecordTerminalNode(BplusNode terminalNode) 
		{
			if (terminalNode==this.root) 
			{
				return; // never record the root node
			}
			if (this.TerminalNodeToId.ContainsKey(terminalNode) )
			{
				return; // don't record it again
			}
			int id = this.TerminalNodeCount;
			this.TerminalNodeCount++;
			this.TerminalNodeToId[terminalNode] = id;
			this.IdToTerminalNode[id] = terminalNode;
		}
		public void ForgetTerminalNode(BplusNode nonterminalNode) 
		{
			if (!this.TerminalNodeToId.ContainsKey(nonterminalNode)) 
			{
				// silently ignore (?)
				return;
			}
			int id = (int) this.TerminalNodeToId[nonterminalNode];
			if (id == this.LowerTerminalNodeCount) 
			{
				this.LowerTerminalNodeCount++;
			}
			this.IdToTerminalNode.Remove(id);
			this.TerminalNodeToId.Remove(nonterminalNode);
		}
		public void ShrinkFootprint() 
		{
			this.InvalidateTerminalNodes(this.FifoLimit);
		}
		public void InvalidateTerminalNodes(int toLimit) 
		{
			while (this.TerminalNodeToId.Count>toLimit) 
			{
				// choose oldest nonterminal and deallocate it
				while (!this.IdToTerminalNode.ContainsKey(this.LowerTerminalNodeCount)) 
				{
					this.LowerTerminalNodeCount++; // since most nodes are terminal this should usually be a short walk
					//System.Diagnostics.Debug.WriteLine("<BR>WALKING "+this.LowerTerminalNodeCount);
					//System.Console.WriteLine("<BR>WALKING "+this.LowerTerminalNodeCount);
					if (this.LowerTerminalNodeCount>this.TerminalNodeCount) 
					{
						throw new BplusTreeException("internal error counting nodes, lower limit went too large");
					}
				}
				//System.Console.WriteLine("<br> done walking");
				int id = this.LowerTerminalNodeCount;
				BplusNode victim = (BplusNode) this.IdToTerminalNode[id];
				//System.Diagnostics.Debug.WriteLine("\r\n<br>selecting "+victim.myBufferNumber+" for deallocation from fifo");
				this.IdToTerminalNode.Remove(id);
				this.TerminalNodeToId.Remove(victim);
				if (victim.myBufferNumber!=NULLBUFFERNUMBER) 
				{
					victim.Invalidate(true);
				}
			}
		}
		void readHeader() 
		{
			// prefix | version | node size | key size | culture id | buffer number of root | buffer number of free list head
			byte[] header = new byte[this.headersize];
			this.fromfile.Seek(this.seekStart, System.IO.SeekOrigin.Begin);
			this.fromfile.Read(header, 0, this.headersize);
			int index = 0;
			// check prefix
			foreach (byte b in HEADERPREFIX) 
			{
				if (header[index]!=b) 
				{
					throw new BufferFileException("invalid header prefix");
				}
				index++;
			}
			// skip version (for now)
			index++;
			this.NodeSize = BufferFile.Retrieve(header, index);
			index+= BufferFile.INTSTORAGE;
			this.KeyLength = BufferFile.Retrieve(header, index);
			index+= BufferFile.INTSTORAGE;
			int CultureId = BufferFile.Retrieve(header, index);
			this.cultureContext = new System.Globalization.CultureInfo(CultureId);
			index+= BufferFile.INTSTORAGE;
			this.rootSeek = BufferFile.RetrieveLong(header, index);
			index+= BufferFile.LONGSTORAGE;
			this.freeHeadSeek = BufferFile.RetrieveLong(header, index);
			this.SanityCheck();
			//this.header = header;
		}
		public byte[] makeHeader() 
		{
			// prefix | version | node size | key size | culture id | buffer number of root | buffer number of free list head
			byte[] result = new byte[this.headersize];
			HEADERPREFIX.CopyTo(result, 0);
			result[HEADERPREFIX.Length] = VERSION;
			int index = HEADERPREFIX.Length+1;
			BufferFile.Store(this.NodeSize, result, index);
			index+= BufferFile.INTSTORAGE;
			BufferFile.Store(this.KeyLength, result, index);
			index+= BufferFile.INTSTORAGE;
			if (this.cultureContext!=null) 
			{
				BufferFile.Store(this.cultureContext.LCID, result, index);
			} 
			else 
			{
				BufferFile.Store(System.Globalization.CultureInfo.InvariantCulture.LCID, result, index);
			}
			index+= BufferFile.INTSTORAGE;
			BufferFile.Store(this.rootSeek, result, index);
			index+= BufferFile.LONGSTORAGE;
			BufferFile.Store(this.freeHeadSeek, result, index);
			return result;
		}
	}
	public class BplusNode 
	{
		public bool isLeaf = true;
		// the maximum number of children to each node.
		int Size;
		// false if the node is no longer active and should not be used.
		//bool isValid = true;
		// true if the materialized node needs to be persisted.
		bool Dirty = true;
		// if non-root reference to the parent node containing this node
		BplusNode parent = null;
		// tree containing this node
		BplusTreeLong owner = null;
		// buffer number of this node
		public long myBufferNumber = BplusTreeLong.NULLBUFFERNUMBER;
		// number of children used by this node
		//int NumberOfValidKids = 0;
		long[] ChildBufferNumbers;
		string[] ChildKeys;
		BplusNode[] MaterializedChildNodes;
		int indexInParent = -1;
		/// <summary>
		/// Create a new BplusNode and install in parent if parent is not null.
		/// </summary>
		/// <param name="owner">tree containing the node</param>
		/// <param name="parent">parent node (if provided)</param>
		/// <param name="indexInParent">location in parent if provided</param>
		public BplusNode(BplusTreeLong owner, BplusNode parent, int indexInParent, bool isLeaf) 
		{
			this.isLeaf = isLeaf;
			this.owner = owner;
			this.parent = parent;
			this.Size = owner.NodeSize;
			//this.isValid = true;
			this.Dirty = true;
			//			this.ChildBufferNumbers = new long[this.Size+1];
			//			this.ChildKeys = new string[this.Size];
			//			this.MaterializedChildNodes = new BplusNode[this.Size+1];
			this.Clear();
			if (parent!=null && indexInParent>=0) 
			{
				if (indexInParent>this.Size) 
				{
					throw new BplusTreeException("parent index too large");
				}
				// key info, etc, set elsewhere
				this.parent.MaterializedChildNodes[indexInParent] = this;
				this.myBufferNumber = this.parent.ChildBufferNumbers[indexInParent];
				this.indexInParent = indexInParent;
			}
		}
		public BplusNode FirstChild() 
		{
			BplusNode result = this.MaterializeNodeAtIndex(0);
			if (result==null) 
			{
				throw new BplusTreeException("no first child");
			}
			return result;
		}
		public long makeRoot() 
		{
			this.parent = null;
			this.indexInParent = -1;
			if (this.myBufferNumber==BplusTreeLong.NULLBUFFERNUMBER) 
			{
				throw new BplusTreeException("no root seek allocated to new root");
			}
			return this.myBufferNumber;
		}
		public void Free() 
		{
			if (this.myBufferNumber!=BplusTreeLong.NULLBUFFERNUMBER) 
			{
				if (this.owner.FreeBuffersOnAbort.ContainsKey(this.myBufferNumber)) 
				{
					// free it now
					this.owner.FreeBuffersOnAbort.Remove(this.myBufferNumber);
					this.owner.deallocateBuffer(this.myBufferNumber);
				} 
				else 
				{
					// free on commit
					//this.owner.FreeBuffersOnCommit.Add(this.myBufferNumber);
					this.owner.FreeBuffersOnCommit[this.myBufferNumber] = this.myBufferNumber;
				}
			}
			this.myBufferNumber = BplusTreeLong.NULLBUFFERNUMBER; // don't do it twice...
		}
		public void SerializationCheck() 
		{ 
			BplusNode A = new BplusNode(this.owner, null, -1, false);
			for (int i=0; i<this.Size; i++) 
			{
				long j = i*((long)0xf0f0f0f0f0f0f01);
				A.ChildBufferNumbers[i] = j;
				A.ChildKeys[i] = "k"+i;
			}
			A.ChildBufferNumbers[this.Size] = 7;
			A.TestRebuffer();
			A.isLeaf = true;
			for (int i=0; i<this.Size; i++) 
			{
				long j = -i*((long)0x3e3e3e3e3e3e666);
				A.ChildBufferNumbers[i] = j;
				A.ChildKeys[i] = "key"+i;
			}
			A.ChildBufferNumbers[this.Size] = -9097;
			A.TestRebuffer();
		}
		void TestRebuffer() 
		{
			bool IL = this.isLeaf;
			long[] Ns = this.ChildBufferNumbers;
			string[] Ks = this.ChildKeys;
			byte[] buffer = new byte[this.owner.buffersize];
			this.Dump(buffer);
			this.Clear();
			this.Load(buffer);
			for (int i=0; i<this.Size; i++) 
			{
				if (this.ChildBufferNumbers[i]!=Ns[i]) 
				{
					throw new BplusTreeException("didn't get back buffernumber "+i+" got "+this.ChildBufferNumbers[i]+" not "+Ns[i]);
				}
				if (!this.ChildKeys[i].Equals(Ks[i])) 
				{
					throw new BplusTreeException("didn't get back key "+i+" got "+this.ChildKeys[i]+" not "+Ks[i]);
				}
			}
			if (this.ChildBufferNumbers[this.Size]!=Ns[this.Size]) 
			{
				throw new BplusTreeException("didn't get back buffernumber "+this.Size+" got "+this.ChildBufferNumbers[this.Size]+" not "+Ns[this.Size]);
			}
			if (this.isLeaf!=IL) 
			{
				throw new BplusTreeException("isLeaf should be "+IL+" got "+this.isLeaf);
			}
		}
		public string SanityCheck(Hashtable visited) 
		{
			string result = null;
			if (visited==null) 
			{
				visited = new Hashtable();
			}
			if (visited.ContainsKey(this)) 
			{
				throw new BplusTreeException("node visited twice "+this.myBufferNumber);
			}
			visited[this] = this.myBufferNumber;
			if (this.myBufferNumber!=BplusTreeLong.NULLBUFFERNUMBER) 
			{
				if (visited.ContainsKey(this.myBufferNumber))
				{
					throw new BplusTreeException("buffer number seen twice "+this.myBufferNumber);
				}
				visited[this.myBufferNumber] = this;
			}
			if (this.parent!=null) 
			{
				if (this.parent.isLeaf) 
				{
					throw new BplusTreeException("parent is leaf");
				}
				this.parent.MaterializeNodeAtIndex(this.indexInParent);
				if (this.parent.MaterializedChildNodes[this.indexInParent]!=this) 
				{
					throw new BplusTreeException("incorrect index in parent");
				}
				// since not at root there should be at least size/2 keys
				int limit = this.Size/2;
				if (this.isLeaf) 
				{
					limit--;
				}
				for (int i=0; i<limit; i++) 
				{
					if (this.ChildKeys[i]==null) 
					{
						throw new BplusTreeException("null child in first half");
					}
				}
			}
			result = this.ChildKeys[0]; // for leaf
			if (!this.isLeaf) 
			{
				this.MaterializeNodeAtIndex(0);
				result = this.MaterializedChildNodes[0].SanityCheck(visited);
				for (int i=0; i<this.Size; i++) 
				{
					if (this.ChildKeys[i]==null) 
					{
						break;
					}
					this.MaterializeNodeAtIndex(i+1);
					string least = this.MaterializedChildNodes[i+1].SanityCheck(visited);
					if (least==null) 
					{
						throw new BplusTreeException("null least in child doesn't match node entry "+this.ChildKeys[i]);
					}
					if (!least.Equals(this.ChildKeys[i])) 
					{
						throw new BplusTreeException("least in child "+least+" doesn't match node entry "+this.ChildKeys[i]);
					}
				}
			}
			// look for duplicate keys
			string lastkey = this.ChildKeys[0];
			for (int i=1; i<this.Size; i++) 
			{
				if (this.ChildKeys[i]==null) 
				{
					break;
				}
				if (lastkey.Equals(this.ChildKeys[i]) ) 
				{
					throw new BplusTreeException("duplicate key in node "+lastkey);
				}
				lastkey = this.ChildKeys[i];
			}
			return result;
		}
		void Destroy() 
		{
			// make sure the structure is useless, it should no longer be used.
			this.owner = null;
			this.parent = null;
			this.Size = -100;
			this.ChildBufferNumbers = null;
			this.ChildKeys = null;
			this.MaterializedChildNodes = null;
			this.myBufferNumber = BplusTreeLong.NULLBUFFERNUMBER;
			this.indexInParent = -100;
			this.Dirty = false;
		}
		public int SizeInUse() 
		{
			int result = 0;
			for (int i=0; i<this.Size; i++) 
			{
				if (this.ChildKeys[i]==null) 
				{
					break;
				}
				result++;
			}
			return result;
		}
		public static BplusNode BinaryRoot(BplusNode LeftNode, string key, BplusNode RightNode, BplusTreeLong owner) 
		{
			BplusNode newRoot = new BplusNode(owner, null, -1, false);
			//newRoot.Clear(); // redundant
			newRoot.ChildKeys[0] = key;
			LeftNode.Reparent(newRoot, 0);
			RightNode.Reparent(newRoot, 1);
			// new root is stored elsewhere
			return newRoot;
		}
		void Reparent(BplusNode newParent, int ParentIndex) 
		{
			// keys and existing parent structure must be updated elsewhere.
			this.parent = newParent;
			this.indexInParent = ParentIndex;
			newParent.ChildBufferNumbers[ParentIndex] = this.myBufferNumber;
			newParent.MaterializedChildNodes[ParentIndex] = this;
			// parent is no longer terminal
			this.owner.ForgetTerminalNode(parent);
		}
		void Clear() 
		{
			this.ChildBufferNumbers = new long[this.Size+1];
			this.ChildKeys = new string[this.Size];
			this.MaterializedChildNodes = new BplusNode[this.Size+1];
			for (int i=0; i<this.Size; i++) 
			{
				this.ChildBufferNumbers[i] = BplusTreeLong.NULLBUFFERNUMBER;
				this.MaterializedChildNodes[i] = null;
				this.ChildKeys[i] = null;
			}
			this.ChildBufferNumbers[this.Size] = BplusTreeLong.NULLBUFFERNUMBER;
			this.MaterializedChildNodes[this.Size] = null;
			// this is now a terminal node
			this.owner.RecordTerminalNode(this);
		}
		/// <summary>
		/// Find first index in self associated with a key same or greater than CompareKey
		/// </summary>
		/// <param name="CompareKey">CompareKey</param>
		/// <param name="LookPastOnly">if true and this is a leaf then look for a greater value</param>
		/// <returns>lowest index of same or greater key or this.Size if no greater key.</returns>
		int FindAtOrNextPosition(string CompareKey, bool LookPastOnly) 
		{
			int insertposition = 0;
			//System.Globalization.CultureInfo culture = this.owner.cultureContext;
			//System.Globalization.CompareInfo cmp = culture.CompareInfo;
			if (this.isLeaf && !LookPastOnly) 
			{
				// look for exact match or greater or null
				while (insertposition<this.Size && this.ChildKeys[insertposition]!=null &&
					//cmp.Compare(this.ChildKeys[insertposition], CompareKey)<0) 
					this.owner.Compare(this.ChildKeys[insertposition], CompareKey)<0)
				{
					insertposition++;
				}
			} 
			else 
			{
				// look for greater or null only
				while (insertposition<this.Size && this.ChildKeys[insertposition]!=null &&
					this.owner.Compare(this.ChildKeys[insertposition], CompareKey)<=0) 
				{
					insertposition++;
				}
			}
			return insertposition;
		}
		/// <summary>
		/// Find the first key below atIndex, or if no such node traverse to the next key to the right.
		/// If no such key exists, return nulls.
		/// </summary>
		/// <param name="atIndex">where to look in this node</param>
		/// <param name="FoundInLeaf">leaf where found</param>
		/// <param name="KeyFound">key value found</param>
		void TraverseToFollowingKey(int atIndex, out BplusNode FoundInLeaf, out string KeyFound) 
		{
			FoundInLeaf = null;
			KeyFound = null;
			bool LookInParent = false;
			if (this.isLeaf) 
			{
				LookInParent = (atIndex>=this.Size) || (this.ChildKeys[atIndex]==null);
			} 
			else 
			{
				LookInParent = (atIndex>this.Size) ||
					(atIndex>0 && this.ChildKeys[atIndex-1]==null);
			}
			if (LookInParent) 
			{
				// if it's anywhere it's in the next child of parent
				if (this.parent!=null && this.indexInParent>=0) 
				{
					this.parent.TraverseToFollowingKey(this.indexInParent+1, out FoundInLeaf, out KeyFound);
					return;
				} 
				else 
				{
					return; // no such following key
				}
			}
			if (this.isLeaf) 
			{
				// leaf, we found it.
				FoundInLeaf = this;
				KeyFound = this.ChildKeys[atIndex];
				return;
			} 
			else 
			{
				// nonleaf, look in child (if there is one)
				if (atIndex==0 || this.ChildKeys[atIndex-1]!=null) 
				{
					BplusNode thechild = this.MaterializeNodeAtIndex(atIndex);
					thechild.TraverseToFollowingKey(0, out FoundInLeaf, out KeyFound);
				}
			}
		}
		public bool FindMatch(string CompareKey, out long ValueFound) 
		{
			ValueFound = 0; // dummy value on failure
			BplusNode leaf;
			int position = this.FindAtOrNextPositionInLeaf(CompareKey, out leaf, false);
			if (position<leaf.Size) 
			{
				string key = leaf.ChildKeys[position];
				if ((key!=null) && this.owner.Compare(key, CompareKey)==0) //(key.Equals(CompareKey)
				{
					ValueFound = leaf.ChildBufferNumbers[position];
					return true;
				}
			}
			return false;
		}
		public string FindNextKey(string CompareKey) 
		{
			string result = null;
			BplusNode leaf;
			int position = this.FindAtOrNextPositionInLeaf(CompareKey, out leaf, true);
			if (position>=leaf.Size || leaf.ChildKeys[position]==null) 
			{
				// try to traverse to the right.
				BplusNode newleaf;
				leaf.TraverseToFollowingKey(leaf.Size, out newleaf, out result);
			} 
			else 
			{
				result = leaf.ChildKeys[position];
			}
			return result;
		}
		/// <summary>
		/// Find near-index of comparekey in leaf under this node. 
		/// </summary>
		/// <param name="CompareKey">the key to look for</param>
		/// <param name="inLeaf">the leaf where found</param>
		/// <param name="LookPastOnly">If true then only look for a greater value, not an exact match.</param>
		/// <returns>index of match in leaf</returns>
		int FindAtOrNextPositionInLeaf(string CompareKey, out BplusNode inLeaf, bool LookPastOnly) 
		{
			int myposition = this.FindAtOrNextPosition(CompareKey, LookPastOnly);
			if (this.isLeaf) 
			{
				inLeaf = this;
				return myposition;
			}
			long childBufferNumber = this.ChildBufferNumbers[myposition];
			if (childBufferNumber==BplusTreeLong.NULLBUFFERNUMBER) 
			{
				throw new BplusTreeException("can't search null subtree");
			}
			BplusNode child = this.MaterializeNodeAtIndex(myposition);
			return child.FindAtOrNextPositionInLeaf(CompareKey, out inLeaf, LookPastOnly);
		}
		BplusNode MaterializeNodeAtIndex(int myposition) 
		{
			if (this.isLeaf) 
			{
				throw new BplusTreeException("cannot materialize child for leaf");
			}
			long childBufferNumber = this.ChildBufferNumbers[myposition];
			if (childBufferNumber==BplusTreeLong.NULLBUFFERNUMBER) 
			{
				throw new BplusTreeException("can't search null subtree at position "+myposition+" in "+this.myBufferNumber);
			}
			// is it already materialized?
			BplusNode result = this.MaterializedChildNodes[myposition];
			if (result!=null) 
			{
				return result;
			}
			// otherwise read it in...
			result = new BplusNode(this.owner, this, myposition, true); // dummy isLeaf value
			result.LoadFromBuffer(childBufferNumber);
			this.MaterializedChildNodes[myposition] = result;
			// no longer terminal
			this.owner.ForgetTerminalNode(this);
			return result;
		}
		public void LoadFromBuffer(long bufferNumber) 
		{
			// freelist bookkeeping done elsewhere
			string parentinfo = "no parent"; // debug
			if (this.parent!=null) 
			{
				parentinfo = "parent="+parent.myBufferNumber; // debug
			}
			//System.Diagnostics.Debug.WriteLine("\r\n<br> loading "+this.indexInParent+" from "+bufferNumber+" for "+parentinfo);
			byte[] rawdata = new byte[this.owner.buffersize];
			this.owner.buffers.getBuffer(bufferNumber, rawdata, 0, rawdata.Length);
			this.Load(rawdata);
			this.Dirty = false;
			this.myBufferNumber = bufferNumber;
			// it's terminal until a child is materialized
			this.owner.RecordTerminalNode(this);
		}
		public long DumpToFreshBuffer() 
		{
			long oldbuffernumber = this.myBufferNumber;
			long freshBufferNumber = this.owner.allocateBuffer();
			//System.Diagnostics.Debug.WriteLine("\r\n<br> dumping "+this.indexInParent+" from "+oldbuffernumber+" to "+freshBufferNumber);
			this.DumpToBuffer(freshBufferNumber);
			if (oldbuffernumber!=BplusTreeLong.NULLBUFFERNUMBER) 
			{
				//this.owner.FreeBuffersOnCommit.Add(oldbuffernumber);
				if (this.owner.FreeBuffersOnAbort.ContainsKey(oldbuffernumber)) 
				{
					// free it now
					this.owner.FreeBuffersOnAbort.Remove(oldbuffernumber);
					this.owner.deallocateBuffer(oldbuffernumber);
				} 
				else 
				{
					// free on commit
					this.owner.FreeBuffersOnCommit[oldbuffernumber] = oldbuffernumber;
				}
			}
			//this.owner.FreeBuffersOnAbort.Add(freshBufferNumber);
			this.owner.FreeBuffersOnAbort[freshBufferNumber] = freshBufferNumber;
			return freshBufferNumber;
		}
		void DumpToBuffer(long buffernumber) 
		{
			byte[] rawdata = new byte[this.owner.buffersize];
			this.Dump(rawdata);
			this.owner.buffers.setBuffer(buffernumber, rawdata, 0, rawdata.Length);
			this.Dirty = false;
			this.myBufferNumber = buffernumber;
			if (this.parent!=null && this.indexInParent>=0 &&
				this.parent.ChildBufferNumbers[this.indexInParent]!=buffernumber) 
			{
				if (this.parent.MaterializedChildNodes[this.indexInParent]!=this) 
				{
					throw new BplusTreeException("invalid parent connection "+this.parent.myBufferNumber+" at "+this.indexInParent);
				}
				this.parent.ChildBufferNumbers[this.indexInParent] = buffernumber;
				this.parent.Soil();
			}
		}
		void reParentAllChildren() 
		{
			for (int i=0; i<=this.Size; i++) 
			{
				BplusNode thisnode = this.MaterializedChildNodes[i];
				if (thisnode!=null) 
				{
					thisnode.Reparent(this, i);
				}
			}
		}
		/// <summary>
		/// Delete entry for key
		/// </summary>
		/// <param name="key">key to delete</param>
		/// <param name="MergeMe">true if the node is less than half full after deletion</param>
		/// <returns>null unless the smallest key under this node has changed in which case it returns the smallest key.</returns>
		public string Delete(string key, out bool MergeMe) 
		{
			MergeMe = false; // assumption
			string result = null;
			if (this.isLeaf) 
			{
				return this.DeleteLeaf(key, out MergeMe);
			}
			int deleteposition = this.FindAtOrNextPosition(key, false);
			long deleteBufferNumber = this.ChildBufferNumbers[deleteposition];
			if (deleteBufferNumber==BplusTreeLong.NULLBUFFERNUMBER) 
			{
				throw new BplusTreeException("key not followed by buffer number in non-leaf (del)");
			}
			// del in subtree
			BplusNode DeleteChild = this.MaterializeNodeAtIndex(deleteposition);
			bool MergeKid;
			string delresult = DeleteChild.Delete(key, out MergeKid);
			// delete succeeded... now fix up the child node if needed.
			this.Soil(); // redundant ?
			// bizarre special case for 2-3  or 3-4 trees -- empty leaf
			if (delresult!=null && this.owner.Compare(delresult, key)==0) // delresult.Equals(key)
			{
				if (this.Size>3) 
				{
					throw new BplusTreeException("assertion error: delete returned delete key for too large node size: "+this.Size);
				}
				// junk this leaf and shift everything over
				if (deleteposition==0) 
				{
					result = this.ChildKeys[deleteposition];
				} 
				else if (deleteposition==this.Size) 
				{
					this.ChildKeys[deleteposition-1] = null;
				}
				else
				{
					this.ChildKeys[deleteposition-1] = this.ChildKeys[deleteposition];
				}
				if (result!=null && this.owner.Compare(result, key)==0) // result.Equals(key)
				{
					// I'm not sure this ever happens
					this.MaterializeNodeAtIndex(1);
					result = this.MaterializedChildNodes[1].LeastKey();
				}
				DeleteChild.Free();
				for (int i=deleteposition; i<this.Size-1; i++) 
				{
					this.ChildKeys[i] = this.ChildKeys[i+1];
					this.MaterializedChildNodes[i] = this.MaterializedChildNodes[i+1];
					this.ChildBufferNumbers[i] = this.ChildBufferNumbers[i+1];
				}
				this.ChildKeys[this.Size-1] = null;
				if (deleteposition<this.Size) 
				{
					this.MaterializedChildNodes[this.Size-1] = this.MaterializedChildNodes[this.Size];
					this.ChildBufferNumbers[this.Size-1] = this.ChildBufferNumbers[this.Size];
				}
				this.MaterializedChildNodes[this.Size] = null;
				this.ChildBufferNumbers[this.Size] = BplusTreeLong.NULLBUFFERNUMBER;
				MergeMe = (this.SizeInUse()<this.Size/2);
				this.reParentAllChildren();
				return result;
			}
			if (deleteposition==0) 
			{
				// smallest key may have changed.
				result = delresult;
			}
				// update key array if needed
			else if (delresult!=null && deleteposition>0) 
			{
				if (this.owner.Compare(delresult,key)!=0) // !delresult.Equals(key)
				{
					this.ChildKeys[deleteposition-1] = delresult;
				} 
			}
			// if the child needs merging... do it
			if (MergeKid) 
			{
				int leftindex, rightindex;
				BplusNode leftNode;
				BplusNode rightNode;
				string keyBetween;
				if (deleteposition==0) 
				{
					// merge with next
					leftindex = deleteposition;
					rightindex = deleteposition+1;
					leftNode = DeleteChild;
					//keyBetween = this.ChildKeys[deleteposition];
					rightNode = this.MaterializeNodeAtIndex(rightindex);
				} 
				else 
				{
					// merge with previous
					leftindex = deleteposition-1;
					rightindex = deleteposition;
					leftNode = this.MaterializeNodeAtIndex(leftindex);
					//keyBetween = this.ChildKeys[deleteBufferNumber-1];
					rightNode = DeleteChild;
				}
				keyBetween = this.ChildKeys[leftindex];
				string rightLeastKey;
				bool DeleteRight;
				Merge(leftNode, keyBetween, rightNode, out rightLeastKey, out DeleteRight);
				// delete the right node if needed.
				if (DeleteRight) 
				{
					for (int i=rightindex; i<this.Size; i++) 
					{
						this.ChildKeys[i-1] = this.ChildKeys[i];
						this.ChildBufferNumbers[i] = this.ChildBufferNumbers[i+1];
						this.MaterializedChildNodes[i] = this.MaterializedChildNodes[i+1];
					}
					this.ChildKeys[this.Size-1] = null;
					this.MaterializedChildNodes[this.Size] = null;
					this.ChildBufferNumbers[this.Size] = BplusTreeLong.NULLBUFFERNUMBER;
					this.reParentAllChildren();
					rightNode.Free();
					// does this node need merging?
					if (this.SizeInUse()<this.Size/2) 
					{
						MergeMe = true;
					}
				} 
				else 
				{
					// update the key entry
					this.ChildKeys[rightindex-1] = rightLeastKey;
				}
			}
			return result;
		}
		string LeastKey() 
		{
			string result = null;
			if (this.isLeaf) 
			{
				result = this.ChildKeys[0];
			} 
			else 
			{
				this.MaterializeNodeAtIndex(0);
				result = this.MaterializedChildNodes[0].LeastKey();
			}
			if (result==null) 
			{
				throw new BplusTreeException("no key found");
			}
			return result;
		}
		public static void Merge(BplusNode left, string KeyBetween, BplusNode right, out string rightLeastKey, 
			out bool DeleteRight) 
		{
			//System.Diagnostics.Debug.WriteLine("\r\n<br> merging "+right.myBufferNumber+" ("+KeyBetween+") "+left.myBufferNumber);
			//System.Diagnostics.Debug.WriteLine(left.owner.toHtml());
			rightLeastKey = null; // only if DeleteRight
			if (left.isLeaf || right.isLeaf) 
			{
				if (!(left.isLeaf&&right.isLeaf)) 
				{
					throw new BplusTreeException("can't merge leaf with non-leaf");
				}
				MergeLeaves(left, right, out DeleteRight);
				rightLeastKey = right.ChildKeys[0];
				return;
			}
			// merge non-leaves
			DeleteRight = false;
			string[] allkeys = new string[left.Size*2+1];
			long[] allseeks = new long[left.Size*2+2];
			BplusNode[] allMaterialized = new BplusNode[left.Size*2+2];
			if (left.ChildBufferNumbers[0]==BplusTreeLong.NULLBUFFERNUMBER ||
				right.ChildBufferNumbers[0]==BplusTreeLong.NULLBUFFERNUMBER) 
			{
				throw new BplusTreeException("cannot merge empty non-leaf with non-leaf");
			}
			int index = 0;
			allseeks[0] = left.ChildBufferNumbers[0];
			allMaterialized[0] = left.MaterializedChildNodes[0];
			for (int i=0; i<left.Size; i++) 
			{
				if (left.ChildKeys[i]==null) 
				{
					break;
				}
				allkeys[index] = left.ChildKeys[i];
				allseeks[index+1] = left.ChildBufferNumbers[i+1];
				allMaterialized[index+1] = left.MaterializedChildNodes[i+1];
				index++;
			}
			allkeys[index] = KeyBetween;
			index++;
			allseeks[index] = right.ChildBufferNumbers[0];
			allMaterialized[index] = right.MaterializedChildNodes[0];
			int rightcount = 0;
			for (int i=0; i<right.Size; i++) 
			{
				if (right.ChildKeys[i]==null) 
				{
					break;
				}
				allkeys[index] = right.ChildKeys[i];
				allseeks[index+1] = right.ChildBufferNumbers[i+1];
				allMaterialized[index+1] = right.MaterializedChildNodes[i+1];
				index++;
				rightcount++;
			}
			if (index<=left.Size) 
			{
				// it will all fit in one node
				//System.Diagnostics.Debug.WriteLine("deciding to forget "+right.myBufferNumber+" into "+left.myBufferNumber);
				DeleteRight = true;
				for (int i=0; i<index; i++) 
				{
					left.ChildKeys[i] = allkeys[i];
					left.ChildBufferNumbers[i] = allseeks[i];
					left.MaterializedChildNodes[i] = allMaterialized[i];
				}
				left.ChildBufferNumbers[index] = allseeks[index];
				left.MaterializedChildNodes[index] = allMaterialized[index];
				left.reParentAllChildren();
				left.Soil();
				right.Free();
				return;
			}
			// otherwise split the content between the nodes
			left.Clear();
			right.Clear();
			left.Soil();
			right.Soil();
			int leftcontent = index/2;
			int rightcontent = index-leftcontent-1;
			rightLeastKey = allkeys[leftcontent];
			int outputindex = 0;
			for (int i=0; i<leftcontent; i++) 
			{
				left.ChildKeys[i] = allkeys[outputindex];
				left.ChildBufferNumbers[i] = allseeks[outputindex];
				left.MaterializedChildNodes[i] = allMaterialized[outputindex];
				outputindex++;
			}
			rightLeastKey = allkeys[outputindex];
			left.ChildBufferNumbers[outputindex] = allseeks[outputindex];
			left.MaterializedChildNodes[outputindex] = allMaterialized[outputindex];
			outputindex++;
			rightcount = 0;
			for (int i=0; i<rightcontent; i++) 
			{
				right.ChildKeys[i] = allkeys[outputindex];
				right.ChildBufferNumbers[i] = allseeks[outputindex];
				right.MaterializedChildNodes[i] = allMaterialized[outputindex];
				outputindex++;
				rightcount++;
			}
			right.ChildBufferNumbers[rightcount] = allseeks[outputindex];
			right.MaterializedChildNodes[rightcount] = allMaterialized[outputindex];
			left.reParentAllChildren();
			right.reParentAllChildren();
		}
		public static void MergeLeaves(BplusNode left, BplusNode right, out bool DeleteRight) 
		{
			DeleteRight = false;
			string[] allkeys = new string[left.Size*2];
			long[] allseeks = new long[left.Size*2];
			int index = 0;
			for (int i=0; i<left.Size; i++) 
			{
				if (left.ChildKeys[i]==null) 
				{
					break;
				}
				allkeys[index] = left.ChildKeys[i];
				allseeks[index] = left.ChildBufferNumbers[i];
				index++;
			}
			for (int i=0; i<right.Size; i++) 
			{
				if (right.ChildKeys[i]==null) 
				{
					break;
				}
				allkeys[index] = right.ChildKeys[i];
				allseeks[index] = right.ChildBufferNumbers[i];
				index++;
			}
			if (index<=left.Size) 
			{
				left.Clear();
				DeleteRight = true;
				for (int i=0; i<index; i++) 
				{
					left.ChildKeys[i] = allkeys[i];
					left.ChildBufferNumbers[i] = allseeks[i];
				}
				right.Free();
				left.Soil();
				return;
			}
			left.Clear();
			right.Clear();
			left.Soil();
			right.Soil();
			int rightcontent = index/2;
			int leftcontent = index - rightcontent;
			int newindex = 0;
			for (int i=0; i<leftcontent; i++) 
			{
				left.ChildKeys[i] = allkeys[newindex];
				left.ChildBufferNumbers[i] = allseeks[newindex];
				newindex++;
			}
			for (int i=0; i<rightcontent; i++) 
			{
				right.ChildKeys[i] = allkeys[newindex];
				right.ChildBufferNumbers[i] = allseeks[newindex];
				newindex++;
			}
		}
		public string DeleteLeaf(string key, out bool MergeMe) 
		{
			string result = null;
			MergeMe = false;
			bool found = false;
			int deletelocation = 0;
			foreach (string thiskey in this.ChildKeys) 
			{
				// use comparison, not equals, in case different strings sometimes compare same
				if (thiskey!=null && this.owner.Compare(thiskey, key)==0) // thiskey.Equals(key)
				{
					found = true;
					break;
				}
				deletelocation++;
			}
			if (!found) 
			{
				throw new BplusTreeKeyMissing("cannot delete missing key: "+key);
			}
			this.Soil();
			// only keys are important...
			for (int i=deletelocation; i<this.Size-1; i++) 
			{
				this.ChildKeys[i] = this.ChildKeys[i+1];
				this.ChildBufferNumbers[i] = this.ChildBufferNumbers[i+1];
			}
			this.ChildKeys[this.Size-1] = null;
			//this.MaterializedChildNodes[endlocation+1] = null;
			//this.ChildBufferNumbers[endlocation+1] = BplusTreeLong.NULLBUFFERNUMBER;
			if (this.SizeInUse()<this.Size/2)
			{
				MergeMe = true;
			}
			if (deletelocation==0) 
			{
				result = this.ChildKeys[0];
				// this is only relevant for the case of 2-3 trees (empty leaf after deletion)
				if (result==null) 
				{
					result = key; // deleted value
				}
			}
			return result;
		}
		/// <summary>
		/// insert key/position entry in self 
		/// </summary>
		/// <param name="key">Key to associate with the leaf</param>
		/// <param name="position">position associated with key in external structur</param>
		/// <param name="splitString">if not null then the smallest key in the new split leaf</param>
		/// <param name="splitNode">if not null then the node was split and this is the leaf to the right.</param>
		/// <returns>null unless the smallest key under this node has changed, in which case it returns the smallest key.</returns>
		public string Insert(string key, long position, out string splitString, out BplusNode splitNode) 
		{
			if (this.isLeaf) 
			{
				return this.InsertLeaf(key, position, out splitString, out splitNode);
			}
			splitString = null;
			splitNode = null;
			int insertposition = this.FindAtOrNextPosition(key, false);
			long insertBufferNumber = this.ChildBufferNumbers[insertposition];
			if (insertBufferNumber==BplusTreeLong.NULLBUFFERNUMBER) 
			{
				throw new BplusTreeException("key not followed by buffer number in non-leaf");
			}
			// insert in subtree
			BplusNode InsertChild = this.MaterializeNodeAtIndex(insertposition);
			BplusNode childSplit;
			string childSplitString;
			string childInsert = InsertChild.Insert(key, position, out childSplitString, out childSplit);
			// if there was a split the node must expand
			if (childSplit!=null) 
			{
				// insert the child
				this.Soil(); // redundant -- a child will have a change so this node will need to be copied
				int newChildPosition = insertposition+1;
				bool dosplit = false;
				// if there is no free space we must do a split
				if (this.ChildBufferNumbers[this.Size]!=BplusTreeLong.NULLBUFFERNUMBER) 
				{
					dosplit = true;
					this.PrepareForSplit();
				}
				// bubble over the current values to make space for new child
				for (int i=this.ChildKeys.Length-2; i>=newChildPosition-1; i--) 
				{
					int i1 = i+1;
					int i2 = i1+1;
					this.ChildKeys[i1] = this.ChildKeys[i];
					this.ChildBufferNumbers[i2] = this.ChildBufferNumbers[i1];
					BplusNode childNode = this.MaterializedChildNodes[i2] = this.MaterializedChildNodes[i1];
				}
				// record the new child
				this.ChildKeys[newChildPosition-1] = childSplitString;
				//this.MaterializedChildNodes[newChildPosition] = childSplit;
				//this.ChildBufferNumbers[newChildPosition] = childSplit.myBufferNumber;
				childSplit.Reparent(this, newChildPosition);
				// split, if needed
				if (dosplit) 
				{
					int splitpoint = this.MaterializedChildNodes.Length/2-1;
					splitString = this.ChildKeys[splitpoint];
					splitNode = new BplusNode(this.owner, this.parent, -1, this.isLeaf);
					// make copy of expanded node structure
					BplusNode[] materialized = this.MaterializedChildNodes;
					long[] buffernumbers = this.ChildBufferNumbers;
					string[] keys = this.ChildKeys;
					// repair the expanded node
					this.ChildKeys = new string[this.Size];
					this.MaterializedChildNodes = new BplusNode[this.Size+1];
					this.ChildBufferNumbers = new long[this.Size+1];
					this.Clear();
					Array.Copy(materialized, 0, this.MaterializedChildNodes, 0, splitpoint+1);
					Array.Copy(buffernumbers, 0, this.ChildBufferNumbers, 0, splitpoint+1);
					Array.Copy(keys, 0, this.ChildKeys, 0, splitpoint);
					// initialize the new node
					splitNode.Clear(); // redundant.
					int remainingKeys = this.Size-splitpoint;
					Array.Copy(materialized, splitpoint+1, splitNode.MaterializedChildNodes, 0, remainingKeys+1);
					Array.Copy(buffernumbers, splitpoint+1, splitNode.ChildBufferNumbers, 0, remainingKeys+1);
					Array.Copy(keys, splitpoint+1, splitNode.ChildKeys, 0, remainingKeys);
					// fix pointers in materialized children of splitnode
					splitNode.reParentAllChildren();
					// store the new node
					splitNode.DumpToFreshBuffer();
					splitNode.CheckIfTerminal();
					splitNode.Soil();
					this.CheckIfTerminal();
				}
				// fix pointers in children
				this.reParentAllChildren();
			}
			if (insertposition==0) 
			{
				// the smallest key may have changed
				return childInsert;
			}
			return null;  // no change in smallest key
		}
		/// <summary>
		/// Check to see if this is a terminal node, if so record it, otherwise forget it
		/// </summary>
		void CheckIfTerminal() 
		{
			if (!this.isLeaf) 
			{
				for (int i=0; i<this.Size+1; i++) 
				{
					if (this.MaterializedChildNodes[i]!=null) 
					{
						this.owner.ForgetTerminalNode(this);
						return;
					}
				}
			}
			this.owner.RecordTerminalNode(this);
		}
		/// <summary>
		/// insert key/position entry in self (as leaf)
		/// </summary>
		/// <param name="key">Key to associate with the leaf</param>
		/// <param name="position">position associated with key in external structure</param>
		/// <param name="splitString">if not null then the smallest key in the new split leaf</param>
		/// <param name="splitNode">if not null then the node was split and this is the leaf to the right.</param>
		/// <returns>smallest key value in keys, or null if no change</returns>
		public string InsertLeaf(string key, long position, out string splitString, out BplusNode splitNode) 
		{
			splitString = null;
			splitNode = null;
			bool dosplit = false;
			if (!this.isLeaf) 
			{
				throw new BplusTreeException("bad call to InsertLeaf: this is not a leaf");
			}
			this.Soil();
			int insertposition = this.FindAtOrNextPosition(key, false);
			if (insertposition>=this.Size) 
			{
				//throw new BplusTreeException("key too big and leaf is full");
				dosplit = true;
				this.PrepareForSplit();
			} 
			else 
			{
				// if it's already there then change the value at the current location (duplicate entries not supported).
				if (this.ChildKeys[insertposition]==null || this.owner.Compare(this.ChildKeys[insertposition], key)==0) // this.ChildKeys[insertposition].Equals(key)
				{
					this.ChildBufferNumbers[insertposition] = position;
					this.ChildKeys[insertposition] = key;
					if (insertposition==0) 
					{
						return key;
					} 
					else 
					{
						return null;
					}
				}
			}
			// check for a null position
			int nullindex = insertposition;
			while (nullindex<this.ChildKeys.Length && this.ChildKeys[nullindex]!=null) 
			{
				nullindex++;
			}
			if (nullindex>=this.ChildKeys.Length) 
			{
				if (dosplit) 
				{
					throw new BplusTreeException("can't split twice!!");
				}
				//throw new BplusTreeException("no space in leaf");
				dosplit = true;
				this.PrepareForSplit();
			}
			// bubble in the new info XXXX THIS SHOULD BUBBLE BACKWARDS	
			string nextkey = this.ChildKeys[insertposition];
			long nextposition = this.ChildBufferNumbers[insertposition];
			this.ChildKeys[insertposition] = key;
			this.ChildBufferNumbers[insertposition] = position;
			while (nextkey!=null) 
			{
				key = nextkey;
				position = nextposition;
				insertposition++;
				nextkey = this.ChildKeys[insertposition];
				nextposition = this.ChildBufferNumbers[insertposition];
				this.ChildKeys[insertposition] = key;
				this.ChildBufferNumbers[insertposition] = position;
			}
			// split if needed
			if (dosplit) 
			{
				int splitpoint = this.ChildKeys.Length/2;
				int splitlength = this.ChildKeys.Length - splitpoint;
				splitNode = new BplusNode(this.owner, this.parent, -1, this.isLeaf);
				// copy the split info into the splitNode
				Array.Copy(this.ChildBufferNumbers, splitpoint, splitNode.ChildBufferNumbers, 0, splitlength);
				Array.Copy(this.ChildKeys, splitpoint, splitNode.ChildKeys, 0, splitlength);
				Array.Copy(this.MaterializedChildNodes, splitpoint, splitNode.MaterializedChildNodes, 0, splitlength);
				splitString = splitNode.ChildKeys[0];
				// archive the new node
				splitNode.DumpToFreshBuffer();
				// store the node data temporarily
				long[] buffernumbers = this.ChildBufferNumbers;
				string[] keys = this.ChildKeys;
				BplusNode[] nodes = this.MaterializedChildNodes;
				// repair current node, copy in the other part of the split
				this.ChildBufferNumbers = new long[this.Size+1];
				this.ChildKeys = new string[this.Size];
				this.MaterializedChildNodes = new BplusNode[this.Size+1];
				Array.Copy(buffernumbers, 0, this.ChildBufferNumbers, 0, splitpoint);
				Array.Copy(keys, 0, this.ChildKeys, 0, splitpoint);
				Array.Copy(nodes, 0, this.MaterializedChildNodes, 0, splitpoint);
				for (int i=splitpoint; i<this.ChildKeys.Length; i++) 
				{
					this.ChildKeys[i] = null;
					this.ChildBufferNumbers[i] = BplusTreeLong.NULLBUFFERNUMBER;
					this.MaterializedChildNodes[i] = null;
				}
				// store the new node
				//splitNode.DumpToFreshBuffer();
				this.owner.RecordTerminalNode(splitNode);
				splitNode.Soil();
			}
			//return this.ChildKeys[0];
			if (insertposition==0) 
			{
				return key; // smallest key changed.
			} 
			else 
			{
				return null; // no change in smallest key
			}
		}
		/// <summary>
		/// Grow to this.size+1 in preparation for insertion and split
		/// </summary>
		void PrepareForSplit() 
		{
			int supersize = this.Size+1;
			long[] positions = new long[supersize+1];
			string[] keys = new string[supersize];
			BplusNode[] materialized = new BplusNode[supersize+1];
			Array.Copy(this.ChildBufferNumbers, 0, positions, 0, this.Size+1);
			positions[this.Size+1] = BplusTreeLong.NULLBUFFERNUMBER;
			Array.Copy(this.ChildKeys, 0, keys, 0, this.Size);
			keys[this.Size] = null;
			Array.Copy(this.MaterializedChildNodes, 0, materialized, 0, this.Size+1);
			materialized[this.Size+1] = null;
			this.ChildBufferNumbers = positions;
			this.ChildKeys = keys;
			this.MaterializedChildNodes = materialized;
		}
		public void Load(byte[] buffer) 
		{
			// load serialized data
			// indicator | seek position | [ key storage | seek position ]*
			this.Clear();
			if (buffer.Length!=owner.buffersize) 
			{
				throw new BplusTreeException("bad buffer size "+buffer.Length+" should be "+owner.buffersize);
			}
			byte indicator = buffer[0];
			this.isLeaf = false;
			if (indicator==BplusTreeLong.LEAF) 
			{
				this.isLeaf = true;
			} 
			else if (indicator!=BplusTreeLong.NONLEAF) 
			{
				throw new BplusTreeException("bad indicator, not leaf or nonleaf in tree "+indicator);
			}
			int index = 1;
			// get the first seek position
			this.ChildBufferNumbers[0] = BufferFile.RetrieveLong(buffer, index);
			System.Text.Decoder decode = System.Text.Encoding.UTF8.GetDecoder();
			index+= BufferFile.LONGSTORAGE;
			int maxKeyLength = this.owner.KeyLength;
			int maxKeyPayload = maxKeyLength - BufferFile.SHORTSTORAGE;
			//this.NumberOfValidKids = 0;
			// get remaining key storages and seek positions
			string lastkey = "";
			for (int KeyIndex=0; KeyIndex<this.Size; KeyIndex++) 
			{
				// decode and store a key
				short keylength = BufferFile.RetrieveShort(buffer, index);
				if (keylength<-1 || keylength>maxKeyPayload) 
				{
					throw new BplusTreeException("invalid keylength decoded");
				}
				index+= BufferFile.SHORTSTORAGE;
				string key = null;
				if (keylength==0) 
				{
					key = "";
				} 
				else if (keylength>0) 
				{
					int charCount = decode.GetCharCount(buffer, index, keylength);
					char[] ca = new char[charCount];
					decode.GetChars(buffer, index, keylength, ca, 0);
					//this.NumberOfValidKids++;
					key = new String(ca);
				}
				this.ChildKeys[KeyIndex] = key;
				index+= maxKeyPayload;
				// decode and store a seek position
				long seekPosition = BufferFile.RetrieveLong(buffer, index);
				if (!this.isLeaf) 
				{
					if (key==null & seekPosition!=BplusTreeLong.NULLBUFFERNUMBER) 
					{
						throw new BplusTreeException("key is null but position is not "+KeyIndex);
					} 
					else if (lastkey==null && key!=null) 
					{
						throw new BplusTreeException("null key followed by non-null key "+KeyIndex);
					}
				}
				lastkey = key;
				this.ChildBufferNumbers[KeyIndex+1] = seekPosition;
				index+= BufferFile.LONGSTORAGE;
			}
		}
		/// <summary>
		/// check that key is ok for node of this size (put here for locality of relevant code).
		/// </summary>
		/// <param name="key">key to check</param>
		/// <param name="owner">tree to contain node containing the key</param>
		/// <returns>true if key is ok</returns>
		public static bool KeyOK(string key, BplusTreeLong owner) 
		{
			if (key==null) 
			{ 
				return false;
			}
			System.Text.Encoder encode = System.Text.Encoding.UTF8.GetEncoder();
			int maxKeyLength = owner.KeyLength;
			int maxKeyPayload = maxKeyLength - BufferFile.SHORTSTORAGE;
			char[] keyChars = key.ToCharArray();
			int charCount = encode.GetByteCount(keyChars, 0, keyChars.Length, true);
			if (charCount>maxKeyPayload) 
			{
				return false;
			}
			return true;
		}
		public void Dump(byte[] buffer) 
		{
			// indicator | seek position | [ key storage | seek position ]*
			if (buffer.Length!=owner.buffersize) 
			{
				throw new BplusTreeException("bad buffer size "+buffer.Length+" should be "+owner.buffersize);
			}
			buffer[0] = BplusTreeLong.NONLEAF;
			if (this.isLeaf) { buffer[0] = BplusTreeLong.LEAF; }
			int index = 1;
			// store first seek position
			BufferFile.Store(this.ChildBufferNumbers[0], buffer, index);
			index+= BufferFile.LONGSTORAGE;
			System.Text.Encoder encode = System.Text.Encoding.UTF8.GetEncoder();
			// store remaining keys and seeks
			int maxKeyLength = this.owner.KeyLength;
			int maxKeyPayload = maxKeyLength - BufferFile.SHORTSTORAGE;
			string lastkey = "";
			for (int KeyIndex=0; KeyIndex<this.Size; KeyIndex++) 
			{
				// store a key
				string theKey = this.ChildKeys[KeyIndex];
				short charCount = -1;
				if (theKey!=null) 
				{
					char[] keyChars = theKey.ToCharArray();
					charCount = (short) encode.GetByteCount(keyChars, 0, keyChars.Length, true);
					if (charCount>maxKeyPayload) 
					{
						throw new BplusTreeException("string bytes to large for use as key "+charCount+">"+maxKeyPayload);
					}
					BufferFile.Store(charCount, buffer, index);
					index+= BufferFile.SHORTSTORAGE;
					encode.GetBytes(keyChars, 0, keyChars.Length, buffer, index, true);
				} 
				else 
				{
					// null case (no string to read)
					BufferFile.Store(charCount, buffer, index);
					index+= BufferFile.SHORTSTORAGE;
				}
				index+= maxKeyPayload;
				// store a seek
				long seekPosition = this.ChildBufferNumbers[KeyIndex+1];
				if (theKey==null && seekPosition!=BplusTreeLong.NULLBUFFERNUMBER && !this.isLeaf) 
				{
					throw new BplusTreeException("null key paired with non-null location "+KeyIndex);
				}
				if (lastkey==null && theKey!=null) 
				{
					throw new BplusTreeException("null key followed by non-null key "+KeyIndex);
				}
				lastkey = theKey;
				BufferFile.Store(seekPosition, buffer, index);
				index+= BufferFile.LONGSTORAGE;
			}
		}
		/// <summary>
		/// Close the node:
		/// invalidate all children, store state if needed, remove materialized self from parent.
		/// </summary>
		public long Invalidate(bool destroyRoot) 
		{
			long result = this.myBufferNumber;
			if (!this.isLeaf) 
			{
				// need to invalidate kids
				for (int i=0; i<this.Size+1; i++) 
				{
					if (this.MaterializedChildNodes[i]!=null) 
					{
						// new buffer numbers are recorded automatically
						this.ChildBufferNumbers[i] = this.MaterializedChildNodes[i].Invalidate(true);
					}
				}
			} 
			// store if dirty
			if (this.Dirty) 
			{
				result = this.DumpToFreshBuffer();
//				result = this.myBufferNumber;
			}
			// remove from owner archives if present
			this.owner.ForgetTerminalNode(this);
			// remove from parent
			if (this.parent!=null && this.indexInParent>=0) 
			{
				this.parent.MaterializedChildNodes[this.indexInParent] = null;
				this.parent.ChildBufferNumbers[this.indexInParent] = result; // should be redundant
				this.parent.CheckIfTerminal();
				this.indexInParent = -1;
			}
			// render all structures useless, just in case...
			if (destroyRoot) 
			{
				this.Destroy();
			}
			return result;
		}
		/// <summary>
		/// Mark this as dirty and all ancestors too.
		/// </summary>
		void Soil() 
		{
			if (this.Dirty) 
			{
				return; // don't need to do it again
			} 
			else 
			{
				this.Dirty = true;
				if (this.parent!=null) 
				{
					this.parent.Soil();
				}
			}
		}
		public void AsHtml(System.Text.StringBuilder sb) 
		{
			string hygeine = "clean";
			if (this.Dirty) { hygeine = "dirty"; }
			int keycount = 0;
			if (this.isLeaf) 
			{
				for (int i=0; i<this.Size; i++) 
				{
					string key = this.ChildKeys[i];
					long seek = this.ChildBufferNumbers[i];
					if (key!=null) 
					{
						key = PrintableString(key);
						sb.Append("'"+key+"' : "+seek+"<br>\r\n");
						keycount++;
					}
				}
				sb.Append("leaf "+this.indexInParent+" at "+this.myBufferNumber+" #keys=="+keycount+" "+hygeine+"\r\n");
			} 
			else 
			{
				sb.Append("<table border>\r\n");
				sb.Append("<tr><td colspan=2>nonleaf "+this.indexInParent+" at "+this.myBufferNumber+" "+hygeine+"</td></tr>\r\n");
				if (this.ChildBufferNumbers[0]!=BplusTreeLong.NULLBUFFERNUMBER) 
				{
					this.MaterializeNodeAtIndex(0);
					sb.Append("<tr><td></td><td>"+this.ChildBufferNumbers[0]+"</td><td>\r\n");
					this.MaterializedChildNodes[0].AsHtml(sb);
					sb.Append("</td></tr>\r\n");
				}
				for (int i=0; i<this.Size; i++) 
				{
					string key =  this.ChildKeys[i];
					if (key==null) 
					{
						break;
					}
					key = PrintableString(key);
					sb.Append("<tr><th>'"+key+"'</th><td></td><td></td></tr>\r\n");
					try 
					{
						this.MaterializeNodeAtIndex(i+1);
						sb.Append("<tr><td></td><td>"+this.ChildBufferNumbers[i+1]+"</td><td>\r\n");
						this.MaterializedChildNodes[i+1].AsHtml(sb);
						sb.Append("</td></tr>\r\n");
					} 
					catch (BplusTreeException) 
					{
						sb.Append("<tr><td></td><th>COULDN'T MATERIALIZE NODE "+(i+1)+"</th></tr>");
					}
					keycount++;
				}
				sb.Append("<tr><td colspan=2> #keys=="+keycount+"</td></tr>\r\n");
				sb.Append("</table>\r\n");
			}
		}
		public static string PrintableString(string s) 
		{
			if (s==null) { return "[NULL]"; }
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			foreach (char c in s) 
			{
				if (Char.IsLetterOrDigit(c) || Char.IsPunctuation(c)) 
				{
					sb.Append(c);
				} 
				else 
				{
					sb.Append("["+Convert.ToInt32(c)+"]");
				}
			}
			return sb.ToString();
		}
	}
	/// <summary>
	/// Generic error including programming errors.
	/// </summary>
	public class BplusTreeException: ApplicationException 
	{
		public BplusTreeException(string message): base(message) 
		{
			// do nothing extra
		}
	}
	/// <summary>
	/// No such key found for attempted retrieval.
	/// </summary>
	public class BplusTreeKeyMissing: ApplicationException 
	{
		public BplusTreeKeyMissing(string message): base(message) 
		{
			// do nothing extra
		}
	}
	/// <summary>
	/// Key cannot be null or too large.
	/// </summary>
	public class BplusTreeBadKeyValue: ApplicationException 
	{
		public BplusTreeBadKeyValue(string message): base(message) 
		{
			// do nothing extra
		}
	}
}
