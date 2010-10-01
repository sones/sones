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

/* <id name=”Libraries Datastructures – BPlusTree” />
 * <copyright file=”Node.cs”
 *            company=”sones GmbH”>
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Martin Junghanns</developer>
 */

#region usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using sones.Lib.DataStructures.Indices;

#endregion

namespace sones.Lib.DataStructures.BPlusTree
{
    abstract class Node<TKey, TValue>
        where TKey : IComparable
    {
        #region private members

        /// <summary>
        /// Holds a reference to the parent node.
        /// </summary>
        private Node<TKey, TValue> _Parent;

        /// <summary>
        /// Holds a reference to the right Node
        /// </summary>
        private Node<TKey, TValue> _RightSibling;

        /// <summary>
        /// Holds a reference to the left Node
        /// </summary>
        private Node<TKey, TValue> _LeftSibling;

        #endregion

        #region protected members

        /// <summary>
        /// Holds a reference to the corresponding tree to access the settings
        /// </summary>
        protected BPlusTree<TKey, TValue> _CorrespondingTree;

        /// <summary>
        /// Holds the keys in that node
        /// </summary>
        protected TKey[] _Keys;        
        
        /// <summary>
        /// Count of keys currently inserted in that node.
        /// </summary>
        protected Int32 _KeyCount;

        /// <summary>
        /// Used when deleting keys and nodes have to be marked "balance-me"
        /// </summary>
        protected bool _DoBalance;

        #endregion

        #region contants

        /// <summary>
        /// Used as return value for better readability
        /// 
        /// true
        /// </summary>
        protected const bool SPLITTED = true;

        /// <summary>
        /// Used as return value for better readability
        /// 
        /// false
        /// </summary>
        protected const bool NOT_SPLITTED = false;

        #endregion

        #region constructors

        /// <summary>
        /// Instantiates a new Node with an empty ElementList
        /// </summary>
        public Node(BPlusTree<TKey, TValue> myTree)
        {
            _Keys               = new TKey[myTree.MaxKeysPerNode + 1];
            _CorrespondingTree  = myTree;
            _KeyCount           = 0;
            _LeftSibling        = null;
            _RightSibling       = null;
            _DoBalance          = false;
        }

        #endregion

        #region Getter / Setter

        /// <summary>
        /// Sets / Returns the right sibling of that node
        /// </summary>
        public Node<TKey, TValue> RightSibling
        {
            get { return _RightSibling; }
            set { _RightSibling = value; }
        }

        /// <summary>
        /// Sets / Returns the left sibling of that node
        /// </summary>
        public Node<TKey, TValue> LeftSibling
        {
            get { return _LeftSibling; }
            set { _LeftSibling = value; }
        }

        /// <summary>
        /// Returns / Sets the reference to the parent Node
        /// </summary>
        public Node<TKey, TValue> Parent
        {
            get { return _Parent; }
            set { _Parent = value; }
        }

        /// <summary>
        /// Returns / Set the current count of keys of that node
        /// </summary>
        public Int32 KeyCount
        {
            get { return _KeyCount; }
            set { _KeyCount = value; }
        }

        /// <summary>
        /// Returns / Set the current count of children of that node
        /// </summary>
        public Int32 ChildrenCount
        {
            get { return _KeyCount + 1; }
        }

        /// <summary>
        /// Returns / Sets the current list of attached NodeElements of that node
        /// </summary>
        public TKey[] Keys
        {
            get { return _Keys; }
            set { _Keys = value; }
        }

        /// <summary>
        /// Returns / Sets the "balance-me"-flag
        /// </summary>
        public bool DoBalance
        {
            get { return _DoBalance; }
            set { _DoBalance = value; }
        }
      
        #endregion

        #region protected abstract members

        /// <summary>
        /// Abstract Method
        /// 
        /// Shall be overriden to insert a key-value pair into that node.
        /// </summary>
        /// <param name="myKey">The key to insert</param>
        /// <param name="myValue">The value belonging to the key</param>
        /// <param name="mySplitInfo">
        /// The splitInfo is used to propagate which key has to be inserted 
        /// into a node if a child has been splitted.
        /// </param>
        /// <returns>true if the node was splitted, false if not</returns>
        public abstract bool Insert(TKey myKey, HashSet<TValue> myValue, ref SplitInfo<TKey, TValue> mySplitInfo, IndexSetStrategy myIndexSetStrategy);

        /// <summary>
        /// Abstract Method
        /// 
        /// Override this to remove a key and it's child / value
        /// </summary>
        /// <param name="myKey"></param>
        /// <returns></returns>
        protected abstract bool RemoveInternal(TKey myKey);

        protected abstract void Transfer(int mySourceStartIndex, int myDestinationStartIndex, int myLength, Node<TKey, TValue> myDestinationNode);        

        #endregion

        #region protected members

        /// <summary>
        /// Method returns the sibling which has more Keys, or null if the node has no sibling (root)
        /// </summary>
        /// <returns></returns>
        protected Node<TKey, TValue> GetFullerSibling()
        {
            //left
            if (_LeftSibling != null)
            {
                if (_RightSibling != null)
                {
                    //node has two siblings, we can check their sizes
                    return (_LeftSibling.KeyCount > _RightSibling.KeyCount) ? _LeftSibling : _RightSibling;
                }
                //only left sibling
                return _LeftSibling;
            }

            //only right sibling
            if (_RightSibling != null)
            {
                return _RightSibling;
            }

            //no siblings -> root
            return null;
            //right
        }

        #endregion

        #region public members

        /// <summary>
        /// checks if the current count of elements of that node is greater
        /// then the maximum allowed number of keys per node.
        /// 
        /// It's "greater then" because a key is first inserted into a node, and then
        /// a check is performed to see if the node has to be splitted.
        /// </summary>
        /// <returns>True if the node is full, false if there is still some place in here</returns>
        public bool IsFull()
        {
            return _KeyCount.CompareTo(_CorrespondingTree.MaxKeysPerNode) == 1;
        }

        /// <summary>
        /// checks if the current count of elements of that node is zero        
        /// </summary>
        /// <returns>True if the node is empty, false if there is at least one element</returns>
        public bool IsEmpty()
        {
            return _KeyCount.Equals(0);
        }

        /// <summary>
        /// checks if the node has less then k keys
        /// </summary>
        /// <returns></returns>
        public bool HasUnderflow()
        {
            return (_KeyCount.CompareTo(_CorrespondingTree.MinKeysPerNode) == -1);
        }

        /// <summary>
        /// checks if the node has exactly k keys
        /// </summary>
        /// <returns></returns>
        public bool HasMinimalKeyCount()
        {
            return (_KeyCount.CompareTo(_CorrespondingTree.MinKeysPerNode) == 0);
        }

        /// <summary>
        /// method does a binary search on the Keys-Array to check if the key is in here
        /// </summary>
        /// <param name="myKey">The Key to search for</param>
        /// <returns></returns>
        public bool ContainsKey(TKey myKey)
        {
            int outIndex;

            return ContainsKey(myKey, out outIndex);
        }

        /// <summary>
        /// method does a binary search on the Keys-Array to check if the key is in here
        /// 
        /// if it returns true the out param contains the index of the key, else the outindex
        /// contains the binary complement of the index the key would have if it was in.
        /// </summary>
        /// <param name="myKey">The Key to search for</param>
        /// <param name="outIndex">out param to store the index of the key</param>
        /// <returns></returns>
        public bool ContainsKey(TKey myKey, out int outIndex)
        {
            outIndex = Array.BinarySearch(_Keys, 0, _KeyCount, myKey);

            return outIndex > -1;
        }        

        /// <summary>
        /// Used to remove a key from the tree
        /// 
        /// Inspired by the "Removing in B+Trees"-Algorithm by Jan Jannink
        /// 
        /// see: http://infolab.stanford.edu/~widom/cs346/jannink.pdf for more information
        ///         
        /// 
        /// </summary>
        /// <param name="myKey">The key to be removed</param>
        /// <returns>true if the key was successfully returned</returns>
        public Node<TKey, TValue> Remove(ref TKey myKey, Node<TKey, TValue> myLeftSibling, Node<TKey, TValue> myRightSibling, Node<TKey, TValue> myLeftAnchestor, Node<TKey, TValue> myRightAnchestor, Node<TKey, TValue> myParentNode, ref bool myRemovedFlag)
        {
            #region data

            //used to go down the tree
            Node<TKey, TValue> childNode;
            //left child
            Node<TKey, TValue> childsLeftNode, childsLeftAnchor;
            //right child
            Node<TKey, TValue> childsRightNode, childsRightAnchor;
            //the node where the key has been removed
            Node<TKey, TValue> removeNode;           

            //the children slot of the key
            int keySlot     = FindSlot(myKey);
            //the slot next to it
            int rightSlot   = keySlot + 1;
            //and left to it
            int leftSlot    = keySlot - 1;

            #endregion

            if (this is InnerNode<TKey, TValue>)
            {
                var currentNode = this as InnerNode<TKey, TValue>;

                //childnode represents the next step in recursion
                childNode = currentNode.Children[keySlot + 1];

                //if there is a node pointer left from keyslot take that as leftChild, if not and we have a left sibling, take it's most right node pointer as leftchild
                childsLeftNode     = (leftSlot >= -1)        ? currentNode.Children[keySlot]     : (_LeftSibling != null)    ? (_LeftSibling as InnerNode<TKey, TValue>).Children[_LeftSibling.KeyCount] : null;
                //if there is a node pointer right from keyslot, take that as rightChild, if not and we have a right sibling, take it's most left node pointer as right child
                childsRightNode    = (rightSlot < _KeyCount) ? currentNode.Children[keySlot + 2] : (_RightSibling != null)   ? (_RightSibling as InnerNode<TKey, TValue>).Children[0] : null;

                childsLeftAnchor      = (leftSlot >= -1)        ? this : myLeftAnchestor;
                childsRightAnchor     = (rightSlot < _KeyCount) ? this : myRightAnchestor;

                removeNode = childNode.Remove(ref myKey, childsLeftNode, childsRightNode, childsLeftAnchor, childsRightAnchor, this, ref myRemovedFlag);
            }
            //just for leaf nodes
            else if (ContainsKey(myKey))
            {
                removeNode = this;
            }
            else
            {
                removeNode = null;
            }

            //check if there is something to search in
            if (removeNode != null)
            {
                //if so, remove the key from that node
                myRemovedFlag = removeNode.RemoveInternal(myKey);
            }

            //check if there are less then minimum number of keys at that node
            if (!HasUnderflow())
            {
                //no underflow. nothing to do. return null and we are finished here
                removeNode = null;
            }
            else
            {
                //prepare some values to check the different cases
                bool notLeft    = myLeftSibling  == null;
                bool notRight   = myRightSibling == null;
                bool fewLeft    = (notLeft)     ? false : myLeftSibling.HasMinimalKeyCount();
                bool fewRight   = (notRight)    ? false : myRightSibling.HasMinimalKeyCount();

                //case 1: Root Node
                if (notLeft && notRight)
                {
                    //if this is a leaf and we have no siblings, then it's the root node
                    if (this is LeafNode<TKey, TValue>)
                    {
                        removeNode = null;
                    }
                    else
                    {
                        //if the root has no more keys, there is one child left
                        removeNode = (IsEmpty()) ? (this as InnerNode<TKey, TValue>).Children[0] : null;
                    }
                    
                }
                //case 2: we have to merge in that case
                else if((notLeft || fewLeft) && (notRight || fewRight))
                {
                    removeNode = (myLeftAnchestor != myParentNode) ? Merge(this, myRightSibling, myRightAnchestor, ref myKey) : Merge(myLeftSibling, this, myLeftAnchestor, ref myKey);
                }
                //case 3: choose the better of a merge or a shift
                else if (!notLeft && fewLeft && !notRight && !fewRight)
                {
                    //if the parent of that node is equal to right anchor of that node and the right has enough keys, we shift between current and right
                    //else we merge them
                    removeNode = (myRightAnchestor != myParentNode) ? Merge(myLeftSibling, this, myLeftAnchestor, ref myKey) : Shift(this, myRightSibling, myRightAnchestor);
                }
                //case 4: also choose between a merge or a shift
                else if (!notLeft && !fewLeft && !notRight && fewRight)
                {
                    //if the parent of that node is equal to left anchor of that node and the right has enough keys, we shift between current and left
                    //else we merge them
                    removeNode = (myLeftAnchestor != myParentNode) ? Merge(this, myRightSibling, myRightAnchestor, ref myKey) : Shift(myLeftSibling, this, myLeftAnchestor);
                }
                //case 5: both nodes have the same parent, so we check which one has less number of keys
                else if (myLeftAnchestor == myRightAnchestor)
                { 		
                    int keyCountLeft = (myLeftSibling!=null) ? myLeftSibling.KeyCount : 0;
                    int keyCountRight = (myRightSibling != null) ? myRightSibling.KeyCount : 0;

                    removeNode = (keyCountLeft <= keyCountRight) ? Shift(this, myRightSibling, myRightAnchestor) : Shift(myLeftSibling, this, myLeftAnchestor);
                }
                //case 6: choose the shift with more local effect
                else
                {
                    removeNode = (myLeftAnchestor == myParentNode) ? Shift(myLeftSibling, this, myLeftAnchestor) : Shift(this, myRightSibling, myRightAnchestor);
                }
            }

            return removeNode;
        }

        /// <summary>
        /// Helper Method
        /// 
        /// Method uses binary search to find the correct index whithin a nodes Key-Array.
        /// </summary>
        /// <param name="myNode"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public int FindSlot(TKey myKey)
        {
            //we are at an inner node and search for a way to go
            int nextKeyIndex = Array.BinarySearch<TKey>(_Keys, 0, _KeyCount, myKey);

            if (nextKeyIndex < 0) //not found
            {
                /* calc bitwise complement
                 * 
                 * If value is not found and value is less than one or more elements in array, 
                 * a negative number which is the bitwise complement of the index of the first 
                 * element that is larger than value. If value is not found and value is greater
                 * than any of the elements in array, a negative number which is the bitwise 
                 * complement of (the index of the last element plus 1)
                 */
                nextKeyIndex = ~(nextKeyIndex);
                //if the binary complement is greater then the highest index in the Keys Array
                if (nextKeyIndex > _KeyCount)
                {
                    //then we continue search at most right children
                    nextKeyIndex = _KeyCount;
                }
                else
                {
                    nextKeyIndex--;
                }
            }

            return nextKeyIndex;
        }

        #endregion

        #region private members

        /// <summary>
        /// Method does the shifting between two nodes if one of them has an underflow and a sibling has enough nodes to share
        /// them.
        /// Method is used to shift leafs and inner nodes.
        /// 
        /// See inner documentation for implementation detail.
        /// </summary>
        /// <param name="myLeftNode">left node</param>
        /// <param name="myRightNode">right node</param>
        /// <param name="myParentAnchor">anchor which left child is left and right child is right</param>
        /// <returns></returns>
        private Node<TKey, TValue> Shift(Node<TKey, TValue> myLeftNode, Node<TKey, TValue> myRightNode, Node<TKey, TValue> myAnchor)
        {
            #region data

            //Debug.WriteLine("----------------");
            //Debug.WriteLine("| SHIFT PRE");
            //Debug.WriteLine("| anchor node: {0}", myAnchor);
            //Debug.WriteLine("| left node: {0}", myLeftNode);
            //Debug.WriteLine("| right node: {0}", myRightNode);
            //Debug.WriteLine("----------------");


            bool isInternal = myLeftNode is InnerNode<TKey, TValue>;

            //number of elements to shift to the sibling
            int shiftCount;
            int newKeyCountLeft;
            int keySlotInAnchor;

            TKey newAnchorKey;

            #endregion  
  
            if (myLeftNode.KeyCount < myRightNode.KeyCount)
            {
                //shift entries from right node to left node
                //the half of the overweight can be spend to the other node
                shiftCount = (myRightNode.KeyCount - myLeftNode.KeyCount) / 2;

                newKeyCountLeft = myLeftNode.KeyCount + shiftCount;

                /*
                 * the new anchor value must be greater then the greatest key which is shifted to the left side
                 * 
                 * p.e.
                 * if there is something like this in a leaf:
                 * 
                 * 4
                 * |
                 * 4 5 6 7 8
                 * 
                 * and we shift 4 and 5 to the left side cause of deletion then 6 has to be the new anchor in the upper node:
                 * 
                 * 6
                 * |
                 * 6 7 8
                 * 
                 */
                newAnchorKey = myRightNode.Keys[shiftCount];

                //find the correct slot in the anchor node
                keySlotInAnchor = myAnchor.FindSlot(newAnchorKey);

                if (isInternal)
                {
                    //if tree order == 1 the count can be null                    
                    if (_CorrespondingTree.Order == 1)
                    {
                        shiftCount = 0;
                        newAnchorKey = myRightNode.Keys[shiftCount];
                    }

                    var tmpLeft = myLeftNode as InnerNode<TKey, TValue>;
                    var tmpRight = myRightNode as InnerNode<TKey, TValue>;
                    /*
                     * only for internal nodes - we do a little rotation to the right
                     * 
                     * starting situation 
                     * 
                     * inner        4
                     * 
                     * inner    1 -     5 7 8 11 12
                     * 
                     * (1)  move 4 to empty slot left
                     * 
                     * inner        4
                     * 
                     * inner    1 4     5 7 8 11 12
                     * 
                     * (2) first child of right node is now last child of left node
                     * 
                     * (3) continue process to make it look like that
                     * 
                     * inner            7
                     * 
                     * inner    1 4 5       8 11 12
                     * 
                     */
                    
                    //set key in anchor node as most right key in the left node
                    tmpLeft.Keys[tmpLeft.KeyCount] = myAnchor.Keys[keySlotInAnchor];
                    //set most left child in the right node as most right child in the left node
                    tmpLeft.Children[tmpLeft.KeyCount + 1] = tmpRight.Children[0];

                    //update counts
                    tmpLeft.KeyCount++;
                    tmpRight.KeyCount--;

                    //shift all key child pairs smaller then shiftkey to the left
                    for (int i = 0; i < shiftCount; i++)
                    {
                        //shift keys
                        tmpLeft.Keys[tmpLeft.KeyCount + i] = tmpRight.Keys[i];
                        tmpLeft.Children[tmpLeft.KeyCount + 1 + i] = tmpRight.Children[i + 1];
                    }
                    //update counts
                    //if tree order == 1
                    if (_CorrespondingTree.Order > 1)
                    {
                        tmpLeft.KeyCount += (shiftCount - 1);
                        //-2 because the shiftkey is rotated into the anchorNode
                        tmpRight.KeyCount -= (shiftCount);
                    }

                    //rearrange right node
                    int k, j;
                    for (k = 0, j = shiftCount + 1; k < tmpRight.KeyCount; k++, j++)
                    {
                        tmpRight.Keys[k] = tmpRight.Keys[j];
                        tmpRight.Children[k] = tmpRight.Children[j];
                    }
                    //move last child
                    tmpRight.Children[k] = tmpRight.Children[j];                    
                }
                else
                {
                    //now we do the shifting from right to left
                    myRightNode.Transfer(0, myLeftNode.KeyCount, shiftCount, myLeftNode);
                }

                /*
                 * replace the original parentAnchor key with the new one
                 * 
                 * in the example above we set the 6 in the inner node where the 4 has been before
                 * 
                 */
                myAnchor.Keys[keySlotInAnchor] = newAnchorKey;
                
            }
            else
            {
                //shift entries from left node to right node
                //the half of the overweight can be spend to the other node
                shiftCount = (myLeftNode.KeyCount - myRightNode.KeyCount) / 2;

                newKeyCountLeft = myLeftNode.KeyCount - shiftCount;

                /*
                 * the new anchor value must be smaller then the smallest key which is shifted to the right side
                 * 
                 * p.e.
                 * if there is something like this in a leaf:
                 * 
                 *      4
                 *      |
                 *  1 2 3 
                 * 
                 * and we shift 2 and 3 to the left side cause of deletion then 6 has to be the new anchor in the upper node:
                 * 
                 *  2
                 *  |
                 *  1
                 * 
                 */
                newAnchorKey = myLeftNode.Keys[newKeyCountLeft];

                /*
                 * make room in the right node
                 * 
                 * if you have something like this:
                 * 
                 * 3 4 5 6 - -
                 * 
                 * and shiftCount is 2
                 * it will result in
                 * 
                 * - - 3 4 5 6
                 * 
                 * 
                 */
                if (myRightNode.IsEmpty()) //can happen when tree order is 1
                {
                    myRightNode.Transfer(0, myRightNode.KeyCount + 1, shiftCount, myRightNode);
                }
                else
                {
                    myRightNode.Transfer(0, myRightNode.KeyCount, shiftCount, myRightNode);
                }

                //find the correct slot in the anchor node
                //+ 1 because it must now point to equal or greater values
                keySlotInAnchor = myAnchor.FindSlot(newAnchorKey) + 1;

                if (isInternal)
                {
                    var tmpLeft = myLeftNode as InnerNode<TKey, TValue>;
                    var tmpRight = myRightNode as InnerNode<TKey, TValue>;
                    /*
                     * only for internal nodes - we do a little rotation to the left
                     * 
                     * starting situation
                     * 
                     * inner                    122
                     * 
                     * inner    5 7 8 11 121        - 123
                     * 
                     * (1)  move 4 to empty slot left
                     * 
                     * inner                    122
                     * 
                     * inner    5 7 8 11 121        122 123
                     * 
                     * (2) first child of right node is now last child of left node
                     * 
                     * (3) continue process to make it look like that
                     * 
                     * inner                121
                     * 
                     * inner    5 7 8 11        122 123
                     * 
                     */
                    //move 122 to correct slot rightNode
                    tmpRight.Keys[shiftCount - 1] = myAnchor.Keys[keySlotInAnchor];
                    //move most right child in the left node to the "first" in the right node
                    tmpRight.Children[shiftCount - 1] = tmpLeft.Children[tmpLeft.KeyCount];

                    //now move all values greater then the shiftkey to the right node
                    int i, j;
                    for (i = tmpLeft.KeyCount - shiftCount + 1, j = 0; i < tmpLeft.KeyCount; i++)
                    {
                        tmpRight.Keys[j] = tmpLeft.Keys[i];
                        tmpRight.Children[j] = tmpLeft.Children[i];
                    }
                    //update counts
                    tmpLeft.KeyCount -= (shiftCount);
                    tmpRight.KeyCount += (shiftCount);
                }
                else
                {
                    //in case of leaf we do the shifting from left to right
                    myLeftNode.Transfer(myLeftNode.KeyCount - shiftCount, 0, shiftCount, myRightNode);
                }
                /*
                 * replace the original parentAnchor key with the new one
                 * 
                 * in the example above we set the 121 in the inner node where the 122 has been before
                 * 
                 */
                myAnchor.Keys[keySlotInAnchor] = newAnchorKey;
            }

            //Debug.WriteLine("----------------");
            //Debug.WriteLine("| SHIFT POST");
            //Debug.WriteLine("| anchor node: {0}", myAnchor);
            //Debug.WriteLine("| left node: {0}", myLeftNode);
            //Debug.WriteLine("| right node: {0}", myRightNode);
            //Debug.WriteLine("----------------");

            //finished here, nothing must happen in parent nodes
            return null;
        }


        /// <summary>
        /// Merges the content of two inner nodes or two leafs. Merging alway happens from the right to the left node, this means
        /// all entries from the right node are attached to the left node. This modification forces the parent node to remove one entry
        /// to fulfill the b-tree condition of n+1 keys per node.
        /// 
        /// TODO:
        /// think about merging from left to right, could be less effort especially the parent handling
        /// </summary>
        /// <param name="myLeft">the left node</param>
        /// <param name="myRight">the right node</param>
        /// <param name="myAnchor">the anchor of these nodes</param>
        /// <param name="myRemoveKey">the key to be removed, it's a reference value and can be updated to mark the key to be deleted in parent node</param>
        /// <returns>the node which must be handled next</returns>
        private Node<TKey, TValue> Merge(Node<TKey, TValue> myLeftNode, Node<TKey, TValue> myRightNode, Node<TKey, TValue> myAnchor, ref TKey myRemoveKey)
        {                       
            //Debug.WriteLine("----------------");
            //Debug.WriteLine("| MERGE PRE");
            //Debug.WriteLine("| anchor node: {0}", myAnchor);
            //Debug.WriteLine("| Remove Key: {0}", myRemoveKey);
            //Debug.WriteLine("| left node: {0}", myLeftNode);
            //Debug.WriteLine("| right node: {0}", myRightNode);
            //Debug.WriteLine("----------------");

            #region data

            int	keySlotInAnchor;
            bool isInternal = myLeftNode is InnerNode<TKey, TValue>;
            TKey removeKey;

            #endregion

            #region update siblings

            if (myRightNode.RightSibling != null)
            {
                myRightNode.RightSibling.LeftSibling = myLeftNode;
            }
            myLeftNode.RightSibling = myRightNode.RightSibling;

            #endregion

            if (isInternal)
            {
                //increment left nodes' keycount
                myLeftNode.KeyCount++;

                #region rotation

                #region find the correct key to remove in the anchor node

                removeKey       = myRightNode.Keys[0];
                keySlotInAnchor = myAnchor.FindSlot(removeKey);
                removeKey       = myAnchor.Keys[keySlotInAnchor];

                #endregion

                myLeftNode.Keys[myLeftNode.KeyCount - 1] = removeKey;
                ((InnerNode<TKey, TValue>)myLeftNode).Children[myLeftNode.KeyCount] = ((InnerNode<TKey, TValue>)myRightNode).Children[0];

                #endregion rotation

                myRemoveKey = removeKey;
            }
            else
            {
                myRemoveKey = myAnchor.Keys[myAnchor.FindSlot(myRightNode.Keys[0])];
            }

            myRightNode.Transfer(0, myLeftNode.KeyCount, myRightNode.KeyCount, myLeftNode);

            //Debug.WriteLine("----------------");
            //Debug.WriteLine("| MERGE POST");            
            //Debug.WriteLine("| Remove Key: {0}", myRemoveKey);
            //Debug.WriteLine("| anchor node: {0}", myAnchor);
            //Debug.WriteLine("| left node: {0}", myLeftNode);
            //Debug.WriteLine("| right node: {0}", myRightNode);
            //Debug.WriteLine("----------------");

            return myAnchor;            
        }

        

        #endregion

    }

    /// <summary>
    /// This class is used to store split information.
    /// 
    /// If a node has been splitted, the key and the new right child of that key
    /// have to be inserted in the parent node. An instance of this class, stores
    /// that information.
    /// </summary>
    /// <typeparam name="TKey">The key which has to be inserted in the parent node</typeparam>
    /// <typeparam name="TValue">A reference to the keys new right child</typeparam>
    class SplitInfo<TKey, TValue>
        where TKey : IComparable
    {
        #region private members

        /// <summary>
        /// The Key that needs to be inserted into the parent node
        /// </summary>
        private TKey _Key;

        /// <summary>
        /// The Keys new right child.
        /// </summary>
        private Node<TKey, TValue> _Node;

        #endregion

        #region getter / setter

        /// <summary>
        /// returns / set the new node reference
        /// </summary>
        public Node<TKey, TValue> Node
        {
            get { return _Node; }
            set { _Node = value; }
        }

        /// <summary>
        /// returns / sets the key
        /// </summary>
        public TKey Key
        {
            get { return _Key; }
            set { _Key = value; }
        }

        #endregion
    }
}
