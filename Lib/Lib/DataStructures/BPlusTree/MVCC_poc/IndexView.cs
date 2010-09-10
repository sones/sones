/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
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
* 
*/

/* <id name=”Libraries Datastructures – MVCC - MultiVersionBplusTree” />
 * <copyright file=”IndexView.cs”
 *            company=”sones GmbH”>
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Martin Junghanns</developer>
 */

#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Collections.ObjectModel;
using sones.Lib.DataStructures.Indices;
using sones.Lib.DataStructures.Timestamp;

#endregion

namespace sones.Lib.DataStructures.BPlusTree.MVCC
{
    /// <summary>
    /// This class is part of the MultiVersionBPlusTree and represents one
    /// view at the index to a given timestamp.
    /// 
    /// IndexView consists of a modificated bplustree enhanced with version
    /// information. It holds the same information like a default bplustree
    /// and provides methods to access and modify the structure.
    /// </summary>
    /// <typeparam name="TKey">The key to identify a set of values</typeparam>
    /// <typeparam name="TValue">Type of the values which are identified by the key</typeparam>
    class IndexView<TKey, TValue>
                where TKey : IComparable
    {
        #region private members

        /// <summary>
        /// The root node of the B+Tree.
        /// </summary>
        private AVersionedNode<TKey, TValue> _Root;

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
        private UInt32 _Height;

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

        private ulong _Timestamp;

        /// <summary>
        /// Holds a reference to the left-most leaf. 
        /// </summary>
        internal VersionedLeafNode<TKey, TValue> _LeftMostLeaf;

        /// <summary>
        /// Holds a reference to the right-most leaf. 
        /// </summary>
        internal VersionedLeafNode<TKey, TValue> _RightMostLeaf;

        private object _LockObj = new object();

        #endregion private members

        #region constructors

        /// <summary>
        /// Instantiates a BStarTree with a default order of k = 60
        /// </summary>
        public IndexView()
            : this(60, TimestampNonce.Ticks)
        {
        }
        
        /// <summary>
        /// Instantiates a BStarTree with a given order k
        /// </summary>
        /// <param name="myOrder"></param>
        public IndexView(Int32 myOrder, ulong myTimestamp)
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

            _Timestamp          = myTimestamp;

            #endregion
        }

        /// <summary>
        /// Method clones the current tree-instance
        /// 
        /// The following members are copied:
        /// - Order
        /// - Count
        /// - KeyCount
        /// - Height
        /// - Min/Max Children/Key per node
        /// 
        /// The following references are copied:
        /// - Left/Rightmost Leaf
        /// - Root Node
        /// </summary>
        /// <returns>A clone of the tree-instance</returns>
        public IndexView<TKey, TValue> Clone()
        {
            lock (_LockObj)
            {
                var clone = new IndexView<TKey, TValue>();
                clone._Root = this._Root;

                clone._Order = this._Order;
                clone._Count = this._Count;
                clone._Height = this._Height;
                clone._KeyCount = this._KeyCount;

                clone._LeftMostLeaf = this._LeftMostLeaf;
                clone._RightMostLeaf = this._RightMostLeaf;

                clone._MaxChildrenPerNode = this._MaxChildrenPerNode;
                clone._MinChildrenPerNode = this._MinChildrenPerNode;
                clone._MaxKeysPerNode = this._MaxKeysPerNode;
                clone._MinKeysPerNode = this._MinKeysPerNode;

                return clone;
            }
        }

        #endregion

        #region getter / setter

        /// <summary>
        /// Returns the number of values in the tree
        /// </summary>
        public ulong Count
        {
            get 
            {
                lock (_LockObj)
                {
                    return _Count;
                }
            }
            set 
            {
                lock (_LockObj)
                {
                    _Count = value;
                }
            }
        }

