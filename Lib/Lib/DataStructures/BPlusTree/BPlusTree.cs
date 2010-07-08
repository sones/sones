/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
*/


/* <id name=”Libraries Datastructures – BStarTree” />
 * <copyright file=”BStarTree.cs”
 *            company=”sones GmbH”>
 * Copyright (c) sones GmbH 2007-2010
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
using sones.Lib.DataStructures.Indices;

#endregion

namespace sones.Lib.DataStructures.BPlusTree
{
    public class BPlusTree<TKey, TValue> : IBPlusTree<TKey, TValue>
                where TKey : IComparable
    {
        #region private members

        /// <summary>
        /// The root node of the b-tree
        /// </summary>
        private Node<TKey, TValue> _Root;

        /// <summary>
        /// The Order k of the b-tree
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
        /// If a tree has an order k there can be at max 2k elements per node.
        /// </summary>
        private Int32 _MaxKeysPerNode;

        /// <summary>
        /// If a tree has an order k there must not be less then k elements per node.
        /// </summary>
        private Int32 _MinKeysPerNode;

        /// <summary>
        /// If a tree has an order k there can be at max 2k + 1 children per node.
        /// </summary>
        private Int32 _MaxChildrenPerNode;

        /// <summary>
        /// If a tree has an order k there must not be leass then k + 1 children per node.
        /// </summary>
        private Int32 _MinChildrenPerNode;

        /// <summary>
        /// The number of values in the tree
        /// </summary>
        private ulong _Count;

        /// <summary>
        /// The number of keys in the tree
        /// </summary>
        private ulong _KeyCount;

        /// <summary>
        /// Holds a reference to the left-most leaf. 
        /// </summary>
        internal LeafNode<TKey, TValue> _LeftMostLeaf;

        /// <summary>
        /// Holds a reference to the right-most leaf. 
        /// </summary>
        internal LeafNode<TKey, TValue> _RightMostLeaf;

        #endregion private members

        #region constructors

        /// <summary>
        /// Instantiates a BPlusTree with a default order of k = 60
        /// </summary>
        public BPlusTree()
            : this(60)
        {
        }
        
        /// <summary>
        /// Instantiates a BPlusTree with a given order k
        /// </summary>
        /// <param name="myOrder"></param>
        public BPlusTree(Int32 myOrder)
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

            #endregion
        }

        #endregion

        #region getter / setter

        /// <summary>
        /// Returns the number of values in the tree
        /// </summary>
        public ulong Count
        {
            get { return _Count; }
            set { _Count = value; }
        }

        /// <summary>
        /// Returns the number of keys in the tree
        /// </summary>
        public ulong KeyCount
        {
            get { return _KeyCount; }
            set { _KeyCount = value; }
        }

        /// <summary>
        /// Returns the order of the tree
        /// </summary>
        public Int32 Order
        {
            get { return _Order; }
        }

        /// <summary>
        /// Returns the current height of the tree
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

        #endregion

        #region Internal Members

        /// <summary>
        /// Checks if the tree contains the key
        /// </summary>
        /// <param name="myKey">TKey</param>
        /// <returns>true if tree contains key</returns>
        public bool ContainsKey(TKey myKey)
        {
            if (IsTreeInitialized())
            {
                return ContainsKeyInternal(_Root, myKey);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Get all keys in the tree.
        /// </summary>
        public IEnumerable<TKey> Keys
        {
            get
            {
                if (_Root == null)
                {
                    return new Collection<TKey>();
                }
                else
                {
                    return GetAllKeys();
                }
            }
        }

        /// <summary>
        /// Remove a key and all associated values from the tree.
        /// </summary>
        /// <param name="key">TKey</param>
        /// <returns>true if deleted</returns>
        public bool Remove(TKey key)
        {
            return RemoveKeyInternal(key);
        }

        /// <summary>
        /// Returns all values of the tree.
        /// </summary>
        /// <returns>All values in the tree</returns>
        public IEnumerable<TValue> Values
        {
            get
            {
                if (_Root == null)
                {
                    return new Collection<TValue>();
                }
                else
                {
                    return GetAllValues();
                }
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
        /// Adds a key and it's value(s) into the tree.
        /// If the key already exists, the value(s) is/are added to the key's associated value-hashset
        /// using the given IndexSetStrategy.
        /// </summary>
        /// <param name="myKey">The key to be added</param>
        /// <param name="myValue">The value(s) associated to the key</param>
        private void AddInternal(TKey myKey, HashSet<TValue> myValue, IndexSetStrategy myIndexSetStrategy)
        {
            if (!IsTreeInitialized()) //tree is empty
            {
                InitRootNode(); //create a new root
            }
            //used to handle split information
            SplitInfo<TKey, TValue> splitInfo = new SplitInfo<TKey, TValue>();

            //insert into the tree
            if (_Root.Insert(myKey, myValue, ref splitInfo, myIndexSetStrategy))
            {
                //root was splitted we have to create a new one
                //create the fist inner node
                InnerNode<TKey, TValue> innerNode = new InnerNode<TKey, TValue>(this);

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

        /// <summary>
        /// Recursive search for myKey. If the search-node is internal, it searches for smaller for the right child
        /// and continues search. If the method reaches a leaf node it checks, if the key is contained in that node.
        /// </summary>
        /// <param name="myNode">The node which shall be searched.</param>
        /// <param name="myKey">They key to search for.</param>
        /// <returns>True if the node contains that key.</returns>
        private bool ContainsKeyInternal(Node<TKey, TValue> myNode, TKey myKey)
        {
            //make some room to store the leaf associated to the key
            LeafNode<TKey, TValue> leaf;

            return ContainsKeyInternal(myNode, myKey, out leaf);
        }

        /// <summary>
        /// Recursive search for myKey. If the search-node is internal, it searches for smaller for the right child
        /// and continues search. If the method reaches a leaf node it checks, if the key is contained in that node.
        /// 
        /// In any case ([not]found), a reference to the corresponding leaf node is stored in an out param.
        /// </summary>
        /// <param name="myNode">The node which shall be searched.</param>
        /// <param name="myKey">They key to search for.</param>
        /// <param name="myCorrespondingLeafNode">A reference to the leaf node where the key is located</param>
        /// <returns>True if the node contains that key.</returns>
        private bool ContainsKeyInternal(Node<TKey, TValue> myNode, TKey myKey, out LeafNode<TKey, TValue> myCorrespondingLeafNode)
        {
            if (myNode is LeafNode<TKey, TValue>) //we reached the bottom of the tree
            {
                //store the leaf node
                myCorrespondingLeafNode = myNode as LeafNode<TKey, TValue>;
                //search the leaf Keys for myKey using binary search between 0 and the number of keys contained in that node
                return myNode.ContainsKey(myKey);
            }

            //we are at an inner node and search for a way to go
            int nextChildIndex = myNode.FindSlot(myKey) + 1;
            //go recursively to the next child node
            return ContainsKeyInternal(((InnerNode<TKey, TValue>)myNode).Children[nextChildIndex], myKey, out myCorrespondingLeafNode);
        }

        /// <summary>
        /// Initializes the tree.
        /// </summary>
        private void InitRootNode()
        {
            #region node

            _Root           = new LeafNode<TKey, TValue>(this);
            _Root.Parent    = null;
            _Root.KeyCount  = 0;

            #endregion

            #region tree

            this._LeftMostLeaf  = _Root as LeafNode<TKey, TValue>;
            this._RightMostLeaf = _Root as LeafNode<TKey, TValue>;
            this._Height        = 1;

            #endregion
        }

        /// <summary>
        /// Returns all (not deleted) keys in sorted order starting at the left-most leaf.
        /// </summary>
        /// <returns>a (sorted) collection which contains all keys of the tree</returns>
        private IEnumerable<TKey> GetAllKeys()
        {
            //using list to use add range
            List<TKey> resultCollection = new List<TKey>();

            //used to make one's way hand over hand along leafs
            LeafNode<TKey, TValue> tmpLeaf = _LeftMostLeaf;

            //still a right sibling
            while (tmpLeaf != null)
            {
                //add all keys of the node
                for (Int32 i = 0; i < tmpLeaf.KeyCount; i++)
                {
                    yield return tmpLeaf.Keys[i];
                }
                tmpLeaf = tmpLeaf.RightSibling as LeafNode<TKey, TValue>;                
            }
        }

        /// <summary>
        /// Returns all values in the tree starting with the left-most leaf.
        /// </summary>
        /// <returns>All values in the tree (using yield return)</returns>
        private IEnumerable<TValue> GetAllValues()
        {
            HashSet<TValue> resultCollection = new HashSet<TValue>();

            LeafNode<TKey, TValue> tmpLeaf = _LeftMostLeaf;

            while (tmpLeaf != null)
            {
                for (Int32 i = 0; i < tmpLeaf.KeyCount; i++)
                {
                    foreach (TValue val in tmpLeaf.Values[i])
                    {
                        yield return val;
                    }
                }
                tmpLeaf = tmpLeaf.RightSibling as LeafNode<TKey, TValue>;
            }
        }

        /// <summary>
        /// Method removes a given key and all associated values from the tree.
        /// </summary>
        /// <param name="myRemoveKey">The key to be removed</param>
        /// <returns>true if successful</returns>
        private bool RemoveKeyInternal(TKey myRemoveKey)
        {
            //stores the result
            bool retValue = false;
            //start removing
            var newNode = _Root.Remove(ref myRemoveKey, null, null, null, null, null, ref retValue);

            //we have a new root and the tree is shrinking
            if (newNode != null)
            {
                //set the new root
                _Root = newNode;
                //remove root's parent
                newNode.Parent = null;
                //decrement tree height
                _Height--;
            }
            //done
            return retValue;
        }

        #endregion

        #region IBPlusTree<TKey> Members

        /// <summary>
        /// see interface IBPlusTree for general documentation
        /// 
        /// Method returns an IEnumerable of values corresponding to all keys between 
        /// the myFromKey and the myToKey including the values of the limit keys.
        /// If myToKey is smaller then myFromKey, they are swapped and the method
        /// continues with the new limits.
        /// 
        /// If the fromKey doesn't exist in the tree, the leaf where it should be inserted
        /// is looked up and then we search for the 'pseudo'-startkey, to start the search 
        /// from there.
        /// 
        /// The Method DOES NOT handle duplicate values assigned to different keys!
        /// 
        /// NOTE: Range methods are implemented by using IIndexInterface
        /// </summary>
        /// <param name="myFromKey"></param>
        /// <param name="myToKey"></param>
        /// <returns></returns>
        public IEnumerable<TValue> GetValuesInRange(TKey myFromKey, TKey myToKey)
        {
            #region data

            HashSet<TValue> resultSet;

            #endregion

            #region handle fromKey and toKey are equal

            //if from and to are equal we just return the HashSet<TValue>
            //associated to one of the keys
            if (myFromKey.CompareTo(myToKey) == 0) //from and to are the same
            {
                TryGetValue(myFromKey, out resultSet);
                if (resultSet == null)
                {
                    resultSet = new HashSet<TValue>();
                }

                foreach (TValue val in resultSet)
                {
                    yield return val;
                }
            }
            else //keys are not equal
            {

            #endregion

                #region handle toKey is smaller then fromKey

                //if toKey is smaller then fromKey we switch them
                if (myToKey.CompareTo(myFromKey) == -1)
                {
                    TKey tmp = myToKey;
                    myToKey = myFromKey;
                    myFromKey = tmp;
                }

                #endregion

                #region range lookup

                #region data

                //used to store the corresponding leaf node
                LeafNode<TKey, TValue> leaf;
                //start range query from there
                Int32 startIndex = 0;
                //intialize the result HashSet
                resultSet = new HashSet<TValue>();

                #endregion

                #region find correct startIndex

                //now get the node of the from key
                if (ContainsKeyInternal(_Root, myFromKey, out leaf))
                {
                    //the tree contains the key so we can start collecting all values from here
                    startIndex = Array.BinarySearch<TKey>(leaf.Keys, 0, leaf.KeyCount, myFromKey);
                }
                else
                {
                    //the tree doesn't contain the key, so we need to determine where key would be in the leaf
                    //note the key could only be in that leaf because of comparison in parent inner node
                    startIndex = leaf.KeyCount - 1;
                    while (startIndex >= 0 && myFromKey.CompareTo(leaf.Keys[startIndex]) == -1)
                    {
                        startIndex--;
                    }
                    //if the lower limit is smaller then the most left key in that node, we start at the most left key
                    if (startIndex < 0)
                    {
                        startIndex = 0;
                    }
                }

                #endregion

                #region move hand over hand along the leafs

                //we need to collect all values from here until we reach the toKey
                LeafNode<TKey, TValue> tmpLeaf = leaf;
                //used to break outer loop
                bool done = false;

                while (tmpLeaf != null)
                {
                    for (Int32 i = startIndex; i < tmpLeaf.KeyCount; i++)
                    {
                        //if the current key is smaller or equal to the upper range limit
                        if (tmpLeaf.Keys[i].CompareTo(myToKey) == -1 || tmpLeaf.Keys[i].CompareTo(myToKey) == 0)
                        {
                            foreach (TValue val in tmpLeaf.Values[i])
                            {
                                yield return val;
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
                    tmpLeaf = tmpLeaf.RightSibling as LeafNode<TKey, TValue>;
                    //and start from the beginning
                    startIndex = 0;
                }
                #endregion
            }
            #endregion
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
            //used to store the leaf which contains the key
            LeafNode<TKey, TValue> leaf;

            if (ContainsKeyInternal(_Root, myKey, out leaf))
            {
                var index = Array.BinarySearch<TKey>(leaf.Keys, 0, leaf.KeyCount, myKey);
                myValues = leaf.Values[index];
                return true;

            }
            //not found
            myValues = new HashSet<TValue>();
            return false;
        }

        /// <summary>
        /// Method returns the value for the best match of the key. 
        /// 
        /// Exact match returns the corresponding set of values.
        /// LowerThan means that if the key is not in the tree, the key which would be
        /// its left neighbour is returned. 
        /// !LowerThen means that if the key is not in the tree, the key which would be
        /// its right neighbour is returned.
        /// 
        /// If myLowerThan is true and there is no lower key, then null will be returned
        /// If myLowerThan is false and there is no greater key, then null will be returned
        /// </summary>
        /// <param name="myKey">the key to find a match</param>
        /// <param name="myLowerThan">the values associated to the best matching key</param>
        /// <returns>Set of values or null if no best key is found</returns>
        public HashSet<TValue> GetBestMatch(TKey myKey, bool myLowerThan = true)
        {
            #region init & data

            if (!IsTreeInitialized())
            {
                return null;
            }

            LeafNode<TKey, TValue> correspondingLeaf = null;

            #endregion

            #region check if key is in the tree and get the corresponding leaf and key's position

            ContainsKeyInternal(_Root, myKey, out correspondingLeaf);
            //if key is not found, the index is the position where the key would be
            int index = correspondingLeaf.FindSlot(myKey);

            #endregion

            #region equality check

            if (index >= 0 && index < correspondingLeaf.KeyCount)
            {
                if (correspondingLeaf.Keys[index].Equals(myKey))
                {
                    return correspondingLeaf.Values[index];
                }
            }

            #endregion

            #region lowerThan = true

            if (myLowerThan)
            {
                if (index >= 0) //same leaf
                {
                    return correspondingLeaf.Values[index];
                }
                else
                {
                    if (correspondingLeaf.LeftSibling == null) //leftMostLeaf
                    {
                        //no smaller key
                        return null;
                    }
                    else
                    {
                        var left = correspondingLeaf.LeftSibling as LeafNode<TKey, TValue>;
                        return left.Values[left.KeyCount - 1];
                    }
                }
            }

            #endregion

            #region lowerThan = false

            index++;
            if (index < correspondingLeaf.KeyCount) //same leaf
            {
                return correspondingLeaf.Values[index];
            }
            else
            {
                if (correspondingLeaf.RightSibling == null) //rightmost
                {
                    //no greater key
                    return null;
                }
                else
                {
                    var right = correspondingLeaf.RightSibling as LeafNode<TKey, TValue>;
                    return right.Values[0];
                }
            }
            
            #endregion
        }

        #endregion

        #region IIndexInterface<TKey, TValue> members

        /// <summary>
        /// Used in the database..
        /// </summary>
        public string IndexName
        {
            get { return "BPlusTree"; }
        }

        /// <summary>
        /// Creates a new instance of the tree with the same order.
        /// </summary>
        /// <returns>a new instance of a BPlusTree</returns>
        public IIndexInterface<TKey, TValue> GetNewInstance()
        {
            return new BPlusTree<TKey, TValue>(this.Order);
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
            if (ContainsKey(myKey))
            {
                return Trinary.TRUE;
            }
            else
            {
                return Trinary.FALSE;
            }
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
            HashSet<TValue> result;

            if (TryGetValue(myKey, out result))
            {
                return result.Contains(myValue);
            }

            return Trinary.FALSE;
        }

        public Trinary Contains(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            var enumerator = GetEnumerator();

            while (enumerator.MoveNext())
            {
                if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(enumerator.Current.Key, enumerator.Current.Value)))
                {
                    return Trinary.TRUE;
                }
            }
            return Trinary.FALSE;
        }

        #endregion

        public HashSet<TValue> this[TKey key]
        {
            get
            {
                HashSet<TValue> result;
                if (TryGetValue(key, out result))
                {
                    return result;
                }
                else
                {
                    return new HashSet<TValue>();
                }
            }
            set
            {
                AddInternal(key, value, IndexSetStrategy.REPLACE);
            }
        }

        IEnumerable<TKey> IIndexInterface<TKey, TValue>.Keys()
        {
            return Keys;
        }

        IEnumerable<TKey> IIndexInterface<TKey, TValue>.Keys(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            var enumerator = GetEnumerator();

            while (enumerator.MoveNext())
            {
                if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(enumerator.Current.Key, enumerator.Current.Value)))
                {
                    yield return enumerator.Current.Key;
                }
            }
        }

        ulong IIndexInterface<TKey, TValue>.KeyCount()
        {
            return this.KeyCount;
        }

        ulong IIndexInterface<TKey, TValue>.KeyCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            var longCount = 0UL;

            var enumerator = this.GetEnumerator();

            while (enumerator.MoveNext())
            {
                if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(enumerator.Current.Key, enumerator.Current.Value)))
                {
                    longCount++;
                }
            }

            return longCount;
        }

        IEnumerable<HashSet<TValue>> IIndexInterface<TKey, TValue>.Values()
        {
            var enumerator = this.GetEnumerator();

            while (enumerator.MoveNext())
            {
                yield return enumerator.Current.Value;
            }
        }

        IEnumerable<HashSet<TValue>> IIndexInterface<TKey, TValue>.Values(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            var enumerator = this.GetEnumerator();

            while (enumerator.MoveNext())
            {
                if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(enumerator.Current.Key, enumerator.Current.Value)))
                {
                    yield return enumerator.Current.Value;
                }
            }
        }

        public IEnumerable<TValue> GetValues()
        {
            return GetAllValues();
        }

        public ulong ValueCount()
        {
            return Count;
        }

        public ulong ValueCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            var longCount = 0UL;
            var enumerator = this.GetEnumerator();

            while (enumerator.MoveNext())
            {
                if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(enumerator.Current.Key, enumerator.Current.Value)))
                {
                    longCount += (ulong) enumerator.Current.Value.Count;
                }
            }

            return longCount;
        }

        public IDictionary<TKey, HashSet<TValue>> GetIDictionary()
        {
            var enumerator = GetEnumerator();

            IDictionary<TKey, HashSet<TValue>> resultDict = new Dictionary<TKey, HashSet<TValue>>();

            while (enumerator.MoveNext())
            {
                resultDict.Add(enumerator.Current.Key, enumerator.Current.Value);
            }

            return resultDict;
        }

        public IDictionary<TKey, HashSet<TValue>> GetIDictionary(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            var enumerator = GetEnumerator();

            IDictionary<TKey, HashSet<TValue>> resultDict = new Dictionary<TKey, HashSet<TValue>>();

            while (enumerator.MoveNext())
            {
                var kvp = new KeyValuePair<TKey, IEnumerable<TValue>>(enumerator.Current.Key, enumerator.Current.Value);

                if(myFunc(kvp))
                    resultDict.Add(enumerator.Current.Key, enumerator.Current.Value);
            }

            return resultDict;
        }

        public IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator()
        {
            LeafNode<TKey, TValue> tmp = _LeftMostLeaf;

            while (tmp != null)
            {
                for (int i = 0; i < tmp.ValueCount; i++)
                {
                    yield return new KeyValuePair<TKey, HashSet<TValue>>(tmp.Keys[i], tmp.Values[i]);
                }
                tmp = (LeafNode<TKey, TValue>) tmp.RightSibling;
            }            
        }

        public IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            LeafNode<TKey, TValue> tmp = _LeftMostLeaf;

            while (tmp != null)
            {
                for (int i = 0; i < tmp.ValueCount; i++)
                {
                    var kvp = new KeyValuePair<TKey, IEnumerable<TValue>>(tmp.Keys[i], tmp.Values[i]);

                    if(myFunc(kvp))
                    {
                        yield return new KeyValuePair<TKey, HashSet<TValue>>(tmp.Keys[i], tmp.Values[i]);
                    } 
                }
                tmp = (LeafNode<TKey, TValue>)tmp.RightSibling;
            }  
        }

        public bool Remove(TKey myKey, TValue myValue)
        {
            HashSet<TValue> values;

            //lookup for the key
            if (TryGetValue(myKey, out values))
            {
                if (values.Contains<TValue>(myValue))
                {
                    //remove myValue from the HashSet
                    return values.Remove(myValue);
                }

                if (values.Count == 0) //last one?
                {
                    //remove whole key
                    return Remove(myKey);
                }
            }
            //no key -> no value -> no delete
            return false;
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

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IIndexInterface Range Queries

        public IEnumerable<TValue> GreaterThan(TKey myKey, bool myOrEqual = true)
        {
            return GreaterThan(myKey, null, myOrEqual);
        }

        public IEnumerable<TValue> GreaterThan(TKey myKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, bool myOrEqual = true)
        {
            TKey mostRightKey = _RightMostLeaf.Keys[_RightMostLeaf.KeyCount - 1];

            if (myKey.CompareTo(mostRightKey) == 1)
            {
                return new HashSet<TValue>();
            }
            else if (myKey.CompareTo(mostRightKey) == 0) //if the key is the upper border then only the myOrEqual decides what to do
            {
                return InRange(myKey, mostRightKey, myFunc, myOrEqual, false);
            }
            else
            {
                return InRange(myKey, mostRightKey, myFunc, myOrEqual, true);
            }
        }

        public IEnumerable<TValue> LowerThan(TKey myKey, bool myOrEqual = true)
        {
            return LowerThan(myKey, null, myOrEqual);
        }

        public IEnumerable<TValue> LowerThan(TKey myKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, bool myOrEqual = true)
        {
            TKey mostLeftKey = _LeftMostLeaf.Keys[0];

            if (myKey.CompareTo(mostLeftKey) == -1)
            {
                return new HashSet<TValue>();
            }
            else if (myKey.CompareTo(mostLeftKey) == 0) //if the key is the lower border then only the myOrEqual decides what to do
            {
                return InRange(mostLeftKey, myKey, myFunc, false, myOrEqual);
            }
            else
            {
                return InRange(mostLeftKey, myKey, myFunc, true, myOrEqual);
            }
        }

        public IEnumerable<TValue> InRange(TKey myFromKey, TKey myToKey, bool myOrEqualFromKey = true, bool myOrEqualToKey = true)
        {
            return InRange(myFromKey, myToKey, null, myOrEqualFromKey, myOrEqualToKey);
        }

        public IEnumerable<TValue> InRange(TKey myFromKey, TKey myToKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, bool myOrEqualFromKey = true, bool myOrEqualToKey = true)
        {
            #region data

            HashSet<TValue> resultSet;

            #endregion

            #region myFromKey == myToKey

            if (myFromKey.CompareTo(myToKey) == 0) //from and to are the same
            {
                //lower or upper bound included?
                if (myOrEqualFromKey || myOrEqualToKey)
                {
                    if (TryGetValue(myFromKey, out resultSet))
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
                    foreach (TValue val in InRange(myFromKey, mostRightKey, myFunc, myOrEqualFromKey, true))
                    {
                        yield return val;
                    }
                }

                if (mostLeftKey.CompareTo(myToKey) == -1 || mostLeftKey.CompareTo(myToKey) == 0)
                {
                    //2nd return all values between the most left key in the tree and the toKey
                    foreach (TValue val in InRange(mostLeftKey, myToKey, myFunc, true, myOrEqualToKey))
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

                LeafNode<TKey, TValue> tmp;
                var startIndex = 0;

                #endregion

                #region find correct start leaf and start index

                if (ContainsKeyInternal(_Root, myFromKey, out tmp))
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
                        tmp = (LeafNode<TKey, TValue>)tmp.LeftSibling;
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
                            var values = tmp.Values[i];

                            if (myFunc != null)
                            {
                                if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(tmp.Keys[i], values)))
                                {
                                    foreach (TValue val in values)
                                    {
                                        yield return val;
                                    }
                                }
                            }
                            else
                            {
                                foreach (TValue val in values)
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
                    tmp = tmp.RightSibling as LeafNode<TKey, TValue>;
                    //and start from the beginning
                    startIndex = 0;
                }

                #endregion
            }
            #endregion
        }


        #endregion

        #region debug stuff

        public String Draw()
        {
            if (IsTreeInitialized())
            {
                StringBuilder sb = new StringBuilder();

                DrawInternal(_Root, ref sb, _Height);
                return sb.ToString();
            }
            return "";
        }


        private void DrawInternal(Node<TKey, TValue> myNode, ref StringBuilder myStringBuilder, int tabs)
        {
            Node<TKey, TValue> tmp = myNode;

            int index;
            for (index = 0; index < tabs * tabs; index++)
            {
                myStringBuilder.Append("\t");
            }

            while (tmp != null)
            {
                myStringBuilder.Append("|");
                for (index = 0; index < tmp.KeyCount; index++)
                {
                    myStringBuilder.Append(tmp.Keys[index].ToString() + "|");
                }
                tmp = tmp.RightSibling;

                for (index = tabs * tabs; index >= 0; index--)
                    myStringBuilder.Append("\t");
            }
            if (myNode is InnerNode<TKey, TValue>)
            {
                myStringBuilder.Append("\n\n");
                DrawInternal((myNode as InnerNode<TKey, TValue>).Children[0], ref myStringBuilder, --tabs);
            }
        }

        #endregion
    }
}
