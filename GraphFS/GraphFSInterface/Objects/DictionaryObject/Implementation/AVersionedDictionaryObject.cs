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


/* PandoraFS - AVersionedDictionaryObject
 * Achim Friedland, 2009
 * 
 * Lead programmer:
 *      Achim Friedland
 * 
 * */

#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using sones.GraphFS.DataStructures;
using sones.GraphFS.Session;

using sones.Lib;
using sones.Lib.BTree;
using sones.Lib.Session;
using sones.Lib.Serializer;
using sones.Lib.DataStructures;
using sones.Lib.NewFastSerializer;
using sones.Lib.DataStructures.Timestamp;
using sones.Lib.DataStructures.Indices;
using sones.Lib.DataStructures.Dictionaries;
using sones.Lib.DataStructures.Big;

#endregion

namespace sones.GraphFS.Objects
{

    /// <summary>
    /// An abstract implementation of a DictionaryObject to store a versioned mapping TKey => TValue which may be embedded into other objects.
    /// The internal DictionaryValueHistory&lt;TValue&gt; datastructure will keep up to $historysize older versions of TValue.
    /// </summary>
    /// <typeparam name="TKey">Must implement IComparable</typeparam>

    public abstract class AVersionedDictionaryObject<TKey, TValue> : AFSObject
        where TKey : IComparable
    {


        #region Data

        protected IDictionary<TKey, DictionaryValueHistory<TValue>> _IDictionary;

        #endregion

        #region Properties

        #region HistorySize

        private UInt64 _HistorySize;

        protected UInt64 HistorySize
        {

            get
            {
                return _HistorySize;
            }

            set
            {
                _HistorySize = value;
                TruncateHistory(_HistorySize);
            }

        }

        #endregion

        #endregion


        #region Constructors

        #region AVersionedDictionaryObject()

        /// <summary>
        /// This will create an empty AVersionedDictionaryObject using a Dictionary&lt;TKey, TValue&gt; for the internal IDictionary&lt;TKey, DictionaryValueHistory&lt;TValue&gt;&gt; object.
        /// The size of the value history will be set to 3.
        /// </summary>
        protected AVersionedDictionaryObject()
            : this(new BigDictionary<TKey, TValue>())
        {
        }

        #endregion

        #region AVersionedDictionaryObject(myIDictionaryObject)

        /// <summary>
        /// This will create an empty VersionedDictionaryObject using the given IDictionary object for the internal IDictionary&lt;TKey, DictionaryValueHistory&lt;TValue&gt;&gt; object.
        /// The size of the value history will be set to 3.
        /// </summary>s
        public AVersionedDictionaryObject(IDictionary<TKey, TValue> myIDictionaryType)
        {

            // Members of AGraphStructure
            _StructureVersion           = 1;

            // Members of AGraphObject
            _ObjectStream               = FSConstants.DEFAULT_INDEXSTREAM;


            // Object specific data...
            if (myIDictionaryType == null)
                throw new ArgumentNullException("Type '" + myIDictionaryType.ToString() + "' must not be null!");

            var _IDictionaryType        = myIDictionaryType.GetType().GetGenericTypeDefinition();
            if (_IDictionaryType == null)
                throw new ArgumentException("Type '" + myIDictionaryType.ToString() + "' is not a generic type with two type parameters!");

            var _IDictionaryGenericType = Activator.CreateInstance(_IDictionaryType.MakeGenericType(new Type[] { typeof(TKey), typeof(DictionaryValueHistory<TValue>) }));
            if (_IDictionaryGenericType == null)
                throw new ArgumentException("Type '" + myIDictionaryType + "' could not be instantiated as " + myIDictionaryType + "<" + typeof(TKey).ToString() + ", DictionaryValueHistory<" + typeof(TValue).ToString() + ">>!");

            _IDictionary                = _IDictionaryGenericType as IDictionary<TKey, DictionaryValueHistory<TValue>>;
            if (_IDictionary == null)
                throw new ArgumentException("Type '" + _IDictionaryGenericType.ToString() + "' does not implement IDictionary<..., ...>!");

            _HistorySize                = 3;

        }

        #endregion

        #region AVersionedDictionaryObject(myObjectLocation, mySerializedData)

        /// <summary>
        /// A constructor used for fast deserializing
        /// </summary>
        /// <param name="mySerializedData">A bunch of bytes[] containing the serialized IndexObject_HashTable</param>
        protected AVersionedDictionaryObject(String myObjectLocation, Byte[] mySerializedData)
            : this()
        {

            if (mySerializedData == null || mySerializedData.Length == 0)
                throw new ArgumentNullException("mySerializedData must not be null or its length be zero!");

            Deserialize(mySerializedData);

        }

        #endregion

        #endregion


        #region Members of AGraphStructure

        #region SerializeInnerObject(ref mySerializationWriter)

        public override void Serialize(ref SerializationWriter mySerializationWriter)
        {
            SerializeObject(ref mySerializationWriter, 0);
        }

        #endregion

        #region SerializeObject(ref mySerializationWriter, myNotificationHandling)

        public void SerializeObject(ref SerializationWriter mySerializationWriter, UInt64 myNotificationHandling)
        {

            lock (this)
            {

                try
                {                    

                    #region Write HistorySize

                    mySerializationWriter.WriteUInt64(_HistorySize);

                    #endregion

                    #region NotificationHandling

                    mySerializationWriter.WriteUInt64(myNotificationHandling);

                    #endregion

                    //#region Total number of entries

                    //var _TotalNumberOfEntries = (from _Values in _IDictionary.Values select _Values.VersionCount).Sum();
                    //mySerializationWriter.WriteObject(_TotalNumberOfEntries);

                    //#endregion

                    #region Write Key+Timestamp+Value pairs

                    foreach (var _KeyValuePair in _IDictionary)
                    {
                        foreach (var _TimestampValuePair in (from _TVPair in _KeyValuePair.Value select _TVPair).Reverse())
                        {                            
                                                        
                            // Write TKey...
                            mySerializationWriter.WriteObject(_KeyValuePair.Key);

                            // Write Timestamp...
                            mySerializationWriter.WriteUInt64(_TimestampValuePair.Timestamp);                            

                            // Write TValue...
                            
                            if (default(TValue) == null && _TimestampValuePair.Value == null)
                            {
                                mySerializationWriter.WriteObject(null);
                            }
                            else
                            {
                                mySerializationWriter.WriteObject(_TimestampValuePair.Value);
                                //((IFastSerialize)_TimestampValuePair.Value).Serialize(ref mySerializationWriter);
                            }
                        }
                    }

                    #endregion

                }

                catch (SerializationException e)
                {
                    throw new SerializationException("AVersionedDictionaryObject_HashTable serialization error: " + e.Message, e);
                }

                catch (Exception e)
                {
                    throw new Exception("AVersionedDictionaryObject_HashTable serialization error: " + e.Message, e);
                }

            }

        }

        #endregion

        #region DeserializeInnerObject(ref mySerializationReader)

        public override void Deserialize(ref SerializationReader mySerializationReader)
        {

            try
            {

                #region Read Key- and ValueTypes
                
                /*if (_IndexKeyType != typeof(TKey))
                    throw new GraphFSException_TypeParametersDiffer("Type parameter TKey of IndexObject_HashTable<" + typeof(TKey).ToString() + ", " + typeof(TValue).ToString() + "> is different from the serialized IndexObject_HashTable<" + _IndexKeyType.ToString() + ", " + _IndexValueType.ToString() + ">!");

                if (_IndexValueType != typeof(TValue) && (!(typeof(TValue) is Object)))
                    throw new GraphFSException_TypeParametersDiffer("Type parameter PT of IndexObject_HashTable<" + typeof(TKey).ToString() + ", " + typeof(TValue).ToString() + "> is different from the serialized IndexObject_HashTable<" + _IndexKeyType.ToString() + ", " + _IndexValueType.ToString() + ">!");*/

                #endregion

                #region Read HistorySize

                _HistorySize = mySerializationReader.ReadUInt64();

                #endregion

                #region NotificationHandling

                UInt64 _NotificationHandling = mySerializationReader.ReadUInt64();

                #endregion

                #region Read KeyTimestampValuePairs

                //var _IndexHashTableNrOfEntries = (UInt64) mySerializationReader.ReadObject();

                TKey   KeyObject;
                UInt64 Timestamp;
                TValue ValueObject = default(TValue);
                Object _Object;

                //for (UInt64 i = 0; i < _IndexHashTableNrOfEntries; i++)
                while (mySerializationReader.BytesRemaining>0)
                {

                    #region Read TKey

                    KeyObject = (TKey)mySerializationReader.ReadObject();
                        

                    #endregion

                    Timestamp = mySerializationReader.ReadUInt64();

                    #region Read TValue
                    
                    ValueObject = default(TValue);
                    _Object     = mySerializationReader.ReadObject();

                    #endregion

                    #region Add key/timestamp/value...

                    if (_Object != null)
                    {

                        ValueObject = (TValue) _Object;

                        //_IndexHashTable.Add((TKey)KeyObject, new DictionaryValueHistory<TValue>((TValue)ValueObject));

                        DictionaryValueHistory<TValue> _DictionaryValueHistory;

                        if (_IDictionary.TryGetValue(KeyObject, out _DictionaryValueHistory))
                            _DictionaryValueHistory.Add(Timestamp, ValueObject);

                        else
                            _IDictionary.Add(KeyObject, new DictionaryValueHistory<TValue>(Timestamp, ValueObject));

                    }

                    #endregion

                    #region Remove key/timestamp...

                    else
                    {

                        DictionaryValueHistory<TValue> _DictionaryValueHistory;

                        if (_IDictionary.TryGetValue(KeyObject, out _DictionaryValueHistory))
                            _DictionaryValueHistory.Delete(Timestamp);

                        else
                        {
                            // Do not do anything!
                        }

                    }

                    #endregion

                }

                #endregion

            }

            catch (Exception e)
            {
                throw new Exception("ADictionaryObject_HashTable<" + typeof(TKey).ToString() + ", " + typeof(TValue).ToString() + "> could not be deserialized!\n\n" + e);
            }

        }

        #endregion

        #endregion


        #region Members of ADictionaryObject

        #region Add - will fail if the key already exists

        #region (private) AddOnFileSystem(myKey, myValue, myTimestamp)

        private void AddOnFileSystem(TKey myKey, TValue myValue, UInt64 myTimestamp)
        {

            if (_IGraphFSReference.IsPersistent())
            {

                var _IFSStream = this._IGraphFSReference.Value.OpenStream(new SessionToken(new FSSessionInfo("AVersionedDictionaryObject")), _ObjectLocation, _ObjectStream, _ObjectEdition, null, 0);

                var _AppendingData = new SerializationWriter().WriteObject(myKey)
                                                              .WriteObject(myTimestamp)
                                                              .WriteObject(myValue)
                                                              .ToArray();

                _IFSStream.Value.Write(_AppendingData, SeekOrigin.End);
                _IFSStream.Value.Close();

            }

            else if (_IGraphFSSessionReference.IsPersistent())
            {
            //    var _IGraphStream = this._FSSessionReference.OpenStream(_ObjectLocation, _ObjectStream, _ObjectEdition, null, 0);

            //    var _AppendingData = new SerializationWriter().WriteObject(myKey)
            //                                                  .WriteObject(myTimestamp)
            //                                                  .WriteObject(myValue)
            //                                                  .ToArray();

            //    _IGraphStream.Write(_AppendingData, SeekOrigin.End);
            //    _IGraphStream.Close();

            }

        }

        #endregion

        #region Add(myKey, myValue)

        /// <summary>
        /// Adds mykey and myValue, but fails if the key already exists.
        /// </summary>
        /// <param name="myKey">the key to access the value</param>
        /// <param name="myValue">the value storing some information</param>
        /// <returns>0 if it failed, 1 for a success</returns>
        protected virtual UInt64 Add(TKey myKey, TValue myValue)
        {

            lock (this)
            {

                DictionaryValueHistory<TValue> _DictionaryHistoryList;
                var _Timestamp = TimestampNonce.Ticks;

                // The key already exists!
                if (_IDictionary.TryGetValue(myKey, out _DictionaryHistoryList))
                {
                    _DictionaryHistoryList.Add(_Timestamp, myValue);
                    _DictionaryHistoryList.TruncateHistory(_HistorySize);
                    AddOnFileSystem(myKey, myValue, _Timestamp);
                    isDirty = true;
                    return _DictionaryHistoryList.LatestTimestamp;
                }

                else
                {
                    _IDictionary.Add(myKey, new DictionaryValueHistory<TValue>(myValue));
                    AddOnFileSystem(myKey, myValue, _Timestamp);
                    isDirty = true;
                    return _Timestamp;
                }

            }

        }

        #endregion

        #region Add(myKeyValuePair)

        protected virtual UInt64 Add(KeyValuePair<TKey, TValue> myKeyValuePair)
        {
            return Add(myKeyValuePair.Key, myKeyValuePair.Value);
        }

        #endregion

        #region Add(myKeyValuePairs)

        protected virtual UInt64 Add(IEnumerable<KeyValuePair<TKey, TValue>> myKeyValuePairs)
        {
            return 0;
        }

        #endregion

        #region Add(myDictionary)

        protected virtual UInt64 Add(Dictionary<TKey, TValue> myDictionary)
        {
            return 0;
        }

        #endregion

        #endregion

        #region Set

        #region Set(myKey, myValue)

        /// <summary>
        /// Adds a value of type T to the index using a key of type TKey.
        /// In contrast to Add(myKey, myValue) this methods will replace an
        /// existing value without saving its prior history!
        /// </summary>
        /// <param name="myKey">the key to access the value</param>
        /// <param name="myValue">the value storing some information</param>
        protected virtual UInt64 Set(TKey myKey, TValue myValue)
        {
            lock (this)
            {

                DictionaryValueHistory<TValue> _DictionaryHistoryList;

                // The key already exists!
                if (_IDictionary.TryGetValue(myKey, out _DictionaryHistoryList))
                    _DictionaryHistoryList.Set(myValue);

                else
                    _IDictionary.Add(myKey, new DictionaryValueHistory<TValue>(myValue));

                isDirty = true;
                return 1;

            }
        }

        #endregion

        #region Set(myKeyValuePair)

        protected virtual UInt64 Set(KeyValuePair<TKey, TValue> myKeyValuePair)
        {
            return Set(myKeyValuePair.Key, myKeyValuePair.Value);
        }

        #endregion

        #region Set(myKeyValuePairs, myIndexSetStrategy)

        protected virtual UInt64 Set(IEnumerable<KeyValuePair<TKey, TValue>> myKeyValuePairs, IndexSetStrategy myIndexSetStrategy)
        {
            lock (this)
            {

                if (myIndexSetStrategy == IndexSetStrategy.REPLACE)
                    _IDictionary.Clear();

                foreach (var _KeyValuePair in myKeyValuePairs)
                    if (Set(_KeyValuePair) == 0)
                        return 0;

                return 1;

            }
        }

        #endregion

        #region Set(myDictionary, myIndexSetStrategy)

        protected virtual UInt64 Set(Dictionary<TKey, TValue> myDictionary, IndexSetStrategy myIndexSetStrategy)
        {
            lock (this)
            {

                if (myIndexSetStrategy == IndexSetStrategy.REPLACE)
                    _IDictionary.Clear();

                foreach (var _KeyValuePair in myDictionary)
                    if (Set(_KeyValuePair) == 0)
                        return 0;

                isDirty = true;
                return 1;

            }
        }

        #endregion

        #endregion

        #region Replace - will fail if actualValue != oldValue or the key does not exist

        #region Replace(myKey, myOldValue, myNewValue)

        /// <summary>
        /// Replaces myOldValue of myKey with myNewValue.
        /// Will fail if the actual value is not quals myOldValue due to concurrency or the key is not existant.
        /// </summary>
        /// <param name="myKey">the key to access the value</param>
        /// <param name="myOldValue">the old value</param>
        /// <param name="myNewValue">the new value</param>
        /// <returns>0 if it failed, 1 for a success</returns>
        protected virtual UInt64 Replace(TKey myKey, TValue myOldValue, TValue myNewValue)
        {
            lock (this)
            {

                DictionaryValueHistory<TValue> _DictionaryHistoryList;

                if (_IDictionary.TryGetValue(myKey, out _DictionaryHistoryList))
                {
                    if (myOldValue.Equals(_DictionaryHistoryList.LatestValue))
                    {
                        _DictionaryHistoryList.Add(myNewValue);
                        isDirty = true;
                        return _DictionaryHistoryList.LatestTimestamp;
                    }
                }

                return 0;

            }
        }

        #endregion

        #endregion

        #region Contains

        #region ContainsKey(myKey)

        protected virtual Trinary ContainsKey(TKey myKey)
        {
            return ContainsKey(myKey, 0);
        }

        #endregion

        #region ContainsValue(myValue)

        protected virtual Trinary ContainsValue(TValue myValue)
        {
            return ContainsValue(myValue, 0);
        }

        #endregion

        #region Contains(myKey, myValue)

        protected virtual Trinary Contains(TKey myKey, TValue myValue)
        {
            return Contains(myKey, myValue, 0);
        }

        #endregion

        #region Contains(myFunc)

        protected virtual Trinary Contains(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc)
        {
            return Contains(myFunc, 0);
        }

        #endregion

        #endregion

        #region Get/Keys/Values/Enumerator

        #region this[myKey]

        protected virtual TValue this[TKey myKey]
        {

            get
            {

                DictionaryValueHistory<TValue> _DictionaryHistoryList;

                var _Success = _IDictionary.TryGetValue(myKey, out _DictionaryHistoryList);

                if (_Success && _DictionaryHistoryList != null && !_DictionaryHistoryList.isDeleted)
                    return _DictionaryHistoryList.LatestValue;

                return default(TValue);

            }

            set
            {
                Set(myKey, value);
            }

        }

        #endregion

        #region TryGetValue(myKey, out myValue)

        protected virtual Boolean TryGetValue(TKey myKey, out TValue myValue)
        {
            lock (this)
            {

                DictionaryValueHistory<TValue> _DictionaryHistoryList;

                var _Success = _IDictionary.TryGetValue(myKey, out _DictionaryHistoryList);

                if (_Success && _DictionaryHistoryList != null && !_DictionaryHistoryList.isDeleted)
                    myValue = _DictionaryHistoryList.LatestValue;

                else
                    myValue = default(TValue);

                return _Success;

            }
        }

        #endregion


        #region Keys()

        protected virtual IEnumerable<TKey> Keys()
        {
            return Keys(0);
        }

        #endregion

        #region Keys(myFunc)

        protected virtual IEnumerable<TKey> Keys(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc)
        {
            return Keys(myFunc, 0);
        }

        #endregion

        #region KeyCount()

        protected virtual UInt64 KeyCount()
        {
            return KeyCount(0);
        }

        #endregion

        #region KeyCount(myFunc)

        protected virtual UInt64 KeyCount(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc)
        {
            return KeyCount(myFunc, 0);
        }

        #endregion


        #region Values()

        protected virtual IEnumerable<TValue> Values()
        {
            return Values(0);
        }

        #endregion

        #region Values(myFunc)

        protected virtual IEnumerable<TValue> Values(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc)
        {
            return Values(myFunc, 0);
        }

        #endregion

        #region ValueCount()

        protected virtual UInt64 ValueCount()
        {
            return ValueCount(0);
        }

        #endregion

        #region ValueCount(myFunc)

        protected virtual UInt64 ValueCount(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc)
        {
            return ValueCount(myFunc, 0);
        }

        #endregion


        #region GetIDictionary()

        protected virtual IDictionary<TKey, TValue> GetIDictionary()
        {
            return GetIDictionary(0);
        }

        #endregion

        #region GetIDictionary(myKeys)

        protected virtual IDictionary<TKey, TValue> GetIDictionary(params TKey[] myKeys)
        {
            lock (this)
            {

                DictionaryValueHistory<TValue> _DictionaryHistoryList;

                var _newIDictionary = new Dictionary<TKey, TValue>();

                foreach (var _Key in myKeys)
                {

                    if (_IDictionary.TryGetValue(_Key, out _DictionaryHistoryList) &&
                        _DictionaryHistoryList != null &&
                        !_DictionaryHistoryList.isDeleted)

                        _newIDictionary.Add(_Key, _DictionaryHistoryList.LatestValue);

                }

                return _newIDictionary;

            }
        }

        #endregion

        #region GetIDictionary(myFunc)

        protected virtual IDictionary<TKey, TValue> GetIDictionary(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc)
        {
            return GetIDictionary(myFunc, 0);
        }

        #endregion


        #region GetEnumerator()

        protected virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {

            var _Enumerator = _IDictionary.GetEnumerator();

            while (_Enumerator.MoveNext())
            {
                if (!_Enumerator.Current.Value.isDeleted)
                    yield return new KeyValuePair<TKey, TValue>(_Enumerator.Current.Key, _Enumerator.Current.Value.LatestValue);
            }

        }

        //protected override System.Collections.IEnumerator GetEnumerator()
        //{
        //    return GetEnumerator(0);
        //}

        #endregion

        #region GetEnumerator(myFunc)

        protected virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc)
        {

            KeyValuePair<TKey, TValue> _KeyValuePair;

            var _Enumerator = _IDictionary.GetEnumerator();

            while (_Enumerator.MoveNext())
            {
                // Ignore deleted values!
                if (!_Enumerator.Current.Value.isDeleted)
                {

                    _KeyValuePair = new KeyValuePair<TKey, TValue>(_Enumerator.Current.Key, _Enumerator.Current.Value.LatestValue);

                    if (myFunc(_KeyValuePair))
                        yield return _KeyValuePair;

                }
            }

        }

