///* GraphFS - AVersionedIndexObject
// * (c) Achim Friedland, 2009
// * 
// * Lead programmer:
// *      Achim Friedland
// * 
// * */

//#region Usings

//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Text;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters.Binary;

//using sones.Lib.BTree;
//using sones.Lib.Serializer;

//using sones.Lib;
//using sones.Lib.Cryptography.IntegrityCheck;
//using sones.Lib.Cryptography.SymmetricEncryption;
//using sones.Lib.DataStructures;
//using sones.Lib.NewFastSerializer;
//using sones.GraphFS.DataStructures;
//using sones.Lib.DataStructures.Indices;

//#endregion

//namespace sones.GraphFS.Objects
//{


//    /// <summary>
//    /// An abstract implementation of a DictionaryObject to store a versioned mapping TKey => HashSet&lt;TValue&gt; which may be embedded into other objects.
//    /// The internal IndexValueHistoryList&lt;TValue&gt; datastructure will keep up to $historysize older versions of HashSet&lt;TValue&gt;.
//    /// </summary>
//    /// <typeparam name="TKey">Must implement IComparable</typeparam>
//    public abstract class AVersionedIndexObject<TKey, TValue> : HashIndexObject<TKey, TValue>
//        where TKey : IComparable
//    {


//        #region Data

//        public new IDictionary<TKey, IndexValueHistoryList<TValue>> _IDictionary;

//        #endregion

//        #region Properties

//        #region HistorySize

//        private UInt64 _HistorySize;

//        public UInt64 HistorySize
//        {

//            get
//            {
//                return _HistorySize;
//            }

//            set
//            {
                
//                _HistorySize = value;

//                lock (this)
//                {
//                    foreach (var _KeyValuePair in _IDictionary)
//                        _KeyValuePair.Value.TruncateHistory(_HistorySize);
//                }

//            }

//        }

//        #endregion

//        #endregion


//        #region Constructors

//        #region AVersionedIndexObject()

//        /// <summary>
//        /// This will create an empty AVersionedIndexObject using a Dictionary&lt;TKey, TValue&gt; for the internal IDictionary&lt;TKey, IndexValueHistoryList&lt;TValue&gt;&gt; object.
//        /// The size of the value history will be set to 3.
//        /// </summary>
//        public AVersionedIndexObject()
//        {

//            // Members of AGraphStructure
//            _StructureVersion   = 1;

//            // Members of AGraphObject
//            _ObjectStream       = FSConstants.DEFAULT_INDEXSTREAM;

//            _IDictionary        = new Dictionary<TKey, IndexValueHistoryList<TValue>>();

//            _HistorySize        = 3;

//        }

//        #endregion

//        #region AVersionedIndexObject(myIDictionaryObject)

//        /// <summary>
//        /// This will create an empty AVersionedIndexObject using the given dictionary type for the internal IDictionary&lt;TKey, IndexValueHistoryList&lt;TValue&gt;&gt;.
//        /// The size of the value history will be set to 3.
//        /// </summary>
//        public AVersionedIndexObject(IDictionary<TKey, TValue> myIDictionaryType)
//            // IDictionary<TKey, IndexValueHistoryList<TValue>> myIDictionaryObject
//        {

//            // Members of AGraphStructure
//            _StructureVersion   = 1;

//            // Members of AGraphObject
//            _ObjectStream       = FSConstants.DEFAULT_INDEXSTREAM;


//            // Object specific data...
//            if (myIDictionaryType == null)
//                throw new ArgumentNullException("Type '" + myIDictionaryType.ToString() + "' must not be null!");

//            var _IDictionaryType = myIDictionaryType.GetType().GetGenericTypeDefinition();
//            if (_IDictionaryType == null)
//                throw new ArgumentException("Type '" + myIDictionaryType.ToString() + "' is not a generic type with two type parameters!");

//            var _IDictionaryGenericType = Activator.CreateInstance(_IDictionaryType.MakeGenericType(new Type[] { typeof(TKey), typeof(IndexValueHistoryList<TValue>) }));
//            if (_IDictionaryGenericType == null)
//                throw new ArgumentException("Type '" + myIDictionaryType + "' could not be instantiated as " + myIDictionaryType + "<" + typeof(TKey).ToString() + ", DictionaryValueHistory<" + typeof(TValue).ToString() + ">>!");

