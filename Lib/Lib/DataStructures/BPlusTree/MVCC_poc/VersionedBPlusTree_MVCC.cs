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

/* <id name=”Libraries Datastructures – MultiVersionBPlusTree” />
 * <copyright file=”MultiVersionBPlusTree.cs”
 *            company=”sones GmbH”>
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Martin Junghanns</developer>
 */
using System;
using System.Collections.Generic;

using sones.Lib.DataStructures.BPlusTree.MVCC;
using sones.Lib.DataStructures.Indices;
using sones.Lib.DataStructures.Timestamp;
using sones.Lib.DataStructures;
using System.Threading;
using sones.Lib.DataStructures.BPlusTree;

/*
 * MVCC stands for Multi Version Concurrency Control (http://en.wikipedia.org/wiki/Multiversion_concurrency_control)
 * 
 * Instead of fielding locks to avoid inconsistent read states, MVCC uses copies of the respective object. During a
 * modification (add, update, remove) a consistent state of the object can be read.
 * 
 * The concept of using copies instead of read locks has beend transfered to this bplustree. Read accesses use defined
 * views of the tree, to read their data. The disadvantages are a higher consumption of main memory and the maintenance
 * of the tree (removing unused versions,...).
 * 
 * The tree consists of three areas. The version layer consists of a tree, which manages entries of the form (timestamp,
 * reference of the tree) and returns a view of the tree at a given timestamp using nearest neighbour search.
 * The second layer is the index which consists of multiple bplustrees whose nodes are partially shared among each other or
 * copies of nodes which were modified.
 * The third layer consists of the leaf nodes which are connected to each other. These siblingreferences are identified
 * by a timestamp and a reference to the respective sibling. By these structure range queries on different views are still
 * possible.
 */
