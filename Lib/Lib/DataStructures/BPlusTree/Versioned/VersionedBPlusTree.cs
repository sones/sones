/* <id name=”Libraries Datastructures – VersionedBPlusTree” />
 * <copyright file=”VersionedBPlusTree.cs”
 *            company=”sones GmbH”>
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Martin Junghanns</developer>
 */ 

#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using sones.Lib.DataStructures.Indices;

#endregion

namespace sones.Lib.DataStructures.BPlusTree.Versioned
{
    public class VersionedBPlusTree<TKey, TValue> : IVersionedBPlusTree<TKey, TValue>, IEstimable
        where TKey : IComparable, IEstimable
        where TValue : IEstimable
    {
        #region private members

        /// <summary>
        /// The root node of the b-tree.
        /// </summary>
        private VersionedNode<TKey, TValue> _Root;

        /// <summary>
        /// The order k of the b-tree
        /// * every node contains at max 2k elements
        /// * every node except the root has at least k elements
        /// * every node is a leaf and has no successor, or it's an "inner-node" and has i + 1 successor if i is the element count        
        /// </summary>
        private Int32 _Order;

        /// <summary>
        /// The Height of the b-tree.
        /// </summary>
        private Int32 _Height;

        /// <summary>
        /// If a b-tree has an order k there can be at max 2k elements per node.
        /// </summary>
        private Int32 _MaxKeysPerNode;

        /// <summary>
        /// If a b-tree has an order k there must not be less then k elements per node.
        /// </summary>
        private Int32 _MinKeysPerNode;

        /// <summary>
        /// If a b-tree has an order k there can be at max 2k + 1 children per node.
        /// </summary>
        private Int32 _MaxChildrenPerNode;

        /// <summary>
        /// If a tree has an order k there must not be leass then k + 1 children per node.
        /// </summary>
        private Int32 _MinChildrenPerNode;

        /// <summary>
        /// The number of values of the latest version in the b-tree
        /// </summary>
        private ulong _Count;

        /// <summary>
        /// The number of keys if the latest version in the b-tree
        /// </summary>
        private ulong _KeyCount;

        /// <summary>
        /// The number of revisions to store in the leafs.
        /// </summary>
        private UInt64 _HistorySize;

        #endregion private members

        #region internal members

        /// <summary>
        /// Holds a reference to the left-most leaf. 
        /// </summary>
        internal VersionedLeafNode<TKey, TValue> _LeftMostLeaf;

        /// <summary>
        /// Holds a reference to the right-most leaf. 
        /// </summary>
        internal VersionedLeafNode<TKey, TValue> _RightMostLeaf;

        #endregion

        #region constructors

        /// <summary>
        /// Creates a b-tree with a default order of k = 60
        /// </summary>
        public VersionedBPlusTree()
            : this(60)
        {
        }
        
        /// <summary>
        /// Instantiates a b-tree with a given order k
        /// </summary>
        /// <param name="myOrder"></param>
        public VersionedBPlusTree(Int32 myOrder)
        {
            #region illegal argument exception

            if (myOrder < 1)
            {
                throw new ArgumentException("minimum order is 2");
            }

            #endregion

            #region tree settings

            _Order              = myOrder;
            _KeyCount           = 0;
            _Count              = 0;
            _Height             = 0;
            _MaxKeysPerNode     = (2 * myOrder);
            _MinKeysPerNode     = myOrder;
            _MaxChildrenPerNode = (2 * myOrder) + 1;
            _MinChildrenPerNode = myOrder + 1;
            _HistorySize        = 3;

            InitRootNode();

            #endregion
        }

        #endregion

        #region getter / setter

        /// <summary>
        /// Returns the number of values of the latest version in the b-tree
        /// </summary>
        public ulong Count
        {
            get { return _Count; }
            set { _Count = value; }
        }

        /// <summary>
        /// Returns the number of keys of the latest version in the tree
        /// </summary>
        public ulong KeyCount
        {
            get { return _KeyCount; }
            set { _KeyCount = value; }
        }

        /// <summary>
        /// Returns the Order of the tree
        /// </summary>
        public Int32 Order
        {
            get { return _Order; }
        }

        /// <summary>
        /// Returns the current Height of the BStarTree
        /// </summary>
        public Int32 Height
        {
            get { return _Height; }
        }

        /// <summary>
        /// Returns the maximum number of keys per node (2k)
        /// </summary>
        public Int32 MaxKeysPerNode
        {
            get { return _MaxKeysPerNode; }
        }

        /// <summary>
        /// Returns the minimum number of keys per node (k)
        /// </summary>
        public Int32 MinKeysPerNode
        {
            get { return _MinKeysPerNode; }
        }

        /// <summary>
        /// Returns the MaxChildrenPerNode.. (2k + 1)
        /// </summary>
        public Int32 MaxChildrenPerNode
        {
            get { return _MaxChildrenPerNode; }
        }

        /// <summary>
        /// Returns the minimum number of children per node.. (k + 1)
        /// </summary>
        public Int32 MinChildrenPerNode
        {
            get { return _MinChildrenPerNode; }
        }

        /// <summary>
        /// Returns the number of revisions to store in the leafs.
        /// </summary>
        public UInt64 HistorySize
        {
            get
            {
                return _HistorySize;
            }
            set
            {
                //TODO: _history size truncate
                _HistorySize = value;
            }
        }

        #endregion

        #region Internal Members

        /// <summary>
        /// Checks if the tree contains the key
        /// </summary>
        /// <param name="myKey">TKey</param>
        /// <returns>true if tree contains key</returns>
        public Trinary ContainsKey(TKey myKey)
        {
            return ContainsKey(myKey, 0);
        }

        /// <summary>
        /// Get all Keys in the tree.
        /// </summary>
        public IEnumerable<TKey> Keys
        {
            get
            {
                return GetAllKeys();
            }
        }

        /// <summary>
        /// Remove a Key and all associated Values from the tree.
        /// </summary>
        /// <param name="key">TKey</param>
        /// <returns>true if deleted</returns>
        public bool Remove(TKey myKey)
        {
            IndexValueHistoryList<TValue> indexValueHistoryList;

            if(TryGetValue(myKey, out indexValueHistoryList, 0))
            {
                //decrement tree's value count
                _Count -= (ulong)indexValueHistoryList.Values.Count;
                //decrement tree's key count (of the latest revision)
                _KeyCount--;
                //mark latest revision as deleted (creates new revision)
                indexValueHistoryList.isLatestDeleted = true;
                //clean up revision history
                while (indexValueHistoryList.VersionCount > _HistorySize)
                {
                    indexValueHistoryList.RemoveLatestFromHistory();
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns all values of the tree as ICollection
        /// </summary>
        /// <returns>All values in the tree</returns>
        public IEnumerable<TValue> Values
        {
            get
            {
                return GetAllVersionedValues(0);
            }
        }

        /// <summary>
        /// Checks if the tree is initialized
        /// </summary>
        /// <returns></returns>
        private bool IsTreeInitialized()
        {
            return _Root != null;
        }

        /// <summary>
        /// Adds a key and it's value into the tree.
        /// If the key already exists, the value is added to the key's associated value-hashset.
        /// myKey must implement IComparable
        /// </summary>
        /// <param name="myKey">The Key to be added</param>
        /// <param name="myValue">The Value associated to the key</param>
        private void AddInternal(TKey myKey, HashSet<TValue> myValue, IndexSetStrategy myIndexSetStrategy)
        {
            //used to handle split information
            VersionedSplitInfo<TKey, TValue> splitInfo = new VersionedSplitInfo<TKey, TValue>();

            //insert into the tree
            if (_Root.Insert(myKey, myValue, ref splitInfo, myIndexSetStrategy))
            {
                //root was splitted we have to create a new one
                //create the fist inner node
                VersionedInnerNode<TKey, TValue> innerNode = new VersionedInnerNode<TKey, TValue>(this);

                //first key in the root is the propagad
                innerNode.Keys[0]       = splitInfo.Key;
                //left child is old root
                innerNode.Children[0]   = _Root;
                //right child is new splitted node
                innerNode.Children[1] = splitInfo.Node;
                //left childs parent is new root
                innerNode.Children[0].Parent = innerNode;
                //right childs parent is new root
                innerNode.Children[1].Parent = innerNode;
                //and update the keyCount
                innerNode.KeyCount++;
                
                //set the new inner node as root node
                _Root           = innerNode;

                //increment the height of the tree
                _Height++;
            }
        }

        private void AddInternal(TKey myKey, IndexValueHistoryList<TValue> myValue)
        {
            //used to handle split information
            VersionedSplitInfo<TKey, TValue> splitInfo = new VersionedSplitInfo<TKey, TValue>();

            //insert into the tree
            if (_Root.Insert(myKey, myValue, ref splitInfo))
            {
                //root was splitted we have to create a new one
                //create the fist inner node
                VersionedInnerNode<TKey, TValue> innerNode = new VersionedInnerNode<TKey, TValue>(this);

                //first key in the root is the propagad
                innerNode.Keys[0] = splitInfo.Key;
                //left child is old root
                innerNode.Children[0] = _Root;
                //right child is new splitted node
                innerNode.Children[1] = splitInfo.Node;
                //left childs parent is new root
                innerNode.Children[0].Parent = innerNode;
                //right childs parent is new root
                innerNode.Children[1].Parent = innerNode;
                //and update the keyCount
                innerNode.KeyCount++;

                //set the new inner node as root node
                _Root = innerNode;

                //increment the height of the tree
                _Height++;
            }
        }

        /// <summary>
        /// Recursive search for myKey. If the search-node is internal, it searches for smaller for the right child
        /// and continues search. If the method reaches a leaf node it checks, if the key is contained in that node.
        /// </summary>
        /// <param name="myNode">The node which shall be searched.</param>
        /// <param name="myKey">They key to search for.</param>
        /// <returns>True if the node contains that key.</returns>
        private Trinary ContainsKeyInternal(VersionedNode<TKey, TValue> myNode, TKey myKey, long myVersion)
        {
            //make some room to store the leaf associated to the key
            VersionedLeafNode<TKey, TValue> leaf;

            return ContainsKeyInternal(myNode, myKey, out leaf, myVersion);
                
        }

        /// <summary>
        /// Recursive search for myKey. If the search-node is internal, it searches for smaller for the right child
        /// and continues search. If the method reaches a leaf node it checks, if the key is contained in that node.
        /// 
        /// Versioned:
        /// A Key can have 3 states
        /// - exists  -> key AND a value with the given version exists
        /// - !exists -> key doesn't exist OR key exists but no value with the given version
        /// - deleted -> the key has been deleted (flag)
        /// 
        /// If the key is found in the tree, a reference to the corresponding leaf node is stored in an extra reference variable
        /// </summary>
        /// <param name="myNode">The node which shall be searched.</param>
        /// <param name="myKey">They key to search for.</param>
        /// <param name="myCorrespondingLeafNode">A reference to the leaf node where the key is located</param>
        /// <returns>True if the node contains that key.</returns>
        private Trinary ContainsKeyInternal(VersionedNode<TKey, TValue> myNode, TKey myKey, out VersionedLeafNode<TKey, TValue> myCorrespondingLeafNode, long myVersion)
        {
            if (myNode is VersionedLeafNode<TKey, TValue>) //we reached the bottom of the tree
            {
                //store the leaf node
                myCorrespondingLeafNode = myNode as VersionedLeafNode<TKey, TValue>;
                //search the leaf Keys for myKey using binary search between 0 and the number of keys contained in that node
                int index;
                if (myNode.ContainsKey(myKey, out index))
                {
                    var indexValueHistoryList = myCorrespondingLeafNode.IndexValueHistoryLists[index][myVersion];

                    //check if version exists
                    if (indexValueHistoryList != null)
                    {
                        //the values in this revision are deleted
                        if(indexValueHistoryList.Count == 0) 
                        {
                            return Trinary.DELETED;
                        }
                        else
                        {
                            //exists
                            return Trinary.TRUE;
                        }
                    }
                }
                else
                {
                    //doesn't exist
                    return Trinary.FALSE;
                }
            }
            //we are at an inner node and search for a way to go
            int nextChildIndex = myNode.FindSlot(myKey) + 1;
            //go recursively to the next child node
            return ContainsKeyInternal(((VersionedInnerNode<TKey, TValue>)myNode).Children[nextChildIndex], myKey, out myCorrespondingLeafNode, myVersion);
        }

        /// <summary>
        /// Initializes the tree.
        /// </summary>
        private void InitRootNode()
        {
            #region node

            _Root = new VersionedLeafNode<TKey, TValue>(this);
            _Root.Parent    = null;
            _Root.KeyCount  = 0;

            #endregion

            #region tree

            this._LeftMostLeaf  = _Root as VersionedLeafNode<TKey, TValue>;
            this._RightMostLeaf = _Root as VersionedLeafNode<TKey, TValue>;
            this._Height        = 1;

            #endregion
        }

        /// <summary>
        /// starts with the left-most leaf and adds all keys to a list, until the leaf has no right sibling
        /// </summary>
        /// <returns>a (sorted) collection which contains all keys of the tree</returns>
        private IEnumerable<TKey> GetAllKeys()
        {
            return GetAllVersionedKeys(0);
        }

        /// <summary>
        /// starts with the left-most leaf and adds all keys to a list, until the leaf has no right sibling
        /// </summary>
        /// <returns>a (sorted) collection which contains all keys of the tree</returns>
        private IEnumerable<TKey> GetAllVersionedKeys(long myVersion)
        {
            //used to make one's way hand over hand along leafs
            VersionedLeafNode<TKey, TValue> tmpLeaf = _LeftMostLeaf;

            //still a right sibling
            while (tmpLeaf != null)
            {
                //add all keys of the node
                for (Int32 i = 0; i < tmpLeaf.KeyCount; i++)
                {
                    //exisiting and not deleted
                    if (tmpLeaf.IndexValueHistoryLists[i][myVersion] != null && tmpLeaf.IndexValueHistoryLists[i][myVersion].Count > 0)
                    {
                        yield return tmpLeaf.Keys[i];
                    }
                }
                tmpLeaf = tmpLeaf.RightSibling as VersionedLeafNode<TKey, TValue>;
            }
        }

        /// <summary>
        /// starts with the left-most leaf and yield returns all values associated to the keys
        /// </summary>
        /// <returns></returns>
        private IEnumerable<TValue> GetAllVersionedValues(long myVersion)
        {
            VersionedLeafNode<TKey, TValue> tmpLeaf = _LeftMostLeaf;

            while (tmpLeaf != null)
            {
                for (Int32 i = 0; i < tmpLeaf.KeyCount; i++)
                {
                    if (tmpLeaf.IndexValueHistoryLists[i][myVersion] != null)
                    {
                        var values = tmpLeaf.IndexValueHistoryLists[i][myVersion];

                        foreach (TValue val in values)
                        {
                            yield return val;
                        }
                    }
                }
                tmpLeaf = tmpLeaf.RightSibling as VersionedLeafNode<TKey, TValue>;
            }
        }

        /// <summary>
        /// Method checks if myKey exists in the tree. If it exists, the associated HashSet of TValues
        /// is assigned to the out param.
        /// </summary>
        /// <param name="myKey">the key which values shall be loaded</param>
        /// <param name="myValues">reference which holds the value HashSet or null if the key doesn't exist</param>
        /// <returns>
        /// - true if the key exists and the values are assigned to the out param
        /// - false if the key doesn't exist, out param myValues is null
        /// </returns>
        public bool TryGetValue(TKey myKey, out HashSet<TValue> myValues)
        {
            return TryGetValue(myKey, out myValues, 0);
        }

        /// <summary>
        /// Method checks if myKey exists in the tree. If it exists, the associated HashSet of TValues
        /// is assigned to the out param.
        /// </summary>
        /// <param name="myKey">the key which values shall be loaded</param>
        /// <param name="myValues">reference which holds the value HashSet or null if the key doesn't exist</param>
        /// <returns>
        /// - true if the key exists and the values are assigned to the out param
        /// - false if the key doesn't exist, out param myValues is null
        /// </returns>
        public Trinary TryGetValue(TKey myKey, out IndexValueHistoryList<TValue> myIndexValueHistoryList, long myVersion)
        {
            //used to store the leaf which contains the key
            VersionedLeafNode<TKey, TValue> leaf;

            //check if key exists or is revision has been deleted
            var res = ContainsKeyInternal(_Root, myKey, out leaf, myVersion);

            //it doesn't matter if the values are deleted at that version because we need the complete history
            if (res == Trinary.TRUE || res == Trinary.DELETED)
            {
                var index = Array.BinarySearch<TKey>(leaf.Keys, 0, leaf.KeyCount, myKey);

                //get the indexValueHistoryList
                myIndexValueHistoryList = leaf.IndexValueHistoryLists[index];

                return res;
            }
            //not found
            myIndexValueHistoryList = new IndexValueHistoryList<TValue>();
            return res;
        }

        #endregion

        #region IIndexInterface<TKey, TValue> members

        public string IndexName
        {
            get { return "BPlusTree"; }
        }

        public IIndexInterface<TKey, TValue> GetNewInstance()
        {
            return new VersionedBPlusTree<TKey, TValue>();
        }

        #region Add

        public void Add(TKey myKey, TValue myValue)
        {
            AddInternal(myKey, new HashSet<TValue>() { myValue }, IndexSetStrategy.MERGE);
        }

        public void Add(TKey myKey, IEnumerable<TValue> myValues)
        {
            AddInternal(myKey, new HashSet<TValue>(myValues), IndexSetStrategy.MERGE);
        }

        public void Add(KeyValuePair<TKey, TValue> myKeyValuePair)
        {
            AddInternal(myKeyValuePair.Key, new HashSet<TValue>() { myKeyValuePair.Value }, IndexSetStrategy.MERGE);
        }

        public void Add(KeyValuePair<TKey, IEnumerable<TValue>> myKeyValuesPair)
        {
            AddInternal(myKeyValuesPair.Key, new HashSet<TValue>(myKeyValuesPair.Value), IndexSetStrategy.MERGE);
        }

        public void Add(Dictionary<TKey, TValue> myDictionary)
        {
            foreach (KeyValuePair<TKey, TValue> item in myDictionary)
            {
                AddInternal(item.Key, new HashSet<TValue>() { item.Value }, IndexSetStrategy.MERGE);                   
            }
        }

        public void Add(Dictionary<TKey, IEnumerable<TValue>> myDictionary)
        {
            foreach (KeyValuePair<TKey, IEnumerable<TValue>> item in myDictionary)
            {
                AddInternal(item.Key, new HashSet<TValue>(item.Value), IndexSetStrategy.MERGE);
            }
        }

        #endregion

        #region Clear

        /// <summary>
        /// Clears the tree by creating a new root
        /// </summary>
        public void Clear()
        {
            InitRootNode();
            _KeyCount = 0;
            _Count = 0;
        }

        #endregion

        #region Set

        public void Set(TKey myKey, TValue myValue, IndexSetStrategy myIndexSetStrategy)
        {            
            AddInternal(myKey, new HashSet<TValue>() { myValue} , myIndexSetStrategy);
        }

        public void Set(TKey myKey, IEnumerable<TValue> myValues, IndexSetStrategy myIndexSetStrategy)
        {
            AddInternal(myKey, new HashSet<TValue>(myValues), myIndexSetStrategy);
        }

        public void Set(KeyValuePair<TKey, TValue> myKeyValuePair, IndexSetStrategy myIndexSetStrategy)
        {
            AddInternal(myKeyValuePair.Key, new HashSet<TValue>() { myKeyValuePair.Value }, myIndexSetStrategy);
        }

        public void Set(KeyValuePair<TKey, IEnumerable<TValue>> myKeyValuesPair, IndexSetStrategy myIndexSetStrategy)
        {
            AddInternal(myKeyValuesPair.Key, new HashSet<TValue>(myKeyValuesPair.Value), myIndexSetStrategy);
        }

        public void Set(IEnumerable<KeyValuePair<TKey, TValue>> myKeyValuePairs, IndexSetStrategy myIndexSetStrategy)
        {
            foreach (KeyValuePair<TKey, TValue> item in myKeyValuePairs)
            {
                AddInternal(item.Key, new HashSet<TValue>() { item.Value }, myIndexSetStrategy); 
            }
        }

        public void Set(Dictionary<TKey, TValue> myDictionary, IndexSetStrategy myIndexSetStrategy)
        {
            foreach (KeyValuePair<TKey, TValue> item in myDictionary)
            {
                AddInternal(item.Key, new HashSet<TValue>() { item.Value }, myIndexSetStrategy);
            }
        }

        public void Set(Dictionary<TKey, IEnumerable<TValue>> myMultiValueDictionary, IndexSetStrategy myIndexSetStrategy)
        {
            foreach (KeyValuePair<TKey, IEnumerable<TValue>> item in myMultiValueDictionary)
            {
                AddInternal(item.Key, new HashSet<TValue>(item.Value), myIndexSetStrategy);
            }
        }

        #endregion

        #region contains

        Trinary IIndexInterface<TKey, TValue>.ContainsKey(TKey myKey)
        {
            return ContainsKey(myKey, 0);
        }
        
        //TODO: find a better way
        public Trinary ContainsValue(TValue myValue)
        {
            if (Values.Contains(myValue))
            {
                return Trinary.TRUE;
            }
            else
            {
                return Trinary.FALSE;
            }
        }

        public Trinary Contains(TKey myKey, TValue myValue)
        {
            return Contains(myKey, myValue, 0);
        }

        public Trinary Contains(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            return Contains(myFunc, 0);
        }

        #endregion

        #region this[]

        public HashSet<TValue> this[TKey myKey]
        {
            get
            {
                HashSet<TValue> result;

                if (TryGetValue(myKey, out result))
                {
                    return result;
                }

                return null;
            }
            set
            {
                AddInternal(myKey, value, IndexSetStrategy.REPLACE);
            }
        }

        #endregion

        #region IIndexInterface<TKey, TValue>.Keys()

        IEnumerable<TKey> IIndexInterface<TKey, TValue>.Keys()
        {
            return GetAllVersionedKeys(0);
        }

        IEnumerable<TKey> IIndexInterface<TKey, TValue>.Keys(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            return ((IVersionedIndexInterface<TKey, TValue>) this).Keys(myFunc, 0);
        }

        #endregion

        #region IIndexInterface<TKey, TValue>.KeyCount()

        ulong IIndexInterface<TKey, TValue>.KeyCount()
        {
            return ((IVersionedIndexInterface<TKey, TValue>)this).KeyCount(0);
        }

        ulong IIndexInterface<TKey, TValue>.KeyCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            return ((IVersionedIndexInterface<TKey, TValue>)this).KeyCount(myFunc, 0);
        }

        #endregion

        #region IIndexInterface<TKey, TValue>.Values()

        IEnumerable<HashSet<TValue>> IIndexInterface<TKey, TValue>.Values()
        {
            return ((IVersionedIndexInterface<TKey, TValue>)this).Values(0);
        }

        IEnumerable<HashSet<TValue>> IIndexInterface<TKey, TValue>.Values(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            return ((IVersionedIndexInterface<TKey, TValue>)this).Values(myFunc, 0);
        }

        #endregion

        #region GetValues()

        public IEnumerable<TValue> GetValues()
        {
            return GetAllVersionedValues(0);
        }

        #endregion

        #region ValueCount()

        public ulong ValueCount()
        {
            return ValueCount(0);
        }

        #endregion

        #region ValueCount()

        public ulong ValueCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            return ValueCount(myFunc, 0);
        }

        #endregion

        #region GetIDictionary()

        public IDictionary<TKey, HashSet<TValue>> GetIDictionary()
        {
            return GetIDictionary(0);
        }

        public IDictionary<TKey, HashSet<TValue>> GetIDictionary(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            return GetIDictionary(myFunc, 0);   
        }

        #endregion

        #region GetEnumerator()

        public IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator()
        {
            return GetEnumerator(0);
        }

        public IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            return GetEnumerator(myFunc, 0);
        }

        #endregion

        #region Remove()

        public bool Remove(TKey myKey, TValue myValue)
        {
            IndexValueHistoryList<TValue> indexValueHistoryList;
            var deleted = false;
            //lookup for the key
            if (TryGetValue(myKey, out indexValueHistoryList, 0))
            {
                deleted = indexValueHistoryList.Remove(myValue);

                if (deleted)
                {
                    //decrement tree's key count if it was the last key
                    if (indexValueHistoryList.isLatestDeleted)
                    {
                        _KeyCount--;
                    }
                    //decrement tree's value count
                    _Count--;
                    //clean up history
                    while (indexValueHistoryList.VersionCount > _HistorySize)
                    {
                        indexValueHistoryList.RemoveLatestFromHistory();
                    }
                }
            }
            //no key -> no value -> no delete
            return deleted;
        }

        public bool Remove(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {            
            var enumerator = GetEnumerator();

            bool removed = false;

            while (enumerator.MoveNext())
            {
                if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(enumerator.Current.Key, enumerator.Current.Value)))
                {
                    if (Remove(enumerator.Current.Key))
                    {
                        removed = true;
                    }
                }
            }            
            return removed;
        }