//            _IDictionary = _IDictionaryGenericType as IDictionary<TKey, IndexValueHistoryList<TValue>>;
//            if (_IDictionary == null)
//                throw new ArgumentException("Type '" + _IDictionaryGenericType.ToString() + "' does not implement IDictionary<..., ...>!");


//            _HistorySize        = 3;

//        }

//        #endregion

//        #region AVersionedIndexObject(myObjectLocation, mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm)

//        /// <summary>
//        /// A constructor used for fast deserializing
//        /// </summary>
//        /// <param name="mySerializedData">A bunch of bytes[] containing the serialized AVersionedIndexObject</param>
//        public AVersionedIndexObject(String myObjectLocation, Byte[] mySerializedData, IIntegrityCheck myIntegrityCheckAlgorithm, ISymmetricEncryption myEncryptionAlgorithm)
//        {

//            if (mySerializedData == null || mySerializedData.Length == 0)
//                throw new ArgumentNullException("mySerializedData must not be null or its length be zero!");

//            Deserialize(mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm, false);

//        }

//        #endregion

//        #endregion


//        #region Members of AGraphStructure

//        #region SerializeInnerObject(ref mySerializationWriter)

//        public override void Serialize(ref SerializationWriter mySerializationWriter)
//        {
//            throw new NotImplementedException();
//        //    SerializeObject(ref mySerializationWriter, 0);
//        }

//        #endregion

//        #region SerializeObject(ref mySerializationWriter, myNotificationHandling)

//        public void SerializeObject(ref SerializationWriter mySerializationWriter, UInt64 myNotificationHandling)
//        {
//            throw new NotImplementedException();

//        //    try
//        //    {

//        //        #region NotificationHandling

//        //        mySerializationWriter.WriteObject(myNotificationHandling);

//        //        #endregion

//        //        #region Write TKey and T Types

//        //        mySerializationWriter.WriteObject(typeof(TKey));
//        //        mySerializationWriter.WriteObject(typeof(TValue));

//        //        #endregion

//        //        #region Write if TKey and/or T are IFastSerializeable

//        //        Boolean TKeyIsIFastSerializeable = (typeof(IFastSerialize)).IsAssignableFrom(typeof(TKey));
//        //        Boolean TValueIsIFastSerializeable = (typeof(IFastSerialize)).IsAssignableFrom(typeof(TValue));

//        //        mySerializationWriter.WriteObject(TKeyIsIFastSerializeable);
//        //        mySerializationWriter.WriteObject(TValueIsIFastSerializeable);

//        //        #endregion

//        //        #region Write Data

//        //        mySerializationWriter.WriteObject((UInt64)_IndexHashTable.Count);

//        //        foreach (KeyValuePair<TKey, List<TValue>> keyValuePair in _IndexHashTable)
//        //        {

//        //            if (TKeyIsIFastSerializeable)
//        //                mySerializationWriter.WriteObject(((IFastSerialize)keyValuePair.Key).Serialize());
//        //            else
//        //                mySerializationWriter.WriteObject(keyValuePair.Key);

//        //            mySerializationWriter.WriteObject((UInt64)keyValuePair.Value.Count);

//        //            foreach (TValue val in keyValuePair.Value)
//        //            {
//        //                if (TValueIsIFastSerializeable)
//        //                    mySerializationWriter.WriteObject(((IFastSerialize)val).Serialize());
//        //                else
//        //                    mySerializationWriter.WriteObject(val);
//        //            }

//        //        }

//        //        #endregion

//        //    }

//        //    catch (SerializationException e)
//        //    {
//        //        throw new SerializationException(e.Message);
//        //    }

//        }

//        #endregion

//        #region DeserializeInnerObject(ref mySerializationReader

//        public override void Deserialize(ref SerializationReader mySerializationReader)
//        {
//            throw new NotImplementedException();

//        //    try
//        //    {

//        //        #region NotificationHandling

//        //        UInt64 _NotificationHandling = (UInt64)mySerializationReader.ReadObject();

//        //        #endregion

//        //        #region Read TKey and T Types

//        //        Type KeyType = (Type)mySerializationReader.ReadObject();
//        //        Type ValueType = (Type)mySerializationReader.ReadObject();