namespace sones.Lib.DataStructures.BPlusTree.MVCC
{
    public class MultiVersionBPlusTree<TKey, TValue> : IEnumerable<KeyValuePair<TKey, HashSet<TValue>>>
        where TKey : IComparable
    {

        #region Private Members

        /// <summary>
        /// Used to manage the different versions of the tree
        /// </summary>
        private sones.Lib.DataStructures.BPlusTree.BPlusTree<ulong, IndexView<TKey, TValue>> _VersionTree;

        /// <summary>
        /// The order of the version tree
        /// </summary>
        private UInt32 _VersionTreeOrder;

        /// <summary>
        /// the version of the inner tree structure
        /// </summary>
        private UInt32 _InnerTreeOrder;

        /// <summary>
        /// The number of versions of the tree.
        /// </summary>
        private UInt32 _HistorySize;

        /// <summary>
        /// HistorySize determines the maximum revision count of the inner tree.
        /// VersionBuffer sets the number of "extra" version which will be stored before removing.
        /// This is a performance detail, because when reaching the maximum HistorySize, any
        /// modification of the inner tree would result in a reorganization of the version tree, which
        /// costs a lot of time.
        /// </summary>
        private Int32 _VersionBuffer;

        /// <summary>
        /// The count of "extra" versions in the VersionBuffer
        /// </summary>
        private Int32 _CurrentBufferSize;

        /// <summary>
        /// The count of failed modification operations (insert / update / remove)
        /// </summary>
        private Int32 _MissCounter;

        /// <summary>
        /// The reference to the latest version of the tree.
        /// </summary>
        private IndexView<TKey, TValue> _LatestVersion;

        /// <summary>
        /// Used to manage parallel read-/write-access
        /// </summary>
        private object _LockObj = new object();
    
        #endregion

        #region Properties

        /// <summary>
        /// Returns the order k of that tree.
        /// </summary>
        public UInt32 Order
        {
            get 
            { 
                return _InnerTreeOrder; 
            }
        }

        /// <summary>
        /// Returns the actual height of that tree.
        /// </summary>
        public UInt32 Height
        {
            get
            {
                return _LatestVersion.Height;
            } 
        }

        /// <summary>
        /// Returns / Sets the HistorySize, this is the maximum
        /// number of revisions to store (+ VersionBuffer).
        /// </summary>
        public UInt32 HistorySize
        {
            get
            {
                return _HistorySize;
            }
            set
            {
                _HistorySize = value;
            }
        }

        /// <summary>
        /// Returns / Sets the VersionBuffer's Size.
        /// 
        /// VersionBuffer sets the number of "extra" version 
        /// which will be stored before removing.
        /// </summary>
        public Int32 VersionBuffer
        {
            get
            {
                return _VersionBuffer;
            }
            set
            {
                _VersionBuffer = value;
            }
        }

        /// <summary>
        /// Returns the number of misses during modification, 
        /// just a evaluation information.
        /// </summary>
        public Int32 MissCounter
        {
            get
            {
                return _MissCounter;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Instantiates a mvcc btree with a version-tree 
        /// order of 60 and an inner order of 60.
        /// HistorySize (maximum number of versions) is set to 10.
        /// VersionBuffer Size is set to 100.
        /// </summary>
        public MultiVersionBPlusTree() : this(60)
        {
        }

        /// <summary>
        /// Instantiates a mvcc btree with version-tree 
        /// order of 60 and a given inner order.
        /// HistorySize (maximum number of versions) is set to 10.
        /// VersionBuffer Size is set to 100.
        /// </summary>
        /// <param name="myOrder">inner order of the tree</param>
        public MultiVersionBPlusTree(UInt32 myOrder) : this(myOrder, 60, 10, 100)
        {
        }

        /// <summary>
        /// Instantiates a mvcc btree with a given inner and versiontree order.
        /// HistorySize (maximum number of versions) is set to 10.
        /// VersionBuffer Size is set to 100.
        /// </summary>
        /// <param name="myOrder">inner order of the tree</param>
        public MultiVersionBPlusTree(UInt32 myOrder, UInt32 myVersionTreeOrder)
            : this(myOrder, myVersionTreeOrder, 10, 100)
        {
        }

        /// <summary>
        /// Instantiates a mvcc btree with a given inner and versiontree order
        /// and a given history Size.
        /// VersionBuffer Size is set to 100.
        /// </summary>
        /// <param name="myOrder">inner order of the tree</param>
        public MultiVersionBPlusTree(UInt32 myOrder, UInt32 myVersionTreeOrder, UInt32 myHistorySize)
            : this(myOrder, myVersionTreeOrder, myHistorySize, 100)
        {
        }

        /// <summary>
        /// Instantiates a mvcc btree with given version-tree
        /// and inner order.
        /// <param name="myOrder">the order of the inner tree</param>
        /// <param name="myVersionTreeOrder">the order of the version tree</param>
        /// <param name="myHistorySize">the maximum number of revisions</param>
        /// <param name="myVersionBufferSize">buffersize before deleting old versions</param>
        public MultiVersionBPlusTree(UInt32 myOrder, UInt32 myVersionTreeOrder, UInt32 myHistorySize, Int32 myVersionBufferSize)
        {
            #region param check

            if (myOrder < 1 || myVersionTreeOrder < 1)
            {
                throw new ArgumentException("tree order and version tree order must be greater then 0");
            }
            if (myVersionBufferSize < 0)
            {
                throw new ArgumentException("buffer size may not be less then zero");
            }

            #endregion

            #region set data

            _VersionTreeOrder   = myVersionTreeOrder;
            _InnerTreeOrder     = myOrder;
            _VersionTree        = new BPlusTree<ulong, IndexView<TKey, TValue>>((int)_VersionTreeOrder);
            _LatestVersion      = new IndexView<TKey, TValue>((int)_InnerTreeOrder, TimestampNonce.Ticks);
            _HistorySize        = myHistorySize;

            _VersionBuffer      = myVersionBufferSize;
            _CurrentBufferSize  = 0;

            _MissCounter        = 0;

            #endregion
        }

        #endregion

        #region Version Stuff

        /// <summary>
        /// Returns the current number of versions stored in that tree.
        /// </summary>
        /// <returns>number of versions</returns>
        public ulong VersionCount()
        {
            return _VersionTree.Count;
        }

        /// <summary>
        /// Removes all revisions except the latest.
        /// </summary>
        public void ClearHistory()
        {
            lock (_LockObj)
            {
                var newVersionTree = new BPlusTree<ulong, IndexView<TKey, TValue>>((int)_VersionTreeOrder);

                newVersionTree.Add(_LatestVersion.Timestamp, _LatestVersion);

                _VersionTree = newVersionTree;
            }
        }

        #endregion

        #region IndexName

        public string IndexName
        {
            get { return "3DTree"; }
        }

        #endregion

        #region Add

        /// <summary>
        /// This methods adds a key an corresponding values into the tree and creates a new version
        /// based on the insertion timestamp.
        /// 
        /// 1. Store timestamp of the latest version of the tree
        /// 2. Store current timestamp
        /// 3. Generate a new tree version (which holds the copies of the updated path)
        /// 4. Insert into the latest version of the tree (copies will be updated in the new version of the tree)
        /// 5. Check if the latest version is still the latest version (avoid lost update situation)
        /// 6. If it's still the latest, update the versiontree, else restart the insertion
        /// </summary>
        /// <param name="myKey">the key to be inserted</param>
        /// <param name="myValues">the set of values to be inserted</param>
        public ulong Add(TKey myKey, IEnumerable<TValue> myValues)
        {
            #region data

            var latestModificationTimeStamp = _LatestVersion.Timestamp;
            var currentModificationTimeStamp = TimestampNonce.Ticks;
            var versionInfoDTO = GetNewTreeVersion(currentModificationTimeStamp);

            #endregion

            #region insert

            //Debug.WriteLine(String.Format("Start Insert: TaskNo. {0} ==> KeyCount = {1}, ValueCount = {2}, InsertTimeStamp = {3}, LatestModificationTimeStamp = {4}", Task.CurrentId, versionInfoDTO.Tree.KeyCount, versionInfoDTO.Tree.ValueCount(), currentModificationTimeStamp, latestModificationTimeStamp));
            versionInfoDTO.Tree.Add(myKey, new HashSet<TValue>(myValues), versionInfoDTO);

            #endregion

            #region update version or repeat if failed

            if (!UpdateVersionTree(versionInfoDTO, latestModificationTimeStamp, currentModificationTimeStamp))
            {
                //Debug.WriteLine(String.Format("Repeat Inserting: TaskNo. {0} ==> KeyCount = {1}, ValueCount = {2}", Task.CurrentId, versionInfoDTO.Tree.KeyCount, versionInfoDTO.Tree.ValueCount()));
                return Add(myKey, myValues);
            }
            else
            {
                return currentModificationTimeStamp;
                //Debug.WriteLine(String.Format("Finished Insert: TaskNo. {0} ==> KeyCount = {1}, ValueCount = {2}", Task.CurrentId, tmpTree.KeyCount, tmpTree.ValueCount()));
            }

            #endregion
        }

        /// <summary>
        /// Look at 
        /// public void Add(TKey myKey, IEnumerable<TValue> myValues)
        /// for a detailed description
        /// </summary>
        /// <param name="myKeyValuePair">a keyvaluepair containing a key and the corresponding value</param>
        public ulong Add(KeyValuePair<TKey, TValue> myKeyValuePair)
        {
            #region data

            var revision = 0UL;

            #endregion

            #region insert

            revision = Add(myKeyValuePair.Key, new HashSet<TValue>() { myKeyValuePair.Value });

            return revision;

            #endregion
        }

        /// <summary>
        /// Look at 
        /// public void Add(TKey myKey, IEnumerable<TValue> myValues)
        /// for a detailed description
        /// </summary>
        /// <param name="myKeyValuesPair">a keyvaluepair containing a key and the corresponding set of values</param>
        public ulong Add(KeyValuePair<TKey, IEnumerable<TValue>> myKeyValuesPair)
        {
            #region data

            var revision = 0UL;

            #endregion

            #region insert

            revision = Add(myKeyValuesPair.Key, new HashSet<TValue>(myKeyValuesPair.Value));

            return revision;

            #endregion
        }

        /// <summary>
        /// look at 
        /// public void Add(TKey myKey, IEnumerable<TValue> myValues)
        /// for a detailed description
        /// </summary>
        /// <param name="myDictionary">a key-single value-store</param>
        public ulong Add(Dictionary<TKey, TValue> myDictionary)
        {
            #region insert

            var revision = 0UL;

            foreach (var kvp in myDictionary)
            {
                revision = Add(kvp.Key, new HashSet<TValue>() { kvp.Value });
            }

            return revision;

            #endregion
        }

        /// <summary>
        /// look at 
        /// public void Add(TKey myKey, IEnumerable<TValue> myValues)
        /// for a detailed description
        /// </summary>
        /// <param name="myDictionary">a key-multiple value-store</param>
        public ulong Add(Dictionary<TKey, IEnumerable<TValue>> myDictionary)
        {
            #region data

            var revision = 0UL;

            #endregion

            #region insert

            foreach (var kvp in myDictionary)
            {
                revision = Add(kvp.Key, new HashSet<TValue>(kvp.Value));
            }

            return revision;

            #endregion
        }

        /// <summary>
        /// look at 
        /// public void Add(TKey myKey, IEnumerable<TValue> myValues)
        /// for a detailed description
        /// </summary>
        /// <param name="myKey">the key to be inserted</param>
        /// <param name="myValues">the value associated to the key</param>
        public ulong Add(TKey myKey, TValue myValue)
        {
            return Add(myKey, new HashSet<TValue>() { myValue });
        }

        #endregion

        #region Set (not yet implemented)

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

        #endregion

        #region Contains

        public Trinary ContainsKey(TKey myKey)
        {
            return _LatestVersion.ContainsKey(myKey);
        }

        public Trinary ContainsValue(TValue myValue)
        {
            return _LatestVersion.ContainsValue(myValue);
        }

        public Trinary Contains(TKey myKey, TValue myValue)
        {
            return _LatestVersion.Contains(myKey, myValue);
        }

        public Trinary Contains(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            return _LatestVersion.Contains(myFunc);
        }

        public Trinary ContainsKey(TKey myKey, ulong myVersion)
        {
            return ContainsKey(myKey, (long)myVersion);
        }

        public Trinary ContainsKey(TKey myKey, long myVersion)
        {
            var tree = GetTreeByVersion(myVersion);
           
            if (tree != null)
            {
                return tree.ContainsKey(myKey); //there is only one tree assigned
            }

            return Trinary.FALSE;
        }

        public Trinary ContainsValue(TValue myValue, long myVersion)
        {
            var tree = GetTreeByVersion(myVersion);

            if (tree != null)
            {
                return tree.ContainsValue(myValue);
            }
            return Trinary.FALSE;
        }

        public Trinary Contains(TKey myKey, TValue myValue, long myVersion)
        {
            var tree = GetTreeByVersion(myVersion);

            if (tree != null)
            {
                return tree.Contains(myKey, myValue);
            }
            return Trinary.FALSE;
        }

        public Trinary Contains(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion)
        {
            var tree = GetTreeByVersion(myVersion);

            if (tree != null)
            {
                return tree.Contains(myFunc);
            }
            return Trinary.FALSE;
        }

        #endregion

        #region this[]

        public HashSet<TValue> this[TKey key]
        {
            get
            {
                return _LatestVersion[key];
            }
        }

        public HashSet<TValue> this[TKey myKey, long myVersion]
        {
            get
            {
                var tree = GetTreeByVersion(myVersion);

                if (tree != null)
                {
                    return tree[myKey];
                }

                return null;
            }
        }

        #endregion

        #region TryGetValue

        public bool TryGetValue(TKey key, out HashSet<TValue> value)
        {
            return _LatestVersion.TryGetValue(key, out value);
        }

        #endregion

        #region Keys

        public IEnumerable<TKey> Keys()
        {
            return _LatestVersion.Keys;
        }

        public IEnumerable<TKey> Keys(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            return ((IIndexInterface<TKey, TValue>)_LatestVersion).Keys(myFunc);
        }

        public IEnumerable<TKey> Keys(long myVersion)
        {
            var tree = GetTreeByVersion(myVersion) as IIndexInterface<TKey, TValue>;

            if (tree != null)
            {
                return tree.Keys();
            }

            return null;
        }

        public IEnumerable<TKey> Keys(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion)
        {
            var tree = GetTreeByVersion(myVersion) as IIndexInterface<TKey, TValue>;

            if (tree != null)
            {
                return tree.Keys(myFunc);
            }

            return null;
        }

        #endregion

        #region KeyCount

        public ulong KeyCount()
        {
            return _LatestVersion.KeyCount;
        }

        public ulong KeyCount(long myVersion)
        {
            var tree = GetTreeByVersion(myVersion);

            if (tree != null)
            {
                return tree.KeyCount;
            }
            return 0UL;
        }

        #endregion

        #region Values

        public IEnumerable<HashSet<TValue>> Values()
        {
            return ((IIndexInterface<TKey, TValue>)_LatestVersion).Values();
        }

        public IEnumerable<HashSet<TValue>> Values(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            return ((IIndexInterface<TKey, TValue>)_LatestVersion).Values(myFunc);
        }

        public IEnumerable<HashSet<TValue>> Values(long myVersion)
        {
            var tree = GetTreeByVersion(myVersion) as IIndexInterface<TKey, TValue>;

            if (tree != null)
            {
                return tree.Values();
            }

            return null;
        }

        public IEnumerable<HashSet<TValue>> Values(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion)
        {
            var tree = GetTreeByVersion(myVersion) as IIndexInterface<TKey, TValue>;

            if (tree != null)
            {
                return tree.Values(myFunc);
            }

            return null;
        }

        public IEnumerable<TValue> GetValues()
        {
            return ((IIndexInterface<TKey, TValue>)_LatestVersion).GetValues();
        }

        public bool TryGetValue(TKey key, out HashSet<TValue> value, long myVersion)
        {
            var tree = GetTreeByVersion(myVersion) as IIndexInterface<TKey, TValue>;

            if (tree != null)
            {
                return tree.TryGetValue(key, out value);
            }

            value = new HashSet<TValue>();
            return false;
        }

        #endregion

        #region ValueCount

        public ulong ValueCount()
        {
            return _LatestVersion.ValueCount();
        }

        public ulong ValueCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            return _LatestVersion.ValueCount(myFunc);
        }

        public ulong ValueCount(long myVersion)
        {
            var tree = GetTreeByVersion(myVersion);

            if (tree != null)
            {
                return tree.ValueCount();
            }
            return 0UL;
        }

        public ulong ValueCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion)
        {
            var tree = GetTreeByVersion(myVersion);

            if (tree != null)
            {
                return tree.ValueCount(myFunc);
            }
            return 0UL;
        }

        #endregion

        #region IDictionary

        public IDictionary<TKey, HashSet<TValue>> GetIDictionary()
        {
            return _LatestVersion.GetIDictionary();
        }

        public IDictionary<TKey, HashSet<TValue>> GetIDictionary(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            return _LatestVersion.GetIDictionary(myFunc);
        }

        public IDictionary<TKey, HashSet<TValue>> GetIDictionary(long myVersion)
        {
            var tree = GetTreeByVersion(myVersion);

            if (tree != null)
            {
                return tree.GetIDictionary();
            }

            return null;
        }

        public IDictionary<TKey, HashSet<TValue>> GetIDictionary(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion)
        {
            var tree = GetTreeByVersion(myVersion);

            if (tree != null)
            {
                return tree.GetIDictionary(myFunc);
            }

            return null;
        }

        #endregion

        #region Enumerator

        public IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc)
        {
            return _LatestVersion.GetEnumerator(myFunc);
        }