        #endregion

        #region IIndexInterface Range Queries

        public IEnumerable<TValue> GreaterThan(TKey myKey, bool myOrEqual = true)
        {
            return GreaterThan(myKey, null, myOrEqual);
        }

        public IEnumerable<TValue> GreaterThan(TKey myKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, bool myOrEqual = true)
        {
            return GreaterThan(myKey, myFunc, 0, myOrEqual);
        }

        public IEnumerable<TValue> LowerThan(TKey myKey, bool myOrEqual = true)
        {
            return LowerThan(myKey, null, myOrEqual);
        }

        public IEnumerable<TValue> LowerThan(TKey myKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, bool myOrEqual = true)
        {
            return LowerThan(myKey, myFunc, 0, myOrEqual);
        }

        public IEnumerable<TValue> InRange(TKey myFromKey, TKey myToKey, bool myOrEqualFromKey = true, bool myOrEqualToKey = true)
        {
            return InRange(myFromKey, myToKey, null, 0, myOrEqualFromKey, myOrEqualToKey);
        }

        public IEnumerable<TValue> InRange(TKey myFromKey, TKey myToKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, bool myOrEqualFromKey = true, bool myOrEqualToKey = true)
        {
            return InRange(myFromKey, myToKey, myFunc, 0, myOrEqualFromKey, myOrEqualToKey);
        }