//        //        if (KeyType != typeof(TKey))
//        //            throw new GraphFSException_TypeParametersDiffer("Type parameter TKey of IndexObject_HashTable<" + typeof(TKey).ToString() + ", " + typeof(TValue).ToString() + "> is different from the serialized IndexObject_HashTable<" + KeyType.ToString() + ", " + ValueType.ToString() + ">!");

//        //        if (ValueType != typeof(TValue))
//        //            throw new GraphFSException_TypeParametersDiffer("Type parameter PT of IndexObject_HashTable<" + typeof(TKey).ToString() + ", " + typeof(TValue).ToString() + "> is different from the serialized IndexObject_HashTable<" + KeyType.ToString() + ", " + ValueType.ToString() + ">!");

//        //        #endregion

//        //        #region Read IndexObject items

//        //        Boolean TKeyIsIFastSerializeable = (Boolean)mySerializationReader.ReadObject();
//        //        Boolean TValueIsIFastSerializeable = (Boolean)mySerializationReader.ReadObject();
//        //        UInt64 IndexHashTableNrOfEntries = (UInt64)mySerializationReader.ReadObject();
//        //        Object KeyObject;
//        //        Object ValueObject;

//        //        #endregion

//        //        #region Read Data

//        //        for (UInt64 i = 0; i < IndexHashTableNrOfEntries; i++)
//        //        {
//        //            if (TKeyIsIFastSerializeable)
//        //            {
//        //                Byte[] KeyBytes = (Byte[])mySerializationReader.ReadObject();
//        //                KeyObject = Activator.CreateInstance(KeyType, KeyBytes);
//        //            }
//        //            else
//        //            {
//        //                KeyObject = mySerializationReader.ReadObject();
//        //            }


//        //            _IndexHashTable.Add((TKey)KeyObject, new List<TValue>());

//        //            UInt64 IndexHashTableNrOfValues = (UInt64)mySerializationReader.ReadObject();

//        //            for (UInt64 k = 0; k < IndexHashTableNrOfValues; k++)
//        //            {
//        //                if (TValueIsIFastSerializeable)
//        //                {
//        //                    Byte[] ValueBytes = (Byte[])mySerializationReader.ReadObject();
//        //                    ValueObject = Activator.CreateInstance(ValueType, ValueBytes);
//        //                }
//        //                else
//        //                {
//        //                    ValueObject = mySerializationReader.ReadObject();
//        //                }

//        //                _IndexHashTable[(TKey)KeyObject].Add((TValue)ValueObject);
//        //            }

//        //        }

//        //        #endregion

//        //    }

//        //    catch (Exception e)
//        //    {
//        //        throw new Exception("IndexObject_HashTable<" + typeof(TKey).ToString() + ", " + typeof(TValue).ToString() + "> could not be deserialized!\n\n" + e);
//        //    }

//        }

//        #endregion

//        #endregion

        
//        #region Members of AIndexObject

//        #region Add

//        #region Add(myKey, myValue)

//        public override void Add(TKey myKey, TValue myValue)
//        {
//            Set(myKey, myValue, IndexSetStrategy.MERGE);
//        }

//        #endregion

//        #region Add(myKey, myValues)

//        public override void Add(TKey myKey, IEnumerable<TValue> myValues)
//        {
//            Set(myKey, myValues, IndexSetStrategy.MERGE);
//        }

//        #endregion

//        #region Add(myKeyValuePair)

//        public override void Add(KeyValuePair<TKey, TValue> myKeyValuePair)
//        {
//            Add(myKeyValuePair.Key, myKeyValuePair.Value);
//        }

//        #endregion

//        #region Add(myKeyValuesPair)

//        public override void Add(KeyValuePair<TKey, IEnumerable<TValue>> myKeyValuesPair)
//        {
//            Add(myKeyValuesPair.Key, myKeyValuesPair.Value);
//        }

//        #endregion

//        #region Add(myDictionary)

//        public override void Add(Dictionary<TKey, TValue> myDictionary)
//        {
//            Set(myDictionary, IndexSetStrategy.MERGE);
//        }

//        #endregion

//        #region Add(myMultiValueDictionary)

//        public override void Add(Dictionary<TKey, IEnumerable<TValue>> myMultiValueDictionary)
//        {
//            Set(myMultiValueDictionary, IndexSetStrategy.MERGE);
//        }

//        #endregion

//        #endregion

//        #region Set

//        #region Set(myKey, myValue, myIndexSetStrategy)

//        public override void Set(TKey myKey, TValue myValue, IndexSetStrategy myIndexSetStrategy)
//        {

