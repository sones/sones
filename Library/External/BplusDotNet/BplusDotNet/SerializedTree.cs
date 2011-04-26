// The bplusdotnet package is Copywrite Aaron Watters 2004. 
// the package is licensed under the BSD open source license

using System;
using System.Runtime.Serialization.Formatters.Binary;

namespace BplusDotNet
{
	/// <summary>
	/// Wrapper for any IByteTree implementation which implements automatic object serialization/deserialization
	/// for serializable objects.
	/// </summary>
	public class SerializedTree : IObjectTree, ITreeIndex
	{
		IByteTree tree;
		BinaryFormatter formatter = new BinaryFormatter();
		public SerializedTree(IByteTree tree)
		{
			this.tree = tree;
		}
		#region IObjectTree Members

		public object this[string key]
		{
			get
			{
				byte[] bytes = this.tree[key];
				System.IO.Stream bstream = new System.IO.MemoryStream(bytes);
				object result = formatter.Deserialize(bstream);
				return result;
			}
			set
			{
				System.IO.MemoryStream bstream = new System.IO.MemoryStream();
				formatter.Serialize(bstream, value);
				byte[] bytes = bstream.ToArray();
				this.tree[key] = bytes;
			}
		}

		#endregion

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
			if (this.tree.ContainsKey(key)) 
			{
				return this[key];
			}
			return defaultValue;
		}

		public void Set(string key, object map)
		{
			this[key] = map;
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
	}
}