        #endregion

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IVersionedIndexInterface<TKey, TValue> members

        #region ContainsKey()

        public Trinary ContainsKey(TKey myKey, long myVersion)
        {
            if (IsTreeInitialized())
            {
                return ContainsKeyInternal(_Root, myKey, myVersion);
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region ContainsValue()

        public Trinary ContainsValue(TValue myValue, long myVersion)
        {
            return GetAllVersionedValues(myVersion).Contains<TValue>(myValue);
        }

        #endregion

        #region Contains()

        /// <summary>
        /// checks if the tree contains a versioned value assigned to a key
        /// </summary>
        /// <param name="myKey">TKey</param>
        /// <param name="myValue">TValue</param>
        /// <param name="myVersion">values version</param>
        /// <returns>Trinary</returns>
        public Trinary Contains(TKey myKey, TValue myValue, long myVersion)
        {
            IndexValueHistoryList<TValue> result;

            if(TryGetValue(myKey, out result, myVersion))
            {
                if (result[myVersion] != null)
                {
                    if (result[myVersion].Count > 0)
                    {
                        //correct value?
                        return (result[myVersion].Contains(myValue));
                    }
                    return Trinary.DELETED;
                }
            }
            //doesn't exist ot no value with that version
            return Trinary.FALSE;
        }

        public Trinary Contains(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion)
        {
            var enumerator = GetEnumerator(myVersion);

            while (enumerator.MoveNext())
            {
                var versionSet = enumerator.Current.Value;
                //function returns true and valueverion is not deleted
                if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(enumerator.Current.Key, versionSet)) && versionSet.Count > 0)
                {
                    return Trinary.TRUE;
                }
            }
            return Trinary.FALSE;
        }