//            lock (this)
//            {

//                IndexValueHistoryList<TValue> _IndexValueHistoryList;

//                // The key already exists!
//                if (_IDictionary.TryGetValue(myKey, out _IndexValueHistoryList))
//                {

//                    _IndexValueHistoryList.Set(myValue, myIndexSetStrategy);

//                    while (_IndexValueHistoryList.VersionCount > _HistorySize)
//                        _IndexValueHistoryList.RemoveLatestFromHistory();

//                }

//                else
//                    _IDictionary.Add(myKey, new IndexValueHistoryList<TValue>(myValue));

//                isDirty = true;

//            }

//        }

//        #endregion

//        #region Set(myKey, myValues, myIndexSetStrategy)

//        public override void Set(TKey myKey, IEnumerable<TValue> myValues, IndexSetStrategy myIndexSetStrategy)
//        {

//            lock (this)
//            {

//                IndexValueHistoryList<TValue> _IndexValueHistoryList;

//                // The key already exists!
//                if (_IDictionary.TryGetValue(myKey, out _IndexValueHistoryList))
//                {

//                    _IndexValueHistoryList.Set(myValues, myIndexSetStrategy);

//                    while (_IndexValueHistoryList.VersionCount > _HistorySize)
//                        _IndexValueHistoryList.RemoveLatestFromHistory();

//                }

//                else
//                    _IDictionary.Add(myKey, new IndexValueHistoryList<TValue>(myValues));

//                isDirty = true;

//            }

//        }

//        #endregion

//        #region Set(myKeyValuePair, myIndexSetStrategy)

//        public override void Set(KeyValuePair<TKey, TValue> myKeyValuePair, IndexSetStrategy myIndexSetStrategy)
//        {
//            Set(myKeyValuePair.Key, myKeyValuePair.Value, myIndexSetStrategy);
//        }

//        #endregion

//        #region Set(myKeyValuesPair, myIndexSetStrategy)

//        public override void Set(KeyValuePair<TKey, IEnumerable<TValue>> myKeyValuesPair, IndexSetStrategy myIndexSetStrategy)
//        {
//            Set(myKeyValuesPair.Key, myKeyValuesPair.Value, myIndexSetStrategy);
//        }

//        #endregion

//        #region Set(myKeyValuePairs, myIndexSetStrategy)

//        public void Set(IEnumerable<KeyValuePair<TKey, TValue>> myKeyValuePairs, IndexSetStrategy myIndexSetStrategy)
//        {

//            lock (this)
//            {

//                switch (myIndexSetStrategy)
//                {

//                    case IndexSetStrategy.MERGE:
//                        //hack: just a workaround as this will create too much versions!
//                        foreach (var _KeyValuePair in myKeyValuePairs)
//                            Set(_KeyValuePair, myIndexSetStrategy);

//                        break;

//                    case IndexSetStrategy.REPLACE:

//                        _IDictionary.Clear();

//                        foreach (var _KeyValuePair in myKeyValuePairs)
//                            Set(_KeyValuePair, myIndexSetStrategy);

//                        break;

//                }

//                isDirty = true;

//            }

//        }

//        #endregion

//        #region Set(myDictionary, myIndexSetStrategy)

//        public override void Set(Dictionary<TKey, TValue> myDictionary, IndexSetStrategy myIndexSetStrategy)
//        {

//            lock (this)
//            {

//                switch (myIndexSetStrategy)
//                {

//                    case IndexSetStrategy.MERGE:
//                        //hack: just a workaround as this will create too much versions!
//                        foreach (var _KeyValuePair in myDictionary)
//                            Set(_KeyValuePair, myIndexSetStrategy);

//                        break;

//                    case IndexSetStrategy.REPLACE:

//                        _IDictionary.Clear();

//                        foreach (var _KeyValuePair in myDictionary)
//                            Set(_KeyValuePair, myIndexSetStrategy);

//                        break;

//                }

//                isDirty = true;

//            }

//        }

//        #endregion

//        #region Set(myMultiValueDictionary, myIndexSetStrategy)

//        public override void Set(Dictionary<TKey, IEnumerable<TValue>> myMultiValueDictionary, IndexSetStrategy myIndexSetStrategy)
//        {

//            lock (this)
//            {

//                switch (myIndexSetStrategy)
//                {

