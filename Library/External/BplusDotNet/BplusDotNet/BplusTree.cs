// The bplusdotnet package is Copywrite Aaron Watters 2004. 
// the package is licensed under the BSD open source license

using System;

namespace BplusDotNet
{
	/// <summary>
	/// Tree index mapping strings to strings.
	/// </summary>
	public class BplusTree : IStringTree
	{
		/// <summary>
		/// Internal tree mapping strings to bytes (for conversion to strings).
		/// </summary>
		public ITreeIndex tree;
		public BplusTree(ITreeIndex tree)
		{
			if (!(tree is BplusTreeBytes) && this.checkTree()) 
			{
				throw new BplusTreeException("Bplustree (superclass) can only wrap BplusTreeBytes, not other ITreeIndex implementations");
			}
			this.tree = tree;
		}
		
		protected virtual bool checkTree() 
		{
			// this is to prevent accidental misuse with the wrong ITreeIndex implementation,
			// but to also allow subclasses to override the behaviour... (there must be a better way...)
			return true;
		}

		public static BplusTree Initialize(string treefileName, string blockfileName, int KeyLength, int CultureId,
			int nodesize, int buffersize) 
		{
			BplusTreeBytes tree = BplusTreeBytes.Initialize(treefileName, blockfileName, KeyLength, CultureId, nodesize, buffersize);
			return new BplusTree(tree);
		}
		public static BplusTree Initialize(string treefileName, string blockfileName, int KeyLength, int CultureId) 
		{
			BplusTreeBytes tree = BplusTreeBytes.Initialize(treefileName, blockfileName, KeyLength, CultureId);
			return new BplusTree(tree);
		}
		public static BplusTree Initialize(string treefileName, string blockfileName, int KeyLength) 
		{
			BplusTreeBytes tree = BplusTreeBytes.Initialize(treefileName, blockfileName, KeyLength);
			return new BplusTree(tree);
		}
		
		public static BplusTree Initialize(System.IO.Stream treefile, System.IO.Stream blockfile, int KeyLength, int CultureId,
			int nodesize, int buffersize) 
		{
			BplusTreeBytes tree = BplusTreeBytes.Initialize(treefile, blockfile, KeyLength, CultureId, nodesize, buffersize);
			return new BplusTree(tree);
		}
		public static BplusTree Initialize(System.IO.Stream treefile, System.IO.Stream blockfile, int KeyLength, int CultureId) 
		{
			BplusTreeBytes tree = BplusTreeBytes.Initialize(treefile, blockfile, KeyLength, CultureId);
			return new BplusTree(tree);
		}
		public static BplusTree Initialize(System.IO.Stream treefile, System.IO.Stream blockfile, int KeyLength) 
		{
			BplusTreeBytes tree = BplusTreeBytes.Initialize(treefile, blockfile, KeyLength);
			return new BplusTree(tree);
		}
		
		public static BplusTree ReOpen(System.IO.Stream treefile, System.IO.Stream blockfile) 
		{
			BplusTreeBytes tree = BplusTreeBytes.ReOpen(treefile, blockfile);
			return new BplusTree(tree);
		}
		public static BplusTree ReOpen(string treefileName, string blockfileName) 
		{
			BplusTreeBytes tree = BplusTreeBytes.ReOpen(treefileName, blockfileName);
			return new BplusTree(tree);
		}
		public static BplusTree ReadOnly(string treefileName, string blockfileName) 
		{
			BplusTreeBytes tree = BplusTreeBytes.ReadOnly(treefileName, blockfileName);
			return new BplusTree(tree);
		}

		#region ITreeIndex Members

		public void Recover(bool CorrectErrors)
		{
			this.tree.Recover(CorrectErrors);
		}

		public void RemoveKey(string key)
		{
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
			object test = this.tree.Get(key, "");
			if (test is byte[]) 
			{
				return BytesToString((byte[]) test);
			}
			return defaultValue;
		}

		public void Set(string key, object map)
		{
			if (!(map is string)) 
			{
				throw new BplusTreeException("BplusTree only stores strings as values");
			}
			string thestring = (string) map;
			byte[] bytes = StringToBytes(thestring);
			//this.tree[key] = bytes;
			this.tree.Set(key, bytes);
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
		
		public int Compare(string left, string right) 
		{
			return this.tree.Compare(left, right);
		}

		#endregion
		public string this[string key] 
		{
			get 
			{
				object theGet = this.tree.Get(key, "");
				if (theGet is byte[]) 
				{
					byte[] bytes = (byte[]) theGet;
					return BytesToString(bytes);
				}
				//System.Diagnostics.Debug.WriteLine(this.toHtml());
				throw new BplusTreeKeyMissing("key not found "+key);
			} 
			set 
			{
				byte[] bytes = StringToBytes(value);
				//this.tree[key] = bytes;
				this.tree.Set(key, bytes);
			}
		}
		public static string BytesToString(byte[] bytes) 
		{
			System.Text.Decoder decode = System.Text.Encoding.UTF8.GetDecoder();
			long length = decode.GetCharCount(bytes, 0, bytes.Length);
			char[] chars = new char[length];
			decode.GetChars(bytes, 0, bytes.Length, chars, 0);
			string result = new String(chars);
			return result;
		}
		public static byte[] StringToBytes(string thestring) 
		{
			System.Text.Encoder encode = System.Text.Encoding.UTF8.GetEncoder();
			char[] chars = thestring.ToCharArray();
			long length = encode.GetByteCount(chars, 0, chars.Length, true);
			byte[] bytes = new byte[length];
			encode.GetBytes(chars, 0, chars.Length,bytes, 0, true);
			return bytes;
		}
		public virtual string toHtml() 
		{
			return ((BplusTreeBytes) this.tree).toHtml();
		}
	}
}
