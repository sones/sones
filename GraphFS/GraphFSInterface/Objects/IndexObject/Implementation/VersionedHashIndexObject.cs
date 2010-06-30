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


/* 
 * VersionedHashIndexObject
 * Achim Friedland, 2009 - 2010
 */

#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

using sones.Lib;

using sones.Lib.Cryptography.IntegrityCheck;
using sones.Lib.Cryptography.SymmetricEncryption;
using sones.Lib.DataStructures;
using sones.Lib.NewFastSerializer;
using sones.GraphFS.DataStructures;
using sones.Lib.DataStructures.Indices;
using sones.Lib.DataStructures.Timestamp;
using sones.Lib.Session;
using sones.GraphFS.Session;

#endregion

namespace sones.GraphFS.Objects
{

    public class VersionedHashIndexObject
    {
        public const String Name = "HASHTABLE";
    }

    /// <summary>
    /// An abstract implementation of a IndexObject to store a mapping TKey => Hashset&lt;TValue&gt; which may be embedded into other objects.
    /// </summary>
    /// <typeparam name="TKey">Must implement IComparable</typeparam>
    public class VersionedHashIndexObject<TKey, TValue> : AFSObject, IVersionedIndexObject<TKey, TValue>
        where TKey : IComparable
    {


        #region Data

        protected IDictionary<TKey, IndexValueHistoryList<TValue>> _IDictionary;

        #endregion

        #region Properties

        #region HistorySize

        protected UInt64 _HistorySize;

        public UInt64 HistorySize
        {
            get
            {
                return _HistorySize;
            }

            set
            {

                _HistorySize = value;

                lock (this)
                {
                    foreach (var _KeyValuePair in _IDictionary)
                        _KeyValuePair.Value.TruncateHistory(_HistorySize);
                }

            }

        }

        #endregion

        #endregion


        #region Constructors

        #region VersionedHashIndexObject()

        /// <summary>
        /// This will create an empty AIndexObject using a Dictionary&lt;TKey, HashSet&lt;TValue&gt;&gt; for the internal IDictionary&lt;TKey, HashSet&lt;TValue&gt;&gt; object.
        /// </summary>
        public VersionedHashIndexObject()
            : this (new Dictionary<TKey, TValue>())
        {
            HistorySize = 3;
        }

        #endregion

        #region VersionedHashIndexObject(myIDictionary)

        /// <summary>
        /// This will create an empty AIndexObject using the given IDictionary object for the internal IDictionary&lt;TKey, HashSet&lt;TValue&gt;&gt; object.
        /// </summary>
        public VersionedHashIndexObject(IDictionary<TKey, TValue> myIDictionary)
        {

            // Members of APandoraStructure
            _StructureVersion       = 1;

            // Members of APandoraObject
            _ObjectStream           = FSConstants.DEFAULT_INDEXSTREAM;

            // Construct new _IDictionary...
            if (myIDictionary == null)
                throw new ArgumentNullException("Type '" + myIDictionary.ToString() + "' must not be null!");

            var _IDictionaryType = myIDictionary.GetType().GetGenericTypeDefinition();
            if (_IDictionaryType == null)
                throw new ArgumentException("Type '" + myIDictionary.ToString() + "' is not a generic type with two type parameters!");

            var _IDictionaryGenericType = Activator.CreateInstance(_IDictionaryType.MakeGenericType(new Type[] { typeof(TKey), typeof(IndexValueHistoryList<TValue>) }));
            if (_IDictionaryGenericType == null)
                throw new ArgumentException("Type '" + myIDictionary + "' could not be instantiated as " + myIDictionary + "<" + typeof(TKey).ToString() + ", DictionaryValueHistory<" + typeof(TValue).ToString() + ">>!");

            _IDictionary = _IDictionaryGenericType as IDictionary<TKey, IndexValueHistoryList<TValue>>;
            if (_IDictionary == null)
                throw new ArgumentException("Type '" + _IDictionaryGenericType.ToString() + "' does not implement IDictionary<..., ...>!");

        }

        #endregion

        #region VersionedHashIndexObject(myObjectLocation, mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm)

        /// <summary>
        /// A constructor used for fast deserializing
        /// </summary>
        /// <param name="mySerializedData">A bunch of bytes[] containing the serialized IndexObject</param>
        public VersionedHashIndexObject(String myObjectLocation, Byte[] mySerializedData, IIntegrityCheck myIntegrityCheckAlgorithm, ISymmetricEncryption myEncryptionAlgorithm)
            : this()
        {

            if (mySerializedData == null || mySerializedData.Length == 0)
                throw new ArgumentNullException("mySerializedData must not be null or its length be zero!");

            Deserialize(mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm, false);

        }

        #endregion

        #endregion


        #region Members of APandoraStructure

        #region SerializeInnerObject(ref mySerializationWriter)

        public override void Serialize(ref SerializationWriter mySerializationWriter)
        {
            SerializeObject(ref mySerializationWriter, 0);
        }

        #endregion

        #region SerializeObject(ref mySerializationWriter, myNotificationHandling)

        public void SerializeObject(ref SerializationWriter mySerializationWriter, UInt64 myNotificationHandling)
        {

            try
            {

                #region NotificationHandling

                mySerializationWriter.WriteObject(myNotificationHandling);

                #endregion

                foreach (var _KeyValuePair in _IDictionary)
                    foreach (var _Value in _KeyValuePair.Value.Values)
                        mySerializationWriter.WriteObject((Byte)OP.ADD).
                                              WriteObject(_KeyValuePair.Key).
                                              WriteObject(_KeyValuePair.Value.LatestTimestamp).
                                              WriteObject(_Value);

            }

            catch (SerializationException e)
            {
                throw new SerializationException(e.Message);
            }

        }

        #endregion

        #region DeserializeInnerObject(ref mySerializationReader

        public override void Deserialize(ref SerializationReader mySerializationReader)
        {

            try
            {

                #region NotificationHandling

                UInt64 _NotificationHandling = (UInt64) mySerializationReader.ReadObject();

                #endregion

                IndexValueHistoryList<TValue> _IndexValueHistoryList;

                while (mySerializationReader.BytesRemaining>0)
                {

                    var _OP         = (OP) (Byte) mySerializationReader.ReadObject();
                    var _Key        = (TKey)   mySerializationReader.ReadObject();
                    var _Timestamp  = (UInt64) mySerializationReader.ReadObject();
                    var _Value      = (TValue) mySerializationReader.ReadObject();

                    // _OP and _Timestamp will never be null...
                    if (_Key != null)
                    {

                        // The key already exists!
                        if (_IDictionary.TryGetValue(_Key, out _IndexValueHistoryList))
                        {

                            switch (_OP)
                            {
                                case OP.ADD: _IndexValueHistoryList.Set(_Value, _Timestamp, IndexSetStrategy.MERGE); break;
                                case OP.REM: _IndexValueHistoryList.Remove(_Value, _Timestamp); break;
                            }

                            while (_IndexValueHistoryList.VersionCount > _HistorySize)
                                _IndexValueHistoryList.RemoveLatestFromHistory();

                        }

                        else
                        {
                            if (_OP == OP.ADD)
                                _IDictionary.Add(_Key, new IndexValueHistoryList<TValue>(_Value));
                        }
                    
                    }

                }

            }

            catch (Exception e)
            {
                throw new Exception("IndexObject_HashTable<" + typeof(TKey).ToString() + ", " + typeof(TValue).ToString() + "> could not be deserialized!\n\n" + e);
            }

        }

        #endregion

        #endregion

        public override AFSObject Clone()
        {
            throw new NotImplementedException();
        }


        #region Members of IIndexObject

        #region Add

        #region Add(myKey, myValue)

        public void Add(TKey myKey, TValue myValue)
        {
            Set(myKey, myValue, IndexSetStrategy.MERGE);
        }

        #endregion

        #region Add(myKey, myValues)

        public void Add(TKey myKey, IEnumerable<TValue> myValues)
        {
            Set(myKey, myValues, IndexSetStrategy.MERGE);
        }

        #endregion

        #region Add(myKeyValuePair)

        public void Add(KeyValuePair<TKey, TValue> myKeyValuePair)
        {
            Add(myKeyValuePair.Key, myKeyValuePair.Value);
        }

        #endregion

        #region Add(myKeyValuesPair)

        public void Add(KeyValuePair<TKey, IEnumerable<TValue>> myKeyValuesPair)
        {
            Add(myKeyValuesPair.Key, myKeyValuesPair.Value);
        }

        #endregion

        #region Add(myDictionary)

        public void Add(Dictionary<TKey, TValue> myDictionary)
        {
            Set(myDictionary, IndexSetStrategy.MERGE);
        }

        #endregion

        #region Add(myMultiValueDictionary)

        public void Add(Dictionary<TKey, IEnumerable<TValue>> myMultiValueDictionary)
        {
            Set(myMultiValueDictionary, IndexSetStrategy.MERGE);
        }

        #endregion

        #endregion

        #region Set

        #region Set(myKey, myValue, myIndexSetStrategy)

        #region (private) AddOnFileSystem(myKey, myValue, myTimestamp)

        

        private void SetOnFileSystem(TKey myKey, TValue myValue, UInt64 myTimestamp, OP myOP)
        {

            if (_IGraphFSReference.CheckPersistency())
            {

                var _IFSStream = this._IGraphFSReference.Value.OpenStream(new SessionToken(new FSSessionInfo("AVersionedDictionaryObject")), _ObjectLocation, _ObjectStream, _ObjectEdition, null, 0);

                var _AppendingData = new SerializationWriter().WriteObject((Byte)myOP)
                                                              .WriteObject(myKey)
                                                              .WriteObject(myTimestamp)
                                                              .WriteObject(myValue)
                                                              .ToArray();

                _IFSStream.Value.Write(_AppendingData, SeekOrigin.End);
                _IFSStream.Value.Close();

            }

            else if (_IGraphFSSessionReference.CheckPersistency())
            {

                var _IFSStream = this._IGraphFSSessionReference.Value.OpenStream(_ObjectLocation, _ObjectStream, _ObjectEdition, null, 0);

                var _AppendingData = new SerializationWriter().WriteObject((Byte)myOP)
                                                              .WriteObject(myKey)
                                                              .WriteObject(myTimestamp)
                                                              .WriteObject(myValue)
                                                              .ToArray();

                _IFSStream.Value.Write(_AppendingData, SeekOrigin.End);
                _IFSStream.Value.Close();

            }

        }

        #endregion

        public void Set(TKey myKey, TValue myValue, IndexSetStrategy myIndexSetStrategy)
        {

            lock (this)
            {

                IndexValueHistoryList<TValue> _IndexValueHistoryList;
                var _Timestamp = TimestampNonce.Ticks;

                // The key already exists!
                if (_IDictionary.TryGetValue(myKey, out _IndexValueHistoryList))
                {

                    _IndexValueHistoryList.Set(myValue, _Timestamp, myIndexSetStrategy);

                    while (_IndexValueHistoryList.VersionCount > _HistorySize)
                        _IndexValueHistoryList.RemoveLatestFromHistory();

                    SetOnFileSystem(myKey, myValue, _Timestamp, OP.ADD);

                }

                else
                {
                    _IDictionary.Add(myKey, new IndexValueHistoryList<TValue>(myValue));
                    SetOnFileSystem(myKey, myValue, _Timestamp, OP.ADD);
                }

                isDirty = true;

            }

        }

        #endregion

        #region Set(myKey, myValues, myIndexSetStrategy)

        public void Set(TKey myKey, IEnumerable<TValue> myValues, IndexSetStrategy myIndexSetStrategy)
        {

            lock (this)
            {

                IndexValueHistoryList<TValue> _IndexValueHistoryList;

                // The key already exists!
                if (_IDictionary.TryGetValue(myKey, out _IndexValueHistoryList))
                {

                    _IndexValueHistoryList.Set(myValues, myIndexSetStrategy);

                    while (_IndexValueHistoryList.VersionCount > _HistorySize)
                        _IndexValueHistoryList.RemoveLatestFromHistory();

                }

                else
                    _IDictionary.Add(myKey, new IndexValueHistoryList<TValue>(myValues));

                isDirty = true;

            }

        }

        #endregion

        #region Set(myKeyValuePair, myIndexSetStrategy)

        public void Set(KeyValuePair<TKey, TValue> myKeyValuePair, IndexSetStrategy myIndexSetStrategy)
        {
            Set(myKeyValuePair.Key, myKeyValuePair.Value, myIndexSetStrategy);
        }

        #endregion

        #region Set(myKeyValuesPair, myIndexSetStrategy)

        public void Set(KeyValuePair<TKey, IEnumerable<TValue>> myKeyValuesPair, IndexSetStrategy myIndexSetStrategy)
        {
            Set(myKeyValuesPair.Key, myKeyValuesPair.Value, myIndexSetStrategy);
        }

        #endregion

        #region Set(myKeyValuePairs, myIndexSetStrategy)

        public void Set(IEnumerable<KeyValuePair<TKey, TValue>> myKeyValuePairs, IndexSetStrategy myIndexSetStrategy)
        {

            lock (this)
            {

                switch (myIndexSetStrategy)
                {

                    case IndexSetStrategy.MERGE:
                        //hack: just a workaround as this will create too much versions!
                        foreach (var _KeyValuePair in myKeyValuePairs)
                            Set(_KeyValuePair, myIndexSetStrategy);

                        break;

                    case IndexSetStrategy.REPLACE:

                        _IDictionary.Clear();

                        foreach (var _KeyValuePair in myKeyValuePairs)
                            Set(_KeyValuePair, myIndexSetStrategy);

                        break;

                }

                isDirty = true;

            }

        }

        #endregion

        #region Set(myDictionary, myIndexSetStrategy)

        public void Set(Dictionary<TKey, TValue> myDictionary, IndexSetStrategy myIndexSetStrategy)
        {

            lock (this)
            {

                switch (myIndexSetStrategy)
                {

                    case IndexSetStrategy.MERGE:
                        //hack: just a workaround as this will create too much versions!
                        foreach (var _KeyValuePair in myDictionary)
                            Set(_KeyValuePair, myIndexSetStrategy);

                        break;

                    case IndexSetStrategy.REPLACE:

                        _IDictionary.Clear();

                        foreach (var _KeyValuePair in myDictionary)
                            Set(_KeyValuePair, myIndexSetStrategy);

                        break;

                }

                isDirty = true;

            }

        }

        #endregion

        #region Set(myMultiValueDictionary, myIndexSetStrategy)

        public void Set(Dictionary<TKey, IEnumerable<TValue>> myMultiValueDictionary, IndexSetStrategy myIndexSetStrategy)
        {

            lock (this)
            {

                switch (myIndexSetStrategy)
                {

                    case IndexSetStrategy.MERGE:
                        //hack: just a workaround as this will create too much versions!
                        foreach (var _KeyValuePair in myMultiValueDictionary)
                            Set(_KeyValuePair, myIndexSetStrategy);

                        break;

                    case IndexSetStrategy.REPLACE:

                        _IDictionary.Clear();

                        foreach (var _KeyValuePair in myMultiValueDictionary)
                            Set(_KeyValuePair, myIndexSetStrategy);

                        break;

                }

                isDirty = true;

            }

        }

        #endregion

        #endregion

        #region Contains

        #region ContainsKey(myKey)

        public Trinary ContainsKey(TKey myKey)
        {
            return ContainsKey(myKey, 0);
        }

        #endregion

        #region ContainsValue(myValue)

        public Trinary ContainsValue(TValue myValue)
        {
            return ContainsValue(myValue, 0);
        }

        #endregion

        #region Contains(myKey, myValue)

        public Trinary Contains(TKey myKey, TValue myValue)
        {
            return Contains(myKey, myValue, 0);
        }

        #endregion

        #region Contains(myFunc)

        public Trinary Contains(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
        {
            lock (this)
            {

                var _Enumerator = _IDictionary.GetEnumerator();

                while (_Enumerator.MoveNext())
                {
                    if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(_Enumerator.Current.Key, _Enumerator.Current.Value.Values)))
                        return Trinary.TRUE;
                }

                return Trinary.FALSE;

            }
        }