        /// <summary>
        /// Returns the number of keys in the tree
        /// </summary>
        public ulong KeyCount
        {
            get 
            {
                lock (_LockObj)
                {
                    ulong keyCount = 0;
                    var kvpEnum = GetEnumerator();

                    while (kvpEnum.MoveNext())
                    {
                        if (kvpEnum.Current.Value.Count > 0) //not deleted
                        {
                            keyCount++;
                        }
                    }

                    return keyCount;
                }
            }
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
        public UInt32 Height
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
        public Trinary ContainsKey(TKey myKey)
        {
            if (IsTreeInitialized())
            {
                return ContainsKeyInternal(_Root, myKey);
            }
            else
            {
                return Trinary.FALSE;
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
        public bool Remove(VersionInfoDTO<TKey, TValue> versionInfoDTO, TKey key, TValue myValue = default(TValue))
        {
            return RemoveKeyInternal(versionInfoDTO, key, myValue);
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
        /// Returns / Sets the ID (as TimeStamp) of that tree
        /// used by the MultiVersionBPlusTree
        /// </summary>
        public ulong Timestamp
        {
            get
            {
                lock (_LockObj)
                {
                    return _Timestamp;
                }
            }
            set
            {
                lock (_LockObj)
                {
                    _Timestamp = value;
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
        private AVersionedNode<TKey, TValue> AddInternal(TKey myKey, HashSet<TValue> myValue, IndexSetStrategy myIndexSetStrategy, VersionInfoDTO<TKey, TValue> myVersionInfoDTO)
        {
            if (!IsTreeInitialized()) //tree is empty
            {
                //create a new root for latest version and the clone
                this.InitRootNode(); 
                myVersionInfoDTO.Tree.InitRootNode();
            }
            //used to handle split information
            NodeInfoStorage_MVCC<TKey, TValue> splitInfo = new NodeInfoStorage_MVCC<TKey, TValue>();

            //insert into the tree
            if (_Root.Insert(myKey, myValue, ref splitInfo, myIndexSetStrategy, myVersionInfoDTO))
            {
                //root was splitted we have to create a new one
                //create the fist inner node
                VersionedInnerNode<TKey, TValue> innerNode = new VersionedInnerNode<TKey, TValue>(this);
                //InnerNode_MVCC<TKey, TValue> innerNode = new InnerNode_MVCC<TKey, TValue>(myCorrespondingTree);

                //first key in the root is the propagad
                innerNode.Keys[0] = splitInfo.Key;
                //left child is old root
                innerNode.Children[0] = splitInfo.CloneNode;
                //right child is new splitted node
                innerNode.Children[1] = splitInfo.SplitNode;
                //left childs parent is new root
                innerNode.Children[0].Parent = innerNode;
                //right childs parent is new root
                innerNode.Children[1].Parent = innerNode;
                //and update the keyCount
                innerNode.KeyCount++;

                //set the new inner node as root node
                //myCorrespondingTree._Root   = innerNode;

                //increment the height of the tree
                myVersionInfoDTO.Tree._Height++;

                //return the new root node for that version (new node)
                return innerNode;
            }
            //return the new root node for that version (cloned root)
            return splitInfo.CloneNode;
        }

        /// <summary>
        /// Recursive search for myKey. If the search-node is internal, it searches for smaller for the right child
        /// and continues search. If the method reaches a leaf node it checks, if the key is contained in that node.
        /// </summary>
        /// <param name="myNode">The node which shall be searched.</param>
        /// <param name="myKey">They key to search for.</param>
        /// <returns>True if the node contains that key.</returns>
        private Trinary ContainsKeyInternal(AVersionedNode<TKey, TValue> myNode, TKey myKey)
        {
            //make some room to store the leaf associated to the key
            VersionedLeafNode<TKey, TValue> leaf;

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
        private Trinary ContainsKeyInternal(AVersionedNode<TKey, TValue> myNode, TKey myKey, out VersionedLeafNode<TKey, TValue> myCorrespondingLeafNode)
        {
            if (myNode is VersionedLeafNode<TKey, TValue>) //we reached the bottom of the tree
            {
                //store the leaf node
                myCorrespondingLeafNode = myNode as VersionedLeafNode<TKey, TValue>;
                //search the leaf Keys for myKey using binary search between 0 and the number of keys contained in that node
                int index;
                if(myNode.ContainsKey(myKey, out index))
                {
                    //deleted?
                    if (myCorrespondingLeafNode.Values[index].Count == 0)
                    {
                        return Trinary.DELETED;
                    }
                    //key exists
                    return Trinary.TRUE;
                }
                //key not exist
                return Trinary.FALSE;
            }

            //we are at an inner node and search for a way to go
            int nextChildIndex = myNode.FindSlot(myKey) + 1;
            //go recursively to the next child node
            return ContainsKeyInternal(((VersionedInnerNode<TKey, TValue>)myNode).Children[nextChildIndex], myKey, out myCorrespondingLeafNode);
        }

        /// <summary>
        /// Initializes the tree.
        /// </summary>
        private void InitRootNode()
        {
            #region node

            _Root           = new VersionedLeafNode<TKey, TValue>(this);
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
            //using list to use add range
            List<TKey> resultCollection = new List<TKey>();

            //used to make one's way hand over hand along leafs
            VersionedLeafNode<TKey, TValue> tmpLeaf = _LeftMostLeaf;

            //still a right sibling
            while (tmpLeaf != null)
            {
                //add all keys of the node
                for (Int32 i = 0; i < tmpLeaf.KeyCount; i++)
                {
                    yield return tmpLeaf.Keys[i];
                }
                tmpLeaf.RightSiblings.TryGetValue(Timestamp, out tmpLeaf);
            }
        }

        /// <summary>
        /// starts with the left-most leaf and yield returns all values associated to the keys
        /// </summary>
        /// <returns></returns>
        private IEnumerable<TValue> GetAllValues()
        {
            HashSet<TValue> resultCollection = new HashSet<TValue>();

            VersionedLeafNode<TKey, TValue> tmpLeaf = _LeftMostLeaf;

            while (tmpLeaf != null)
            {
                for (Int32 i = 0; i < tmpLeaf.KeyCount; i++)
                {
                    foreach (TValue val in tmpLeaf.Values[i])
                    {
                        yield return val;
                    }
                }
                tmpLeaf.RightSiblings.TryGetValue(Timestamp, out tmpLeaf);
            }
        }

        /// <summary>
        /// Methods goes down the tree 
        /// </summary>
        /// <param name="myCurrent"></param>
        /// <param name="myRemoveKey"></param>
        /// <returns></returns>
        private bool RemoveKeyInternal(VersionInfoDTO<TKey, TValue> versionInfoDTO, TKey myKey, TValue myValue = default(TValue))
        {
            AVersionedNode<TKey, TValue> copyNode = null;

            if (_Root.Remove(versionInfoDTO, ref copyNode, myKey, myValue))
            {
                versionInfoDTO.Tree._Root = copyNode;
                return true;
            }

            return false;
        }

        #endregion

        #region add

        public void Add(TKey myKey, TValue myValue, VersionInfoDTO<TKey, TValue> myVersionInfoDTO)
        {
            Add(myKey, new HashSet<TValue>() { myValue }, myVersionInfoDTO);
        }

        public void Add(TKey myKey, HashSet<TValue> myValues, VersionInfoDTO<TKey, TValue> myVersionInfoDTO)
        {
            //during insert a new copy of root accrues which will be the new root of that version of the tree
            myVersionInfoDTO.Tree._Root = AddInternal(myKey, myValues, IndexSetStrategy.MERGE, myVersionInfoDTO);
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
            if (!IsTreeInitialized())
            {
                myValues = new HashSet<TValue>();
                return false;
            }
            //used to store the leaf which contains the key
            VersionedLeafNode<TKey, TValue> leaf;

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
            VersionedLeafNode<TKey, TValue> tmp = _LeftMostLeaf;

            while (tmp != null)
            {
                for (int i = 0; i < tmp.KeyCount; i++)
                {
                    yield return new KeyValuePair<TKey, HashSet<TValue>>(tmp.Keys[i], tmp.Values[i]);
                }
                tmp = tmp.GetRightNeighbourByVersion(Timestamp);
            }            
        }

        public IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            VersionedLeafNode<TKey, TValue> tmp = _LeftMostLeaf;

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
                tmp = tmp.RightSiblings[Timestamp];
            }  
        }

        //public bool Remove(ref BPlusTree_MVCC<TKey, TValue> myCorrespondingTree, TKey myKey, TValue myValue = default(TValue))
        //{
        //    return false;
        //}

        public bool Remove(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            throw new NotImplementedException();
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
            //set
            //{
            //    AddInternal(myKey, value, IndexSetStrategy.REPLACE);
            //}
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

                VersionedLeafNode<TKey, TValue> tmp;
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
                    if (tmp.LeftSiblings[Timestamp] != null)
                    {
                        tmp = tmp.LeftSiblings[Timestamp];
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
                    tmp = tmp.RightSiblings[Timestamp];
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
