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
 * <copyright file=”LeafNode.cs”
 *            company=”sones GmbH”>
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Martin Junghanns</developer>
 */

#region usings

using System;
using System.Collections.Generic;
using System.Linq;
using sones.Lib.DataStructures.Indices;
using System.Collections.Concurrent;

#endregion

namespace sones.Lib.DataStructures.BPlusTree.MVCC
{
    /// <summary>
    /// A leaf node within an IndexView. It's equal to a leaf node
    /// in a default bplustree except the possibility to clone the node
    /// during modify-operations.
    /// </summary>
    /// <typeparam name="TKey">The type of the key which identifies the values</typeparam>
    /// <typeparam name="TValue">The values which are identified by the key</typeparam>
    class VersionedLeafNode<TKey, TValue> : AVersionedNode<TKey, TValue>        
        where TKey : IComparable
    {

        #region private members

        /// <summary>
        /// Holds the values (each key can have a set of values assigned)
        /// </summary>
        private HashSet<TValue>[] _Values;

        /// <summary>
        /// This concurrent dictionary contains references to all right siblings of that leaf.
        /// An entry is identified by the revision (timestamp) of the corresponding tree and
        /// holds a reference to the appropriate leaf.
        /// </summary>
        private ConcurrentDictionary<ulong, VersionedLeafNode<TKey, TValue>> _RightSiblings;

        /// <summary>
        /// This concurrent dictionary contains references to all left siblings of that leaf.
        /// An entry is identified by the revision (timestamp) of the corresponding tree and
        /// holds a reference to the appropriate leaf.
        /// </summary>
        private ConcurrentDictionary<ulong, VersionedLeafNode<TKey, TValue>> _LeftSiblings;

        #endregion

        #region constructors

        /// <summary>
        /// creates a a new Leaf Node
        /// </summary>
        /// <param name="myTree"></param>
        public VersionedLeafNode(IndexView<TKey, TValue> myTree) : base(myTree)
        {
            _Values         = new HashSet<TValue>[myTree.MaxChildrenPerNode + 1];
            _RightSiblings  = new ConcurrentDictionary<ulong, VersionedLeafNode<TKey, TValue>>();
            _LeftSiblings   = new ConcurrentDictionary<ulong, VersionedLeafNode<TKey, TValue>>();
        }

        #endregion

        #region Getter / Setter
                
        /// <summary>
        /// Sets / Returns the array HashSet of values associated to this leaf
        /// </summary>
        public HashSet<TValue>[] Values
        {
            get { return _Values; }
            set { _Values = value; }
        }

        /// <summary>
        /// Sets / Returns the number of values contained in this leaf
        /// </summary>
        public Int32 ValueCount
        {
            get 
            {
                Int32 valueCount = 0;

                for(int i = 0; i < _KeyCount; i++)
                {
                    valueCount += _Values[i].Count;
                }
                return valueCount; 
            }
        }

        /// <summary>
        /// Returns all left siblings of that leaf, siblings are identified by the revision
        /// corresponding tree.
        /// </summary>
        public ConcurrentDictionary<ulong, VersionedLeafNode<TKey, TValue>> LeftSiblings
        {
            get
            {
                return _LeftSiblings;
            }
        }

        /// <summary>
        /// Returns all right siblings of that leaf, siblings are identified by the revision
        /// corresponding tree.
        /// </summary>
        public ConcurrentDictionary<ulong, VersionedLeafNode<TKey, TValue>> RightSiblings
        {
            get
            {
                return _RightSiblings;
            }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Method inserts a key-value pair into the leaf node using the following steps:
        /// 
        /// 1)  Create a clone of that node, so that all read accesses can be performed without locking.
        /// 2)  Insert the key-value-pair into the cloned node.
        /// 3)  If the clone is not full goto step 5 else step 4.
        /// 4)  Create a new node and move all key-value-pairs right from the middle index into the new node.
        /// 5)  Propagate a reference of the cloned node and, if split happened, the split key and a
        ///     reference to the new node to the parent node.
        /// </summary>
        /// <param name="myKey">The Key to be inserted</param>
        /// <param name="myValue">The Value belongig to the key</param>
        /// <param name="myIndexSetStrategy">MERGE: merge current with new values, REPLACE: replace current with new values</param>
        /// <param name="mySplitInfo">In case of split it contains the key to be inserted in the parent node.</param>
        /// <returns></returns>
        public override bool Insert(TKey myKey, HashSet<TValue> myValue, ref NodeInfoStorage_MVCC<TKey, TValue> mySplitInfo, IndexSetStrategy myIndexSetStrategy, VersionInfoDTO<TKey, TValue> myVersionInfoDTO)
        {
            #region cloning

            var clonedLeaf = this.Clone() as VersionedLeafNode<TKey, TValue>;

            clonedLeaf._CorrespondingTree = myVersionInfoDTO.Tree;

            //if its the root node of the tree, the left and rightmost leaf have to be updated
            if (clonedLeaf.Parent == null) //root?
            {
                clonedLeaf._CorrespondingTree._LeftMostLeaf = clonedLeaf;
                clonedLeaf._CorrespondingTree._RightMostLeaf = clonedLeaf;
            }

            if (_CorrespondingTree._LeftMostLeaf == this)
            {
                myVersionInfoDTO.Tree._LeftMostLeaf = clonedLeaf;
            }
            if (_CorrespondingTree._RightMostLeaf == this)
            {
                myVersionInfoDTO.Tree._RightMostLeaf = clonedLeaf;
            }

            #endregion

            #region data

            Int32 arrayIndex    = this.KeyCount - 1; //used to access the internal arrays
            bool added          = false; //used to check if key already exists, and we just add a new value

            #endregion

            #region insert

            if (!clonedLeaf.IsEmpty())
            {
                /* start searching from the most right element 
                 * in the node to find the 
                 * correct slot to insert the new key                 
                 */
                //while myKey is smaller and we're not at the most left key
                while ((arrayIndex >= 0) && (myKey.CompareTo(this.Keys[arrayIndex]) == -1))
                {
                    //..move current element one step right
                    clonedLeaf.Keys[arrayIndex + 1] = clonedLeaf.Keys[arrayIndex];
                    clonedLeaf.Values[arrayIndex + 1] = clonedLeaf.Values[arrayIndex];
                    arrayIndex--;
                }
                //check if that key already exists (key-collision)
                if (arrayIndex >= 0 && myKey.CompareTo(clonedLeaf.Keys[arrayIndex]) == 0)
                {
                    //subtract hashset count from tree count
                    clonedLeaf._CorrespondingTree.Count -= (ulong)clonedLeaf.Values[arrayIndex].Count;
                    switch (myIndexSetStrategy)
                    {
                        case IndexSetStrategy.REPLACE:
                            //assign new HashMap
                            clonedLeaf.Values[arrayIndex] = myValue;
                            break;
                        default: //it's merging
                            //merge existing and new hashset
                            clonedLeaf.Values[arrayIndex].AddRange<TValue>(myValue);
                            //and update the tree count
                            break;
                    }

                    //update global KeyCount
                    clonedLeaf._CorrespondingTree.Count += (ulong)clonedLeaf.Values[arrayIndex].Count;
                    
                    //set added to true to avoid inserting at current position
                    added = true;
                }
                else
                {
                    arrayIndex++;
                }
            }
            else //first element in that node
            {
                arrayIndex = 0;
            }

            if (!added)
            {
                //insert new element into leaf
                clonedLeaf.Keys[arrayIndex] = myKey;

                clonedLeaf.Values[arrayIndex] = myValue;

                //increment the element and value count for that node and the value count for the whole tree
                clonedLeaf._KeyCount++;

                clonedLeaf._CorrespondingTree.Count += (ulong)myValue.Count;
            }

            #endregion

            #region splitting

            if (clonedLeaf.IsFull())
            {
                /*
                 * inserting the new key has persuaded our leaf to split
                 */
                #region splitting

                #region data

                //at first we create a new leaf
                VersionedLeafNode<TKey, TValue> newLeaf = new VersionedLeafNode<TKey, TValue>(clonedLeaf._CorrespondingTree);
                                
                //get the element in the middle, it must be attached to the parent node
                Int32 splitIndex    = clonedLeaf._KeyCount >> 1;
                var middleKey       = clonedLeaf._Keys[splitIndex];

                #endregion

                #region store the node and siblings for later updating

                //set the siblings (new node is right of splitnode)
                //connect cloned node and the new one (splitNode)
                newLeaf.LeftSiblings[newLeaf._CorrespondingTree.Timestamp] = clonedLeaf; // 1

                var clonedNodeInfo  = new SiblingInfoDTO<TKey, TValue>();
                var newLeafNodeInfo = new SiblingInfoDTO<TKey, TValue>();

                VersionedLeafNode<TKey, TValue> tmp = null;

                #region clonedNode Info

                //assign node
                clonedNodeInfo.Node = clonedLeaf;

                //right siblings
                clonedNodeInfo.RightSiblings.Add(newLeaf);

                //left siblings
                //if (clonedLeaf.LeftSiblings.TryGetValue(this._CorrespondingTree.Timestamp, out tmp))
                //{
                //    clonedNodeInfo.LeftSiblings.Add(tmp);
                //}
                clonedNodeInfo.LeftSiblings.AddRange(clonedLeaf.LeftSiblings.Values);

                #endregion

                #region newNode Info

                //assign node
                newLeafNodeInfo.Node = newLeaf;

                //right siblings
                //latest
                var tmp2 = clonedLeaf.RightSiblings.OrderByDescending(kvp => kvp.Key).FirstOrDefault().Value;

                if (tmp2 != null)
                {
                    newLeafNodeInfo.RightSiblings.Add(tmp2);
                }
                //if (clonedLeaf.RightSiblings.TryGetValue(this._CorrespondingTree.Timestamp, out tmp))
                //{
                //    newLeafNodeInfo.RightSiblings.Add(tmp);
                //}
                //newLeafNodeInfo.RightSiblings.AddRange(clonedLeaf.RightSiblings.Values);

                //left siblings
                newLeafNodeInfo.LeftSiblings.Add(clonedLeaf);

                #endregion

                //new leaf needs right siblings from clonedleaf
                foreach (var oldRightSibling in clonedLeaf.RightSiblings)
                {
                    while(!newLeaf.RightSiblings.TryAdd(oldRightSibling.Key, oldRightSibling.Value));
                }

                // used during atomic tree update
                myVersionInfoDTO.ModifiedNodes.Add(clonedNodeInfo);
                myVersionInfoDTO.ModifiedNodes.Add(newLeafNodeInfo);

                #endregion

                #region parent

                //set the parent for the new node
                newLeaf.Parent = clonedLeaf.Parent;

                #endregion

                #region update trees rightmost leaf if necessary

                if (myVersionInfoDTO.Tree._RightMostLeaf == clonedLeaf)
                {
                    myVersionInfoDTO.Tree._RightMostLeaf = newLeaf;
                }

                if (myVersionInfoDTO.Tree._LeftMostLeaf == clonedLeaf)
                {
                    myVersionInfoDTO.Tree._LeftMostLeaf = clonedLeaf;
                }

                #endregion

                #region moving key-value-pairs

                //move middle key-value-pair and all key-value-pairs greater then the middle key into the new Node
                clonedLeaf.Transfer(splitIndex, 0, clonedLeaf._KeyCount - splitIndex, newLeaf);

                #endregion

                #region propagate split information

                //set split information to be handled by calling node
                mySplitInfo.Key         = middleKey;
                mySplitInfo.SplitNode   = newLeaf;
                mySplitInfo.CloneNode   = clonedLeaf;

                #endregion

                //done
                return SPLITTED;

                #endregion
            }
            else
            {
                /*
                 * inserting was ok, we don't need to split
                 */
                #region no splitting

                #region store the node and siblings for later updating

                var clonedNodeInfo = new SiblingInfoDTO<TKey, TValue>();

                VersionedLeafNode<TKey, TValue> tmp;

                #region cloneNode Info

                //assign node
                clonedNodeInfo.Node = clonedLeaf;

                //left
                //if (clonedLeaf.LeftSiblings.TryGetValue(this._CorrespondingTree.Timestamp, out tmp))
                //{
                //    clonedNodeInfo.LeftSiblings.Add(tmp);
                //}
                //clonedNodeInfo.LeftSiblings.Add(clonedLeaf.LeftSiblings[this._CorrespondingTree.Timestamp]);

                //right
                //if (clonedLeaf.RightSiblings.TryGetValue(this._CorrespondingTree.Timestamp, out tmp))
                //{
                //    clonedNodeInfo.RightSiblings.Add(tmp);
                //}
                //clonedNodeInfo.RightSiblings.Add(clonedLeaf.RightSiblings[this._CorrespondingTree.Timestamp]);

                #endregion

                //old

                clonedNodeInfo.LeftSiblings.AddRange(clonedLeaf.LeftSiblings.Values);

                var tmp2 = clonedLeaf.RightSiblings.OrderByDescending(kvp => kvp.Key).FirstOrDefault().Value;

                if (tmp2 != null)
                {
                    clonedNodeInfo.RightSiblings.Add(tmp2);
                }
                //clonedNodeInfo.RightSiblings.AddRange(clonedLeaf.RightSiblings.Values);

                myVersionInfoDTO.ModifiedNodes.Add(clonedNodeInfo);

                #endregion

                #region update trees rightmost leaf if necessary

                if (this._CorrespondingTree._RightMostLeaf == this)
                {
                    myVersionInfoDTO.Tree._RightMostLeaf = clonedLeaf;
                }
                if (this._CorrespondingTree._LeftMostLeaf == this)
                {
                    myVersionInfoDTO.Tree._LeftMostLeaf = clonedLeaf;
                }

                #endregion

                //clone info
                mySplitInfo.CloneNode = clonedLeaf;

                return NOT_SPLITTED;

                #endregion
            }

            #endregion
        }

        /// <summary>
        /// Transfers entries of a leaf node to another or the same node.
        /// </summary>
        /// <param name="mySourceStartIndex">Index to from in the source node</param>
        /// <param name="myDestinationStartIndex">Index to start from in the destination node</param>
        /// <param name="myLength">Number of entries to transfer</param>
        /// <param name="myDestinationNode">The destination node (can be the same as source node)</param>
        protected override void Transfer(int mySourceStartIndex, int myDestinationStartIndex, int myLength, AVersionedNode<TKey, TValue> myDestinationNode)
        {
            //it's a leaf node, so cast it
            VersionedLeafNode<TKey, TValue> destinationNode = myDestinationNode as VersionedLeafNode<TKey, TValue>;

            /*
             * Node looks like that
             * 
             *  k1 k2 k3 k4 k5
             *  v1 v2 v3 v4 v5
             * 
             */
            //if we transfer within the same node and we transfer from left to right we must use reverse order
            if (this.Equals(destinationNode) && mySourceStartIndex < myDestinationStartIndex)
            {
                for (int index = myDestinationStartIndex - 1; index >= 0; index--)
                {
                    //transfer keys
                    _Keys[index + myLength] = _Keys[index];
                    //transfer values
                    _Values[index + myLength] = _Values[index];
                    //set old key to default value
                    _Keys[index] = default(TKey);
                    //and value hashset null
                    _Values[index] = null;
                }
            }
            else
            {
                //different nodes or we shift within the same node from right to left
                for (int sourceIndex = mySourceStartIndex, targetIndex = myDestinationStartIndex; sourceIndex < (mySourceStartIndex + myLength); sourceIndex++, targetIndex++)
                {
                    //transfer keys
                    myDestinationNode.Keys[targetIndex] = _Keys[sourceIndex];
                    //transfer values
                    destinationNode.Values[targetIndex] = _Values[sourceIndex];
                    //default value for key
                    _Keys[sourceIndex] = default(TKey);
                    //default value for value
                    _Values[sourceIndex] = null;
                    //count values
                    _KeyCount--;
                    myDestinationNode.KeyCount++;
                }

                /*
                 * if the source index is less or equal then the destination index (left to right shift)
                 * we have to balance the node order.
                 * 
                 * Node could look like that now:
                 * 
                 *  - - - k4 k5
                 *  - - - v4 v5
                 * 
                 * - are not need values
                 * 
                 * we shift them to the beginning now
                 */
                if (mySourceStartIndex <= myDestinationStartIndex)
                {
                    Array.Copy(_Keys, (mySourceStartIndex + myLength), _Keys, 0, _KeyCount);
                    Array.Copy(_Values, (mySourceStartIndex + myLength), _Values, 0, _KeyCount);
                }
                /*
                 * Node looks like that now:
                 * 
                 * k4 k5 - - -
                 * v4 v4 - - -
                 * 
                 */
            }
        }
        
        public override bool Remove(VersionInfoDTO<TKey, TValue> versionInfoDTO, ref AVersionedNode<TKey, TValue> myCopy, TKey myKey, TValue myValue = default(TValue))
        {
            int index;

            if (ContainsKey(myKey, out index))
            {
                #region cloning

                var clonedLeaf = this.Clone() as VersionedLeafNode<TKey, TValue>;

                clonedLeaf._CorrespondingTree = versionInfoDTO.Tree;

                //if its the root node of the tree, the left and rightmost leaf have to be updated
                if (clonedLeaf.Parent == null) //root?
                {
                    clonedLeaf._CorrespondingTree._LeftMostLeaf = clonedLeaf;
                    clonedLeaf._CorrespondingTree._RightMostLeaf = clonedLeaf;
                }

                #endregion

                #region removing

                if (!myValue.Equals(default(TValue)) && clonedLeaf.Values[index].Contains(myValue)) //remove single value
                {
                    clonedLeaf.Values[index].Remove(myValue);
                    clonedLeaf._CorrespondingTree.Count--;
                }
                else //remove all associated values
                {
                    var tmpCount = clonedLeaf.Values[index].Count;
                    clonedLeaf.Values[index].Clear();
                    clonedLeaf._CorrespondingTree.Count -= (ulong)tmpCount;
                }

                // key is not really deleted, but the keyCount is used to check if a node is full
                // 
                //if (clonedLeaf.Values[index].Count == 0)
                //{
                //    update keyCount of tree and leaf
                //    clonedLeaf.KeyCount--;
                //    clonedLeaf._CorrespondingTree.KeyCount--;
                //}

                #endregion

                myCopy = clonedLeaf;

                return true;
            }
            else
            {
                // nothing to remove
                return false;
            }
        }

        public VersionedLeafNode<TKey, TValue> GetRightNeighbourByVersion(ulong myTimestamp)
        {
            return this.RightSiblings.Where(kvp => kvp.Key <= myTimestamp).OrderByDescending(kvp => kvp.Key).FirstOrDefault().Value;
        }

        public VersionedLeafNode<TKey, TValue> GetLeftNeighbourByVersion(ulong myTimestamp)
        {
            return this.LeftSiblings.Where(kvp => kvp.Key <= myTimestamp).OrderByDescending(kvp => kvp.Key).FirstOrDefault().Value;
        }

        public override AVersionedNode<TKey, TValue> Clone()
        {
            lock (_LockObject)
            {
                var node = new VersionedLeafNode<TKey, TValue>(this._CorrespondingTree);

                #region members

                node._DoBalance = this._DoBalance;
                node._KeyCount = this._KeyCount;
                node.Parent = this.Parent;

                #endregion

                #region arrays

                //both require O(n) :/
                //clone keys
                node._Keys      = (TKey[])this._Keys.Clone();

                //clone values
                node._Values = new HashSet<TValue>[this._CorrespondingTree.MaxChildrenPerNode + 1];

                for (int i = 0; i < this.KeyCount; i++)
                {
                    node._Values[i] = new HashSet<TValue>(this._Values[i]);
                }

                #endregion

                #region siblings

                //right
                foreach (var rightSibling in this._RightSiblings)
                {
                    while(!node._RightSiblings.TryAdd(rightSibling.Key, rightSibling.Value));
                }

                //left
                foreach (var leftSibling in this._LeftSiblings)
                {
                    while(!node._LeftSiblings.TryAdd(leftSibling.Key, leftSibling.Value));
                }

                #endregion

                return node;
            }
        }

        #endregion
    }
}