        #endregion

        #endregion

        #region Remove/Clear

        #region (private) RemoveFromFileSystem(myKey, myTimestamp)

        private void RemoveFromFileSystem(TKey myKey, UInt64 myTimestamp)
        {

            if (_IGraphFSReference != null && _IGraphFSReference.IsAlive && _IGraphFSReference.Value.IsPersistent)
            {

                var _IGraphFSStream = this._IGraphFSReference.Value.OpenStream(new SessionToken(new FSSessionInfo("AVersionedDictionaryObject")), _ObjectLocation, _ObjectStream, _ObjectEdition, null, 0);

                var _AppendingData = new SerializationWriter().WriteObject(myKey)
                                                              .WriteObject(myTimestamp)
                                                              .WriteObject(null)
                                                              .ToArray();

                _IGraphFSStream.Value.Write(_AppendingData, SeekOrigin.End);
                _IGraphFSStream.Value.Close();

            }

            else if (_IGraphFSSessionReference != null && _IGraphFSSessionReference.IsAlive && _IGraphFSSessionReference.Value.IsPersistent)
            {
                //    var _IGraphStream = this._FSSessionReference.OpenStream(_ObjectLocation, _ObjectStream, _ObjectEdition, null, 0);

                //    var _AppendingData = new SerializationWriter().WriteObject(myKey)
                //                                                  .WriteObject(myTimestamp)
                //                                                  .WriteObject(null)
                //                                                  .ToArray();

                //    _IGraphStream.Write(_AppendingData, SeekOrigin.End);
                //    _IGraphStream.Close();

            }

        }

