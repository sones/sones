using System;

namespace BplusDotNet
{
	/// <summary>
	/// Tree index mapping strings to strings with unlimited key length
	/// </summary>
	public class hBplusTree : BplusTree
	{
		hBplusTreeBytes xtree;
		public hBplusTree(hBplusTreeBytes tree) : base(tree)
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
		public static new hBplusTree Initialize(string treefileName, string blockfileName, int PrefixLength, int CultureId,
			int nodesize, int buffersize) 
		{
			hBplusTreeBytes tree = hBplusTreeBytes.Initialize(treefileName, blockfileName, PrefixLength, CultureId, nodesize, buffersize);
			return new hBplusTree(tree);
		}
		public static new hBplusTree Initialize(string treefileName, string blockfileName, int PrefixLength, int CultureId) 
		{
			hBplusTreeBytes tree = hBplusTreeBytes.Initialize(treefileName, blockfileName, PrefixLength, CultureId);
			return new hBplusTree(tree);
		}
		public static new hBplusTree Initialize(string treefileName, string blockfileName, int PrefixLength) 
		{
			hBplusTreeBytes tree = hBplusTreeBytes.Initialize(treefileName, blockfileName, PrefixLength);
			return new hBplusTree(tree);
		}
		
		public static new hBplusTree Initialize(System.IO.Stream treefile, System.IO.Stream blockfile, int PrefixLength, int CultureId,
			int nodesize, int buffersize) 
		{
			hBplusTreeBytes tree = hBplusTreeBytes.Initialize(treefile, blockfile, PrefixLength, CultureId, nodesize, buffersize);
			return new hBplusTree(tree);
		}
		public static new hBplusTree Initialize(System.IO.Stream treefile, System.IO.Stream blockfile, int PrefixLength, int CultureId) 
		{
			hBplusTreeBytes tree = hBplusTreeBytes.Initialize(treefile, blockfile, PrefixLength, CultureId);
			return new hBplusTree(tree);
		}
		public static new hBplusTree Initialize(System.IO.Stream treefile, System.IO.Stream blockfile, int KeyLength) 
		{
			hBplusTreeBytes tree = hBplusTreeBytes.Initialize(treefile, blockfile, KeyLength);
			return new hBplusTree(tree);
		}
		
		public static new hBplusTree ReOpen(System.IO.Stream treefile, System.IO.Stream blockfile) 
		{
			hBplusTreeBytes tree = hBplusTreeBytes.ReOpen(treefile, blockfile);
			return new hBplusTree(tree);
		}
		public static new hBplusTree ReOpen(string treefileName, string blockfileName) 
		{
			hBplusTreeBytes tree = hBplusTreeBytes.ReOpen(treefileName, blockfileName);
			return new hBplusTree(tree);
		}
		public static new hBplusTree ReadOnly(string treefileName, string blockfileName) 
		{
			hBplusTreeBytes tree = hBplusTreeBytes.ReadOnly(treefileName, blockfileName);
			return new hBplusTree(tree);
		}
		public override string toHtml() 
		{
			return ((hBplusTreeBytes) this.tree).toHtml();
		}
	}
}
