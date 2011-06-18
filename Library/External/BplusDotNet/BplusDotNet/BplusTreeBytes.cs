// The bplusdotnet package is Copywrite Aaron Watters 2004. 
// the package is licensed under the BSD open source license

using System;
using System.Collections;

namespace BplusDotNet
{
	/// <summary>
	/// BPlus tree implementation mapping strings to bytes with fixed key length
	/// </summary>
	public class BplusTreeBytes : IByteTree
	{
		BplusTreeLong tree;
		LinkedFile archive;
		Hashtable FreeChunksOnCommit = new Hashtable();
		Hashtable FreeChunksOnAbort = new Hashtable();
		static int DEFAULTBLOCKSIZE = 1024;
		static int DEFAULTNODESIZE = 32;
		public BplusTreeBytes(BplusTreeLong tree, LinkedFile archive)
		{
			this.tree = tree;
			this.archive = archive;
		}

		public static BplusTreeBytes Initialize(string treefileName, string blockfileName, int KeyLength, int CultureId,
			int nodesize, int buffersize) 
		{
			System.IO.Stream treefile = new System.IO.FileStream(treefileName, System.IO.FileMode.CreateNew, 
				System.IO.FileAccess.ReadWrite);
			System.IO.Stream blockfile = new System.IO.FileStream(blockfileName, System.IO.FileMode.CreateNew, 
				System.IO.FileAccess.ReadWrite);
			return Initialize(treefile, blockfile, KeyLength, CultureId, nodesize, buffersize);
		}
		public static BplusTreeBytes Initialize(string treefileName, string blockfileName, int KeyLength, int CultureId) 
		{
			System.IO.Stream treefile = new System.IO.FileStream(treefileName, System.IO.FileMode.CreateNew, 
				System.IO.FileAccess.ReadWrite);
			System.IO.Stream blockfile = new System.IO.FileStream(blockfileName, System.IO.FileMode.CreateNew, 
				System.IO.FileAccess.ReadWrite);
			return Initialize(treefile, blockfile, KeyLength, CultureId);
		}
		public static BplusTreeBytes Initialize(string treefileName, string blockfileName, int KeyLength) 
		{
			System.IO.Stream treefile = null;
			System.IO.Stream blockfile = null;

			try
			{
				treefile = new System.IO.FileStream(treefileName, System.IO.FileMode.CreateNew, System.IO.FileAccess.ReadWrite);
				blockfile = new System.IO.FileStream(blockfileName, System.IO.FileMode.CreateNew, System.IO.FileAccess.ReadWrite);

				return Initialize(treefile, blockfile, KeyLength);
			}
			catch(Exception ex)
			{
				if(treefile != null)
					treefile.Dispose();

				if(blockfile != null)
					blockfile.Dispose();

				throw ex;
			}
		}
		public static BplusTreeBytes Initialize(System.IO.Stream treefile, System.IO.Stream blockfile, int KeyLength, int CultureId,
			int nodesize, int buffersize) 
		{
			BplusTreeLong tree = BplusTreeLong.InitializeInStream(treefile, KeyLength, nodesize, CultureId);
			LinkedFile archive = LinkedFile.InitializeLinkedFileInStream(blockfile, buffersize);
			return new BplusTreeBytes(tree, archive);
		}
		public static BplusTreeBytes Initialize(System.IO.Stream treefile, System.IO.Stream blockfile, int KeyLength, int CultureId) 
		{
			return Initialize(treefile, blockfile, KeyLength, CultureId, DEFAULTNODESIZE, DEFAULTBLOCKSIZE);
		}
		public static BplusTreeBytes Initialize(System.IO.Stream treefile, System.IO.Stream blockfile, int KeyLength) 
		{
			int CultureId = System.Globalization.CultureInfo.InvariantCulture.LCID;
			return Initialize(treefile, blockfile, KeyLength, CultureId, DEFAULTNODESIZE, DEFAULTBLOCKSIZE);
		}
		public static BplusTreeBytes ReOpen(System.IO.Stream treefile, System.IO.Stream blockfile) 
		{
			BplusTreeLong tree = BplusTreeLong.SetupFromExistingStream(treefile);
			LinkedFile archive = LinkedFile.SetupFromExistingStream(blockfile);
			return new BplusTreeBytes(tree, archive);
		}
		public static BplusTreeBytes ReOpen(string treefileName, string blockfileName, System.IO.FileAccess access) 
		{
			System.IO.Stream treefile = null;
			System.IO.Stream blockfile = null;

			try
			{
				treefile = new System.IO.FileStream(treefileName, System.IO.FileMode.Open, access);
				blockfile = new System.IO.FileStream(blockfileName, System.IO.FileMode.Open, access);

				return ReOpen(treefile, blockfile);
			}
			catch (System.Exception ex)
			{
				if(treefile != null)
					treefile.Dispose();

				if(blockfile != null)
					blockfile.Dispose();

				throw ex;
			}
		}
		public static BplusTreeBytes ReOpen(string treefileName, string blockfileName) 
		{
			return ReOpen(treefileName, blockfileName, System.IO.FileAccess.ReadWrite);
		}
		public static BplusTreeBytes ReadOnly(string treefileName, string blockfileName)
		{
			return ReOpen(treefileName, blockfileName, System.IO.FileAccess.Read);
		}