        #endregion

        #region Remove(myKey)

        protected virtual Boolean Remove(TKey myKey)
        {

            lock (this)
            {

                DictionaryValueHistory<TValue> _DictionaryHistoryList;
                var _Timestamp = TimestampNonce.Ticks;

                // The key already exists!
                if (_IDictionary.TryGetValue(myKey, out _DictionaryHistoryList))
                {

                    if (!_DictionaryHistoryList.LatestValue.Equals(default(TValue)))
                    {

                        _DictionaryHistoryList.Delete();
                        _DictionaryHistoryList.TruncateHistory(_HistorySize);

                        RemoveFromFileSystem(myKey, _Timestamp);
                        isDirty = true;

                        return true;

                    }

                }

                return false;

            }

        }

        #endregion

        #region Remove(myKey, myValue)

        protected virtual Boolean Remove(TKey myKey, TValue myValue)
        {
            lock (this)
            {

                DictionaryValueHistory<TValue> _DictionaryHistoryList;

                var Success = _IDictionary.TryGetValue(myKey, out _DictionaryHistoryList);

                if (Success && myValue.Equals(_DictionaryHistoryList.LatestValue))
                {
                    Remove(myKey);
                    return true;
                }

                return false;

            }
        }

        #endregion

        #region Remove(myFunc)