//                    case IndexSetStrategy.MERGE:
//                        //hack: just a workaround as this will create too much versions!
//                        foreach (var _KeyValuePair in myMultiValueDictionary)
//                            Set(_KeyValuePair, myIndexSetStrategy);

//                        break;

//                    case IndexSetStrategy.REPLACE:

//                        _IDictionary.Clear();

//                        foreach (var _KeyValuePair in myMultiValueDictionary)
//                            Set(_KeyValuePair, myIndexSetStrategy);

//                        break;

//                }

//                isDirty = true;

//            }

//        }

//        #endregion

//        #endregion

//        #region Contains

//        #region ContainsKey(myKey)

//        public override Trinary ContainsKey(TKey myKey)
//        {
//            return ContainsKey(myKey, 0);
//        }

//        #endregion

//        #region ContainsValue(myValue)

//        public override Trinary ContainsValue(TValue myValue)
//        {
//            return ContainsValue(myValue, 0);
//        }

//        #endregion

//        #region Contains(myKey, myValue)

//        public override Trinary Contains(TKey myKey, TValue myValue)
//        {
//            return Contains(myKey, myValue, 0);
//        }

//        #endregion

//        #region Contains(myFunc)

//        public override Trinary Contains(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
//        {
//            lock (this)
//            {

//                var _Enumerator = _IDictionary.GetEnumerator();

//                while (_Enumerator.MoveNext())
//                {
//                    if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(_Enumerator.Current.Key, _Enumerator.Current.Value.Values)))
//                        return Trinary.TRUE;
//                }

//                return Trinary.FALSE;

//            }
//        }

//        #endregion

//        #endregion

//        #region Get/Keys/Values/Enumerator

//        #region this[myKey]

//        public override HashSet<TValue> this[TKey myKey]
//        {

//            get
//            {
//                lock (this)
//                {

//                    IndexValueHistoryList<TValue> _IndexValueHistoryList;

//                    var _Success = _IDictionary.TryGetValue(myKey, out _IndexValueHistoryList);

//                    if (_Success && _IndexValueHistoryList != null && !_IndexValueHistoryList.isDeleted)
//                        return _IndexValueHistoryList.Values;

//                    return new HashSet<TValue>();

//                }
//            }

//            set
//            {
//                Set(myKey, value, IndexSetStrategy.REPLACE);
//            }

//        }

//        #endregion

//        #region TryGetValue(myKey, out myValue)

//        public override Boolean TryGetValue(TKey myKey, out HashSet<TValue> myValue)
//        {
//            lock (this)
//            {

//                IndexValueHistoryList<TValue> _IndexValueHistoryList;

//                var _Success = _IDictionary.TryGetValue(myKey, out _IndexValueHistoryList);

//                if (_Success && _IndexValueHistoryList != null && !_IndexValueHistoryList.isDeleted)
//                    myValue = _IndexValueHistoryList.Values;

//                else
//                    myValue = new HashSet<TValue>();

//                return _Success;

//            }
//        }

//        #endregion


//        #region Keys()

//        public override IEnumerable<TKey> Keys()
//        {
//            return Keys(0);
//        }

//        #endregion

//        #region Keys(myFunc)

//        public override IEnumerable<TKey> Keys(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
//        {
//            return Keys(myFunc, 0);
//        }

//        #endregion

//        #region KeyCount()

//        public override UInt64 KeyCount()
//        {
//            return KeyCount(0);
//        }

//        #endregion

//        #region KeyCount(myFunc)

//        public override UInt64 KeyCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
//        {
//            return KeyCount(myFunc, 0);
//        }

//        #endregion


//        #region Values()

//        public override IEnumerable<HashSet<TValue>> Values()
//        {
//            return Values(0);
//        }

//        #endregion

//        #region Values(myFunc)

//        public override IEnumerable<HashSet<TValue>> Values(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
//        {
//            return Values(myFunc, 0);
//        }

//        #endregion

//        #region ValueCount()

//        public override UInt64 ValueCount()
//        {
//            return ValueCount(0);
//        }

//        #endregion

//        #region ValueCount(myFunc)

//        public override UInt64 ValueCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
//        {
//            return ValueCount(myFunc, 0);
//        }

//        #endregion


//        #region GetIDictionary()

//        public override IDictionary<TKey, HashSet<TValue>> GetIDictionary()
//        {
//            return GetIDictionary(0);
//        }

