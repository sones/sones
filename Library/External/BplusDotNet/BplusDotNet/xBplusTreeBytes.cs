using System;
using System.Collections;

namespace BplusDotNet
{
	/// <summary>
	/// Bplustree with unlimited length strings (but only a fixed prefix is indexed in the tree directly).
	/// </summary>
	public class xBplusTreeBytes : IByteTree
	{
		public BplusTreeBytes tree;
		public int prefixLength;
		public int BucketSizeLimit = -1;
		public xBplusTreeBytes(BplusTreeBytes tree, int prefixLength)
		{
			if (prefixLength<3) 
			{
				throw new BplusTreeException("prefix cannot be smaller than 3 :: "+prefixLength); 
			}
			if (prefixLength>tree.MaxKeyLength()) 
			{
				throw new BplusTreeException("prefix length cannot exceed keylength for internal tree");
			}
			this.tree = tree;
			this.prefixLength = prefixLength;
		}
		public void LimitBucketSize(int limit) 
		{
			this.BucketSizeLimit = limit;
		}
		public static xBplusTreeBytes Initialize(string treefileName, string blockfileName, int PrefixLength, int CultureId,
			int nodesize, int buffersize) 
		{
			return new xBplusTreeBytes(
				BplusTreeBytes.Initialize(treefileName, blockfileName, PrefixLength, CultureId, nodesize, buffersize),
				PrefixLength);
		}
		public static xBplusTreeBytes Initialize(string treefileName, string blockfileName, int PrefixLength, int CultureId) 
		{
			return new xBplusTreeBytes(
				BplusTreeBytes.Initialize(treefileName, blockfileName, PrefixLength, CultureId),
				PrefixLength);
		}
		public static xBplusTreeBytes Initialize(string treefileName, string blockfileName, int PrefixLength) 
		{
			return new xBplusTreeBytes(
				BplusTreeBytes.Initialize(treefileName, blockfileName, PrefixLength),
				PrefixLength);
		}
		public static xBplusTreeBytes Initialize(System.IO.Stream treefile, System.IO.Stream blockfile, int PrefixLength, int CultureId,
			int nodesize, int buffersize) 
		{
			return new xBplusTreeBytes(
				BplusTreeBytes.Initialize(treefile, blockfile, PrefixLength, CultureId, nodesize, buffersize),
				PrefixLength);
		}
		public static xBplusTreeBytes Initialize(System.IO.Stream treefile, System.IO.Stream blockfile, int PrefixLength, int CultureId) 
		{
			return new xBplusTreeBytes(
				BplusTreeBytes.Initialize(treefile, blockfile, PrefixLength, CultureId),
				PrefixLength);
		}
		public static xBplusTreeBytes Initialize(System.IO.Stream treefile, System.IO.Stream blockfile, int PrefixLength) 
		{
			return new xBplusTreeBytes(
				BplusTreeBytes.Initialize(treefile, blockfile, PrefixLength),
				PrefixLength);
		}

		public static xBplusTreeBytes ReOpen(System.IO.Stream treefile, System.IO.Stream blockfile) 
		{
			BplusTreeBytes tree = BplusTreeBytes.ReOpen(treefile, blockfile);
			int prefixLength = tree.MaxKeyLength();
			return new xBplusTreeBytes(tree, prefixLength);
		}
		public static xBplusTreeBytes ReOpen(string treefileName, string blockfileName) 
		{
			BplusTreeBytes tree = BplusTreeBytes.ReOpen(treefileName, blockfileName);
			int prefixLength = tree.MaxKeyLength();
			return new xBplusTreeBytes(tree, prefixLength);
		}
		public static xBplusTreeBytes ReadOnly(string treefileName, string blockfileName) 
		{
			BplusTreeBytes tree = BplusTreeBytes.ReadOnly(treefileName, blockfileName);
			int prefixLength = tree.MaxKeyLength();
			return new xBplusTreeBytes(tree, prefixLength);
		}

		public virtual string PrefixForByteCount(string s, int maxbytecount) 
		{
			if (s.Length<1) 
			{
				return "";
			}
			int prefixcharcount = maxbytecount;
			if (prefixcharcount>s.Length) 
			{
				prefixcharcount = s.Length;
			}
			System.Text.Encoder encode = System.Text.Encoding.UTF8.GetEncoder();
			char[] chars = s.ToCharArray(0, prefixcharcount);
			long length = encode.GetByteCount(chars, 0, prefixcharcount, true);
			while (length>maxbytecount) 
			{
				prefixcharcount--;
				length = encode.GetByteCount(chars, 0, prefixcharcount, true);
			}
			return s.Substring(0, prefixcharcount);
		}
		public bool FindBucketForPrefix(string key, out xBucket bucket, out string prefix, bool keyIsPrefix) 
		{
			bucket = null;
			prefix = key;
			if (!keyIsPrefix) 
			{
				prefix = PrefixForByteCount(key, this.prefixLength);
			}
			object datathing = this.tree.Get(prefix, "");
			if (datathing is byte[]) 
			{
				byte[] databytes = (byte[]) datathing;
				bucket = new xBucket(this);
				bucket.Load(databytes);
				if (bucket.Count()<1) 
				{
					throw new BplusTreeException("empty bucket loaded");
				}
				return true;
			}
			return false; // default
		}

		
		#region ITreeIndex Members

		
		public int Compare(string left, string right) 
		{
			return this.tree.Compare(left, right);
		}

