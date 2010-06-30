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
using System.Text;
using sones.Lib.DataStructures.Indices;

#endregion

namespace sones.Lib.DataStructures.BPlusTree.MVCC
{
    class LeafNode_MVCC<TKey, TValue> : Node_MVCC<TKey, TValue>        
        where TKey : IComparable
    {

        #region private members

        /// <summary>
        /// Holds the values (each key can have a set of values assigned)
        /// </summary>
        private HashSet<TValue>[] _Values;
        
        #endregion

        #region constructors

        /// <summary>
        /// creates a a new Leaf Node
        /// </summary>
        /// <param name="myTree"></param>
        public LeafNode_MVCC(BPlusTree_MVCC<TKey, TValue> myTree) : base(myTree)
        {
            _Values = new HashSet<TValue>[myTree.MaxChildrenPerNode + 1];           
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
        public override bool Insert(TKey myKey, HashSet<TValue> myValue, ref SplitInfo<TKey, TValue> mySplitInfo, IndexSetStrategy myIndexSetStrategy)
        {
            #region data

            Int32 arrayIndex    = this.KeyCount - 1; //used to access the internal arrays
            bool added          = false; //used to check if key already exists, and we just add a new value

            #endregion

            #region insert

            if (!this.IsEmpty())
            {
                /* start searching from the most right element 
                 * in the node to find the 
                 * correct slot to insert the new key                 
                 */
                //while myKey is smaller and we're not at the most left key
                while ((arrayIndex >= 0) && (myKey.CompareTo(this.Keys[arrayIndex]) == -1))
                {
                    //..move current element one step right
                    this.Keys[arrayIndex + 1] = this.Keys[arrayIndex];
                    this.Values[arrayIndex + 1] = this.Values[arrayIndex];
                    arrayIndex--;
                }
                //check if that key already exists (key-collision)
                if (arrayIndex >= 0 && myKey.CompareTo(this.Keys[arrayIndex]) == 0)
                {
                    //subtract hashset count from tree count
                    _CorrespondingTree.Count -= (ulong)this.Values[arrayIndex].Count;
                    switch (myIndexSetStrategy)
                    {
                        case IndexSetStrategy.REPLACE:
                            //assign new HashMap
                            this.Values[arrayIndex] = myValue;
                            break;
                        default: //it's merging
                            //merge existing and new hashset
                            this.Values[arrayIndex].AddRange<TValue>(myValue);
                            //and update the tree count
                            break;
                    }

                    //update global KeyCount
                    _CorrespondingTree.Count += (ulong)this.Values[arrayIndex].Count;
                    
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

                this.Values[arrayIndex] = myValue;

                //increment the element and value count for that node and the value count for the whole tree
                _KeyCount++;
                _CorrespondingTree.KeyCount++;
                _CorrespondingTree.Count += (ulong) myValue.Count;
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
                LeafNode_MVCC<TKey, TValue> newLeaf = new LeafNode_MVCC<TKey, TValue>(_CorrespondingTree);
                                
                //get the element in the middle, it must be attached to the parent node
                Int32 splitIndex    = _KeyCount >> 1;
                var middleKey       = _Keys[splitIndex];

                #endregion

                #region siblings

                //set the siblings (new node is right of splitnode)
                newLeaf.LeftSibling     = this;
                newLeaf.RightSibling    = this.RightSibling;
                this.RightSibling       = newLeaf;

                #endregion

                #region parent

                //set the parent for the new node
                newLeaf.Parent = this.Parent;

                #endregion

                #region update trees rightmost leaf

                _CorrespondingTree._RightMostLeaf = newLeaf;

                #endregion

                #region moving key-value-pairs

                //move middle key-value-pair and all key-value-pairs greater then the middle key into the new Node
                Transfer(splitIndex, 0, _KeyCount - splitIndex, newLeaf);

                #endregion

                #region propagate split information

                //set split information to be handled by calling node
                mySplitInfo.Key     = middleKey;
                mySplitInfo.Node    = newLeaf;

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
        protected override void Transfer(int mySourceStartIndex, int myDestinationStartIndex, int myLength, Node_MVCC<TKey, TValue> myDestinationNode)
        {
            //it's a leaf node, so cast it
            LeafNode_MVCC<TKey, TValue> destinationNode = myDestinationNode as LeafNode_MVCC<TKey, TValue>;

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

        /// <summary>
        /// Array.Copy works like that
        ///              
        ///  post: 
        ///  { 0, 1, 2, 3, 4, 5 }
        ///  toDeleteIndex = 2
        ///  
        ///  pre:
        ///  { 0, 1, 3, 4, 5, 5 }
        ///  dec(keyCount)
        /// </summary>
        /// <param name="myIndex"></param>
        /// <returns></returns>
        protected override bool RemoveInternal(TKey myKey)
        {
            int myIndex;
            if (!ContainsKey(myKey, out myIndex))
            {
                //nothing to remove in here
                return false;
            }
            else
            {
                //check how many values will be deleted
                var valueCount = _Values[myIndex].Count;
                //move keys greater then myIndex one slot left
                Array.Copy(_Keys, myIndex + 1, _Keys, myIndex, _KeyCount - myIndex - 1);

                //move values greater then myIndex one slot left
                Array.Copy(_Values, myIndex + 1, _Values, myIndex, _KeyCount - myIndex - 1);

                //set the last value to it's default value
                _Keys[KeyCount - 1] = default(TKey);
                //and the last HashSet is dereferenced
                _Values[KeyCount - 1] = null;

                //finally increment node and tree keyCount / valueCount
                _KeyCount--;
                _CorrespondingTree.KeyCount--;
                _CorrespondingTree.Count -= (ulong) valueCount;


                //if the removed key was the most left, we have a parent and it wasn't the last key left
                if (myIndex == 0  && _KeyCount > 0 && Parent != null)
                {
                    int keySlotInParent;
                    if(Parent.ContainsKey(myKey, out keySlotInParent))
                    {
                        Parent.Keys[keySlotInParent] = _Keys[0];
                    }
                }

                //done
                return true;
            }
        }

        protected override Node_MVCC<TKey, TValue> Clone()
        {
            var node = (LeafNode_MVCC<TKey, TValue>)this.MemberwiseClone();

            node._Keys      = (TKey[])this._Keys.Clone();
            node._Values    = (HashSet<TValue>[])this._Values.Clone();

            return node;
        }

        #endregion
    }
}