        public IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator()
        {
            return _LatestVersion.GetEnumerator();
        }

        public IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator(long myVersion)
        {
            var tree = GetTreeByVersion(myVersion);

            if (tree != null)
            {
                return tree.GetEnumerator();
            }

            return null;
        }

        public IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator(Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion)
        {
            var tree = GetTreeByVersion(myVersion);

            if (tree != null)
            {
                return tree.GetEnumerator(myFunc);
            }

            return null;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _LatestVersion.GetEnumerator();
        }

        #endregion

        #region Remove

        /// <summary>
        /// If myKey exists, this method removes all values assigned to that
        /// key (which means that the key is deleted) and creates a new tree 
        /// version which is stored in the out param.
        /// </summary>
        /// <param name="myKey">The key to be deleted</param>
        /// <param name="myVersion">The revision of the tree after deleting</param>
        /// <returns>true if the key has been deleted</returns>
        public bool Remove(TKey myKey, out ulong myVersion)
        {
            return Remove(myKey, default(TValue), out myVersion);
        }
        /// <summary>
        /// If myKey and the specified value exists, this method removes the value
        /// and creates a new tree version which is stored in the out param.
        /// </summary>
        /// <param name="myKey">The key to be deleted</param>
        /// <param name="myValue">The value to be deleted</param>
        /// <param name="myVersion">The revision of the tree after deleting</param>
        /// <returns>true if the key has been deleted</returns>
        public bool Remove(TKey myKey, TValue myValue, out ulong myVersion)
        {
            #region data

            var latestModificationTimeStamp = _LatestVersion.Timestamp;
            var currentModificationTimeStamp = TimestampNonce.Ticks;
            var versionInfoDTO = GetNewTreeVersion(currentModificationTimeStamp);

            #endregion

            #region remove

            var removed = versionInfoDTO.Tree.Remove(versionInfoDTO, myKey, myValue);

            #endregion

            #region update version or repeat if failed

            if (removed)
            {
                if (!UpdateVersionTree(versionInfoDTO, latestModificationTimeStamp, currentModificationTimeStamp))
                {
                    return Remove(myKey, myValue, out myVersion);
                }
            }

            myVersion = currentModificationTimeStamp;

            return removed; 

            #endregion
        }