        #endregion

        #endregion

        #region Get/Keys/Values/Enumerator

        #region this[myKey]

        public HashSet<TValue> this[TKey myKey]
        {

            get
            {
                lock (this)
                {

                    IndexValueHistoryList<TValue> _IndexValueHistoryList;

                    var _Success = _IDictionary.TryGetValue(myKey, out _IndexValueHistoryList);

                    if (_Success && _IndexValueHistoryList != null && !_IndexValueHistoryList.isLatestDeleted)
                        return _IndexValueHistoryList.Values;

                    return new HashSet<TValue>();

                }
            }

            set
            {
                Set(myKey, value, IndexSetStrategy.REPLACE);
            }

        }

        #endregion

        #region TryGetValue(myKey, out myValue)

        public Boolean TryGetValue(TKey myKey, out HashSet<TValue> myValue)
        {
            lock (this)
            {

                IndexValueHistoryList<TValue> _IndexValueHistoryList;

                var _Success = _IDictionary.TryGetValue(myKey, out _IndexValueHistoryList);

                if (_Success && _IndexValueHistoryList != null && !_IndexValueHistoryList.isLatestDeleted)
                    myValue = _IndexValueHistoryList.Values;

                else
                    myValue = new HashSet<TValue>();

                return _Success;

            }
        }