//        #endregion

//        #region GetIDictionary(myFunc)

//        public override IDictionary<TKey, HashSet<TValue>> GetIDictionary(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
//        {
//            return GetIDictionary(myFunc, 0);
//        }

//        #endregion


//        #region GetEnumerator()

//        public override IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator()
//        {
//            return GetEnumerator(0);
//        }

//        #endregion

//        #region GetEnumerator(myFunc)

//        public override IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
//        {
//            return GetEnumerator(myFunc, 0);
//        }

//        #endregion

//        #endregion

//        #region Remove/Clear

//        #region Remove(myKey)

//        public override Boolean Remove(TKey myKey)
//        {
//            lock (this)
//            {

//                IndexValueHistoryList<TValue> _IndexValueHistoryList;

//                Boolean _Success = _IDictionary.TryGetValue(myKey, out _IndexValueHistoryList);

//                if (_Success && _IDictionary != null)
//                {

//                    _IndexValueHistoryList.isDeleted = true;

//                    while (_IndexValueHistoryList.VersionCount > _HistorySize)
//                        _IndexValueHistoryList.RemoveLatestFromHistory();

//                }

//                return _Success;

//            }
//        }

//        #endregion

//        #region Remove(myKey, myValue)

//        public override Boolean Remove(TKey myKey, TValue myValue)
//        {
//            lock (this)
//            {

//                IndexValueHistoryList<TValue> _IndexValueHistoryList;

//                var _Success = _IDictionary.TryGetValue(myKey, out _IndexValueHistoryList);

//                if (_Success && _IndexValueHistoryList != null)
//                {

//                    _IndexValueHistoryList.Remove(myValue);

//                    while (_IndexValueHistoryList.VersionCount > _HistorySize)
//                        _IndexValueHistoryList.RemoveLatestFromHistory();
                
//                }

//                return _Success;

//            }
//        }

//        #endregion

//        #region Remove(myFunc)

//        public override Boolean Remove(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
//        {
//            lock (this)
//            {

//                var _Success = true;
//                var _Enumerator = _IDictionary.GetEnumerator();

//                while (_Enumerator.MoveNext())
//                {
//                    if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(_Enumerator.Current.Key, _Enumerator.Current.Value.Values)))
//                        _Success &= _IDictionary.Remove(_Enumerator.Current.Key);
//                }

//                return _Success;

//            }
//        }

//        #endregion

//        #region Clear()

//        public override void Clear()
//        {
//            _IDictionary.Clear();
//        }

//        #endregion

//        #endregion

//        #endregion


//        #region IVersionedIndexObject<TKey,TValue> Members

//        #region Contains

//        #region ContainsKey(myKey, myVersion)

//        public virtual Trinary ContainsKey(TKey myKey, Int64 myVersion)
//        {
//            lock (this)
//            {

//                if (!_IDictionary.ContainsKey(myKey))
//                    return Trinary.FALSE;

//                if (_IDictionary[myKey].isDeleted)
//                    return Trinary.DELETED;

//                return Trinary.TRUE;

//            }
//        }

//        #endregion

//        #region ContainsValue(myValue, myVersion)

//        public virtual Trinary ContainsValue(TValue myValue, Int64 myVersion)
//        {
//            throw new NotImplementedException();
//        }

//        #endregion

//        #region Contains(myKey, myValue, myVersion)

//        public virtual Trinary Contains(TKey myKey, TValue myValue, Int64 myVersion)
//        {
//            lock (this)
//            {

//                IndexValueHistoryList<TValue> _IndexValueHistoryList;

//                var _Success = _IDictionary.TryGetValue(myKey, out _IndexValueHistoryList);

//                if (_Success)
//                    if (_IndexValueHistoryList.Values.Contains(myValue))
//                        return Trinary.TRUE;

//                return Trinary.FALSE;

//            }
//        }

//        #endregion

//        #region Contains(myFunc)

//        public virtual Trinary Contains(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion)
//        {
//            lock (this)
//            {

//                var _Enumerator = _IDictionary.GetEnumerator();

//                while (_Enumerator.MoveNext())
//                {
//                    if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(_Enumerator.Current.Key, _Enumerator.Current.Value[myVersion])))
//                        return Trinary.TRUE;
//                }

//                return Trinary.FALSE;

//            }
//        }

//        #endregion

//        #endregion