        #endregion

        #region Clear

        /// <summary>
        /// Removes complete tree content
        /// </summary>
        public void Clear()
        {
            ClearHistory();

            _LatestVersion = new IndexView<TKey, TValue>((int)_InnerTreeOrder, TimestampNonce.Ticks);
        }

        #endregion

        #region Range Methods

        public IEnumerable<TValue> GreaterThan(TKey myKey, bool myOrEqual = true)
        {
            return _LatestVersion.GreaterThan(myKey, myOrEqual);
        }

        public IEnumerable<TValue> GreaterThan(TKey myKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, bool myOrEqual = true)
        {
            return _LatestVersion.GreaterThan(myKey, myFunc, myOrEqual);
        }

        public IEnumerable<TValue> LowerThan(TKey myKey, bool myOrEqual = true)
        {
            return _LatestVersion.LowerThan(myKey, myOrEqual);
        }

        public IEnumerable<TValue> LowerThan(TKey myKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, bool myOrEqual = true)
        {
            return _LatestVersion.LowerThan(myKey, myFunc, myOrEqual);
        }

        public IEnumerable<TValue> InRange(TKey myFromKey, TKey myToKey, bool myOrEqualFromKey = true, bool myOrEqualToKey = true)
        {
            return _LatestVersion.InRange(myFromKey, myToKey, myOrEqualFromKey, myOrEqualToKey);
        }