        #endregion


        #region Keys()

        public IEnumerable<TKey> Keys()
        {
            return Keys(0);
        }

        #endregion

        #region Keys(myFunc)

        public IEnumerable<TKey> Keys(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
        {
            return Keys(myFunc, 0);
        }

        #endregion

        #region KeyCount()

        public UInt64 KeyCount()
        {
            return KeyCount(0);
        }

        #endregion

        #region KeyCount(myFunc)

        public UInt64 KeyCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
        {
            return KeyCount(myFunc, 0);
        }

        #endregion


        #region Values()

        public IEnumerable<HashSet<TValue>> Values()
        {
            return Values(0);
        }

        #endregion

        #region Values(myFunc)

        public IEnumerable<HashSet<TValue>> Values(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
        {
            return Values(myFunc, 0);
        }

        #endregion

        #region ValueCount()

        public UInt64 ValueCount()
        {
            return ValueCount(0);
        }

        #endregion

        #region ValueCount(myFunc)

        public UInt64 ValueCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
        {
            return ValueCount(myFunc, 0);
        }

        #endregion


        #region GetIDictionary()

        public IDictionary<TKey, HashSet<TValue>> GetIDictionary()
        {
            return GetIDictionary(0);
        }

        #endregion

        #region GetIDictionary(myFunc)

        public IDictionary<TKey, HashSet<TValue>> GetIDictionary(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
        {
            return GetIDictionary(myFunc, 0);
        }

        #endregion


        #region GetEnumerator()

        public IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator()
        {
            return GetEnumerator(0);
        }

        #endregion

        #region GetEnumerator(myFunc)

        public IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
        {
            return GetEnumerator(myFunc, 0);
        }

        #endregion

        #endregion

        #region Remove/Clear

        #region Remove(myKey)

        public Boolean Remove(TKey myKey)
        {
            lock (this)
            {

                IndexValueHistoryList<TValue> _IndexValueHistoryList;
                var _Timestamp = TimestampNonce.Ticks;

                Boolean _Success = _IDictionary.TryGetValue(myKey, out _IndexValueHistoryList);

                if (_Success && _IDictionary != null)
                {

                    foreach (var _Value in _IndexValueHistoryList.Values)
                        SetOnFileSystem(myKey, _Value, _Timestamp, OP.REM);

                    _IndexValueHistoryList.Clear(_Timestamp);

                    while (_IndexValueHistoryList.VersionCount > _HistorySize)
                        _IndexValueHistoryList.RemoveLatestFromHistory();

                }

                return _Success;

            }
        }

        #endregion

        #region Remove(myKey, myValue)

        public Boolean Remove(TKey myKey, TValue myValue)
        {
            lock (this)
            {

                IndexValueHistoryList<TValue> _IndexValueHistoryList;
                var _Timestamp = TimestampNonce.Ticks;

                var _Success = _IDictionary.TryGetValue(myKey, out _IndexValueHistoryList);

                if (_Success && _IndexValueHistoryList != null)
                {

                    _IndexValueHistoryList.Remove(myValue);

                    while (_IndexValueHistoryList.VersionCount > _HistorySize)
                        _IndexValueHistoryList.RemoveLatestFromHistory();

                    SetOnFileSystem(myKey, myValue, _Timestamp, OP.REM);

                }

                return _Success;

            }
        }

        #endregion

        #region Remove(myFunc)

        public Boolean Remove(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
        {
            lock (this)
            {

                var _Success = true;
                var _Enumerator = _IDictionary.GetEnumerator();

                while (_Enumerator.MoveNext())
                {
                    if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(_Enumerator.Current.Key, _Enumerator.Current.Value.Values)))
                        _Success &= _IDictionary.Remove(_Enumerator.Current.Key);
                }

                return _Success;

            }
        }

        #endregion

        #region Clear()

        public void Clear()
        {
            _IDictionary.Clear();
        }

        #endregion

        #endregion

        #region Range Methods

        public IEnumerable<TValue> GreaterThan(TKey myKey, bool myOrEqual = true)
        {
            return GreaterThan(myKey, null, myOrEqual);
        }

        public IEnumerable<TValue> GreaterThan(TKey myKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, bool myOrEqual = true)
        {
            return GreaterThan(myKey, myFunc, 0, myOrEqual);
        }

        public IEnumerable<TValue> LowerThan(TKey myKey, bool myOrEqual = true)
        {
            return LowerThan(myKey, null, myOrEqual);
        }

        public IEnumerable<TValue> LowerThan(TKey myKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, bool myOrEqual = true)
        {
            return LowerThan(myKey, myFunc, 0, myOrEqual);
        }

        public IEnumerable<TValue> InRange(TKey myFromKey, TKey myToKey, bool myOrEqualFromKey = true, bool myOrEqualToKey = true)
        {
            return InRange(myFromKey, myToKey, null, 0, myOrEqualFromKey, myOrEqualToKey);
        }

        public IEnumerable<TValue> InRange(TKey myFromKey, TKey myToKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, bool myOrEqualFromKey = true, bool myOrEqualToKey = true)
        {
            return InRange(myFromKey, myToKey, myFunc, 0, myOrEqualFromKey, myOrEqualToKey);
        }


        #endregion

        #endregion

        #region IVersionedIndexObject<TKey,TValue> Members

        #region Contains

        #region ContainsKey(myKey, myVersion)

        public virtual Trinary ContainsKey(TKey myKey, Int64 myVersion)
        {
            lock (this)
            {

                if (!_IDictionary.ContainsKey(myKey))
                    return Trinary.FALSE;

                if (_IDictionary[myKey].isLatestDeleted)
                    return Trinary.DELETED;

                return Trinary.TRUE;

            }
        }

        #endregion

        #region ContainsValue(myValue, myVersion)

        public virtual Trinary ContainsValue(TValue myValue, Int64 myVersion)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Contains(myKey, myValue, myVersion)

        public virtual Trinary Contains(TKey myKey, TValue myValue, Int64 myVersion)
        {
            lock (this)
            {

                IndexValueHistoryList<TValue> _IndexValueHistoryList;

                var _Success = _IDictionary.TryGetValue(myKey, out _IndexValueHistoryList);

                if (_Success)
                    if (_IndexValueHistoryList.Values.Contains(myValue))
                        return Trinary.TRUE;

                return Trinary.FALSE;

            }
        }

        #endregion

        #region Contains(myFunc)

        public virtual Trinary Contains(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion)
        {
            lock (this)
            {

                var _Enumerator = _IDictionary.GetEnumerator();

                while (_Enumerator.MoveNext())
                {
                    if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(_Enumerator.Current.Key, _Enumerator.Current.Value[myVersion])))
                        return Trinary.TRUE;
                }

                return Trinary.FALSE;

            }
        }