		/// <summary>
		/// Use non-culture sensitive total order on binary strings.
		/// </summary>
		public void NoCulture() 
		{
			this.tree.DontUseCulture = true;
			this.tree.cultureContext = null;
		}
		public int MaxKeyLength() 
		{
			return this.tree.MaxKeyLength();
		}
		#region ITreeIndex Members

		
		public int Compare(string left, string right) 
		{
			return this.tree.Compare(left, right);
		}
		public void Shutdown()
		{
			this.tree.Shutdown();
			this.archive.Shutdown();
		}

		public void Recover(bool CorrectErrors)
		{
			this.tree.Recover(CorrectErrors);
			Hashtable ChunksInUse = new Hashtable();
			string key = this.tree.FirstKey();
			while (key!=null) 
			{
				long buffernumber = this.tree[key];
				if (ChunksInUse.ContainsKey(buffernumber)) 
				{
					throw new BplusTreeException("buffer number "+buffernumber+" associated with more than one key '"
						+key+"' and '"+ChunksInUse[buffernumber]+"'");
				}
				ChunksInUse[buffernumber] = key;
				key = this.tree.NextKey(key);
			}
			// also consider the un-deallocated chunks to be in use
			foreach (DictionaryEntry thing in this.FreeChunksOnCommit) 
			{
				long buffernumber = (long) thing.Key;
				ChunksInUse[buffernumber] = "awaiting commit";
			}
			this.archive.Recover(ChunksInUse, CorrectErrors);
		}

		public void RemoveKey(string key)
		{
			long map = this.tree[key];
			//this.archive.ReleaseBuffers(map);
			//this.FreeChunksOnCommit.Add(map);
			if (this.FreeChunksOnAbort.ContainsKey(map)) 
			{
				// free it now
				this.FreeChunksOnAbort.Remove(map);
				this.archive.ReleaseBuffers(map);
			} 
			else 
			{
				// free when committed
				this.FreeChunksOnCommit[map] = map;
			}
			this.tree.RemoveKey(key);
		}

		public string FirstKey()
		{
			return this.tree.FirstKey();
		}

		public string NextKey(string AfterThisKey)
		{
			return this.tree.NextKey(AfterThisKey);
		}

		public bool ContainsKey(string key)
		{
			return this.tree.ContainsKey(key);
		}

		public object Get(string key, object defaultValue)
		{
			long map;
			if (this.tree.ContainsKey(key, out map)) 
			{
				return (object) this.archive.GetChunk(map);
			}
			return defaultValue;
		}

		public void Set(string key, object map)
		{
			if (!(map is byte[]) )
			{
				throw new BplusTreeBadKeyValue("BplusTreeBytes can only archive byte array as value");
			}
			byte[] thebytes = (byte[]) map;
			this[key] = thebytes;
		}
		public byte[] this[string key] 
		{
			set 
			{
				long storage = this.archive.StoreNewChunk(value, 0, value.Length);
				//this.FreeChunksOnAbort.Add(storage);
				this.FreeChunksOnAbort[storage] = storage;
				long valueFound;
				if (this.tree.ContainsKey(key, out valueFound)) 
				{
					//this.archive.ReleaseBuffers(valueFound);
					if (this.FreeChunksOnAbort.ContainsKey(valueFound)) 
					{
						// free it now
						this.FreeChunksOnAbort.Remove(valueFound);
						this.archive.ReleaseBuffers(valueFound);
					} 
					else 
					{
						// release at commit.
						this.FreeChunksOnCommit[valueFound] = valueFound;
					}
				}
				this.tree[key] = storage;
			}
			get 
			{
				long map = this.tree[key];
				return this.archive.GetChunk(map);
			}
		}

		public void Commit()
		{
			// store all new bufferrs
			this.archive.Flush();
			// commit the tree
			this.tree.Commit();
			// at this point the new buffers have been committed, now free the old ones
			//this.FreeChunksOnCommit.Sort();
			ArrayList toFree = new ArrayList();
			foreach (DictionaryEntry d in this.FreeChunksOnCommit) 
			{
				toFree.Add(d.Key);
			}
			toFree.Sort();
			toFree.Reverse();
			foreach (object thing in toFree) 
			{
				long chunknumber = (long) thing;
				this.archive.ReleaseBuffers(chunknumber);
			}
			this.archive.Flush();
			this.ClearBookKeeping();
		}

		public void Abort()
		{
			//this.FreeChunksOnAbort.Sort();
			ArrayList toFree = new ArrayList();
			foreach (DictionaryEntry d in this.FreeChunksOnAbort) 
			{
				toFree.Add(d.Key);
			}
			toFree.Sort();
			toFree.Reverse();
			foreach (object thing in toFree) 
			{
				long chunknumber = (long) thing;
				this.archive.ReleaseBuffers(chunknumber);
			}
			this.tree.Abort();
			this.archive.Flush();
			this.ClearBookKeeping();
		}
		
		public void SetFootPrintLimit(int limit) 
		{
			this.tree.SetFootPrintLimit(limit);
		}

		void ClearBookKeeping() 
		{
			this.FreeChunksOnCommit.Clear();
			this.FreeChunksOnAbort.Clear();
		}

		#endregion

		public string toHtml() 
		{
			string treehtml = this.tree.toHtml();
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append(treehtml);
			sb.Append("\r\n<br> free on commit "+this.FreeChunksOnCommit.Count+" ::");
			foreach (DictionaryEntry thing in this.FreeChunksOnCommit) 
			{
				sb.Append(" "+thing.Key);
			}
			sb.Append("\r\n<br> free on abort "+this.FreeChunksOnAbort.Count+" ::");
			foreach (DictionaryEntry thing in this.FreeChunksOnAbort) 
			{
				sb.Append(" "+thing.Key);
			}
			return sb.ToString(); // archive info not included
		}
	}
}
