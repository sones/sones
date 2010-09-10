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

/* <id name=”Libraries Datastructures – BStarTree” />
 * <copyright file=”Node.cs”
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
using System.Diagnostics;
using sones.Lib.DataStructures.Indices;

#endregion

namespace sones.Lib.DataStructures.BPlusTree.Versioned
{
    abstract class VersionedNode<TKey, TValue>
        where TKey : IComparable
    {
        #region private members

        //NLOG: temporarily commented
        //Logger logger = LogManager.GetLogger("UnitTestLogger"); 

        /// <summary>
        /// Holds a reference to the parent node.
        /// </summary>
        private VersionedNode<TKey, TValue> _Parent;

        /// <summary>
        /// Holds a reference to the right Node
        /// </summary>
        private VersionedNode<TKey, TValue> _RightSibling;

        /// <summary>
        /// Holds a reference to the left Node
        /// </summary>
        private VersionedNode<TKey, TValue> _LeftSibling;

        #endregion

        #region protected members

        /// <summary>
        /// Holds a reference to the corresponding tree to access the settings
        /// </summary>
        protected VersionedBPlusTree<TKey, TValue> _CorrespondingTree;

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
        public VersionedNode(VersionedBPlusTree<TKey, TValue> myTree)
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
        public VersionedNode<TKey, TValue> RightSibling
        {
            get { return _RightSibling; }
            set { _RightSibling = value; }
        }

        /// <summary>
        /// Sets / Returns the left sibling of that node
        /// </summary>
        public VersionedNode<TKey, TValue> LeftSibling
        {
            get { return _LeftSibling; }
            set { _LeftSibling = value; }
        }

        /// <summary>
        /// Returns / Sets the reference to the parent Node
        /// </summary>
        public VersionedNode<TKey, TValue> Parent
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
        public abstract bool Insert(TKey myKey, HashSet<TValue> myValue, ref VersionedSplitInfo<TKey, TValue> mySplitInfo, IndexSetStrategy myIndexSetStrategy);

        /// <summary>
        /// Inserts TKey, IndexValueHistoryList of TValue into the node.
        /// </summary>
        /// <param name="mySourceStartIndex"></param>
        /// <param name="myDestinationStartIndex"></param>
        /// <param name="myLength"></param>
        /// <param name="myDestinationNode"></param>
        public abstract bool Insert(TKey myKey, IndexValueHistoryList<TValue> myValue, ref VersionedSplitInfo<TKey, TValue> mySplitInfo);

        protected abstract void Transfer(int mySourceStartIndex, int myDestinationStartIndex, int myLength, VersionedNode<TKey, TValue> myDestinationNode);        

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
    class VersionedSplitInfo<TKey, TValue>
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
        private VersionedNode<TKey, TValue> _Node;

        #endregion

        #region getter / setter

        /// <summary>
        /// returns / set the new node reference
        /// </summary>
        public VersionedNode<TKey, TValue> Node
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