        #endregion

        #endregion

        #region Get/Keys/Values/Enumerator

        #region this[myKey, myVersion]  // Int64

        public virtual HashSet<TValue> this[TKey myKey, Int64 myVersion]
        {

            get
            {
                lock (this)
                {

                    IndexValueHistoryList<TValue> _IndexValueHistoryList;

                    var _Success = _IDictionary.TryGetValue(myKey, out _IndexValueHistoryList);

                    // Do _not_ check if the value is deleted!
                    if (_Success && _IndexValueHistoryList != null)
                        return _IndexValueHistoryList[myVersion];

                    return new HashSet<TValue>();

                }
            }

        }

        #endregion

        #region this[myKey, myVersion]  // UInt64

        public virtual HashSet<TValue> this[TKey myKey, UInt64 myVersion]
        {

            get
            {
                lock (this)
                {

                    IndexValueHistoryList<TValue> _IndexValueHistoryList;

                    var _Success = _IDictionary.TryGetValue(myKey, out _IndexValueHistoryList);

                    // Do _not_ check if the value is deleted!
                    if (_Success && _IndexValueHistoryList != null)
                        return _IndexValueHistoryList[myVersion];

                    return new HashSet<TValue>();

                }
            }

        }

        #endregion

        #region TryGetValue(myKey, out myValue, myVersion)