        #endregion

        #region this[]

        public HashSet<TValue> this[TKey myKey, long myVersion]
        {
            get 
            {
                HashSet<TValue> result;

                if (TryGetValue(myKey, out result, myVersion))
                {
                    return result;
                }

                return null;
            }
        }

        #endregion

        #region TryGetValue()

        public bool TryGetValue(TKey myKey, out HashSet<TValue> myValues, long myVersion)
        {
            //used to store the leaf which contains the key
            VersionedLeafNode<TKey, TValue> leaf;

            if (ContainsKeyInternal(_Root, myKey, out leaf, myVersion))
            {
                var index = Array.BinarySearch<TKey>(leaf.Keys, 0, leaf.KeyCount, myKey);

                //get the indexValueHistoryList's values
                var indexValueHistoryListValues = leaf.IndexValueHistoryLists[index][myVersion];
                //version exists?
                if (indexValueHistoryListValues != null)
                {
                    //and not deleted?
                    if (indexValueHistoryListValues.Count > 0)
                    {
                        //no .. return the values
                        myValues = indexValueHistoryListValues;
                        return Trinary.TRUE;
                    }
                }
            }
            //value version doesn't exist or key is deleted
            myValues = new HashSet<TValue>();
            return Trinary.FALSE;
        }

        #endregion

