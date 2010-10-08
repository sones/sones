/* <id name=”Libraries Datastructures – BStarTree” />
 * <copyright file=”VersionedLeafNode.cs”
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
using sones.Lib.DataStructures.Indices;
using sones.Lib.DataStructures.Timestamp;

#endregion

namespace sones.Lib.DataStructures.BPlusTree.Versioned
{
    class VersionedLeafNode<TKey, TValue> : VersionedNode<TKey, TValue>
        where TKey : IComparable, IEstimable
        where TValue : IEstimable
    {

        #region private members

        /// <summary>
        /// Holds the values (each key can have a set of values assigned)
        /// </summary>
        private IndexValueHistoryList<TValue>[] _IndexValueHistoryList;

        #endregion

        #region constructors

        /// <summary>
        /// creates a a new Leaf Node
        /// </summary>
        /// <param name="myTree"></param>
        public VersionedLeafNode(VersionedBPlusTree<TKey, TValue> myTree)
            : base(myTree)
        {
            _IndexValueHistoryList = new IndexValueHistoryList<TValue>[myTree.MaxChildrenPerNode + 1];
        }

        #endregion

        #region Getter / Setter

        /// <summary>
        /// Sets / Returns the array HashSet of values associated to this leaf
        /// </summary>
        public IndexValueHistoryList<TValue>[] IndexValueHistoryLists
        {
            get { return _IndexValueHistoryList; }
            set { _IndexValueHistoryList = value; }
        }

        /// <summary>
        /// Sets / Returns the number of values of the latest revision contained in this leaf
        /// </summary>
        public Int32 ValueCount
        {
            get
            {
                Int32 valueCount = 0;

                for (int i = 0; i < _KeyCount; i++)
                {
                    valueCount += _IndexValueHistoryList[i].Values.Count;
                }
                return valueCount;
            }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Method inserts a key-value pair into the leaf node.
        ///
        /// It first inserts the key-value pair. After that a check is performed
        /// if the node has to be split. If the node is splitted, the middle key
        /// is stored in the reference param splitInfo to be handled by the calling
        /// parent node.
        /// If no split happened, all is fine, the key is inserted and we're done here.
        /// 
        /// If there is a duplicate key in that node, the new value is added to that key.
        /// It is NOT updated, because every key hold a reference to a HashSet which 
        /// does not allow duplicate values.
        /// </summary>
        /// <param name="myKey">The Key to be inserted</param>
        /// <param name="myValue">The Value belongig to the key</param>
        /// <param name="mySplitInfo">In case of split it contains the key to be inserted in the parent node.</param>
        /// <returns></returns>
        public override bool Insert(TKey myKey, HashSet<TValue> myValues, ref VersionedSplitInfo<TKey, TValue> mySplitInfo, IndexSetStrategy myIndexSetStrategy)
        {
            #region data

            Int32 arrayIndex;
            bool added          = false; //used to check if key already exists, and we just add a new value
            var timestamp       = TimestampNonce.Ticks;

            #endregion

            #region insert

            if (!this.IsEmpty())
            {                
                //check if leaf already contains that key
                if(!this.ContainsKey(myKey, out arrayIndex))
                {
                    /* 
                     * shift all elements one slot to the right until the 
                     * insert position is reached
                     */
                    arrayIndex = this.KeyCount - 1;
                    //while myKey is smaller and we're not at the most left key
                    while ((arrayIndex >= 0) && (myKey.CompareTo(this.Keys[arrayIndex]) == -1))
                    {
                        //..move current element one step right
                        this.Keys[arrayIndex + 1] = this.Keys[arrayIndex];
                        this.IndexValueHistoryLists[arrayIndex + 1] = this.IndexValueHistoryLists[arrayIndex];
                        arrayIndex--;
                    }
                }
                //check if that key already exists (key-collision)
                if (arrayIndex >= 0 && myKey.CompareTo(this.Keys[arrayIndex]) == 0)
                {
                    //check if the latest revision of that values is deleted, if yes, the trees keycount has to be incremented
                    if (IndexValueHistoryLists[arrayIndex].isLatestDeleted)
                    {
                        _CorrespondingTree.KeyCount++;
                    }
                    //subtract hashset count from tree count
                    _CorrespondingTree.Count -= (ulong)this.IndexValueHistoryLists[arrayIndex].Values.Count;

                    //add all new values to the IndexValueHistoryList
                    this.IndexValueHistoryLists[arrayIndex].Set(myValues, timestamp, myIndexSetStrategy);

                    //update global  value count
                    _CorrespondingTree.Count += (ulong)this.IndexValueHistoryLists[arrayIndex].Values.Count;

                    //remove old versions if necessary
                    while (this.IndexValueHistoryLists[arrayIndex].VersionCount > this._CorrespondingTree.HistorySize)
                    {
                        this.IndexValueHistoryLists[arrayIndex].RemoveLatestFromHistory();
                    }

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
                this.Keys[arrayIndex] = myKey;

                this.IndexValueHistoryLists[arrayIndex] = new IndexValueHistoryList<TValue>(myValues);

                //increment the element and value count for that node and the value count for the whole tree
                _KeyCount++;
                _CorrespondingTree.KeyCount++;
                _CorrespondingTree.Count += (ulong)myValues.Count;
            }

            #endregion

            #region splitting

            if (this.IsFull())
            {
                /*
                 * inserting the new key has persuaded our leaf to split
                 */
                #region splitting

                #region data

                //at first we create a new leaf
                VersionedLeafNode<TKey, TValue> newLeaf = new VersionedLeafNode<TKey, TValue>(_CorrespondingTree);

                //get the element in the middle, it must be attached to the parent node
                Int32 splitIndex = _KeyCount >> 1;
                var middleKey = _Keys[splitIndex];

                #endregion

                #region siblings

                //set the siblings (new node is right of splitnode)
                newLeaf.LeftSibling = this;
                newLeaf.RightSibling = this.RightSibling;
                this.RightSibling = newLeaf;

                #endregion

                #region parent

                //set the parent for the new node
                newLeaf.Parent = this.Parent;

                #endregion

                #region update trees rightmost leaf

                if (_CorrespondingTree._RightMostLeaf == this)
                {
                    _CorrespondingTree._RightMostLeaf = newLeaf;
                }

                #endregion

                #region moving key-value-pairs

                //move middle key-value-pair and all key-value-pairs greater then the middle key into the new Node
                Transfer(splitIndex, 0, _KeyCount - splitIndex, newLeaf);

                #endregion

                #region propagate split information

                //set split information to be handled by calling node
                mySplitInfo.Key = middleKey;
                mySplitInfo.Node = newLeaf;

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

                return NOT_SPLITTED;

                #endregion
            }

            #endregion
        }

        public override bool Insert(TKey myKey, IndexValueHistoryList<TValue> myValue, ref VersionedSplitInfo<TKey, TValue> mySplitInfo)
        {
            #region data

            Int32 arrayIndex;
            bool added = false; //used to check if key already exists, and we just add a new value
            var timestamp = TimestampNonce.Ticks;

            #endregion

            #region insert

            if (!this.IsEmpty())
            {
                //check if leaf already contains that key
                if (!this.ContainsKey(myKey, out arrayIndex))
                {
                    /* 
                     * shift all elements one slot to the right until the 
                     * insert position is reached
                     */
                    arrayIndex = this.KeyCount - 1;
                    //while myKey is smaller and we're not at the most left key
                    while ((arrayIndex >= 0) && (myKey.CompareTo(this.Keys[arrayIndex]) == -1))
                    {
                        //..move current element one step right
                        this.Keys[arrayIndex + 1] = this.Keys[arrayIndex];
                        this.IndexValueHistoryLists[arrayIndex + 1] = this.IndexValueHistoryLists[arrayIndex];
                        arrayIndex--;
                    }
                }
                //check if that key already exists (key-collision)
                if (arrayIndex >= 0 && myKey.CompareTo(this.Keys[arrayIndex]) == 0)
                {
                    //check if the latest revision of that values is deleted, if yes, the trees keycount has to be incremented
                    if (IndexValueHistoryLists[arrayIndex].isLatestDeleted)
                    {
                        _CorrespondingTree.KeyCount++;
                    }
                    //subtract hashset count from tree count
                    _CorrespondingTree.Count -= (ulong)this.IndexValueHistoryLists[arrayIndex].Values.Count;

                    //add all new values to the IndexValueHistoryList
                    this.IndexValueHistoryLists[arrayIndex] = myValue;

                    //update global  value count
                    _CorrespondingTree.Count += (ulong)this.IndexValueHistoryLists[arrayIndex].Values.Count;

                    //remove old versions if necessary
                    while (this.IndexValueHistoryLists[arrayIndex].VersionCount > this._CorrespondingTree.HistorySize)
                    {
                        this.IndexValueHistoryLists[arrayIndex].RemoveLatestFromHistory();
                    }

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
                this.Keys[arrayIndex] = myKey;

                this.IndexValueHistoryLists[arrayIndex] = myValue;
                
                _CorrespondingTree.KeyCount++;
                _CorrespondingTree.Count += (ulong)myValue[myValue.LatestTimestamp].Count;

                //increment leafs keycount
                _KeyCount++;
            }

            #endregion

            #region splitting

            if (this.IsFull())
            {
                /*
                 * inserting the new key has persuaded our leaf to split
                 */
                #region splitting

                #region data

                //at first we create a new leaf
                VersionedLeafNode<TKey, TValue> newLeaf = new VersionedLeafNode<TKey, TValue>(_CorrespondingTree);

                //get the element in the middle, it must be attached to the parent node
                Int32 splitIndex = _KeyCount >> 1;
                var middleKey = _Keys[splitIndex];

                #endregion

                #region siblings

                //set the siblings (new node is right of splitnode)
                newLeaf.LeftSibling = this;
                newLeaf.RightSibling = this.RightSibling;
                this.RightSibling = newLeaf;

                #endregion

                #region parent

                //set the parent for the new node
                newLeaf.Parent = this.Parent;

                #endregion

                #region update trees rightmost leaf

                if (_CorrespondingTree._RightMostLeaf == this)
                {
                    _CorrespondingTree._RightMostLeaf = newLeaf;
                }

                #endregion

                #region moving key-value-pairs

                //move middle key-value-pair and all key-value-pairs greater then the middle key into the new Node
                Transfer(splitIndex, 0, _KeyCount - splitIndex, newLeaf);

                #endregion

                #region propagate split information

                //set split information to be handled by calling node
                mySplitInfo.Key = middleKey;
                mySplitInfo.Node = newLeaf;

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
        protected override void Transfer(int mySourceStartIndex, int myDestinationStartIndex, int myLength, VersionedNode<TKey, TValue> myDestinationNode)
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
                    _IndexValueHistoryList[index + myLength] = _IndexValueHistoryList[index];
                    //set old key to default value
                    _Keys[index] = default(TKey);
                    //and value hashset null
                    _IndexValueHistoryList[index] = null;
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
                    destinationNode.IndexValueHistoryLists[targetIndex] = _IndexValueHistoryList[sourceIndex];
                    //default value for key
                    _Keys[sourceIndex] = default(TKey);
                    //default value for value
                    _IndexValueHistoryList[sourceIndex] = null;
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
                    Array.Copy(_IndexValueHistoryList, (mySourceStartIndex + myLength), _IndexValueHistoryList, 0, _KeyCount);
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
                
        #endregion
    }
}
