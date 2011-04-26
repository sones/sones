// The bplusdotnet package is Copywrite Aaron Watters 2004. 
// the package is licensed under the BSD open source license

using System;

namespace BplusDotNet
{
	/// <summary>
	/// Tree index mapping strings to strings with unlimited key length
	/// </summary>
	public class xBplusTree : BplusTree
	{
		xBplusTreeBytes xtree;
		public xBplusTree(xBplusTreeBytes tree) : base(tree)
		{
			this.xtree = tree;
		}
		protected override bool checkTree()
		{
			return false;
		}
		public void LimitBucketSize(int limit) 
		{
			this.xtree.BucketSizeLimit = limit;
		}
		public static new xBplusTree Initialize(string treefileName, string blockfileName, int PrefixLength, int CultureId,
			int nodesize, int buffersize) 
		{
			xBplusTreeBytes tree = xBplusTreeBytes.Initialize(treefileName, blockfileName, PrefixLength, CultureId, nodesize, buffersize);
			return new xBplusTree(tree);
		}
		public static new xBplusTree Initialize(string treefileName, string blockfileName, int PrefixLength, int CultureId) 
		{
			xBplusTreeBytes tree = xBplusTreeBytes.Initialize(treefileName, blockfileName, PrefixLength, CultureId);
			return new xBplusTree(tree);
		}
		public static new xBplusTree Initialize(string treefileName, string blockfileName, int PrefixLength) 
		{
			xBplusTreeBytes tree = xBplusTreeBytes.Initialize(treefileName, blockfileName, PrefixLength);
			return new xBplusTree(tree);
		}
		
		public static new xBplusTree Initialize(System.IO.Stream treefile, System.IO.Stream blockfile, int PrefixLength, int CultureId,
			int nodesize, int buffersize) 
		{
			xBplusTreeBytes tree = xBplusTreeBytes.Initialize(treefile, blockfile, PrefixLength, CultureId, nodesize, buffersize);
			return new xBplusTree(tree);
		}
		public static new xBplusTree Initialize(System.IO.Stream treefile, System.IO.Stream blockfile, int PrefixLength, int CultureId) 
		{
			xBplusTreeBytes tree = xBplusTreeBytes.Initialize(treefile, blockfile, PrefixLength, CultureId);
			return new xBplusTree(tree);
		}
		public static new xBplusTree Initialize(System.IO.Stream treefile, System.IO.Stream blockfile, int KeyLength) 
		{
			xBplusTreeBytes tree = xBplusTreeBytes.Initialize(treefile, blockfile, KeyLength);
			return new xBplusTree(tree);
		}
		
		public static new xBplusTree ReOpen(System.IO.Stream treefile, System.IO.Stream blockfile) 
		{
			xBplusTreeBytes tree = xBplusTreeBytes.ReOpen(treefile, blockfile);
			return new xBplusTree(tree);
		}
		public static new xBplusTree ReOpen(string treefileName, string blockfileName) 
		{
			xBplusTreeBytes tree = xBplusTreeBytes.ReOpen(treefileName, blockfileName);
			return new xBplusTree(tree);
		}
		public static new xBplusTree ReadOnly(string treefileName, string blockfileName) 
		{
			xBplusTreeBytes tree = xBplusTreeBytes.ReadOnly(treefileName, blockfileName);
			return new xBplusTree(tree);
		}
	}
}