        public IEnumerable<TValue> InRange(TKey myFromKey, TKey myToKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, bool myOrEqualFromKey = true, bool myOrEqualToKey = true)
        {
            return _LatestVersion.InRange(myFromKey, myToKey, myFunc, myOrEqualFromKey, myOrEqualToKey);
        }

        public IEnumerable<TValue> GreaterThan(TKey myKey, long myVersion, bool myOrEqual = true)
        {
            var tree = GetTreeByVersion(myVersion);

            if (tree != null)
            {
                return tree.GreaterThan(myKey, myOrEqual);
            }

            return null;
        }

        public IEnumerable<TValue> GreaterThan(TKey myKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion, bool myOrEqual = true)
        {
            var tree = GetTreeByVersion(myVersion);

            if (tree != null)
            {
                return tree.GreaterThan(myKey, myFunc, myOrEqual);
            }

            return null;
        }

        public IEnumerable<TValue> LowerThan(TKey myKey, long myVersion, bool myOrEqual = true)
        {
            var tree = GetTreeByVersion(myVersion);

            if (tree != null)
            {
                return tree.LowerThan(myKey, myOrEqual);
            }

            return null;
        }

        public IEnumerable<TValue> LowerThan(TKey myKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion, bool myOrEqual = true)
        {
            var tree = GetTreeByVersion(myVersion);

            if (tree != null)
            {
                return tree.LowerThan(myKey, myFunc, myOrEqual);
            }

            return null;
        }

