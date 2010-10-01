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

/* <id name=”Libraries Datastructures – MVCC - MultiVersionBPlusTree” />
 * <copyright file=”VersionedInnerNode.cs”
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

#endregion

namespace sones.Lib.DataStructures.BPlusTree.MVCC
{
    /// <summary>
    /// An inner node within an IndexView. It's equal to an inner node
    /// in a default bplustree except the possibility to clone the node
    /// during modify-operations.
    /// </summary>
    /// <typeparam name="TKey">The type of the key which identifies the values</typeparam>
    /// <typeparam name="TValue">The values which are identified by the key</typeparam>
    class VersionedInnerNode<TKey, TValue> : AVersionedNode<TKey, TValue>        
        where TKey : IComparable
    {

        #region private members

        /// <summary>
        /// The Chidren array holds the referenes to the nodes which
        /// have this node as it's parent.
        /// 
        /// children[first] points at a node which contains keys that are
        /// all smaller then the smallest key in here.
        /// childen[last] points at a node which contains keys that are all
        /// greater or equal to the greatest key in here.
        /// all pointers between point on nodes which contain keys that are
        /// greater or equal to the key which has the same index as the children.
        /// 
        /// example:
        /// 
        /// k1-3 are ordered key in that node
        /// p1-4 point on other nodes considering the information written above
        /// 
        ///  |k1|k2|k3|
        /// |p1|p2|p3|p4|
        ///  
        /// </summary>
        private AVersionedNode<TKey, TValue>[] _Children;

        #endregion

        #region constructors

        public VersionedInnerNode(IndexView<TKey, TValue> myTree) : base(myTree)
        {
            _Children = new AVersionedNode<TKey, TValue>[myTree.MaxChildrenPerNode + 1] ;
        }

        #endregion

        #region getter / setter

        /// <summary>
        /// Sets / Returns the children of that innernode
        /// </summary>
        public AVersionedNode<TKey, TValue>[] Children
        {
            get { return _Children; }
            set { _Children = value; }
        }

        #endregion

        #region public members

        public override bool Insert(TKey myKey, HashSet<TValue> myValue, ref NodeInfoStorage_MVCC<TKey, TValue> mySplitInfo, IndexSetStrategy myIndexSetStrategy, VersionInfoDTO<TKey, TValue> myVersionInfoDTO)
        {
            #region data

            Int32 arrayIndex = KeyCount - 1; //used to access the internal arrays
            bool returnVal;

            #endregion

            #region find slot

            //search for the next node to go
            //if the current key is smaller then the new one, decrement i
            while ((arrayIndex >= 0) && (myKey.CompareTo(Keys[arrayIndex]) == -1))
            {
                arrayIndex--;
            }

            //increment i to access the correct children slot
            arrayIndex++;

            //go down the tree
            returnVal = Children[arrayIndex].Insert(myKey, myValue, ref mySplitInfo, myIndexSetStrategy, myVersionInfoDTO);

            #region clone

            var clonedINode = this.Clone() as VersionedInnerNode<TKey, TValue>;

            clonedINode._CorrespondingTree = myVersionInfoDTO.Tree;

            #endregion

            #endregion

            if (returnVal == SPLITTED)
            {
                /*
                 * the child node was splitted, so we have to add the key and set the new child reference
                 */
                for (Int32 i = clonedINode.KeyCount; i > arrayIndex; i--)
                {
                    clonedINode._Keys[i] = clonedINode._Keys[i - 1];
                    clonedINode._Children[i + 1] = clonedINode._Children[i];
                }

                //insert the propagated key and children
                clonedINode._Keys[arrayIndex] = mySplitInfo.Key;
                //update the node to point to the cloned child
                clonedINode._Children[arrayIndex] = mySplitInfo.CloneNode;
                //and refer to the new child
                clonedINode._Children[arrayIndex + 1] = mySplitInfo.SplitNode;

                //increment the key count
                clonedINode._KeyCount++;

                 /*
                 * now we have to check if the extra key was too much for our node
                 * if it's full, we have to split that inner node in two and propagate
                 * the splitkey to the parent node
                 */
                if(clonedINode.IsFull())
                {
                    #region splitting

                    #region data

                    //create a new node which contains all keys and references greater then the middle key
                    VersionedInnerNode<TKey, TValue> newNode = new VersionedInnerNode<TKey, TValue>(clonedINode._CorrespondingTree);
                    //the index where we split that node
                    Int32 splitIndex    = clonedINode._KeyCount / 2;
                    //the key located at that index
                    var middleKey       = clonedINode._Keys[splitIndex];

                    #endregion

                    //#region siblings

                    ////set the siblings (new node is right of splitnode)
                    //newNode.LeftSibling         = clonedINode;
                    //newNode.RightSibling        = clonedINode.RightSibling;
                    //clonedINode.RightSibling    = newNode;

                    //#endregion

                    #region parent of new and children

                    //set the parent for the new node
                    newNode.Parent = clonedINode.Parent;
                    
                    #endregion

                    #region moving keys and children

                    //move middle key and all keys then the middle key into the new Node
                    clonedINode.Transfer(splitIndex + 1, 0, clonedINode._KeyCount - splitIndex - 1, newNode);

                    //remove the last key-child pair in the old node
                    clonedINode._Keys[_KeyCount - 1] = default(TKey);
                    clonedINode._Children[_KeyCount] = null;
                    clonedINode._KeyCount--;

                    #endregion

                    #region propagate splitinformation

                    /*
                     * the has to be inserted in the parent node
                     * and the new node is its right child because
                     * it contains all keys greater or equal then the
                     * inserted on
                     */
                    mySplitInfo.Key         = middleKey;
                    mySplitInfo.SplitNode   = newNode;
                    mySplitInfo.CloneNode   = clonedINode;

                    #endregion

                    //done at that node
                    //tell the parent node, that there's some work
                    return SPLITTED;

                    #endregion
                }
            }

            //update the node to point to the cloned child
            clonedINode._Children[arrayIndex] = mySplitInfo.CloneNode;
            //and propagate the cloning up to the root
            mySplitInfo.CloneNode = clonedINode;

            return NOT_SPLITTED;
        }

        #endregion

        #region protected members

        /// <summary>
        /// Transfers entries of an inner node to another or the same node.
        /// </summary>
        /// <param name="mySourceStartIndex">Index to from in the source node</param>
        /// <param name="myDestinationStartIndex">Index to start from in the destination node</param>
        /// <param name="myLength">Number of entries to transfer</param>
        /// <param name="myDestinationNode">The destination node (can be the same as source node)</param>
        protected override void Transfer(int mySourceStartIndex, int myDestinationStartIndex, int myLength, AVersionedNode<TKey, TValue> myDestinationNode)
        {
            //it's a leaf node, so cast it
            VersionedInnerNode<TKey, TValue> destinationNode = myDestinationNode as VersionedInnerNode<TKey, TValue>;

            /*
             * node looks like that
             * 
             *      k1 k2 k3 k4 k5
             *     p1 p2 p3 p4 p5 p6
             * 
             */
            int sourceIndex;
            int targetIndex;

            //if we transfer within the same node and we transfer from left to right we must use reverse order
            if (this.Equals(destinationNode) && mySourceStartIndex < myDestinationStartIndex)
            {
                int index;
                for (index = myDestinationStartIndex - 1; index >= 0; index--)
                {
                    //transfer keys
                    _Keys[index + myLength] = _Keys[index];
                    //transfer values
                    _Children[index + myLength + 1] = _Children[index + 1];
                    //set old key to default value
                    _Keys[index] = default(TKey);
                    //and value hashset null
                    _Children[index + 1] = null;
                }
                //move the left child
                _Children[index + myLength + 1] = _Children[index + 1];
                _Children[index + 1] = null;
            }
            else
            {
                //hack, used to check if something happened in the loop
                bool moved = false;

                //different nodes or same node but transfer from right to left
                for (sourceIndex = mySourceStartIndex, targetIndex = myDestinationStartIndex; sourceIndex < (mySourceStartIndex + myLength); sourceIndex++, targetIndex++)
                {
                    moved = true;
                    //transfer keys
                    myDestinationNode.Keys[targetIndex] = _Keys[sourceIndex];
                    //transfer values
                    destinationNode.Children[targetIndex] = _Children[sourceIndex];
                    //update parent pointer
                    destinationNode.Children[targetIndex].Parent = destinationNode;
                    //default value for key
                    _Keys[sourceIndex] = default(TKey);
                    //and child
                    _Children[sourceIndex] = null;
                    //count values
                    _KeyCount--;
                    myDestinationNode.KeyCount++;
                }
                if (moved)
                {
                    //move last child
                    destinationNode.Children[targetIndex] = _Children[sourceIndex];
                    //update childs parent
                    if (destinationNode.Children[targetIndex] != null) //is null in case of root removal
                    {
                        destinationNode.Children[targetIndex].Parent = destinationNode;
                    }
                    //and set source ref to null
                    _Children[sourceIndex] = null;
                }

                /*
                 * if the source index is less or equal then the destination index (left to right shift)
                 * we have to balance the node order.
                 * 
                 * Node looks like that now:
                 * 
                 *      - - - k4 k5
                 *     - - - p4 p5 p6
                 *     
                 * 
                 * we shift them to the beginning now
                 */
                if (mySourceStartIndex <= myDestinationStartIndex)
                {
                    //it's a right to left shift
                    Array.Copy(_Keys, (mySourceStartIndex + myLength), _Keys, 0, _KeyCount);
                    Array.Copy(_Children, (mySourceStartIndex + myLength + 1), _Children, 0, _KeyCount + 1);
                }
                /*
                 * now it looks like that
                 * 
                 *      k4 k5 - - -
                 *     p4 p5 p6 - - -
                 */
            }
        }
        
        public override bool Remove(VersionInfoDTO<TKey, TValue> versionInfoDTO, ref AVersionedNode<TKey, TValue> myCopy, TKey myKey, TValue myValue = default(TValue))
        {
            int nextChildIndex = FindSlot(myKey) + 1;

            if (_Children[nextChildIndex].Remove(versionInfoDTO, ref myCopy, myKey, myValue))
            {
                var clonedInnerNode = this.Clone() as VersionedInnerNode<TKey, TValue>;

                clonedInnerNode.Children[nextChildIndex] = myCopy;

                myCopy = clonedInnerNode;

                return true;
            }
            else
            {
                //not removed
                return false;
            }
        }

        public override AVersionedNode<TKey, TValue> Clone()
        {
            lock (_LockObject)
            {
                var node = new VersionedInnerNode<TKey, TValue>(this._CorrespondingTree);
            
                #region members

                node._DoBalance = this._DoBalance;
                node._KeyCount  = this._KeyCount;

                #endregion

                #region arrays

                //requires O(n) :/
                node._Keys      = (TKey[])this._Keys.Clone();
                node._Children  = (AVersionedNode<TKey, TValue>[])this._Children.Clone();

                #endregion

                return node;
            }
        }

        #endregion
    }
}