		public void Recover(bool CorrectErrors)
		{
			this.tree.Recover(CorrectErrors);
		}

		public void RemoveKey(string key)
		{
			xBucket bucket;
			string prefix;
			bool found = FindBucketForPrefix(key, out bucket, out prefix, false);
			if (!found) 
			{
				throw new BplusTreeKeyMissing("no such key to delete");
			}
			bucket.Remove(key);
			if (bucket.Count()<1) 
			{
				this.tree.RemoveKey(prefix);
			} 
			else 
			{
				this.tree[prefix] = bucket.dump();
			}
		}

		public string FirstKey()
		{
			xBucket bucket;
			string prefix = this.tree.FirstKey();
			if (prefix==null) 
			{
				return null;
			}
			string dummyprefix;
			bool found = FindBucketForPrefix(prefix, out bucket, out dummyprefix, true);
			if (!found) 
			{
				throw new BplusTreeException("internal tree gave bad first key");
			}
			return bucket.FirstKey();
		}

		public string NextKey(string AfterThisKey)
		{
			xBucket bucket;
			string prefix;
			string result = null;
			bool found = FindBucketForPrefix(AfterThisKey, out bucket, out prefix, false);
			if (found) 
			{
				result = bucket.NextKey(AfterThisKey);
				if (result!=null) 
				{
					return result;
				}
			}
			// otherwise look in the next bucket
			string nextprefix = this.tree.NextKey(prefix);
			if (nextprefix==null) 
			{
				return null;
			}
			byte[] databytes = this.tree[nextprefix];
			bucket = new xBucket(this);
			bucket.Load(databytes);
			if (bucket.Count()<1) 
			{
				throw new BplusTreeException("empty bucket loaded");
			}
			return bucket.FirstKey();
		}

		public bool ContainsKey(string key)
		{
			xBucket bucket;
			string prefix;
			bool found = FindBucketForPrefix(key, out bucket, out prefix, false);
			if (!found) 
			{
				return false;
			}
			byte[] map;
			return bucket.Find(key, out map);
		}

		public object Get(string key, object defaultValue)
		{
			xBucket bucket;
			string prefix;
			bool found = FindBucketForPrefix(key, out bucket, out prefix, false);
			if (!found) 
			{
				return defaultValue;
			}
			byte[] map;
			found = bucket.Find(key, out map);
			if (found) 
			{
				return map;
			}
			return defaultValue;
		}

		public void Set(string key, object map)
		{
			
			xBucket bucket;
			string prefix;
			bool found = FindBucketForPrefix(key, out bucket, out prefix, false);
			if (!found) 
			{
				bucket = new xBucket(this);
			}
			if (!(map is byte[])) 
			{
				throw new BplusTreeBadKeyValue("xBplus only accepts byte array values");
			}
			bucket.Add(key, (byte[]) map);
			this.tree[prefix] = bucket.dump();
		}
		public byte[] this[string key] 
		{
			get 
			{
				object test = this.Get(key, "");
				if (test is byte[]) 
				{
					return (byte[]) test;
				}
				throw new BplusTreeKeyMissing("no such key in tree");
			} 
			set 
			{
				this.Set(key, value);
			}
		}

		public void Commit()
		{
			this.tree.Commit();
		}

		public void Abort()
		{
			this.tree.Abort();
		}

		public void SetFootPrintLimit(int limit)
		{
			this.tree.SetFootPrintLimit(limit);
		}

		public void Shutdown()
		{
			this.tree.Shutdown();
		}