        public IEnumerable<TValue> InRange(TKey myFromKey, TKey myToKey, long myVersion, bool myOrEqualFromKey = true, bool myOrEqualToKey = true)
        {
            var tree = GetTreeByVersion(myVersion);

            if (tree != null)
            {
                return tree.InRange(myFromKey, myToKey, myOrEqualFromKey, myOrEqualToKey);
            }

            return null;
        }

        public IEnumerable<TValue> InRange(TKey myFromKey, TKey myToKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion, bool myOrEqualFromKey = true, bool myOrEqualToKey = true)
        {
            var tree = GetTreeByVersion(myVersion);

            if (tree != null)
            {
                return tree.InRange(myFromKey, myToKey, myFunc, myOrEqualFromKey, myOrEqualToKey);
            }

            return null;
        }


        #endregion

        #region Update-methods for version tree

        /// <summary>
        /// Returns a data transfer object (DTO) containing a copy of the tree object
        /// and a list of Sibling-information, which are used during the update of the version
        /// tree.
        /// </summary>
        /// <returns>VersionedInfoDTO holding a copy of the tree and a list of siblinginfos</returns>
        private VersionInfoDTO<TKey, TValue> GetNewTreeVersion(ulong myTimeStamp)
        {
            lock (_LockObj)
            {
                var clone = _LatestVersion.Clone();
                clone.Timestamp = myTimeStamp;
                var modifiedNodes = new List<SiblingInfoDTO<TKey, TValue>>();
                return new VersionInfoDTO<TKey,TValue>() { Tree = clone, ModifiedNodes = modifiedNodes};
            }
        }

