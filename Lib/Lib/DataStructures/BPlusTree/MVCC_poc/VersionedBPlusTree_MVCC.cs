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


using System;
using System.Collections.Generic;

using sones.Lib.DataStructures.BPlusTree;
using sones.Lib.DataStructures.BPlusTree.MVCC;
using sones.Lib.DataStructures.Indices;
using sones.Lib.DataStructures.Timestamp;

/*
 * MVCC stands for Multi Version Concurrency Control (http://en.wikipedia.org/wiki/Multiversion_concurrency_control)
 * and describes a method to provide concurrent access to the objects without locking them.
 * 
 * The MVCC BPlusTree uses copies of nodes to guarantee read consistency. Therefore the tree uses another (internal)
 * tree to manage timestamps and nodes. Every update mechanism (add, edit, remove) creates a copy of all visited nodes 
 * and adds a new entry into the root-tree.
 * 
 * 
 * UNDER CONSTRUCTION
 */
namespace Lib.DataStructures.BPlusTree.MVCC
{
    public class VersionedBPlusTree_MVCC<TKey, TValue> : IVersionedIndexInterface<TKey, TValue>
        where TKey : IComparable
    {

        #region private members

        /// <summary>
        /// Used to manage the different versions of the tree
        /// </summary>
        private BPlusTree<ulong, BPlusTree_MVCC<TKey, TValue>> _VersionTree;

        /// <summary>
        /// The order of the version tree
        /// </summary>
        private UInt32 _VersionTreeOrder;

        /// <summary>
        /// the version of the inner tree structure
        /// </summary>
        private UInt32 _InnerTreeOrder;

        private BPlusTree_MVCC<TKey, TValue> _LatestVersion;

        #endregion

        #region properties

        public UInt32 Order
        {
            get 
            { 
                return _InnerTreeOrder; 
            }
        }

        #endregion

        #region constructors

        /// <summary>
        /// Instantiates a mvcc btree with a version-tree 
        /// order of 60 and an inner order of 60.
        /// </summary>
        public VersionedBPlusTree_MVCC() : this(60)
        {
        }

        /// <summary>
        /// Instantiates a mvcc btree with version-tree 
        /// order of 60 and a given inner order.
        /// </summary>
        /// <param name="myOrder">inner order of the tree</param>
        public VersionedBPlusTree_MVCC(UInt32 myOrder) : this(myOrder, 60)
        {
        }

        /// <summary>
        /// Instantiates a mvcc btree with given version-tree
        /// and inner order.
        /// </summary>
        /// <param name="myOrder">the inner order of the tree</param>
        /// <param name="myRootTreeOrder">the order of the version tree</param>
        public VersionedBPlusTree_MVCC(UInt32 myOrder, UInt32 myVersionTreeOrder)
        {
            if (myOrder < 1 || myVersionTreeOrder < 1)
            {
                throw new ArgumentException("tree order and version tree order must be greater then 0");
            }

            #region set data

            _VersionTreeOrder   = myVersionTreeOrder;
            _InnerTreeOrder     = myOrder;
            _VersionTree = new BPlusTree<ulong, BPlusTree_MVCC<TKey, TValue>>((int)_VersionTreeOrder);

            #endregion
        }

        #endregion

        #region not implemented

        public sones.Lib.DataStructures.Trinary ContainsKey(TKey myKey, long myVersion)
        {
            throw new NotImplementedException();
        }

        public sones.Lib.DataStructures.Trinary ContainsValue(TValue myValue, long myVersion)
        {
            throw new NotImplementedException();
        }

        public sones.Lib.DataStructures.Trinary Contains(TKey myKey, TValue myValue, long myVersion)
        {
            throw new NotImplementedException();
        }

        public sones.Lib.DataStructures.Trinary Contains(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion)
        {
            throw new NotImplementedException();
        }

        public HashSet<TValue> this[TKey myKey, long myVersion]
        {
            get { throw new NotImplementedException(); }
        }

        public bool TryGetValue(TKey key, out HashSet<TValue> value, long myVersion)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TKey> Keys(long myVersion)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TKey> Keys(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion)
        {
            throw new NotImplementedException();
        }

        public ulong KeyCount(long myVersion)
        {
            throw new NotImplementedException();
        }

        public ulong KeyCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<HashSet<TValue>> Values(long myVersion)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<HashSet<TValue>> Values(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion)
        {
            throw new NotImplementedException();
        }

        public ulong ValueCount(long myVersion)
        {
            throw new NotImplementedException();
        }

        public ulong ValueCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion)
        {
            throw new NotImplementedException();
        }

        public IDictionary<TKey, HashSet<TValue>> GetIDictionary(long myVersion)
        {
            throw new NotImplementedException();
        }

        public IDictionary<TKey, HashSet<TValue>> GetIDictionary(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator(long myVersion)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion)
        {
            throw new NotImplementedException();
        }

        public ulong VersionCount(TKey myKey)
        {
            throw new NotImplementedException();
        }

