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

/*
 * DictionaryValueHistory
 * (c) Achim Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Collections;
using System.Collections.Generic;

using sones.Lib.DataStructures.Timestamp;

#endregion

namespace sones.Lib.DataStructures.Dictionaries
{

    /// <summary>
    /// This datastructure implements a linked list of versioned and serialized
    /// values to be used within single-value datastructures
    /// </summary>
    /// <typeparam name="TValue">The type of the stored value</typeparam>

    public class DictionaryValueHistory<TValue> : IEnumerable<TimestampValuePair<TValue>>
        where TValue: IEstimable
    {


        #region Data

        LinkedList<TimestampValuePair<TValue>> _DictionaryHistoryList;

        #endregion

        #region Properties

        #region LatestValue

        public TValue LatestValue
        {
            get
            {
                lock (this)
                {

                    if (_DictionaryHistoryList.ULongCount() > 0)
                        return _DictionaryHistoryList.First.Value.Value;

                    return default(TValue);

                }

            }
        }

        #endregion

        #region LatestTimestamp

        public UInt64 LatestTimestamp
        {
            get
            {
                lock (this)
                {

                    if (_DictionaryHistoryList.ULongCount() > 0)
                        return _DictionaryHistoryList.First.Value.Timestamp;

                    return 0;

                }
            }
        }

        #endregion

        #region isDeleted

        public Boolean isDeleted
        {

            get
            {
                lock (this)
                {

                    if (_DictionaryHistoryList.ULongCount() > 0)
                    {

                        if (default(TValue) == null && _DictionaryHistoryList.First.Value.Value == null)
                            return true;

                        if (!_DictionaryHistoryList.First.Value.Value.Equals(default(TValue)))
                            return false;

                    }

                    return true;

                }
            }
            
        }

        #endregion

        #region VersionCount

        public UInt64 VersionCount
        {
            get
            {
                lock (this)
                {
                    return _DictionaryHistoryList.ULongCount();
                }
            }
        }

        #endregion

        #endregion


        #region Constructors

        #region DictionaryValueHistory()

        /// <summary>
        /// Simple consructor
        /// </summary>
        public DictionaryValueHistory()
        {
            _DictionaryHistoryList = new LinkedList<TimestampValuePair<TValue>>();
        }

        #endregion

        #region DictionaryValueHistory(myValue)

        /// <summary>
        /// Adds the given value using the actual time as timestamp.
        /// </summary>
        /// <param name="myValue"></param>
        public DictionaryValueHistory(TValue myValue)
            : this()
        {
            Add(myValue);
        }

        #endregion

        #region DictionaryValueHistory(myTimestamp, myValue)

        /// <summary>
        /// Adds the given value using the given timestamp. A timestamp less
        /// than zero indicates, that this value was deleted
        /// </summary>
        /// <param name="myTimestamp">the timestamp of the value</param>
        /// <param name="myValue">the value</param>
        public DictionaryValueHistory(UInt64 myTimestamp, TValue myValue)
            : this()
        {
            Add(myTimestamp, myValue);
        }

        #endregion

        #region DictionaryValueHistory(myTimestampedValues)

        /// <summary>
        /// Adds the given value using the given timestamp. A timestamp less
        /// than zero indicates, that this value was deleted
        /// </summary>
        /// <param name="myTimestamp">the timestamp of the value</param>
        /// <param name="myValue">the value</param>
        public DictionaryValueHistory(IEnumerable<TimestampValuePair<TValue>> myTimestampedValues)
            : this()
        {
           // Add(myTimestamp, myValue);
        }

        #endregion

        #endregion


        #region Object-specific methods

        #region Add

        #region Add(myValue)

        public UInt64 Add(TValue myValue)
        {
            lock (this)
            {

                // Mark the actual value as deleted
                //isDeleted = true;

                var _ActualTimestamp = TimestampNonce.Ticks;

                _DictionaryHistoryList.AddFirst(new TimestampValuePair<TValue>(_ActualTimestamp, myValue));

                return _ActualTimestamp;

            }
        }

        #endregion

        #region Add(myValues)

        public UInt64 Add(IEnumerable<TValue> myValues)
        {
            lock (this)
            {

                var _ActualTimestamp = TimestampNonce.Ticks;

                foreach (var _Value in myValues)
                    _DictionaryHistoryList.AddFirst(new TimestampValuePair<TValue>(_ActualTimestamp, _Value));

                return _ActualTimestamp;

            }
        }

        #endregion

        #region Add(myTimestamp, myValue)

        public UInt64 Add(UInt64 myTimestamp, TValue myValue)
        {
            lock (this)
            {

                _DictionaryHistoryList.AddFirst(new TimestampValuePair<TValue>(myTimestamp, myValue));

                return myTimestamp;

            }
        }

        #endregion

        #region Add(myTimestampedValue)

        public UInt64 Add(TimestampValuePair<TValue> myTimestampedValue)
        {
            lock (this)
            {
                _DictionaryHistoryList.AddFirst(myTimestampedValue);
                return myTimestampedValue.Timestamp;
            }
        }

        #endregion

        #region Add(myTimestampedValues)

        public void Add(IEnumerable<TimestampValuePair<TValue>> myTimestampedValues)
        {
            lock (this)
            {
                foreach (var _TimestampedValue in myTimestampedValues)
                    _DictionaryHistoryList.AddFirst(_TimestampedValue);
            }
        }

        #endregion

        #endregion

        #region Set

        #region Set(myValue)

        public void Set(TValue myValue)
        {
            lock (this)
            {

                // Remove all prior entries
                _DictionaryHistoryList.Clear();

                Add(myValue);

            }
        }

        #endregion

        #region Set(myValues)

        public void Set(IEnumerable<TValue> myValues)
        {
            lock (this)
            {

                // Remove all prior entries
                _DictionaryHistoryList.Clear();

                Add(myValues);

            }
        }

        #endregion

        #region Set(myTimestamp, myValue)

        public void Set(UInt64 myTimestamp, TValue myValue)
        {
            lock (this)
            {

                // Remove all prior entries
                _DictionaryHistoryList.Clear();

                Add(myTimestamp, myValue);

            }
        }

        #endregion

        #region Set(myTimestampedValue)

        public void Set(TimestampValuePair<TValue> myTimestampedValue)
        {
            lock (this)
            {

                // Remove all prior entries
                _DictionaryHistoryList.Clear();

                Add(myTimestampedValue);

            }
        }

        #endregion

        #region Set(myTimestampedValues)

        public void Set(IEnumerable<TimestampValuePair<TValue>> myTimestampedValues)
        {
            lock (this)
            {

                // Remove all prior entries
                _DictionaryHistoryList.Clear();

                Add(myTimestampedValues);

            }
        }

        #endregion

        #endregion

        #region this[]

        #region this[myVersion]  // Int64

        /// <summary>
        /// This method will return an older version of the value. If myValue is less
        /// or equals zero it will be interpreted as a relative version, if it is greater
        /// than zero it will be interpreted as a timestamp and the version of the value
        /// will be returned which had been actual at the given timestamp.
        /// </summary>
        /// <param name="myVersion">less or equals zero => a relative version number; greater zero => a timestamp</param>
        /// <returns>An older version of the value</returns>
        public TValue this[Int64 myVersion]
        {

            get
            {

                #region  This will return a relative version of the value...

                if (myVersion <= 0)
                {

                    myVersion = Math.Abs(myVersion);

                    if (_DictionaryHistoryList.Count > myVersion)
                    {

                        var _ActualListEntry = _DictionaryHistoryList.First;

                        while (_ActualListEntry.Next != null && myVersion != 0)
                        {
                            _ActualListEntry = _ActualListEntry.Next;
                            myVersion--;
                        }

                        if (myVersion == 0 && _ActualListEntry != null)
                            return _ActualListEntry.Value.Value;

                    }

                }

                #endregion

                #region This will return the version of the value which had been actual at the given timestamp

                else
                    if (_DictionaryHistoryList.Count > 0)
                        return this[(UInt64)myVersion];

                #endregion

                return default(TValue);

            }

        }

        #endregion

        #region this[myVersion]  // UInt64

        /// <summary>
        /// This method will return the version of the value which had been actual at the given timestamp.
        /// </summary>
        /// <param name="myVersion">The timestamp</param>
        /// <returns>An older version of the value</returns>
        public TValue this[UInt64 myVersion]
        {

            get
            {

                if (_DictionaryHistoryList.Count > 0)
                {

                    var _ActualListEntry = _DictionaryHistoryList.First;
                    var _Version         = (UInt64) myVersion;

                    while (_ActualListEntry.Next != null)
                    {

                        if (_Version <= _ActualListEntry.Value.Timestamp)
                            break;

                        _ActualListEntry = _ActualListEntry.Next;

                    }

                    return _ActualListEntry.Value.Value;

                }

                return default(TValue);


            }

        }

        #endregion

        #region this[myFunc]

        public IEnumerable<TValue> this[Func<TimestampValuePair<TValue>, Boolean> myFunc]
        {
            get
            {
                lock (this)
                {

                    var _List       = new List<TValue>();
                    var _Enumerator = _DictionaryHistoryList.GetEnumerator();

                    while (_Enumerator.MoveNext())
                    {
                        if (myFunc(_Enumerator.Current))
                            _List.Add(_Enumerator.Current.Value);
                    }

                    return _List;

                }
            }
        }

        #endregion

        //#region GetAll()

        //public IEnumerable<TimestampValuePair<TValue>> GetAll()
        //{
        //    return from _TimestampValuePair in _DictionaryHistoryList select _TimestampValuePair;
        //}

        //#endregion

        #endregion

        #region Delete()

        /// <summary>
        /// Marks the actual value as deleted, but keeps the value history
        /// </summary>
        public void Delete()
        {
            lock (this)
            {

                if (_DictionaryHistoryList.Count > 0)
                    if (!_DictionaryHistoryList.First.Value.Value.Equals(default(TValue)))
                        Add(default(TValue));

            }
        }

        #endregion

        #region Delete(myTimestamp)

        /// <summary>
        /// Marks the actual value as deleted, but keeps the value history
        /// </summary>
        public void Delete(UInt64 myTimestamp)
        {
            lock (this)
            {

                if (_DictionaryHistoryList.Count > 0)
                    if (!_DictionaryHistoryList.First.Value.Value.Equals(default(TValue)))
                        Add(myTimestamp, default(TValue));

            }
        }

        #endregion



        #region TruncateHistory(myNewHistorySize)

        /// <summary>
        /// Reduces the number of values stored within the history to the given value
        /// </summary>
        public void TruncateHistory(UInt64 myNewHistorySize)
        {

            lock (this)
            {

                // Remove values
                while ( (UInt64) _DictionaryHistoryList.Count > myNewHistorySize)
                    _DictionaryHistoryList.RemoveLast();

                // Remove "deleted" reference type values...
                if (default(TValue) == null)
                    while (default(TValue) == null && _DictionaryHistoryList.Last.Value.Value == null)
                        _DictionaryHistoryList.RemoveLast();

                // or remove "deleted" value type values...
                //while (_DictionaryHistoryList.Last.Value.Value.Equals(default(TValue)))
                else
                    while (default(TValue).Equals(_DictionaryHistoryList.Last.Value.Value))
                        _DictionaryHistoryList.RemoveLast();


            }

        }

        #endregion

        #region ClearHistory()

        /// <summary>
        /// Clears the history information
        /// </summary>
        public void ClearHistory()
        {
            TruncateHistory(1);
        }

        #endregion

        #endregion


        #region IEnumerable<DictionaryValue<TValue>> Members

        public IEnumerator<TimestampValuePair<TValue>> GetEnumerator()
        {
            return _DictionaryHistoryList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _DictionaryHistoryList.GetEnumerator();
        }

        #endregion


        #region ToString()

        public override String ToString()
        {
            return "DictionaryValueHistory(" + _DictionaryHistoryList.ULongCount() + " of type " + typeof(TValue).Name + ") ";
        }

        #endregion


    }

}