        /// <summary>
        /// Method updates the cersion tree using the following steps:
        /// 
        /// 1.  If the latest Version of the tree before starting modify is still the same else goto step 7
        /// 2.  Lock the version tree and insert the new timestamp and a reference to the view of the tree
        /// 3.  Make this change visible to the leafs siblings by adding them new siblings references.
        /// 4.  If VersionBuffer is full goto step 4 else goto step 6
        /// 5.  Reorganzize the version tree by inserting the last (timestamp, reference) pairs. The number is
        ///     given through the historysize of the tree
        /// 6.  Successfull...return true
        /// 7.  Not Successfull...return false
        /// </summary>
        /// <param name="newEntry">A reference to the new treeversion</param>
        /// <param name="myLatestModificationTimeStamp">the timestamp of the latest version before insertion</param>
        /// <param name="myTimeStamp">the timestamp of the treeversion to be inserted</param>
        private bool UpdateVersionTree(VersionInfoDTO<TKey, TValue> myVersionInfoDTO, ulong myLatestModificationTimeStamp, ulong myTimeStamp)
        {
            //atomic update of the version tree
            lock(_LockObj)
            {
                if (myLatestModificationTimeStamp == _LatestVersion.Timestamp) //still correct version? avoid lost update
                {
                    //Debug.WriteLine("UpdateVersionTree: TaskID {0}, origLatestTimestamp = {1}, currentLatestTimestamp = {2}", Task.CurrentId, myLatestModificationTimeStamp, _LatestVersion.Timestamp);

                    #region update version tree

                    //add the new version to the version tree
                    _VersionTree.Add(myTimeStamp, myVersionInfoDTO.Tree);

                    //and update the reference to the latest version
                    _LatestVersion = myVersionInfoDTO.Tree;

                    #endregion

                    #region update siblings

                    /*
                     * copying nodes affects the left and right siblings. while this atomar process of setting the actual root,
                     * these nodes are being updated. the key is the revision (timestamp) of the tree, and the reference is the cloned
                     * or new (because of splitting) node.
                     */
                    //Debug.WriteLine("TaskID {0} | UPDATING NODES => NodeCount: {1}", Task.CurrentId, myVersionInfoDTO.ModifiedNodes.Count);

                    foreach (var siblingInfo in myVersionInfoDTO.ModifiedNodes)
                    {
                        //Debug.WriteLine("TaskID {0} | leftSiblings: {1} | rightSiblings {2} | keys {3}", Task.CurrentId, siblingInfo.LeftSiblings.Count, siblingInfo.RightSiblings.Count, siblingInfo.Node.ToString());

                        foreach (var leftSibling in siblingInfo.LeftSiblings)
                        {
                            leftSibling.RightSiblings[myTimeStamp] = siblingInfo.Node;
                        }
                        foreach (var rightSibling in siblingInfo.RightSiblings)
                        {
                            rightSibling.LeftSiblings[myTimeStamp] = siblingInfo.Node;
                        }
                    }

                    #endregion

                    #region cleanup version tree if necessary

                    //check version count
                    if (_VersionTree.KeyCount >= _HistorySize)
                    {
                        if (_CurrentBufferSize >= _VersionBuffer) //time to do some cleanup?
                        {
                            //new version tree
                            var newVersionTree = new BPlusTree<ulong, IndexView<TKey, TValue>>(_VersionTree.Order);

                            var kvpEnum = _VersionTree.GetReverseEnumerator();

                            var versionCounter = 0;

                            while (kvpEnum.MoveNext() && versionCounter < _HistorySize)
                            {
                                var val = kvpEnum.Current.Value as IEnumerable<IndexView<TKey, TValue>>;
                                newVersionTree.Add(kvpEnum.Current.Key, val);
                                versionCounter++;
                            }
                            Interlocked.Exchange(ref _CurrentBufferSize, 0);
                        }
                        else
                        {
                           Interlocked.Increment(ref _CurrentBufferSize);
                        }
                    }

                    #endregion

                    return true;
                }
                else
                {
                    Interlocked.Increment(ref _MissCounter);

                    //Debug.WriteLine("INSERT failed! taskID {0} global failcount: {1}", Task.CurrentId, _MissCounter);
                    return false;
                }
            }
        }

