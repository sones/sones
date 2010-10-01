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
 * <copyright file=”AVersionedNode.cs”
 *            company=”sones GmbH”>
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Martin Junghanns</developer>
 */

#region usings

using System;
using System.Collections.Generic;
using System.Text;
using sones.Lib.DataStructures.Indices;

#endregion

namespace sones.Lib.DataStructures.BPlusTree.MVCC
{
    /// <summary>
    /// Abstract class of a node within the IndexView. It's equal to
    /// a node of a standard bplustree but contains methods to clone 
    /// nodes in case of modify-operations.
    /// </summary>
    /// <typeparam name="TKey">The type of the key which identifies the values</typeparam>
    /// <typeparam name="TValue">The values which are identified by the key</typeparam>
    abstract class AVersionedNode<TKey, TValue>
        where TKey : IComparable
    {
        #region private members

        //NLOG: temporarily commented
        //Logger logger = LogManager.GetLogger("UnitTestLogger"); 

        /// <summary>
        /// Holds a reference to the parent node.
        /// </summary>
        private AVersionedNode<TKey, TValue> _Parent;

        #endregion

        #region protected members

        /// <summary>
        /// Holds a reference to the corresponding tree to access the settings
        /// </summary>
        protected IndexView<TKey, TValue> _CorrespondingTree;

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

        protected object _LockObject = new object();

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
        public AVersionedNode(IndexView<TKey, TValue> myTree)
        {
            _Keys               = new TKey[myTree.MaxKeysPerNode + 1];
            _CorrespondingTree  = myTree;
            _KeyCount           = 0;
            _DoBalance          = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Returns / Sets the reference to the parent Node
        /// </summary>
        public AVersionedNode<TKey, TValue> Parent
        {
            get { return _Parent; }
            set { _Parent = value; }
        }

        /// <summary>
        /// Returns / Set the current count of keys of that node
        /// </summary>
        public Int32 KeyCount
        {
            get 
            {
                lock (_LockObject)
                {
                    return _KeyCount;
                }
            }
            set 
            {
                lock (_LockObject)
                {
                    _KeyCount = value;
                }
            }
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
        public abstract bool Insert(TKey myKey, HashSet<TValue> myValue, ref NodeInfoStorage_MVCC<TKey, TValue> mySplitInfo, IndexSetStrategy myIndexSetStrategy, VersionInfoDTO<TKey, TValue> myVersionInfoDTO);

        /// <summary>
        /// Abstract Method
        /// 
        /// Override this to remove a key and it's child / value
        /// </summary>
        /// <param name="myKey"></param>
        /// <returns></returns>
        public abstract bool Remove(VersionInfoDTO<TKey, TValue> versionInfoDTO, ref AVersionedNode<TKey, TValue> myCopy, TKey myKey, TValue myValue = default(TValue));

        /// <summary>
        /// Abstract Method
        /// 
        /// Used to move Key, Value Pairs from this Node to a given destination node
        /// </summary>
        /// <param name="mySourceStartIndex">start index in the source node</param>
        /// <param name="myDestinationStartIndex">start index in the destination node</param>
        /// <param name="myLength">number of key value pairs to copy</param>
        /// <param name="myDestinationNode">the destination node</param>
        protected abstract void Transfer(int mySourceStartIndex, int myDestinationStartIndex, int myLength, AVersionedNode<TKey, TValue> myDestinationNode);

        #endregion

        #region public abstract members

        /// <summary>
        /// Abstract  Method
        /// 
        /// Used to create an exact copy of that node
        /// </summary>
        /// <returns></returns>
        public abstract AVersionedNode<TKey, TValue> Clone();

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
        public Trinary ContainsKey(TKey myKey)
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
        public Trinary ContainsKey(TKey myKey, out int outIndex)
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

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("| ");
            for (int i = 0; i < _KeyCount; i++)
            {
                sb.Append(" " + _Keys[i] + " |");
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// This class is used to store node information which are used
    /// during splitting nodes or to store copy info while removing.
    /// </summary>
    /// <typeparam name="TKey">The key which has to be inserted in the parent node (used by splitting)</typeparam>
    /// <typeparam name="TValue">A reference to the keys new right child (used by splitting and removing)</typeparam>
    class NodeInfoStorage_MVCC<TKey, TValue>
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
        private AVersionedNode<TKey, TValue> _SplitNode;

        /// <summary>
        /// The new version of original node.
        /// </summary>
        private AVersionedNode<TKey, TValue> _CloneNode;

        #endregion

        #region getter / setter

        /// <summary>
        /// returns / sets the key
        /// </summary>
        public TKey Key
        {
            get { return _Key; }
            set { _Key = value; }
        }

        /// <summary>
        /// returns / set the new node reference
        /// </summary>
        public AVersionedNode<TKey, TValue> SplitNode
        {
            get { return _SplitNode; }
            set { _SplitNode = value; }
        }

        /// <summary>
        /// returns / sets the key
        /// </summary>
        public AVersionedNode<TKey, TValue> CloneNode
        {
            get { return _CloneNode; }
            set { _CloneNode = value; }
        }

        #endregion
    }
}