        protected virtual Boolean Remove(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc)
        {
            lock (this)
            {

                var _Success = true;
                var _Enumerator = _IDictionary.GetEnumerator();

                while (_Enumerator.MoveNext())
                {
                    if (myFunc(new KeyValuePair<TKey, TValue>(_Enumerator.Current.Key, _Enumerator.Current.Value.LatestValue)))
                        _Success &= _IDictionary.Remove(_Enumerator.Current.Key);
                }

                return _Success;

            }
        }

        #endregion

        #region Clear()

        protected virtual void Clear()
        {
            _IDictionary.Clear();
        }

        #endregion

        #endregion

        #endregion


        #region Members of AVersionedDictionaryObject

        #region Replace - will fail if the key does not exist or the given timestamp != actual timestamp

        /// <summary>
        /// Replaces the value indexed by myKey with myNewValue as long as the given timestamp matches the actual timestamp.
        /// Will fail if the key is not existent or the actual timestamp is not equals myTimestamp due to concurrency conflicts.
        /// </summary>
        /// <param name="myKey">the key to access the value</param>
        /// <param name="myValue">the value storing some information</param>
        /// <returns>0 if it failed or the timestamp of the replace operation</returns>
        protected virtual UInt64 ReplaceByTimestamp(TKey myKey, UInt64 myTimestamp, TValue myNewValue)
        {
            lock (this)
            {

                DictionaryValueHistory<TValue> _DictionaryHistoryList;

                if (_IDictionary.TryGetValue(myKey, out _DictionaryHistoryList))
                {
                    if (myTimestamp.Equals(_DictionaryHistoryList.LatestTimestamp))
                    {
                        _DictionaryHistoryList.Add(myNewValue);
                        isDirty = true;
                        return _DictionaryHistoryList.LatestTimestamp;
                    }
                }

                return 0;

            }
        }

