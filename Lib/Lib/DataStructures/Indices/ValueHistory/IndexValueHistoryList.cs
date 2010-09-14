/*
 * IndexValueHistoryList
 * (c) Achim Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Linq;
using System.Collections.Generic;

using sones.Lib.DataStructures.Timestamp;
using sones.Lib.DataStructures.Indices;

#endregion

namespace sones.Lib.DataStructures.Indices
{

    public class IndexValueHistoryList<TValue>
    {


        #region Data

        // A datastructure to hold all values and serialized values of the latest version
        HashSet<IndexValue<TValue>>            _IndexValueList;

        // A datastructure to hold the history of the most recent
        // modifications of the _IndexValueList datastructure
        LinkedList<IndexValueHistory<TValue>>  _IndexValueHistoryList;

        #endregion

        #region Properties

        #region Values

        public HashSet<TValue> Values
        {
            get
            {
                lock (_IndexValueList)
                {
                    return new HashSet<TValue>((from items in _IndexValueList
                                                select items.Value).ToList<TValue>());
                }
            }
        }

        #endregion

        #region SerializedValues

        public List<Byte[]> SerializedValues
        {
            get
            {
                lock (_IndexValueList)
                {
                    return (from items in _IndexValueList select items.SerializedValue).ToList<Byte[]>();
                }
            }
        }

        #endregion

        #region IndexValueHistoryList

        public LinkedList<IndexValueHistory<TValue>> InternalIndexValueHistoryList
        {
            get
            {
                return _IndexValueHistoryList;
            }
            set
            {
                _IndexValueHistoryList = value;

                _IndexValueList = new HashSet<IndexValue<TValue>>();

                var val = new IndexValue<TValue>();

                //rebuild the indexvaluelist by adding/removing all sets of the history items
                foreach (var indexValueHistory in _IndexValueHistoryList.Reverse())
                {
                    
                    foreach (var remVal in indexValueHistory.RemSet)
                    {                        
                        _IndexValueList.Remove(new IndexValue<TValue>(remVal));
                    }
                    foreach (var addVal in indexValueHistory.AddSet)
                    {
                        _IndexValueList.Add(new IndexValue<TValue>(addVal));
                    }
                }
            }

        }

        #endregion

        #region LatestTimestamp

        public UInt64 LatestTimestamp
        {
            get
            {
                lock (_IndexValueHistoryList)
                {

                    if (_IndexValueHistoryList.Count > 0)
                    {

                        var _FirstValue = _IndexValueHistoryList.First.Value;

                        if (_FirstValue != null)
                            return _FirstValue.Timestamp;

                    }

                    return 0;

                }
            }
        }

        #endregion


        #region isDeleted


        public Boolean isLatestDeleted
        {
            get
            {
                lock (_IndexValueList)
                {

                    if (_IndexValueList.Count == 0)
                        return true;

                    return false;

                }
            }
            set
            {
                Clear();
            }

        }

        #endregion


        #region VersionCount

        public UInt64 VersionCount
        {
            get
            {
                lock (_IndexValueHistoryList)
                {
                    return (UInt64) _IndexValueHistoryList.Count;
                }
            }
        }

        #endregion

        #endregion


        #region Constructors

        #region IndexValueHistoryList()

        /// <summary>
        /// Simple constructor
        /// </summary>
        public IndexValueHistoryList()
        {
            _IndexValueList        = new HashSet<IndexValue<TValue>>();
            _IndexValueHistoryList = new LinkedList<IndexValueHistory<TValue>>();
        }

        #endregion

        #region IndexValueHistoryList(myValue)

        /// <summary>
        /// Adds the given value using the actual time as timestamp
        /// </summary>
        /// <param name="myValue"></param>
        public IndexValueHistoryList(TValue myValue)
            : this()
        {
            Add(myValue);
        }

        #endregion

        #region IndexValueHistoryList(myValues)

        /// <summary>
        /// Adds the given list of values using the actual time as timestamp
        /// </summary>
        /// <param name="myListOfValues"></param>
        public IndexValueHistoryList(IEnumerable<TValue> myValues)
            : this()
        {
            Add(myValues);
        }

        #endregion

        //#region IndexValueHistoryList(myValue, mySerializedValue)

        ///// <summary>
        ///// Adds the given value using the given timestamp. A timestamp less
        ///// than zero indicates, that this value was deleted
        ///// </summary>
        ///// <param name="myValue">the value</param>
        ///// <param name="mySerializedValue"></param>
        //public IndexValueHistoryList(TValue myValue, Byte[] mySerializedValue)
        //    : this()
        //{
        //    Add(myValue, mySerializedValue);
        //}

        //#endregion

        #region IndexValueHistoryList(myValue, myTimestamp)

        /// <summary>
        /// Adds the given value using the given timestamp. A timestamp less
        /// than zero indicates, that this value was deleted
        /// </summary>
        /// <param name="myValue">the value</param>
        /// <param name="myTimestamp">the timestamp of the value</param>
        public IndexValueHistoryList(TValue myValue, UInt64 myTimestamp)
            : this()
        {
            Add(myValue, myTimestamp);
        }

        #endregion

        //#region IndexValueHistoryList(myValue, mySerializedValue, myTimestamp)

        ///// <summary>
        ///// Adds the given value using the given timestamp. A timestamp less
        ///// than zero indicates, that this value was deleted
        ///// </summary>
        ///// <param name="myTimestamp">the timestamp of the value</param>
        ///// <param name="myValue">the value</param>
        ///// <param name="mySerializedValue"></param>
        //public IndexValueHistoryList(TValue myValue, Byte[] mySerializedValue, Int64 myTimestamp)
        //    : this()
        //{
        //    Add(myValue, mySerializedValue, myTimestamp);
        //}

        //#endregion

        #endregion


        #region Object-specific methods

        #region Add

        #region Add(myValue)

        public void Add(TValue myValue)
        {
            Set(new HashSet<TValue> { myValue }, IndexSetStrategy.MERGE);
        }

        #endregion

        #region Add(myValues)

        public void Add(IEnumerable<TValue> myValues)
        {
            Set(myValues, IndexSetStrategy.MERGE);
        }

        #endregion


        #region Add(myValue, myTimestamp)

        public void Add(TValue myValue, UInt64 myTimestamp)
        {
            Set(new HashSet<TValue> { myValue }, myTimestamp, IndexSetStrategy.MERGE);
        }

        #endregion

        #region Add(myValues, myTimestamp)

        public void Add(IEnumerable<TValue> myValues, UInt64 myTimestamp)
        {
            Set(myValues, myTimestamp, IndexSetStrategy.MERGE);
        }

        #endregion


        //#region Add(myValue, mySerializedValue)

        //public void Add(TValue myValue, Byte[] mySerializedValue)
        //{
        //    Set(myValue, mySerializedValue, IndexSetStrategy.MERGE);
        //}

        //#endregion


        //#region Add(myValue, mySerializedValue, myTimestamp)

        //public void Add(TValue myValue, Byte[] mySerializedValue, Int64 myTimestamp)
        //{
        //    Set(myValue, mySerializedValue, IndexSetStrategy.MERGE);
        //}

        //#endregion

        #endregion

        #region Set

        #region Set(myValue, myIndexSetStrategy)

        public void Set(TValue myValue, IndexSetStrategy myIndexSetStrategy)
        {
            Set(new HashSet<TValue> { myValue }, myIndexSetStrategy);
        }

        #endregion

        #region Set(myValues, myIndexSetStrategy)

        public void Set(IEnumerable<TValue> myValues, IndexSetStrategy myIndexSetStrategy)
        {
            Set(myValues, TimestampNonce.Ticks, myIndexSetStrategy);
        }

        #endregion


        #region Set(myValue, myTimestamp, myIndexSetStrategy)

        public void Set(TValue myValue, UInt64 myTimestamp, IndexSetStrategy myIndexSetStrategy)
        {
            Set(new HashSet<TValue> { myValue }, myTimestamp, myIndexSetStrategy);
        }

        #endregion

        #region Set(myValues, myTimestamp, myIndexSetStrategy)

        public void Set(IEnumerable<TValue> myValues, UInt64 myTimestamp, IndexSetStrategy myIndexSetStrategy)
        {
            lock (_IndexValueList)
            {
                lock (_IndexValueHistoryList)
                {

                    // Get old values
                    var OldValues = new HashSet<TValue>(from Item in _IndexValueList select Item.Value);
                    var NewValues = new HashSet<TValue>(myValues);

                    // Get values which would be removed and readded afterwards!
                    var InterSect = new HashSet<TValue>(from Item in _IndexValueList select Item.Value);
                        InterSect.IntersectWith(NewValues);

                    // Remove the value which would be readded anyways!
                    NewValues.ExceptWith(InterSect);

                    // If there is nothing to add, then there's no need for a new revision
                    if (NewValues.Count > 0)
                    {
                        switch (myIndexSetStrategy)
                        {

                            // Remove all prior entries
                            case IndexSetStrategy.REPLACE:

                                // Remove the value which would be readded anyways!
                                OldValues.ExceptWith(InterSect);

                                // Add all to the history log
                                _IndexValueHistoryList.AddFirst(new IndexValueHistory<TValue>(myTimestamp, NewValues, OldValues));

                                // Remove all old values
                                _IndexValueList.Clear();

                                break;

                            case IndexSetStrategy.MERGE:

                                // Get old values to remove
                                NewValues.ExceptWith(OldValues);

                                // Add all to the history log
                                _IndexValueHistoryList.AddFirst(new IndexValueHistory<TValue>(myTimestamp, NewValues, null));

                                break;

                        }

                        // Add new values
                        foreach (var Item in NewValues)
                            _IndexValueList.Add(new IndexValue<TValue>(Item));
                    }

                }
            }
        }

        #endregion

        #endregion

        #region Remove

        #region Remove(myValue)

        public Boolean Remove(TValue myValue)
        {

            //var _Success = (from ll in _IndexValueList where ll.Value.Equals(myValue) select _IndexValueList.Remove(ll)).First<Boolean>();
            var _Success = (from ll in _IndexValueList where ll.Value.Equals(myValue) select _IndexValueList.Remove(ll)).FirstOrDefault<Boolean>();

            // Add Remove-Operation to the HistoryLog
            if (_Success)
                _IndexValueHistoryList.AddFirst(new LinkedListNode<IndexValueHistory<TValue>>(new IndexValueHistory<TValue>(null, new HashSet<TValue>{ myValue })));

            return _Success;

        }

        #endregion

        #region Remove(myValue, myTimestamp)

        public Boolean Remove(TValue myValue, UInt64 myTimestamp)
        {

            //var _Success = (from ll in _IndexValueList where ll.Value.Equals(myValue) select _IndexValueList.Remove(ll)).First<Boolean>();
            var _Success = (from ll in _IndexValueList where ll.Value.Equals(myValue) select _IndexValueList.Remove(ll)).FirstOrDefault<Boolean>();

            // Add Remove-Operation to the HistoryLog
            if (_Success)
                _IndexValueHistoryList.AddFirst(new LinkedListNode<IndexValueHistory<TValue>>(new IndexValueHistory<TValue>(myTimestamp, null, new HashSet<TValue> { myValue })));

            return _Success;

        }

        #endregion

        #region Remove(myValues)

        public Boolean Remove(IEnumerable<TValue> myValues)
        {

            var _HashSet  = new HashSet<TValue>(from Item in _IndexValueList select Item.Value);
            var _ToDelete = new HashSet<TValue>(_HashSet.Intersect<TValue>(myValues));

            var _Success  = (from Item in _IndexValueList where _ToDelete.Contains<TValue>(Item.Value) select _IndexValueList.Remove(Item)).Max<Boolean>();

            // Add Remove-Operation to the HistoryLog
            _IndexValueHistoryList.AddFirst(new LinkedListNode<IndexValueHistory<TValue>>(new IndexValueHistory<TValue>(null, _ToDelete)));

            return _Success;

        }

        #endregion

        #region Remove(myValues, myTimestamp)

        public Boolean Remove(IEnumerable<TValue> myValues, UInt64 myTimestamp)
        {

            var _HashSet = new HashSet<TValue>(from Item in _IndexValueList select Item.Value);
            var _ToDelete = new HashSet<TValue>(_HashSet.Intersect<TValue>(myValues));

            var _Success = (from Item in _IndexValueList where _ToDelete.Contains<TValue>(Item.Value) select _IndexValueList.Remove(Item)).Max<Boolean>();

            // Add Remove-Operation to the HistoryLog
            _IndexValueHistoryList.AddFirst(new LinkedListNode<IndexValueHistory<TValue>>(new IndexValueHistory<TValue>(myTimestamp, null, _ToDelete)));

            return _Success;

        }

        #endregion

        #region Clear()

        public void Clear()
        {

            lock (_IndexValueList)
            {
                lock (_IndexValueHistoryList)
                {

                    var _HashSet = new HashSet<TValue>(from Item in _IndexValueList select Item.Value);

                    if (_HashSet.Count > 0)
                    {

                        _IndexValueList.Clear();

                        // Add Remove-Operation to the HistoryLog
                        _IndexValueHistoryList.AddFirst(new LinkedListNode<IndexValueHistory<TValue>>(new IndexValueHistory<TValue>(null, _HashSet)));

                    }

                }
            }

        }

        #endregion

        #region Clear(myTimestamp)

        public void Clear(UInt64 myTimestamp)
        {

            lock (_IndexValueList)
            {
                lock (_IndexValueHistoryList)
                {

                    var _HashSet = new HashSet<TValue>(from Item in _IndexValueList select Item.Value);

                    if (_HashSet.Count > 0)
                    {

                        _IndexValueList.Clear();

                        // Add Remove-Operation to the HistoryLog
                        _IndexValueHistoryList.AddFirst(new LinkedListNode<IndexValueHistory<TValue>>(new IndexValueHistory<TValue>(myTimestamp, null, _HashSet)));

                    }

                }
            }

        }

        #endregion

        #endregion


        #region this[myVersion] // Int64

        /// <summary>
        /// This method will return an older version of the value. If myValue is less
        /// or equals zero it will be interpreted as a relative version, if it is greater
        /// than zero it will be interpreted as a timestamp and the version of the value
        /// will be returned which had been actual at the given timestamp.
        /// </summary>
        /// <param name="myVersion">less or equals zero => a relative version number; greater zero => a timestamp</param>
        /// <returns>An older version of the value</returns>
        public HashSet<TValue> this[Int64 myVersion]
        {

            get
            {
                lock (_IndexValueList)
                {
                    lock (_IndexValueHistoryList)
                    {

                        // Get actual HashSet
                        var _HashSet = new HashSet<TValue>(from items in _IndexValueList select items.Value);

                        // Revert changes to the HashSet by reverse-replaying the HistoryLog
                        if (_IndexValueHistoryList.Count > 0)
                        {

                            #region Relative version (myVersion < 0)

                            if (myVersion < 0)
                            {

                                myVersion = Math.Abs(myVersion);

                                var _ActualHistoryEntry = _IndexValueHistoryList.First;
                                
                                for (Int64 i = 1; i <= myVersion; i++)
                                {
                                    if (_ActualHistoryEntry != null)
                                    {

                                        // Revert changes by reverse-replaying the HistoryLog
                                        foreach (var Item in _ActualHistoryEntry.Value.AddSet) { _HashSet.Remove(Item); }
                                        foreach (var Item in _ActualHistoryEntry.Value.RemSet) { _HashSet.Add(Item); }

                                        _ActualHistoryEntry = _ActualHistoryEntry.Next;

                                    }

                                    // No more versions available
                                    else
                                        break;

                                }

                            }

                            #endregion

                            #region Version based on timestamp (myVersion > 0)

                            else if (myVersion > 0)
                                return this[(UInt64)myVersion];

                            #endregion

                        }

                        return _HashSet;

                    }

                }
            }

        }

        #endregion

        #region this[myVersion] // UInt64

        /// <summary>
        /// This method will return an older version of the value. If myValue is less
        /// or equals zero it will be interpreted as a relative version, if it is greater
        /// than zero it will be interpreted as a timestamp and the version of the value
        /// will be returned which had been actual at the given timestamp.
        /// </summary>
        /// <param name="myVersion">less or equals zero => a relative version number; greater zero => a timestamp</param>
        /// <returns>An older version of the value</returns>
        public HashSet<TValue> this[UInt64 myVersion]
        {

            get
            {
                lock (_IndexValueList)
                {
                    lock (_IndexValueHistoryList)
                    {

                        // Get actual HashSet
                        var _HashSet = new HashSet<TValue>(from items in _IndexValueList select items.Value);

                        // Revert changes to the HashSet by reverse-replaying the HistoryLog
                        if (_IndexValueHistoryList.Count > 0)
                        {

                            // Get version based on timestamp (myVersion > 0)
                            if (myVersion > 0)
                            {

                                var _ActualHistoryEntry = _IndexValueHistoryList.First;
                                var _Version            = myVersion;

                                while (_ActualHistoryEntry != null)
                                {

                                    if (_ActualHistoryEntry.Value.Timestamp > _Version)
                                    {

                                        // Revert changes by reverse-replaying the HistoryLog
                                        foreach (var Item in _ActualHistoryEntry.Value.AddSet) { _HashSet.Remove(Item); }
                                        foreach (var Item in _ActualHistoryEntry.Value.RemSet) { _HashSet.Add(Item); }

                                        _ActualHistoryEntry = _ActualHistoryEntry.Next;

                                    }

                                    // 
                                    else
                                        break;

                                }

                            }

                        }

                        return _HashSet;

                    }

                }
            }

        }

        #endregion


        #region RemoveLatestFromHistory()

        /// <summary>
        /// Removes the latest value stored
        /// </summary>
        /// <returns>true if sucseeded, false if not</returns>
        public Boolean RemoveLatestFromHistory()
        {

            if (_IndexValueHistoryList.Count > 1)
            {

                _IndexValueHistoryList.RemoveLast();

                // There might be some empty entries
                while (_IndexValueHistoryList.Last.Value.AddSet.Count == 0 && _IndexValueHistoryList.Last.Value.RemSet.Count == 0)
                    _IndexValueHistoryList.RemoveLast();

                return true;

            }

            return false;

        }

        #endregion

        #region TruncateHistory(myNewHistorySize)

        /// <summary>
        /// Reduces the number of values stored within the history to the given value
        /// </summary>
        public void TruncateHistory(UInt64 myNewHistorySize)
        {

            lock (_IndexValueHistoryList)
            {
                while ( (UInt64) _IndexValueHistoryList.Count > myNewHistorySize)
                    _IndexValueHistoryList.RemoveLast();
            }

        }

        #endregion

        #region ClearHistory()

        /// <summary>
        /// Clears the history information
        /// </summary>
        public void ClearHistory()
        {
            lock (_IndexValueHistoryList)
            {

                var _HashSet = new HashSet<TValue>(from Item in _IndexValueList select Item.Value);

                _IndexValueHistoryList.Clear();
                _IndexValueHistoryList.AddFirst(new LinkedListNode<IndexValueHistory<TValue>>(new IndexValueHistory<TValue>(_HashSet, null)));

            }
        }

        #endregion

        #endregion

    }

}