        public virtual Boolean TryGetValue(TKey myKey, out HashSet<TValue> myValue, Int64 myVersion)
        {
            lock (this)
            {

                IndexValueHistoryList<TValue> _IndexValueHistoryList;

                var _Success = _IDictionary.TryGetValue(myKey, out _IndexValueHistoryList);

                // Do _not_ check if the value is deleted!
                if (_Success && _IndexValueHistoryList != null)
                    myValue = _IndexValueHistoryList.Values;

                else
                    myValue = new HashSet<TValue>();

                return _Success;

            }
        }

        #endregion


        #region Keys(myVersion)

        public virtual IEnumerable<TKey> Keys(Int64 myVersion)
        {
            lock (this)
            {
                return (from _KeyValuePair in _IDictionary
                        where _KeyValuePair.Value.isLatestDeleted == false
                        select _KeyValuePair.Key).ToList<TKey>();
            }
        }

        #endregion

        #region Keys(myFunc, myVersion)

        public virtual IEnumerable<TKey> Keys(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion)
        {
            lock (this)
            {

                var _Enumerator = _IDictionary.GetEnumerator();

                while (_Enumerator.MoveNext())
                {
                    if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(_Enumerator.Current.Key, _Enumerator.Current.Value[myVersion])))
                        yield return _Enumerator.Current.Key;
                }

            }
        }

        #endregion

        #region KeyCount(myVersion)

        public virtual UInt64 KeyCount(Int64 myVersion)
        {
            lock (this)
            {
                return (from _KeyValuePair in _IDictionary
                        where _KeyValuePair.Value.isLatestDeleted == false
                        select true).ULongCount();

            }
        }

        #endregion

        #region KeyCount(myFunc, myVersion)

        public virtual UInt64 KeyCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion)
        {
            lock (this)
            {

                var _LongCount = 0UL;
                var _Enumerator = _IDictionary.GetEnumerator();

                while (_Enumerator.MoveNext())
                {
                    if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(_Enumerator.Current.Key, _Enumerator.Current.Value[myVersion])))
                        _LongCount++;
                }

                return _LongCount;

            }
        }

        #endregion


        #region Values(myVersion)

        public virtual IEnumerable<HashSet<TValue>> Values(Int64 myVersion)
        {
            lock (this)
            {
                return from _KeyValuePair in _IDictionary
                       where _KeyValuePair.Value.isLatestDeleted == false
                       select _KeyValuePair.Value.Values;
            }
        }

        #endregion

        #region Values(myFunc, myVersion)

        public virtual IEnumerable<HashSet<TValue>> Values(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion)
        {
            lock (this)
            {

                var _Enumerator = _IDictionary.GetEnumerator();

                while (_Enumerator.MoveNext())
                {
                    if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(_Enumerator.Current.Key, _Enumerator.Current.Value[myVersion])))
                        yield return _Enumerator.Current.Value[myVersion];
                }

            }
        }

        #endregion

        #region ValueCount(myVersion)

        public virtual UInt64 ValueCount(Int64 myVersion)
        {
            lock (this)
            {
                return (from _KeyValuePair in _IDictionary
                        where _KeyValuePair.Value.isLatestDeleted == false
                        select _KeyValuePair.Value.Values.ULongCount()).Sum();
            }
        }

        #endregion

        #region ValueCount(myFunc, myVersion)

        public virtual UInt64 ValueCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion)
        {
            lock (this)
            {

                var _LongCount = 0UL;
                var _Enumerator = _IDictionary.GetEnumerator();

                while (_Enumerator.MoveNext())
                {
                    if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(_Enumerator.Current.Key, _Enumerator.Current.Value[myVersion])))
                        _LongCount += _Enumerator.Current.Value[myVersion].ULongCount();
                }

                return _LongCount;

            }
        }

        #endregion


        #region GetIDictionary(myVersion)

        public virtual IDictionary<TKey, HashSet<TValue>> GetIDictionary(Int64 myVersion)
        {
            lock (this)
            {
                return (from _KeyValuePair in _IDictionary
                        where _KeyValuePair.Value.isLatestDeleted == false
                        select _KeyValuePair).ToDictionary(key => key.Key, value => value.Value.Values);
            }
        }

        #endregion

        #region GetIDictionary(myFunc, myVersion)

        public virtual IDictionary<TKey, HashSet<TValue>> GetIDictionary(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion)
        {
            lock (this)
            {

                var _Dictionary = new Dictionary<TKey, HashSet<TValue>>();
                var _Enumerator = _IDictionary.GetEnumerator();

                while (_Enumerator.MoveNext())
                {
                    if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(_Enumerator.Current.Key, _Enumerator.Current.Value[myVersion])))
                        _Dictionary.Add(_Enumerator.Current.Key, _Enumerator.Current.Value[myVersion]);
                }

                return _Dictionary;

            }
        }

        #endregion


        #region GetEnumerator(myVersion)

        public virtual IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator(Int64 myVersion)
        {
            lock (this)
            {
                return (from _KeyValuePair in _IDictionary
                        where _KeyValuePair.Value.isLatestDeleted == false
                        select _KeyValuePair).ToDictionary(key => key.Key, value => value.Value.Values).GetEnumerator();
            }
        }

        #endregion

        #region GetEnumerator(myFunc, myVersion)

        public virtual IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion)
        {
            lock (this)
            {

                var _Enumerator = _IDictionary.GetEnumerator();

                while (_Enumerator.MoveNext())
                {
                    if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(_Enumerator.Current.Key, _Enumerator.Current.Value[myVersion])))
                        yield return new KeyValuePair<TKey, HashSet<TValue>>(_Enumerator.Current.Key, _Enumerator.Current.Value[myVersion]);
                }

            }
        }

        #endregion

        #endregion

        #region Additional methods

        #region VersionCount(myKey)

        public virtual UInt64 VersionCount(TKey myKey)
        {
            lock (this)
            {

                IndexValueHistoryList<TValue> _IndexValueHistoryList;

                var _Success = _IDictionary.TryGetValue(myKey, out _IndexValueHistoryList);

                // Do _not_ check if the IndexValue is deleted!
                if (_Success && _IndexValueHistoryList != null)
                    return _IndexValueHistoryList.VersionCount;

                return default(UInt64);

            }
        }

        #endregion

        #region ClearHistory(myKey)

        public virtual void ClearHistory(TKey myKey)
        {
            lock (this)
            {

                IndexValueHistoryList<TValue> _IndexValueHistoryList;

                var _Success = _IDictionary.TryGetValue(myKey, out _IndexValueHistoryList);

                if (_Success && _IndexValueHistoryList != null)
                {
                    _IndexValueHistoryList.ClearHistory();
                    isDirty = true;
                }

            }
        }

        #endregion

        #endregion


        #region Range Methods

        public IEnumerable<TValue> GreaterThan(TKey myKey, long myVersion, bool myOrEqual = true)
        {
            return GreaterThan(myKey, null, myVersion, myOrEqual);
        }

        public IEnumerable<TValue> GreaterThan(TKey myKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion, bool myOrEqual = true)
        {
            var kvps = _IDictionary.Where((kv) => (myOrEqual) ? kv.Key.CompareTo(myKey) >= 0 : kv.Key.CompareTo(myKey) > 0);

            KeyValuePair<TKey, IEnumerable<TValue>> tmp;

            foreach (var kvp in kvps)
            {
                var idxValueHistoryList = kvp.Value[myVersion];

                if (idxValueHistoryList != null && idxValueHistoryList.Count > 0)
                {
                    if (myFunc != null)
                    {
                        tmp = new KeyValuePair<TKey, IEnumerable<TValue>>(kvp.Key, idxValueHistoryList);
                        if (myFunc(tmp))
                        {
                            foreach (var val in idxValueHistoryList)
                            {
                                yield return val;
                            }
                        }
                    }
                    else
                    {
                        foreach (var val in idxValueHistoryList)
                        {
                            yield return val;
                        }
                    }
                }
            }
        }

        public IEnumerable<TValue> LowerThan(TKey myKey, long myVersion, bool myOrEqual = true)
        {
            return LowerThan(myKey, null, myVersion, myOrEqual);
        }

        public IEnumerable<TValue> LowerThan(TKey myKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion, bool myOrEqual = true)
        {
            var kvps = _IDictionary.Where((kv) => (myOrEqual) ? kv.Key.CompareTo(myKey) <= 0 : kv.Key.CompareTo(myKey) < 0);

            KeyValuePair<TKey, IEnumerable<TValue>> tmp;
            foreach (var kvp in kvps)
            {
                var idxValueHistoryList = kvp.Value[myVersion];

                if (idxValueHistoryList != null && idxValueHistoryList.Count > 0)
                {
                    if (myFunc != null)
                    {
                        tmp = new KeyValuePair<TKey, IEnumerable<TValue>>(kvp.Key, idxValueHistoryList);
                        if (myFunc(tmp))
                        {
                            foreach (var val in idxValueHistoryList)
                            {
                                yield return val;
                            }
                        }
                    }
                    else
                    {
                        foreach (var val in idxValueHistoryList)
                        {
                            yield return val;
                        }
                    }
                }
            }
        }

        public IEnumerable<TValue> InRange(TKey myFromKey, TKey myToKey, long myVersion, bool myOrEqualFromKey = true, bool myOrEqualToKey = true)
        {
            return InRange(myFromKey, myToKey, null, myVersion, myOrEqualFromKey, myOrEqualToKey);
        }

        public IEnumerable<TValue> InRange(TKey myFromKey, TKey myToKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, long myVersion, bool myOrEqualFromKey = true, bool myOrEqualToKey = true)
        {
            #region data

            HashSet<TValue> resultSet;

            #endregion

            #region myFromKey == myToKey

            if (myFromKey.CompareTo(myToKey) == 0) //from and to are the same
            {
                //lower or upper bound included?
                if (myOrEqualFromKey || myOrEqualToKey)
                {
                    if (TryGetValue(myFromKey, out resultSet, myVersion))
                    {
                        if (myFunc != null)
                        {
                            if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(myFromKey, resultSet)))
                            {
                                foreach (TValue val in resultSet)
                                {
                                    yield return val;
                                }
                            }
                        }
                        else
                        {
                            foreach (TValue val in resultSet)
                            {
                                yield return val;
                            }
                        }
                    }
                }
                //keys are equal, but the bounds themselves are not included in the search
            }

            #endregion

            #region myFromKey > myToKey

            else if (myFromKey.CompareTo(myToKey) == 1)
            {
                //check bounds

                //1st return all values between fromKey and most right key in the tree
                foreach (var kvp in _IDictionary.Where((kv) => ((myOrEqualFromKey) ? kv.Key.CompareTo(myFromKey) >= 0 : kv.Key.CompareTo(myFromKey) > 0)))
                {
                    var versionedVals = kvp.Value[myVersion];
                    //version exists and not deleted?
                    if (versionedVals != null && versionedVals.Count > 0)
                    {
                        foreach (var val in versionedVals)
                        {
                            yield return val;
                        }
                    }
                }
                

                //2nd return all values between the most left key in the tree and the toKey
                foreach (var kvp in _IDictionary.Where((kv) => ((myOrEqualToKey) ? kv.Key.CompareTo(myToKey) <= 0 : kv.Key.CompareTo(myToKey) < 0)))
                {
                    var versionedVals = kvp.Value[myVersion];
                    //version exists and not deleted?
                    if (versionedVals != null && versionedVals.Count > 0)
                    {
                        foreach (var val in versionedVals)
                        {
                            yield return val;
                        }
                    }
                }
            }

            #endregion

            #region myFromKey < myToKey

            else if (myFromKey.CompareTo(myToKey) == -1)
            {
                #region start returning values

                //get indexValueHistoryLists
                var keyValuePairs = _IDictionary.Where(
                        (kv) =>
                            ((myOrEqualFromKey) ? kv.Key.CompareTo(myFromKey) >= 0 : kv.Key.CompareTo(myFromKey) > 0)
                            &&
                            ((myOrEqualToKey) ? kv.Key.CompareTo(myToKey) <= 0 : kv.Key.CompareTo(myToKey) < 0));

                foreach (var kvp in keyValuePairs)
                {
                    var versionedValues = kvp.Value[myVersion];

                    //version exists and not deleted?
                    if (versionedValues != null && versionedValues.Count > 0)
                    {
                        foreach (var val in versionedValues)
                        {
                            yield return val;
                        }
                    }
                }

                #endregion
            }

            #endregion
        }

        #endregion

        #endregion


        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _IDictionary.GetEnumerator();
        }

        #endregion



        #region Additional IIndexObject<TKey,TValue> Members

        public IIndexObject<TKey, TValue> GetNewInstance()
        {
            return new VersionedHashIndexObject<TKey, TValue>();
        }

        public IVersionedIndexObject<TKey, TValue> GetNewInstance2()
        {
            return new VersionedHashIndexObject<TKey, TValue>();
        }

        #endregion

        #region Additional IIndexInterface<TKey,TValue> Members

        public string IndexName
        {
            get { return VersionedHashIndexObject.Name; }
        }

        public IEnumerable<TValue> GetValues()
        {

            var _Values = new List<TValue>();

            foreach (var _HashSets in Values())
                foreach (var _Value in _HashSets)
                    _Values.Add(_Value);

            return _Values;

        }

        #endregion

    }

}