        #endregion

        #region Contains

        #region ContainsKey(myKey, myVersion)

        protected virtual Trinary ContainsKey(TKey myKey, Int64 myVersion)
        {
            lock (this)
            {

                DictionaryValueHistory<TValue> _DictionaryHistoryList;

                var _Success = _IDictionary.TryGetValue(myKey, out _DictionaryHistoryList);

                if (_Success)
                {

                    if (_DictionaryHistoryList.isDeleted)
                        return Trinary.DELETED;

                    return Trinary.TRUE;

                }

                return Trinary.FALSE;

            }
        }

        #endregion

        #region ContainsValue(myValue, myVersion)

        protected virtual Trinary ContainsValue(TValue myValue, Int64 myVersion)
        {
            lock (this)
            {

                foreach (var _DictionaryHistoryList in _IDictionary.Values)
                    if (_DictionaryHistoryList[myVersion].Equals(myValue))
                        return Trinary.TRUE;

                return Trinary.FALSE;

            }
        }

        #endregion

        #region Contains(myKey, myValue, myVersion)

        protected virtual Trinary Contains(TKey myKey, TValue myValue, Int64 myVersion)
        {
            lock (this)
            {

                DictionaryValueHistory<TValue> _DictionaryHistoryList;

                var _Success = _IDictionary.TryGetValue(myKey, out _DictionaryHistoryList);

                if (_Success)
                {
                    if (_DictionaryHistoryList.isDeleted)
                        return Trinary.DELETED;

                    if (myValue.Equals(_DictionaryHistoryList.LatestValue))
                        return Trinary.TRUE;
                }

                return Trinary.FALSE;

            }
        }