//        #region Get/Keys/Values/Enumerator

//        #region this[myKey, myVersion]  // Int64

//        public virtual HashSet<TValue> this[TKey myKey, Int64 myVersion]
//        {

//            get
//            {
//                lock (this)
//                {

//                    IndexValueHistoryList<TValue> _IndexValueHistoryList;

//                    var _Success = _IDictionary.TryGetValue(myKey, out _IndexValueHistoryList);

//                    // Do _not_ check if the value is deleted!
//                    if (_Success && _IndexValueHistoryList != null)
//                        return _IndexValueHistoryList[myVersion];

//                    return new HashSet<TValue>();

//                }
//            }

//        }

//        #endregion

//        #region this[myKey, myVersion]  // UInt64

//        public virtual HashSet<TValue> this[TKey myKey, UInt64 myVersion]
//        {

//            get
//            {
//                lock (this)
//                {

//                    IndexValueHistoryList<TValue> _IndexValueHistoryList;

//                    var _Success = _IDictionary.TryGetValue(myKey, out _IndexValueHistoryList);

//                    // Do _not_ check if the value is deleted!
//                    if (_Success && _IndexValueHistoryList != null)
//                        return _IndexValueHistoryList[myVersion];

//                    return new HashSet<TValue>();

//                }
//            }

//        }

//        #endregion

//        #region TryGetValue(myKey, out myValue, myVersion)

//        public virtual Boolean TryGetValue(TKey myKey, out HashSet<TValue> myValue, Int64 myVersion)
//        {
//            lock (this)
//            {

//                IndexValueHistoryList<TValue> _IndexValueHistoryList;

//                var _Success = _IDictionary.TryGetValue(myKey, out _IndexValueHistoryList);

//                // Do _not_ check if the value is deleted!
//                if (_Success && _IndexValueHistoryList != null)
//                    myValue = _IndexValueHistoryList.Values;

//                else
//                    myValue = new HashSet<TValue>();

//                return _Success;

//            }
//        }

//        #endregion


//        #region Keys(myVersion)

//        public virtual IEnumerable<TKey> Keys(Int64 myVersion)
//        {
//            lock (this)
//            {
//                return (from _KeyValuePair in _IDictionary
//                        where _KeyValuePair.Value.isDeleted == false
//                        select _KeyValuePair.Key).ToList<TKey>();
//            }
//        }

//        #endregion

//        #region Keys(myFunc, myVersion)

//        public virtual IEnumerable<TKey> Keys(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion)
//        {
//            lock (this)
//            {

//                var _Enumerator = _IDictionary.GetEnumerator();

//                while (_Enumerator.MoveNext())
//                {
//                    if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(_Enumerator.Current.Key, _Enumerator.Current.Value[myVersion])))
//                        yield return _Enumerator.Current.Key;
//                }

//            }
//        }

//        #endregion

//        #region KeyCount(myVersion)

//        public virtual UInt64 KeyCount(Int64 myVersion)
//        {
//            lock (this)
//            {
//                return (from _KeyValuePair in _IDictionary
//                        where _KeyValuePair.Value.isDeleted == false
//                        select true).ULongCount();

//            }
//        }

//        #endregion

//        #region KeyCount(myFunc, myVersion)

//        public virtual UInt64 KeyCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion)
//        {
//            lock (this)
//            {

//                var _LongCount  = 0UL;
//                var _Enumerator = _IDictionary.GetEnumerator();

//                while (_Enumerator.MoveNext())
//                {
//                    if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(_Enumerator.Current.Key, _Enumerator.Current.Value[myVersion])))
//                        _LongCount++;
//                }

//                return _LongCount;

//            }
//        }

//        #endregion


//        #region Values(myVersion)

//        public virtual IEnumerable<HashSet<TValue>> Values(Int64 myVersion)
//        {
//            lock (this)
//            {
//                return from _KeyValuePair in _IDictionary
//                       where _KeyValuePair.Value.isDeleted == false
//                       select _KeyValuePair.Value.Values;
//            }
//        }

//        #endregion

//        #region Values(myFunc, myVersion)

//        public virtual IEnumerable<HashSet<TValue>> Values(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion)
//        {
//            lock (this)
//            {

//                var _Enumerator = _IDictionary.GetEnumerator();