        #region IVersionedIndexInterface<TKey, TValue>.Keys()

        IEnumerable<TKey> IVersionedIndexInterface<TKey, TValue>.Keys(long myVersion)
        {
            return GetAllVersionedKeys(myVersion);
        }

        IEnumerable<TKey> IVersionedIndexInterface<TKey, TValue>.Keys(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion)
        {
            var enumerator = GetEnumerator(myVersion);

            while (enumerator.MoveNext())
            {
                if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(enumerator.Current.Key, enumerator.Current.Value)))
                {
                    yield return enumerator.Current.Key;
                }
            }
        }

        #endregion

        #region IVersionedIndexInterface<TKey, TValue>.KeyCount()

        ulong IVersionedIndexInterface<TKey, TValue>.KeyCount(long myVersion)
        {
            var longCount = 0UL;

            var enumerator = this.GetEnumerator(myVersion);

            while (enumerator.MoveNext())
            {
                longCount++;
            }

            return longCount;
        }

        ulong IVersionedIndexInterface<TKey, TValue>.KeyCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion)
        {
            var longCount = 0UL;

            var enumerator = this.GetEnumerator(myVersion);

            while (enumerator.MoveNext())
            {
                if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(enumerator.Current.Key, enumerator.Current.Value)))
                {
                    longCount++;
                }
            }

            return longCount;
        }

        #endregion

        #region IVersionedIndexInterface<TKey, TValue>.Values()

        IEnumerable<HashSet<TValue>> IVersionedIndexInterface<TKey, TValue>.Values(long myVersion)
        {
            var enumerator = this.GetEnumerator(myVersion);

            while (enumerator.MoveNext())
            {
                yield return enumerator.Current.Value;
            }
        }

        IEnumerable<HashSet<TValue>> IVersionedIndexInterface<TKey, TValue>.Values(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion)
        {
            var enumerator = this.GetEnumerator(myVersion);

            while (enumerator.MoveNext())
            {
                if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(enumerator.Current.Key, enumerator.Current.Value)))
                {
                    yield return enumerator.Current.Value;
                }
            }
        }

        #endregion

        #region IVersionedIndexInterface<TKey, TValue>.ValueCount()

        public ulong ValueCount(long myVersion)
        {
            var longCount = 0UL;
            var enumerator = this.GetEnumerator(myVersion);

            while (enumerator.MoveNext())
            {
                longCount += (ulong)enumerator.Current.Value.Count;
            }

            return longCount;
        }

        public ulong ValueCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion)
        {
            var longCount = 0UL;
            var enumerator = this.GetEnumerator(myVersion);

            while (enumerator.MoveNext())
            {
                if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(enumerator.Current.Key, enumerator.Current.Value)))
                {
                    longCount += (ulong)enumerator.Current.Value.Count;
                }
            }

            return longCount;
        }

        #endregion

        #region GetIDictionary()

        public IDictionary<TKey, HashSet<TValue>> GetIDictionary(long myVersion)
        {
            var enumerator = GetEnumerator(myVersion);

            var resultDict = new Dictionary<TKey, HashSet<TValue>>();

            while (enumerator.MoveNext())
            {
                resultDict.Add(enumerator.Current.Key, enumerator.Current.Value);
            }

            return resultDict;
        }

        public IDictionary<TKey, HashSet<TValue>> GetIDictionary(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion)
        {
            var enumerator = GetEnumerator(myVersion);

            IDictionary<TKey, HashSet<TValue>> resultDict = new Dictionary<TKey, HashSet<TValue>>();

            while (enumerator.MoveNext())
            {
                var kvp = new KeyValuePair<TKey, IEnumerable<TValue>>(enumerator.Current.Key, enumerator.Current.Value);

                if (myFunc(kvp))
                    resultDict.Add(enumerator.Current.Key, enumerator.Current.Value);
            }

            return resultDict;
        }

        #endregion

        #region GetEnumerator()

        public IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator(long myVersion)
        {
            VersionedLeafNode<TKey, TValue> tmp = _LeftMostLeaf;

            while (tmp != null)
            {
                for (int i = 0; i < tmp.KeyCount; i++)
                {
                    var values = tmp.IndexValueHistoryLists[i][myVersion];
                    //check if versioned values exist (!= null) and if they aren't deleted (> 0)
                    if (values != null && values.Count > 0)
                    {
                        yield return new KeyValuePair<TKey, HashSet<TValue>>(tmp.Keys[i], tmp.IndexValueHistoryLists[i][myVersion]);    
                    }
                }
                tmp = (VersionedLeafNode<TKey, TValue>)tmp.RightSibling;
            }
        }

        public IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion)
        {
            VersionedLeafNode<TKey, TValue> tmp = _LeftMostLeaf;

            while (tmp != null)
            {
                for (int i = 0; i < tmp.ValueCount; i++)
                {
                    //check if versioned values exists
                    if (tmp.IndexValueHistoryLists[i][myVersion] != null)
                    {
                        var kvp = new KeyValuePair<TKey, IEnumerable<TValue>>(tmp.Keys[i], tmp.IndexValueHistoryLists[i][myVersion]);

                        if (myFunc(kvp))
                        {
                            yield return new KeyValuePair<TKey, HashSet<TValue>>(tmp.Keys[i], tmp.IndexValueHistoryLists[i][myVersion]);
                        }    
                    }
                }
                tmp = (VersionedLeafNode<TKey, TValue>)tmp.RightSibling;
            }  
        }

        #endregion

        #region VersionCount()

        public ulong VersionCount(TKey myKey)
        {
            IndexValueHistoryList<TValue> result;

            if (TryGetValue(myKey, out result, 0))
            {
                return result.VersionCount;
            }

            return default(UInt64);
        }

        #endregion

        #region ClearHistory()

        public void ClearHistory(TKey myKey)
        {
            IndexValueHistoryList<TValue> result;

            if (TryGetValue(myKey, out result, 0))
            {
                result.ClearHistory();
            }
        }

        #endregion

        #region Range Methods

        public IEnumerable<TValue> GreaterThan(TKey myKey, long myVersion, bool myOrEqual = true)
        {
            return GreaterThan(myKey, null, myVersion, myOrEqual);
        }

        public IEnumerable<TValue> GreaterThan(TKey myKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion, bool myOrEqual = true)
        {
            TKey mostRightKey = _RightMostLeaf.Keys[_RightMostLeaf.KeyCount - 1];

            if (myKey.CompareTo(mostRightKey) == 1)
            {
                return new HashSet<TValue>();
            }
            else if (myKey.CompareTo(mostRightKey) == 0) //if the key is the upper border then only the myOrEqual decides what to do
            {
                return InRange(myKey, mostRightKey, myFunc, myVersion, myOrEqual, false);
            }
            else
            {
                return InRange(myKey, mostRightKey, myFunc, myVersion, myOrEqual, true);
            }
        }

        public IEnumerable<TValue> LowerThan(TKey myKey, long myVersion, bool myOrEqual = true)
        {
            return LowerThan(myKey, null, myVersion, myOrEqual);
        }

        public IEnumerable<TValue> LowerThan(TKey myKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion, bool myOrEqual = true)
        {
            TKey mostLeftKey = _LeftMostLeaf.Keys[0];

            if (myKey.CompareTo(mostLeftKey) == -1)
            {
                return new HashSet<TValue>();
            }
            else if (myKey.CompareTo(mostLeftKey) == 0) //if the key is the lower border then only the myOrEqual decides what to do
            {
                return InRange(mostLeftKey, myKey, myFunc, myVersion, false, myOrEqual);
            }
            else
            {
                return InRange(mostLeftKey, myKey, myFunc, myVersion, true, myOrEqual);
            }
        }

        public IEnumerable<TValue> InRange(TKey myFromKey, TKey myToKey, long myVersion, bool myOrEqualFromKey = true, bool myOrEqualToKey = true)
        {
            return InRange(myFromKey, myToKey, null, myVersion, myOrEqualFromKey, myOrEqualToKey);
        }

        public IEnumerable<TValue> InRange(TKey myFromKey, TKey myToKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion, bool myOrEqualFromKey = true, bool myOrEqualToKey = true)
        {
            #region data

            HashSet<TValue> resultSet;

            #endregion

            #region special case -- tree has no elements

            if (_Count == 0)
            {
                yield break;
            }

            #endregion

            #region myFromKey == myToKey

            if (myFromKey.CompareTo(myToKey) == 0) //from and to are the same
            {
                //lower or upper bound included?
                if (myOrEqualFromKey || myOrEqualToKey)
                {
                    if (TryGetValue(myFromKey, out resultSet, myVersion))
                    {
                        if (myFunc != null)
                        {
                            if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(myFromKey, resultSet)))
                            {
                                foreach (TValue val in resultSet)
                                {
                                    yield return val;
                                }
                            }
                        }
                        else
                        {
                            foreach (TValue val in resultSet)
                            {
                                yield return val;
                            }
                        }
                    }
                }
                //keys are equal, but the bounds themselves are not included in the search
            }

            #endregion

            #region myFromKey > myToKey

            else if (myFromKey.CompareTo(myToKey) == 1)
            {
                //check bounds
                TKey mostLeftKey = _LeftMostLeaf.Keys[0];
                TKey mostRightKey = _RightMostLeaf.Keys[_RightMostLeaf.KeyCount - 1];

                if (myFromKey.CompareTo(mostRightKey) == -1 || myFromKey.CompareTo(mostRightKey) == 0)
                {
                    //1st return all values between fromKey and most right key in the tree
                    foreach (TValue val in InRange(myFromKey, mostRightKey, myFunc, myVersion, myOrEqualFromKey, true))
                    {
                        yield return val;
                    }
                }

                if (mostLeftKey.CompareTo(myToKey) == -1 || mostLeftKey.CompareTo(myToKey) == 0)
                {
                    //2nd return all values between the most left key in the tree and the toKey
                    foreach (TValue val in InRange(mostLeftKey, myToKey, myFunc, myVersion, true, myOrEqualToKey))
                    {
                        yield return val;
                    }
                }
            }

            #endregion

            #region myFromKey < myToKey

            else if (myFromKey.CompareTo(myToKey) == -1)
            {

                #region data

                VersionedLeafNode<TKey, TValue> tmp;
                var startIndex = 0;

                #endregion

                #region find correct start leaf and start index

                if (ContainsKeyInternal(_Root, myFromKey, out tmp, myVersion))
                {
                    startIndex = Array.BinarySearch<TKey>(tmp.Keys, 0, tmp.KeyCount, myFromKey);
                    //decrement startindex if the key itself is not contained in the range
                    if (!myOrEqualFromKey)
                    {
                        startIndex++;
                    }
                }
                else
                {
                    //key is not in leaf
                    startIndex = tmp.KeyCount - 1;
                    while (startIndex >= 0 && myFromKey.CompareTo(tmp.Keys[startIndex]) == -1)
                    {
                        startIndex--;
                    }
                }

                //if the startIndex would be out of bounds, we have to correct that
                if (startIndex < 0)
                {
                    if (tmp.LeftSibling != null)
                    {
                        tmp = (VersionedLeafNode<TKey, TValue>)tmp.LeftSibling;
                        startIndex = tmp.KeyCount - 1;

                    }
                    else
                    {
                        startIndex = 0;
                    }
                }

                #endregion

                #region start returning values

                bool done = false;

                while (tmp != null)
                {
                    for (Int32 i = startIndex; i < tmp.KeyCount; i++)
                    {
                        //if the current key is smaller or it matches the upper limit and equality is allowed
                        if (tmp.Keys[i].CompareTo(myToKey) == -1 || (tmp.Keys[i].CompareTo(myToKey) == 0 && myOrEqualToKey))
                        {
                            var versionedValues = tmp.IndexValueHistoryLists[i][myVersion];

                            if (myFunc != null)
                            {
                                if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(tmp.Keys[i], versionedValues)))
                                {
                                    foreach (TValue val in tmp.IndexValueHistoryLists[i][myVersion])
                                    {
                                        yield return val;
                                    }
                                }
                            }
                            else
                            {
                                foreach (TValue val in tmp.IndexValueHistoryLists[i][myVersion])
                                {
                                    yield return val;
                                }
                            }
                        }
                        else
                        {
                            //out of range..stop here
                            done = true;
                            break;
                        }
                    }
                    if (done)
                    {
                        break;
                    }
                    //hand over hand to the next leaf
                    tmp = tmp.RightSibling as VersionedLeafNode<TKey, TValue>;
                    //and start from the beginning
                    startIndex = 0;
                }

                #endregion
            }
            #endregion
        }

        #endregion

        #endregion

        #region IVersionedBPlusTree<TKey, TValue> members

        public void Add(TKey myKey, IndexValueHistoryList<TValue> myIndexValueHistoryList)
        {
            AddInternal(myKey, myIndexValueHistoryList);
        }

        public IEnumerator<KeyValuePair<TKey, IndexValueHistoryList<TValue>>> GetKVPEnumerator()
        {
            VersionedLeafNode<TKey, TValue> tmp = _LeftMostLeaf;

            while (tmp != null)
            {
                for (int i = 0; i < tmp.KeyCount; i++)
                {
                    yield return new KeyValuePair<TKey, IndexValueHistoryList<TValue>>(tmp.Keys[i], tmp.IndexValueHistoryLists[i]);
                }
                tmp = (VersionedLeafNode<TKey, TValue>)tmp.RightSibling;
            }  
        }

        #endregion

        #region IEstimable members

        public ulong GetEstimatedSize()
        {
            return EstimatedSizeConstants.UndefinedObjectSize;
        }

        #endregion
    }
}