        #endregion

        #region Contains(myFunc, myVersion)

        protected virtual Trinary Contains(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc, Int64 myVersion)
        {
            lock (this)
            {

                var _Enumerator = _IDictionary.GetEnumerator();

                while (_Enumerator.MoveNext())
                {
                    if (myFunc(new KeyValuePair<TKey, TValue>(_Enumerator.Current.Key, _Enumerator.Current.Value[myVersion])))
                        return Trinary.TRUE;
                }

                return Trinary.FALSE;

            }
        }

        #endregion

        #endregion

        #region Get/Keys/Values/Enumerator

        #region this[myKey, myVersion]  // Int64

        protected virtual TValue this[TKey myKey, Int64 myVersion]
        {

            get
            {

                DictionaryValueHistory<TValue> _DictionaryHistoryList;

                var _Success = _IDictionary.TryGetValue(myKey, out _DictionaryHistoryList);

                // Do _not_ check if the value is deleted!
                if (_Success && _DictionaryHistoryList != null)
                    return _DictionaryHistoryList[myVersion];

                return default(TValue);

            }

        }

        #endregion

        #region this[myKey, myVersion]  // UInt64

        protected virtual TValue this[TKey myKey, UInt64 myVersion]
        {

            get
            {

                DictionaryValueHistory<TValue> _DictionaryHistoryList;

                var _Success = _IDictionary.TryGetValue(myKey, out _DictionaryHistoryList);

                // Do _not_ check if the value is deleted!
                if (_Success && _DictionaryHistoryList != null)
                    return _DictionaryHistoryList[myVersion];

                return default(TValue);

            }

        }

