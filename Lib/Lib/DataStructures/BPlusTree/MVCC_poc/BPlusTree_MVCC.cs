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


/* <id name=”Libraries Datastructures – MVCC - BPlusTree” />
 * <copyright file=”BPlusTree.cs”
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
using System.Diagnostics;
using sones.Lib.DataStructures.Indices;

#endregion


namespace sones.Lib.DataStructures.BPlusTree.MVCC
{
    class BPlusTree_MVCC<TKey, TValue>
                where TKey : IComparable
    {
        #region private members

        /// <summary>
        /// The root node of the B+Tree.
        /// </summary>
        private Node_MVCC<TKey, TValue> _Root;

        /// <summary>
        /// The Order k of the B*Tree
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
        internal LeafNode_MVCC<TKey, TValue> _LeftMostLeaf;

        /// <summary>
        /// Holds a reference to the right-most leaf. 
        /// </summary>
        internal LeafNode_MVCC<TKey, TValue> _RightMostLeaf;

        #endregion private members

        #region constructors

        /// <summary>
        /// Instantiates a BStarTree with a default order of k = 60
        /// </summary>
        public BPlusTree_MVCC()
            : this(60)
        {
        }
        
        /// <summary>
        /// Instantiates a BStarTree with a given order k
        /// </summary>
        /// <param name="myOrder"></param>
        public BPlusTree_MVCC(Int32 myOrder)
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

        public BPlusTree_MVCC(BPlusTree_MVCC<TKey, TValue> mySourceTree)
        {
            //copy root node reference
            lock (mySourceTree)
            {
                _Root               = mySourceTree._Root;
                _Order              = mySourceTree._Order;
                _Count              = mySourceTree._Count;
                _Height             = mySourceTree._Height;
                _KeyCount           = mySourceTree._KeyCount;
                _LeftMostLeaf       = mySourceTree._LeftMostLeaf;
                _RightMostLeaf      = mySourceTree._RightMostLeaf;
                _MaxChildrenPerNode = mySourceTree._MaxChildrenPerNode;
                _MinChildrenPerNode = mySourceTree._MinChildrenPerNode;
                _MaxKeysPerNode     = mySourceTree._MaxKeysPerNode;
                _MinKeysPerNode     = mySourceTree._MinKeysPerNode;
            }
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
        /// Get all Keys in the tree.
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
        /// Remove a Key and all associated Values from the tree.
        /// </summary>
        /// <param name="key">TKey</param>
        /// <returns>true if deleted</returns>
        public bool Remove(TKey key)
        {
            return RemoveKeyInternal(key);
        }

        /// <summary>
        /// Returns all values of the tree as ICollection
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
        /// Adds a key and it's value into the tree.
        /// If the key already exists, the value is added to the key's associated value-hashset.
        /// myKey must implement IComparable
        /// </summary>
        /// <param name="myKey">The Key to be added</param>
        /// <param name="myValue">The Value associated to the key</param>
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
                InnerNode_MVCC<TKey, TValue> innerNode = new InnerNode_MVCC<TKey, TValue>(this);

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
        private bool ContainsKeyInternal(Node_MVCC<TKey, TValue> myNode, TKey myKey)
        {
            //make some room to store the leaf associated to the key
            LeafNode_MVCC<TKey, TValue> leaf;

            return ContainsKeyInternal(myNode, myKey, out leaf);
                
        }

        /// <summary>
        /// Recursive search for myKey. If the search-node is internal, it searches for smaller for the right child
        /// and continues search. If the method reaches a leaf node it checks, if the key is contained in that node.
        /// 
        /// If the key is found in the tree, a reference to the corresponding leaf node is stored in an extra reference variable
        /// </summary>
        /// <param name="myNode">The node which shall be searched.</param>
        /// <param name="myKey">They key to search for.</param>
        /// <param name="myCorrespondingLeafNode">A reference to the leaf node where the key is located</param>
        /// <returns>True if the node contains that key.</returns>
        private bool ContainsKeyInternal(Node_MVCC<TKey, TValue> myNode, TKey myKey, out LeafNode_MVCC<TKey, TValue> myCorrespondingLeafNode)
        {
            if (myNode is LeafNode_MVCC<TKey, TValue>) //we reached the bottom of the tree
            {
                //store the leaf node
                myCorrespondingLeafNode = myNode as LeafNode_MVCC<TKey, TValue>;
                //search the leaf Keys for myKey using binary search between 0 and the number of keys contained in that node
                return myNode.ContainsKey(myKey);
            }

            //we are at an inner node and search for a way to go
            int nextChildIndex = myNode.FindSlot(myKey) + 1;
            //go recursively to the next child node
            return ContainsKeyInternal(((InnerNode_MVCC<TKey, TValue>)myNode).Children[nextChildIndex], myKey, out myCorrespondingLeafNode);
        }

        /// <summary>
        /// Initializes the tree.
        /// </summary>
        private void InitRootNode()
        {
            #region node

            _Root           = new LeafNode_MVCC<TKey, TValue>(this);
            _Root.Parent    = null;
            _Root.KeyCount  = 0;

            #endregion

            #region tree

            this._LeftMostLeaf  = _Root as LeafNode_MVCC<TKey, TValue>;
            this._RightMostLeaf = _Root as LeafNode_MVCC<TKey, TValue>;
            this._Height        = 1;

            #endregion
        }

        /// <summary>
        /// starts with the left-most leaf and adds all keys to a list, until the leaf has no right sibling
        /// </summary>
        /// <returns>a (sorted) collection which contains all keys of the tree</returns>
        private IEnumerable<TKey> GetAllKeys()
        {
            //using list to use add range
            List<TKey> resultCollection = new List<TKey>();

            //used to make one's way hand over hand along leafs
            LeafNode_MVCC<TKey, TValue> tmpLeaf = _LeftMostLeaf;

            //still a right sibling
            while (tmpLeaf != null)
            {
                //add all keys of the node
                for (Int32 i = 0; i < tmpLeaf.KeyCount; i++)
                {
                    yield return tmpLeaf.Keys[i];
                }
                tmpLeaf = tmpLeaf.RightSibling as LeafNode_MVCC<TKey, TValue>;                
            }
        }

        /// <summary>
        /// starts with the left-most leaf and yield returns all values associated to the keys
        /// </summary>
        /// <returns></returns>
        private IEnumerable<TValue> GetAllValues()
        {
            HashSet<TValue> resultCollection = new HashSet<TValue>();

            LeafNode_MVCC<TKey, TValue> tmpLeaf = _LeftMostLeaf;

            while (tmpLeaf != null)
            {
                for (Int32 i = 0; i < tmpLeaf.KeyCount; i++)
                {
                    foreach (TValue val in tmpLeaf.Values[i])
                    {
                        yield return val;
                    }
                }
                tmpLeaf = tmpLeaf.RightSibling as LeafNode_MVCC<TKey, TValue>;
            }
        }

        /// <summary>
        /// Methods goes down the tree 
        /// </summary>
        /// <param name="myCurrent"></param>
        /// <param name="myRemoveKey"></param>
        /// <returns></returns>
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

        #region add

        public void Add(TKey myKey, TValue myValue, out BPlusTree_MVCC<TKey, TValue> myLatestVersion)
        {
            Add(myKey, new HashSet<TValue>() { myValue }, out myLatestVersion);
        }

        public void Add(TKey myKey, HashSet<TValue> myValues, out BPlusTree_MVCC<TKey, TValue> myLatestVersion)
        {
            //create a new BPlusTree with the same internal structure references
            myLatestVersion = new BPlusTree_MVCC<TKey, TValue>(this);
            //and insert the key value pair into the copy
            myLatestVersion.AddInternal(myKey, myValues, IndexSetStrategy.MERGE);
        }

        #endregion

        #region trygetvalue

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
            LeafNode_MVCC<TKey, TValue> leaf;

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

        #endregion

        #region clear

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

        #region contains
        
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

        #region getter stuff

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
            LeafNode_MVCC<TKey, TValue> tmp = _LeftMostLeaf;

            while (tmp != null)
            {
                for (int i = 0; i < tmp.ValueCount; i++)
                {
                    yield return new KeyValuePair<TKey, HashSet<TValue>>(tmp.Keys[i], tmp.Values[i]);
                }
                tmp = (LeafNode_MVCC<TKey, TValue>) tmp.RightSibling;
            }            
        }

        public IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            LeafNode_MVCC<TKey, TValue> tmp = _LeftMostLeaf;

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
                tmp = (LeafNode_MVCC<TKey, TValue>)tmp.RightSibling;
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

        #region range methods

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

                LeafNode_MVCC<TKey, TValue> tmp;
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
                        tmp = (LeafNode_MVCC<TKey, TValue>)tmp.LeftSibling;
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
                    tmp = tmp.RightSibling as LeafNode_MVCC<TKey, TValue>;
                    //and start from the beginning
                    startIndex = 0;
                }

                #endregion
            }
            #endregion
        }


        #endregion

    }
}