//                while (_Enumerator.MoveNext())
//                {
//                    if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(_Enumerator.Current.Key, _Enumerator.Current.Value[myVersion])))
//                        yield return _Enumerator.Current.Value[myVersion];
//                }

//            }
//        }

//        #endregion

//        #region ValueCount(myVersion)

//        public virtual UInt64 ValueCount(Int64 myVersion)
//        {
//            lock (this)
//            {
//                return (from _KeyValuePair in _IDictionary
//                        where _KeyValuePair.Value.isDeleted == false
//                        select _KeyValuePair.Value.Values.ULongCount()).Sum();
//            }
//        }

//        #endregion

//        #region ValueCount(myFunc, myVersion)

//        public virtual UInt64 ValueCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion)
//        {
//            lock (this)
//            {

//                var _LongCount  = 0UL;
//                var _Enumerator = _IDictionary.GetEnumerator();

//                while (_Enumerator.MoveNext())
//                {
//                    if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(_Enumerator.Current.Key, _Enumerator.Current.Value[myVersion])))
//                        _LongCount += _Enumerator.Current.Value[myVersion].ULongCount();
//                }

//                return _LongCount;

//            }
//        }

//        #endregion


//        #region GetIDictionary(myVersion)

//        public virtual IDictionary<TKey, HashSet<TValue>> GetIDictionary(Int64 myVersion)
//        {
//            lock (this)
//            {
//                return (from _KeyValuePair in _IDictionary
//                        where _KeyValuePair.Value.isDeleted == false
//                        select _KeyValuePair).ToDictionary(key => key.Key, value => value.Value.Values);
//            }
//        }

//        #endregion

//        #region GetIDictionary(myFunc, myVersion)

//        public virtual IDictionary<TKey, HashSet<TValue>> GetIDictionary(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion)
//        {
//            lock (this)
//            {

//                var _Dictionary = new Dictionary<TKey, HashSet<TValue>>();
//                var _Enumerator = _IDictionary.GetEnumerator();

//                while (_Enumerator.MoveNext())
//                {
//                    if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(_Enumerator.Current.Key, _Enumerator.Current.Value[myVersion])))
//                        _Dictionary.Add(_Enumerator.Current.Key, _Enumerator.Current.Value[myVersion]);
//                }

//                return _Dictionary;

//            }
//        }

//        #endregion


//        #region GetEnumerator(myVersion)

//        public virtual IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator(Int64 myVersion)
//        {
//            lock (this)
//            {
//                return (from _KeyValuePair in _IDictionary
//                        where _KeyValuePair.Value.isDeleted == false
//                        select _KeyValuePair).ToDictionary(key => key.Key, value => value.Value.Values).GetEnumerator();
//            }
//        }

//        #endregion

//        #region GetEnumerator(myFunc, myVersion)

//        public virtual IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion)
//        {
//            lock (this)
//            {

//                var _Enumerator = _IDictionary.GetEnumerator();

//                while (_Enumerator.MoveNext())
//                {
//                    if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(_Enumerator.Current.Key, _Enumerator.Current.Value[myVersion])))
//                        yield return new KeyValuePair<TKey, HashSet<TValue>>(_Enumerator.Current.Key, _Enumerator.Current.Value[myVersion]);
//                }

//            }
//        }

//        #endregion

//        #endregion

//        #region Additional methods

//        #region VersionCount(myKey)

//        public virtual UInt64 VersionCount(TKey myKey)
//        {
//            lock (this)
//            {

//                IndexValueHistoryList<TValue> _IndexValueHistoryList;

//                var _Success = _IDictionary.TryGetValue(myKey, out _IndexValueHistoryList);

//                // Do _not_ check if the IndexValue is deleted!
//                if (_Success && _IndexValueHistoryList != null)
//                    return _IndexValueHistoryList.VersionCount;

//                return default(UInt64);

//            }
//        }

//        #endregion

//        #region ClearHistory(myKey)

//        public virtual void ClearHistory(TKey myKey)
//        {
//            lock (this)
//            {

//                IndexValueHistoryList<TValue> _IndexValueHistoryList;

//                var _Success = _IDictionary.TryGetValue(myKey, out _IndexValueHistoryList);

//                if (_Success && _IndexValueHistoryList != null)
//                {
//                    _IndexValueHistoryList.ClearHistory();
//                    isDirty = true;
//                }

//            }
//        }

//        #endregion

//        #endregion

//        #endregion

//    }

//}