        #endregion

        #region TryGetValue(myKey, out myValue, myVersion)

        protected virtual Boolean TryGetValue(TKey myKey, out TValue myValue, Int64 myVersion)
        {
            lock (this)
            {

                DictionaryValueHistory<TValue> _DictionaryHistoryList;

                var _Success = _IDictionary.TryGetValue(myKey, out _DictionaryHistoryList);

                // Do _not_ check if the value is deleted!
                if (_Success && _DictionaryHistoryList != null)
                    myValue = _DictionaryHistoryList[myVersion];

                else
                    myValue = default(TValue);

                return _Success;

            }
        }

        #endregion


        #region Keys(myVersion)

        protected virtual IEnumerable<TKey> Keys(Int64 myVersion)
        {
            lock (this)
            {
                return from _KeyValuePair in _IDictionary
                       where _KeyValuePair.Value.isDeleted == false
                       select _KeyValuePair.Key;
            }
        }

        #endregion

        #region Keys(myFunc, myVersion)

        protected virtual IEnumerable<TKey> Keys(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc, Int64 myVersion)
        {
            lock (this)
            {

                KeyValuePair<TKey, TValue> _KeyValuePair;
                var _Enumerator = _IDictionary.GetEnumerator();

                while (_Enumerator.MoveNext())
                {

                    _KeyValuePair = new KeyValuePair<TKey, TValue>(_Enumerator.Current.Key, _Enumerator.Current.Value[myVersion]);

                    if (myFunc(_KeyValuePair))
                        yield return _Enumerator.Current.Key;

                }

            }
        }

        #endregion

        #region KeyCount(myVersion)

        protected virtual UInt64 KeyCount(Int64 myVersion)
        {
            lock (this)
            {
                return (from _KeyValuePair in _IDictionary
                        where _KeyValuePair.Value.isDeleted == false
                        select true).ULongCount();
            }
        }

        #endregion

        #region KeyCount(myFunc, myVersion)

        protected virtual UInt64 KeyCount(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc, Int64 myVersion)
        {
            lock (this)
            {

                var _LongCount  = 0UL;
                var _Enumerator = _IDictionary.GetEnumerator();

                while (_Enumerator.MoveNext())
                {
                    if (myFunc(new KeyValuePair<TKey, TValue>(_Enumerator.Current.Key, _Enumerator.Current.Value[myVersion])))
                        _LongCount++;
                }

                return _LongCount;

            }
        }

        #endregion


        #region Values(myVersion)

        protected virtual IEnumerable<TValue> Values(Int64 myVersion)
        {
            lock (this)
            {
                return from _KeyValuePair in _IDictionary
                       where _KeyValuePair.Value.isDeleted == false
                       select _KeyValuePair.Value[myVersion];
            }
        }

        #endregion

        #region Values(myFunc, myVersion)

        protected virtual IEnumerable<TValue> Values(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc, Int64 myVersion)
        {
            lock (this)
            {

                KeyValuePair<TKey, TValue> _KeyValuePair;
                var _Enumerator = _IDictionary.GetEnumerator();

                while (_Enumerator.MoveNext())
                {

                    _KeyValuePair = new KeyValuePair<TKey, TValue>(_Enumerator.Current.Key, _Enumerator.Current.Value[myVersion]);

                    if (myFunc(_KeyValuePair))
                        yield return _Enumerator.Current.Value[myVersion];

                }

            }
        }

        #endregion

        #region ValueCount(myVersion)

        protected virtual UInt64 ValueCount(Int64 myVersion)
        {
            lock (this)
            {
                return (from _KeyValuePair in _IDictionary
                        where _KeyValuePair.Value.isDeleted == false
                        select true).ULongCount();
            }
        }

        #endregion

        #region ValueCount(myFunc, myVersion)

        protected virtual UInt64 ValueCount(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc, Int64 myVersion)
        {
            lock (this)
            {

                var _LongCount  = 0UL;
                var _Enumerator = _IDictionary.GetEnumerator();

                while (_Enumerator.MoveNext())
                {
                    if (myFunc(new KeyValuePair<TKey, TValue>(_Enumerator.Current.Key, _Enumerator.Current.Value[myVersion])))
                        _LongCount++;
                }

                return _LongCount;

            }
        }