		#endregion
	}
	/// <summary>
	/// Bucket for elements with same prefix -- designed for small buckets.
	/// </summary>
	public class xBucket 
	{
		ArrayList keys;
		ArrayList values;
		xBplusTreeBytes owner;
		public xBucket(xBplusTreeBytes owner) 
		{
			this.keys = new ArrayList();
			this.values = new ArrayList();
			this.owner = owner;
		}
		public int Count() 
		{
			return this.keys.Count;
		}
		public void Load(byte[] serialization) 
		{
			int index = 0;
			int byteCount = serialization.Length;
			if (this.values.Count!=0 || this.keys.Count!=0) 
			{
				throw new BplusTreeException("load into nonempty xBucket not permitted");
			}
			while (index<byteCount) 
			{
				// get key prefix and key
				int keylength = BufferFile.Retrieve(serialization, index);
				index += BufferFile.INTSTORAGE;
				byte[] keybytes = new byte[keylength];
				Array.Copy(serialization, index, keybytes, 0, keylength);
				string keystring = BplusTree.BytesToString(keybytes);
				index+= keylength;
				// get value prefix and value
				int valuelength = BufferFile.Retrieve(serialization, index);
				index += BufferFile.INTSTORAGE;
				byte[] valuebytes = new byte[valuelength];
				Array.Copy(serialization, index, valuebytes, 0, valuelength);
				// record new key and value
				this.keys.Add(keystring);
				this.values.Add(valuebytes);
				index+= valuelength;
			}
			if (index!=byteCount) 
			{
				throw new BplusTreeException("bad byte count in serialization "+byteCount);
			}
		}
		public byte[] dump() 
		{
			ArrayList allbytes = new ArrayList();
			int byteCount = 0;
			for (int index=0; index<this.keys.Count; index++) 
			{
				string thisKey = (string) this.keys[index];
				byte[] thisValue = (byte[]) this.values[index];
				byte[] keyprefix = new byte[BufferFile.INTSTORAGE];
				byte[] keybytes = BplusTree.StringToBytes(thisKey);
				BufferFile.Store(keybytes.Length, keyprefix, 0);
				allbytes.Add(keyprefix);
				allbytes.Add(keybytes);
				byte[] valueprefix = new byte[BufferFile.INTSTORAGE];
				BufferFile.Store(thisValue.Length, valueprefix, 0);
				allbytes.Add(valueprefix);
				allbytes.Add(thisValue);
			}
			foreach (object thing in allbytes) 
			{
				byte[] thebytes = (byte[]) thing;
				byteCount+= thebytes.Length;
			}
			int outindex=0;
			byte[] result = new byte[byteCount];
			foreach (object thing in allbytes) 
			{
				byte[] thebytes = (byte[]) thing;
				int thelength = thebytes.Length;
				Array.Copy(thebytes, 0, result, outindex, thelength);
				outindex+= thelength;
			}
			if (outindex!=byteCount) 
			{
				throw new BplusTreeException("error counting bytes in dump "+outindex+"!="+byteCount);
			}
			return result;
		}
		public void Add(string key, byte[] map) 
		{
			int index = 0;
			int limit = this.owner.BucketSizeLimit;
			while (index<this.keys.Count) 
			{
				string thiskey = (string) this.keys[index];
				int comparison = this.owner.Compare(thiskey, key);
				if (comparison==0) 
				{
					this.values[index] = map;
					this.keys[index] = key;
					return;
				}
				if (comparison>0) 
				{
					this.values.Insert(index, map);
					this.keys.Insert(index, key);
					if (limit>0 && this.keys.Count>limit) 
					{
						throw new BplusTreeBadKeyValue("bucket size limit exceeded");
					}
					return;
				}
				index++;
			}
			this.keys.Add(key);
			this.values.Add(map);
			if (limit>0 && this.keys.Count>limit) 
			{
				throw new BplusTreeBadKeyValue("bucket size limit exceeded");
			}
		}
		public void Remove(string key) 
		{
			int index = 0;
			while (index<this.keys.Count) 
			{
				string thiskey = (string) this.keys[index];
				if (this.owner.Compare(thiskey, key)==0) 
				{
					this.values.RemoveAt(index);
					this.keys.RemoveAt(index);
					return;
				}
				index++;
			}
			throw new BplusTreeBadKeyValue("cannot remove missing key: "+key);
		}
		public bool Find(string key, out byte[] map) 
		{
			map = null;
			int index = 0;
			while (index<this.keys.Count) 
			{
				string thiskey = (string) this.keys[index];
				if (this.owner.Compare(thiskey, key)==0) 
				{
					map = (byte[]) this.values[index];
					return true;
				}
				index++;
			}
			return false;
		}
		public string FirstKey() 
		{
			if (this.keys.Count<1) 
			{
				return null;
			}
			return (string) this.keys[0];
		}
		public string NextKey(string AfterThisKey) 
		{
			int index = 0;
			while (index<this.keys.Count) 
			{
				string thiskey = (string) this.keys[index];
				if (this.owner.Compare(thiskey, AfterThisKey)>0) 
				{
					return thiskey;
				}
				index++;
			}
			return null;
		}
	}
}