        /// <summary>
        /// Returns the version of the tree by a given timestamp.
        /// 
        /// If myVersion is below zero, the version relative to the 
        /// latestVersion will be returned.
        /// 
        /// If no tree exists at the given timestamp or relative to 
        /// the latest, null will be returned.
        /// </summary>
        /// <param name="myVersion">Version</param>
        /// <returns>BPlusTree or null if there is no tree at this version</returns>
        private IndexView<TKey, TValue> GetTreeByVersion(long myVersion)
        {
            #region

            HashSet<IndexView<TKey, TValue>> treeSet = null;

            if (myVersion > 0) //using timestamp
            {
                //get the exact timestamp or the smaller one
                treeSet = _VersionTree.GetBestMatch((ulong)myVersion, true);
            }
            else if (myVersion == 0) //latest
            {
                return _LatestVersion;
            }
            else //relative to the latest version (< 0)
            {
                var steps = Math.Abs(myVersion);

                var leaf = _VersionTree._RightMostLeaf;

                while (leaf != null && steps >= leaf.ValueCount)
                {
                    steps -= leaf.ValueCount;
                    leaf = leaf.LeftSibling as LeafNode<ulong, IndexView<TKey, TValue>>;
                }

                if (leaf != null)
                {
                    var index = leaf.ValueCount - steps - 1;
                    if (index > -1) //inbound?
                    {
                        treeSet = leaf.Values[index];
                    }
                }
            }

            #endregion

            if (treeSet != null && treeSet.Count > 0)
            {
                //there can be  only one tree in the valueset
                foreach (var tree in treeSet)
                {
                    return tree;
                }
            }
            return null;
        }

        #endregion
    }

    /// <summary>
    /// This helper class is used during tree modification (add, update, remove).
    /// It holds a reference to a copy of the latest tree and a list of Nodes
    /// whose left and right siblings have to be updated.
    /// </summary>
    class VersionInfoDTO<TKey, TValue> where TKey : IComparable
    {
        private IndexView<TKey, TValue> _Tree;

        private List<SiblingInfoDTO<TKey, TValue>> _ModifiedNodes;

        internal IndexView<TKey, TValue> Tree
        {
            get { return _Tree; }
            set { _Tree = value; }
        }

        internal List<SiblingInfoDTO<TKey, TValue>> ModifiedNodes
        {
            get { return _ModifiedNodes; }
            set { _ModifiedNodes = value; }
        }
    }

    /// <summary>
    /// This helper class is used during tree modification (add, update, remove).
    /// It holds a node and a number of left and right siblings have to be added
    /// to that node during atomic updating the version layer.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    class SiblingInfoDTO<TKey, TValue> where TKey : IComparable
    {
        /// <summary>
        /// The node whose siblings have to be updated
        /// </summary>
        private VersionedLeafNode<TKey, TValue> _Node;

        /// <summary>
        /// The list of new left siblings of the node
        /// </summary>
        private List<VersionedLeafNode<TKey, TValue>> _LeftSiblings = new List<VersionedLeafNode<TKey,TValue>>();

        /// <summary>
        /// The list of new right siblings of the node
        /// </summary>
        private List<VersionedLeafNode<TKey, TValue>> _RightSiblings = new List<VersionedLeafNode<TKey, TValue>>();

        internal VersionedLeafNode<TKey, TValue> Node
        {
            get { return _Node; }
            set { _Node = value; }
        }

        internal List<VersionedLeafNode<TKey, TValue>> LeftSiblings
        {
            get { return _LeftSiblings; }
            set { _LeftSiblings = value; }
        }

        internal List<VersionedLeafNode<TKey, TValue>> RightSiblings
        {
            get { return _RightSiblings; }
            set { _RightSiblings = value; }
        }
    }
}