        #endregion


        #region GetIDictionary(myVersion)

        protected virtual IDictionary<TKey, TValue> GetIDictionary(Int64 myVersion)
        {
            lock (this)
            {
                return (from _KeyValuePair in _IDictionary
                        where _KeyValuePair.Value.isDeleted == false
                        select _KeyValuePair).ToDictionary(key => key.Key, value => value.Value[myVersion]);
            }
        }

        #endregion

        #region GetIDictionary(myKeys)

        protected virtual IDictionary<TKey, TValue> GetIDictionary(Int64 myTimestamp, params TKey[] myKeys)
        {
            lock (this)
            {

                DictionaryValueHistory<TValue> _DictionaryHistoryList;

                var _newIDictionary = new Dictionary<TKey, TValue>();

                foreach (var _Key in myKeys)
                {

                    if (_IDictionary.TryGetValue(_Key, out _DictionaryHistoryList) &&
                        _DictionaryHistoryList != null &&
                        !_DictionaryHistoryList.isDeleted)

                        _newIDictionary.Add(_Key, _DictionaryHistoryList[myTimestamp]);

                }

                return _newIDictionary;

            }
        }

        #endregion

        #region GetIDictionary(myFunc, myVersion)

        protected virtual IDictionary<TKey, TValue> GetIDictionary(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc, Int64 myVersion)
        {
            lock (this)
            {

                KeyValuePair<TKey, TValue> _KeyValuePair;
                var _Dictionary = new Dictionary<TKey, TValue>();
                var _Enumerator = _IDictionary.GetEnumerator();

                while (_Enumerator.MoveNext())
                {

                    _KeyValuePair = new KeyValuePair<TKey, TValue>(_Enumerator.Current.Key, _Enumerator.Current.Value[myVersion]);

                    if (myFunc(_KeyValuePair))
                        _Dictionary.Add(_KeyValuePair.Key, _KeyValuePair.Value);

                }

                return _Dictionary;

            }
        }

        #endregion


        #region GetEnumerator(myVersion)

        protected virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator(Int64 myVersion)
        {

            return (from _KeyValuePair in _IDictionary
                    where _KeyValuePair.Value.isDeleted == false
                    select _KeyValuePair).ToDictionary(key => key.Key, value => value.Value[myVersion]).GetEnumerator();

        }

        #endregion

        #region GetEnumerator(myFunc, myVersion)

        protected virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc, Int64 myVersion)
        {

            KeyValuePair<TKey, TValue> _KeyValuePair;
            var _Enumerator = _IDictionary.GetEnumerator();

            while (_Enumerator.MoveNext())
            {

                _KeyValuePair = new KeyValuePair<TKey, TValue>(_Enumerator.Current.Key, _Enumerator.Current.Value[myVersion]);

                if (myFunc(_KeyValuePair))
                    yield return _KeyValuePair;

            }

        }

        #endregion

        #endregion

        #region Additional methods

        #region GetTimestampValuePair(myKey)

        protected TimestampValuePair<TValue> GetTimestampValuePair(TKey myKey)
        {
            lock (this)
            {

                DictionaryValueHistory<TValue> _DictionaryHistoryList;

                if (_IDictionary.TryGetValue(myKey, out _DictionaryHistoryList))
                    return new TimestampValuePair<TValue>(_DictionaryHistoryList.LatestTimestamp, _DictionaryHistoryList.LatestValue);

                return null;

            }
        }

        #endregion

        #region VersionCount(myKey)

        protected UInt64 VersionCount(TKey myKey)
        {
            lock (this)
            {

                DictionaryValueHistory<TValue> _DictionaryHistoryList;

                var _Success = _IDictionary.TryGetValue(myKey, out _DictionaryHistoryList);

                // Do _not_ check if the value is deleted!
                if (_Success && _DictionaryHistoryList != null)
                    return _DictionaryHistoryList.VersionCount;

                return default(UInt64);

            }
        }

        #endregion

        #region ClearHistory(myKey)

        /// <summary>
        /// Clears the history information of the given key
        /// </summary>
        protected void ClearHistory(TKey myKey)
        {
            lock (this)
            {

                DictionaryValueHistory<TValue> _DictionaryHistoryList;

                var _Success = _IDictionary.TryGetValue(myKey, out _DictionaryHistoryList);

                if (_Success && _DictionaryHistoryList != null)
                {
                    _DictionaryHistoryList.ClearHistory();
                    isDirty = true;
                }

            }
        }

        #endregion

        #region (private) TruncateHistory(myNewHistorySize)

        /// <summary>
        /// Reduces the number of values stored within the history of any value
        /// </summary>
        private void TruncateHistory(UInt64 myNewHistorySize)
        {
            lock (this)
            {
                foreach (var _KeyValuePair in _IDictionary)
                    _KeyValuePair.Value.TruncateHistory(myNewHistorySize);
            }
        }

        #endregion

        #endregion

        #endregion


    }

}