        public void ClearHistory(TKey myKey)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TValue> GreaterThan(TKey myKey, long myVersion, bool myOrEqual = true)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TValue> GreaterThan(TKey myKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion, bool myOrEqual = true)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TValue> LowerThan(TKey myKey, long myVersion, bool myOrEqual = true)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TValue> LowerThan(TKey myKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion, bool myOrEqual = true)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TValue> InRange(TKey myFromKey, TKey myToKey, long myVersion, bool myOrEqualFromKey = true, bool myOrEqualToKey = true)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TValue> InRange(TKey myFromKey, TKey myToKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion, bool myOrEqualFromKey = true, bool myOrEqualToKey = true)
        {
            throw new NotImplementedException();
        }

        public string IndexName
        {
            get { throw new NotImplementedException(); }
        }

        public void Add(TKey myKey, IEnumerable<TValue> myValues)
        {
            throw new NotImplementedException();
        }

        public void Add(KeyValuePair<TKey, TValue> myKeyValuePair)
        {
            throw new NotImplementedException();
        }

        public void Add(KeyValuePair<TKey, IEnumerable<TValue>> myKeyValuesPair)
        {
            throw new NotImplementedException();
        }

        public void Add(Dictionary<TKey, TValue> myDictionary)
        {
            throw new NotImplementedException();
        }

        public void Add(Dictionary<TKey, IEnumerable<TValue>> myDictionary)
        {
            throw new NotImplementedException();
        }

        public void Set(TKey myKey, TValue myValue, IndexSetStrategy myIndexSetStrategy)
        {
            throw new NotImplementedException();
        }

        public void Set(TKey myKey, IEnumerable<TValue> myValues, IndexSetStrategy myIndexSetStrategy)
        {
            throw new NotImplementedException();
        }

        public void Set(KeyValuePair<TKey, TValue> myKeyValuePair, IndexSetStrategy myIndexSetStrategy)
        {
            throw new NotImplementedException();
        }

        public void Set(KeyValuePair<TKey, IEnumerable<TValue>> myKeyValuesPair, IndexSetStrategy myIndexSetStrategy)
        {
            throw new NotImplementedException();
        }

        public void Set(IEnumerable<KeyValuePair<TKey, TValue>> myKeyValuePairs, IndexSetStrategy myIndexSetStrategy)
        {
            throw new NotImplementedException();
        }

        public void Set(Dictionary<TKey, TValue> myDictionary, IndexSetStrategy myIndexSetStrategy)
        {
            throw new NotImplementedException();
        }

        public void Set(Dictionary<TKey, IEnumerable<TValue>> myMultiValueDictionary, IndexSetStrategy myIndexSetStrategy)
        {
            throw new NotImplementedException();
        }

        public sones.Lib.DataStructures.Trinary ContainsKey(TKey myKey)
        {
            throw new NotImplementedException();
        }

        public sones.Lib.DataStructures.Trinary ContainsValue(TValue myValue)
        {
            throw new NotImplementedException();
        }

        public sones.Lib.DataStructures.Trinary Contains(TKey myKey, TValue myValue)
        {
            throw new NotImplementedException();
        }

        public sones.Lib.DataStructures.Trinary Contains(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            throw new NotImplementedException();
        }

        public HashSet<TValue> this[TKey key]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool TryGetValue(TKey key, out HashSet<TValue> value)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TKey> Keys()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TKey> Keys(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            throw new NotImplementedException();
        }

        public ulong KeyCount()
        {
            throw new NotImplementedException();
        }

        public ulong KeyCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<HashSet<TValue>> Values()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<HashSet<TValue>> Values(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TValue> GetValues()
        {
            throw new NotImplementedException();
        }

        public ulong ValueCount()
        {
            throw new NotImplementedException();
        }

        public ulong ValueCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            throw new NotImplementedException();
        }

        public IDictionary<TKey, HashSet<TValue>> GetIDictionary()
        {
            throw new NotImplementedException();
        }

        public IDictionary<TKey, HashSet<TValue>> GetIDictionary(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            throw new NotImplementedException();
        }

        public bool Remove(TKey myKey)
        {
            throw new NotImplementedException();
        }

        public bool Remove(TKey myKey, TValue myValue)
        {
            throw new NotImplementedException();
        }

        public bool Remove(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TValue> GreaterThan(TKey myKey, bool myOrEqual = true)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TValue> GreaterThan(TKey myKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, bool myOrEqual = true)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TValue> LowerThan(TKey myKey, bool myOrEqual = true)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TValue> LowerThan(TKey myKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, bool myOrEqual = true)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TValue> InRange(TKey myFromKey, TKey myToKey, bool myOrEqualFromKey = true, bool myOrEqualToKey = true)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TValue> InRange(TKey myFromKey, TKey myToKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, bool myOrEqualFromKey = true, bool myOrEqualToKey = true)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region add

        public void Add(TKey myKey, TValue myValue)
        {
            //adding results in a new revision of the tree
            BPlusTree_MVCC<TKey, TValue> tmpTree;

            if (_VersionTree.Count == 0)
            {
                tmpTree = new BPlusTree_MVCC<TKey, TValue>((int)_InnerTreeOrder);

                tmpTree.Add(myKey, myValue, out tmpTree);
            }
            else
            {
                //insert into the latest version of the tree
                _LatestVersion.Add(myKey, myValue, out tmpTree);
            }

            //atomic update of the version tree
            lock (_VersionTree)
            {
                //add the new version to the version tree
                _VersionTree.Add(TimestampNonce.Ticks, tmpTree);

                //and update the reference to the latest version
                _LatestVersion = tmpTree;
            }
        }

        #endregion
    }
}

