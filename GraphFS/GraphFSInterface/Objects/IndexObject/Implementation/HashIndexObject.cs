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
 * AHashIndexObject
 * (c) Achim Friedland, 2009
 */

#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using sones.Lib;
using sones.Lib.BTree;
using sones.Lib.Serializer;

using sones.Lib.Cryptography.IntegrityCheck;
using sones.Lib.Cryptography.SymmetricEncryption;
using sones.Lib.DataStructures;
using sones.Lib.NewFastSerializer;
using sones.GraphFS.Exceptions;
using sones.GraphFS.DataStructures;
using sones.Lib.DataStructures.Indices;

#endregion

namespace sones.GraphFS.Objects
{

    public class HashIndexObject
    {
        public const String Name = "OLDHASHTABLE";
    }

    /// <summary>
    /// An abstract implementation of a IndexObject to store a mapping TKey => Hashset&lt;TValue&gt; which may be embedded into other objects.
    /// </summary>
    /// <typeparam name="TKey">Must implement IComparable</typeparam>
    public class HashIndexObject<TKey, TValue> : AFSObject, IIndexObject<TKey, TValue>
        where TKey : IComparable
    {


        #region Data

        protected IDictionary<TKey, HashSet<TValue>> _IDictionary;

        #endregion


        #region Constructors

        #region HashIndexObject()

        /// <summary>
        /// This will create an empty AIndexObject using a Dictionary&lt;TKey, HashSet&lt;TValue&gt;&gt; for the internal IDictionary&lt;TKey, HashSet&lt;TValue&gt;&gt; object.
        /// </summary>
        public HashIndexObject()
            : this (new Dictionary<TKey, TValue>())
        {
        }

        #endregion

        #region HashIndexObject(myIDictionary)

        /// <summary>
        /// This will create an empty AIndexObject using the given IDictionary object for the internal IDictionary&lt;TKey, HashSet&lt;TValue&gt;&gt; object.
        /// </summary>
        public HashIndexObject(IDictionary<TKey, TValue> myIDictionary)
        {

            // Members of AGraphStructure
            _StructureVersion       = 1;

            // Members of AGraphObject
            _ObjectStream           = FSConstants.DEFAULT_INDEXSTREAM;

            // Construct new _IDictionary...
            if (myIDictionary == null)
                throw new ArgumentNullException("Type '" + myIDictionary.ToString() + "' must not be null!");

            var _IDictionaryType = myIDictionary.GetType().GetGenericTypeDefinition();
            if (_IDictionaryType == null)
                throw new ArgumentException("Type '" + myIDictionary.ToString() + "' is not a generic type with two type parameters!");

            var _IDictionaryGenericType = Activator.CreateInstance(_IDictionaryType.MakeGenericType(new Type[] { typeof(TKey), typeof(HashSet<TValue>) }));
            if (_IDictionaryGenericType == null)
                throw new ArgumentException("Type '" + myIDictionary + "' could not be instantiated as " + myIDictionary + "<" + typeof(TKey).ToString() + ", DictionaryValueHistory<" + typeof(TValue).ToString() + ">>!");

            _IDictionary = _IDictionaryGenericType as IDictionary<TKey, HashSet<TValue>>;
            if (_IDictionary == null)
                throw new ArgumentException("Type '" + _IDictionaryGenericType.ToString() + "' does not implement IDictionary<..., ...>!");

        }

        #endregion

        #region HashIndexObject(myObjectLocation, mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm)

        /// <summary>
        /// A constructor used for fast deserializing
        /// </summary>
        /// <param name="mySerializedData">A bunch of bytes[] containing the serialized IndexObject</param>
        public HashIndexObject(String myObjectLocation, Byte[] mySerializedData, IIntegrityCheck myIntegrityCheckAlgorithm, ISymmetricEncryption myEncryptionAlgorithm)
            : this()
        {

            if (mySerializedData == null || mySerializedData.Length == 0)
                throw new ArgumentNullException("mySerializedData must not be null or its length be zero!");

            Deserialize(mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm, false);

        }

        #endregion

        #endregion

        
        #region Members of AGraphStructure

        #region Serialize(ref mySerializationWriter)

        public override void Serialize(ref SerializationWriter mySerializationWriter)
        {
            Serialize(ref mySerializationWriter, 0);
        }

        #endregion

        #region Serialize(ref mySerializationWriter, myNotificationHandling)

        public void Serialize(ref SerializationWriter mySerializationWriter, UInt64 myNotificationHandling)
        {

            try
            {

                #region NotificationHandling

                mySerializationWriter.WriteUInt64(myNotificationHandling);

                #endregion
                
                #region Write Data

                mySerializationWriter.WriteUInt32((UInt32)_IDictionary.Count);

                foreach (var keyValuePair in _IDictionary)
                {

                    mySerializationWriter.WriteObject(keyValuePair.Key);
                    //((IFastSerialize)keyValuePair.Key).Serialize(ref mySerializationWriter);                    

                    mySerializationWriter.WriteUInt32((UInt32) new HashSet<TValue>(keyValuePair.Value).Count);

                    foreach (TValue val in keyValuePair.Value)
                        mySerializationWriter.WriteObject(val);

                }

                #endregion

            }

            catch (SerializationException e)
            {
                throw new SerializationException(e.Message);
            }

        }

        #endregion

        #region DeserializeInnerObject(ref mySerializationReader)

        public override void Deserialize(ref SerializationReader mySerializationReader)
        {

            try
            {

                #region NotificationHandling

                UInt64 _NotificationHandling = mySerializationReader.ReadUInt64();

                #endregion                

                #region Read IndexObject items

                UInt32 IndexHashTableNrOfEntries = mySerializationReader.ReadUInt32();
                Object  KeyObject;
                Object  ValueObject;

                #endregion

                #region Read Data

                for (UInt32 i=0; i < IndexHashTableNrOfEntries; i++)
                {
                    
                    KeyObject = mySerializationReader.ReadObject();

                    _IDictionary.Add((TKey)KeyObject, new HashSet<TValue>());

                    UInt32 IndexHashTableNrOfValues = (UInt32)mySerializationReader.ReadUInt32();

                    for (UInt32 k=0; k < IndexHashTableNrOfValues; k++)
                    {   
                         ValueObject = mySerializationReader.ReadObject();
                        _IDictionary[(TKey)KeyObject].Add((TValue)ValueObject);
                    }

                }

                #endregion

            }

            catch (Exception e)
            {
                throw new Exception("IndexObject_HashTable<" + typeof(TKey).ToString() + ", " + typeof(TValue).ToString() + "> could not be deserialized!\n\n" + e);
            }

        }

        #endregion

        #endregion

      
        #region Members of IIndexObject

        #region IndexName

        public String IndexName { get { return HashIndexObject.Name; } }

        #endregion

        #region GetNewInstance()

        public IIndexObject<TKey, TValue> GetNewInstance()
        {
            return new HashIndexObject<TKey, TValue>();
        }

        #endregion


        #region Add

        #region Add(myKey, myValue)

        public virtual void Add(TKey myKey, TValue myValue)
        {
            Set(myKey, myValue, IndexSetStrategy.MERGE);
        }

        #endregion

        #region Add(myKey, myValues)

        public virtual void Add(TKey myKey, IEnumerable<TValue> myValues)
        {
            Set(myKey, myValues, IndexSetStrategy.MERGE);
        }

        #endregion

        #region Add(myKeyValuePair)

        public virtual void Add(KeyValuePair<TKey, TValue> myKeyValuePair)
        {
            Add(myKeyValuePair.Key, myKeyValuePair.Value);
        }

        #endregion

        #region Add(myKeyValuesPair)

        public virtual void Add(KeyValuePair<TKey, IEnumerable<TValue>> myKeyValuesPair)
        {
            Add(myKeyValuesPair.Key, myKeyValuesPair.Value);
        }

        #endregion

        #region Add(myDictionary)

        public virtual void Add(Dictionary<TKey, TValue> myDictionary)
        {
            Set(myDictionary, IndexSetStrategy.MERGE);
        }

        #endregion

        #region Add(myMultiValueDictionary)

        public virtual void Add(Dictionary<TKey, IEnumerable<TValue>> myMultiValueDictionary)
        {
            Set(myMultiValueDictionary, IndexSetStrategy.MERGE);
        }

        #endregion

        #endregion

        #region Set

        #region Set(myKey, myValue, myIndexSetStrategy)

        public virtual void Set(TKey myKey, TValue myValue, IndexSetStrategy myIndexSetStrategy)
        {

//            //_Logger.Trace("AIndexObject Set myKey: " + myKey.ToString() + " myValues: " + myValue.ToString());

            lock (this)
            {

                switch (myIndexSetStrategy)
                {

                    case IndexSetStrategy.MERGE:

                        HashSet<TValue> _ValueList;

                        // The key already exists!
                        if (_IDictionary.TryGetValue(myKey, out _ValueList))
                            _ValueList.Add(myValue);

                        else
                            _IDictionary.Add(myKey, new HashSet<TValue> { myValue });

                        break;

                    case IndexSetStrategy.REPLACE:

                        // Update values...
                        if (_IDictionary.ContainsKey(myKey))
                            _IDictionary[myKey] = new HashSet<TValue> { myValue };

                        // ...or add new key/values pair!
                        else
                            _IDictionary.Add(myKey, new HashSet<TValue> { myValue });

                        break;

                    case IndexSetStrategy.UNIQUE:

                        if (_IDictionary.ContainsKey(myKey))
                            throw new GraphFSException_IndexKeyAlreadyExist(myKey + " already exist");

                        _IDictionary.Add(myKey, new HashSet<TValue> { myValue });

                        break;

                }

                isDirty = true;

            }

        }

        #endregion

        #region Set(myKey, myValues, myIndexSetStrategy)

        public virtual void Set(TKey myKey, IEnumerable<TValue> myValues, IndexSetStrategy myIndexSetStrategy)
        {
//            //_Logger.Trace("AIndexObject Set myKey: " + myKey.ToString() + " myValues: " + myValues.Count());

            lock (this)
            {

                switch (myIndexSetStrategy)
                {

                    case IndexSetStrategy.MERGE:

                        HashSet<TValue> _ValueList;

                        // The key already exists!
                        if (_IDictionary.TryGetValue(myKey, out _ValueList))
                        {
                            _ValueList.UnionWith(myValues);
                        }

                        else
                            _IDictionary.Add(myKey, new HashSet<TValue>(myValues));

                        break;

                    case IndexSetStrategy.REPLACE:

                        // Update values...
                        if (_IDictionary.ContainsKey(myKey))
                            _IDictionary[myKey] = new HashSet<TValue>(myValues);

                        // ...or add new key/values pair!
                        else
                            _IDictionary.Add(myKey, new HashSet<TValue>(myValues));

                        break;

                }

                isDirty = true;

            }

        }

        #endregion

        #region Set(myKeyValuePairs, myIndexSetStrategy)

        public void Set(IEnumerable<KeyValuePair<TKey, TValue>> myKeyValuePairs, IndexSetStrategy myIndexSetStrategy)
        {
            foreach (var _KeyValuePair in myKeyValuePairs)
                Set(_KeyValuePair, myIndexSetStrategy);
        }

        #endregion

        #region Set(myKeyValuePair, myIndexSetStrategy)

        public virtual void Set(KeyValuePair<TKey, TValue> myKeyValuePair, IndexSetStrategy myIndexSetStrategy)
        {
            Set(myKeyValuePair.Key, myKeyValuePair.Value, myIndexSetStrategy);
        }

        #endregion

        #region Set(myKeyValuesPair, myIndexSetStrategy)

        public virtual void Set(KeyValuePair<TKey, IEnumerable<TValue>> myKeyValuesPair, IndexSetStrategy myIndexSetStrategy)
        {
            Set(myKeyValuesPair.Key, myKeyValuesPair.Value, myIndexSetStrategy);
        }

        #endregion

        #region Set(myDictionary, myIndexSetStrategy)

        public virtual void Set(Dictionary<TKey, TValue> myDictionary, IndexSetStrategy myIndexSetStrategy)
        {

            lock (this)
            {

                switch (myIndexSetStrategy)
                {

                    case IndexSetStrategy.MERGE:

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

        public virtual void Set(Dictionary<TKey, IEnumerable<TValue>> myMultiValueDictionary, IndexSetStrategy myIndexSetStrategy)
        {

            lock (this)
            {

                switch (myIndexSetStrategy)
                {

                    case IndexSetStrategy.MERGE:

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

        public virtual Trinary ContainsKey(TKey myKey)
        {

            if (_IDictionary.ContainsKey(myKey))
                return Trinary.TRUE;

            return Trinary.FALSE;

        }

        #endregion

        #region ContainsValue(myValue)

        public virtual Trinary ContainsValue(TValue myValue)
        {

            foreach (TKey actualKey in _IDictionary.Keys)
                if (_IDictionary[actualKey].Contains(myValue))
                    return Trinary.TRUE;

            return Trinary.FALSE;

        }

        #endregion

        #region Contains(myKey, myValue)

        public virtual Trinary Contains(TKey myKey, TValue myValue)
        {

            HashSet<TValue> _HashSet;

            var _Success = _IDictionary.TryGetValue(myKey, out _HashSet);
            
            if (_Success)
                if (_HashSet.Contains(myValue))
                    return Trinary.TRUE;

            return Trinary.FALSE;

        }

        #endregion

        #region Contains(myFunc)

        public virtual Trinary Contains(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
        {
            lock (this)
            {

                var _Enumerator = _IDictionary.GetEnumerator();

                while (_Enumerator.MoveNext())
                {
                    if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(_Enumerator.Current.Key, _Enumerator.Current.Value)))
                        return Trinary.TRUE;
                }

                return Trinary.FALSE;

            }
        }

        #endregion

        #endregion

        #region Get/Keys/Values/Enumerator

        #region this[myKey]

        public virtual HashSet<TValue> this[TKey myKey]
        {

            get
            {

                HashSet<TValue> _HashSet;

                var Success = _IDictionary.TryGetValue(myKey, out _HashSet);
                
                return _HashSet ?? new HashSet<TValue>();

            }

            set
            {
                Set(myKey, value, IndexSetStrategy.REPLACE);
            }

        }

        #endregion

        #region TryGetValue(myKey, out myValue)

        public virtual Boolean TryGetValue(TKey myKey, out HashSet<TValue> myValue)
        {

            HashSet<TValue> _HashSet;

            var _Success = _IDictionary.TryGetValue(myKey, out _HashSet);

            if (_Success)
                myValue = _HashSet;

            else
                myValue = new HashSet<TValue>();

            return _Success;

        }

        #endregion


        #region Keys()

        public virtual IEnumerable<TKey> Keys()
        {
            return _IDictionary.Keys;
        }

        #endregion

        #region Keys(myFunc)

        public virtual IEnumerable<TKey> Keys(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
        {
            lock (this)
            {

                var _Enumerator = _IDictionary.GetEnumerator();

                while (_Enumerator.MoveNext())
                {
                    if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(_Enumerator.Current.Key, _Enumerator.Current.Value)))
                        yield return _Enumerator.Current.Key;
                }

            }
        }

        #endregion

        #region KeyCount()

        public virtual UInt64 KeyCount()
        {
            return (UInt64) _IDictionary.LongCount();
        }

        #endregion

        #region KeyCount(myFunc)

        public virtual UInt64 KeyCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
        {
            lock (this)
            {

                var _LongCount  = 0UL;
                var _Enumerator = _IDictionary.GetEnumerator();

                while (_Enumerator.MoveNext())
                {
                    if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(_Enumerator.Current.Key, _Enumerator.Current.Value)))
                        _LongCount++;
                }

                return _LongCount;

            }
        }

        #endregion


        #region Values()

        public virtual IEnumerable<HashSet<TValue>> Values()
        {
            return (from _Value in _IDictionary.Values select _Value).ToList<HashSet<TValue>>();
        }

        #endregion

        #region Values(myFunc)

        public virtual IEnumerable<HashSet<TValue>> Values(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
        {
            lock (this)
            {

                var _Enumerator = _IDictionary.GetEnumerator();

                while (_Enumerator.MoveNext())
                {
                    if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(_Enumerator.Current.Key, _Enumerator.Current.Value)))
                        yield return _Enumerator.Current.Value;
                }

            }
        }

        #endregion

        public IEnumerable<TValue> GetValues()
        {
            
            var _Values = new List<TValue>();

            foreach (var _HashSets in Values())
                foreach (var _Value in _HashSets)
                    _Values.Add(_Value);

            return _Values;

        }

        #region ValueCount()

        public virtual UInt64 ValueCount()
        {
            return (UInt64)(from keyVal in _IDictionary select keyVal.Value.ULongCount()).Sum();
        }

        #endregion

        #region ValueCount(myFunc)

        public virtual UInt64 ValueCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
        {
            lock (this)
            {

                var _LongCount  = 0UL;
                var _Enumerator = _IDictionary.GetEnumerator();

                while (_Enumerator.MoveNext())
                {
                    if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(_Enumerator.Current.Key, _Enumerator.Current.Value)))
                        _LongCount += (UInt64)_Enumerator.Current.Value.LongCount();
                }

                return _LongCount;

            }
        }

        #endregion


        #region GetIDictionary()

        public virtual IDictionary<TKey, HashSet<TValue>> GetIDictionary()
        {
            //return (from _KeyValuePair in _IndexHashTable select _KeyValuePair).ToDictionary(key => key.Key, value => value.Value.AsEnumerable<TValue>());
            return _IDictionary;
        }

        #endregion

        #region GetIDictionary(myFunc)

        public virtual IDictionary<TKey, HashSet<TValue>> GetIDictionary(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
        {
            lock (this)
            {

                var _Dictionary = new Dictionary<TKey, HashSet<TValue>>();
                var _Enumerator = _IDictionary.GetEnumerator();

                while (_Enumerator.MoveNext())
                {
                    if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(_Enumerator.Current.Key, _Enumerator.Current.Value)))
                        _Dictionary.Add(_Enumerator.Current.Key, _Enumerator.Current.Value);
                }

                return _Dictionary;

            }
        }

        #endregion


        #region GetEnumerator()

        public virtual IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator()
        {
            //return (from _KeyValuePair in _IndexHashTable select new KeyValuePair<TKey, IEnumerable<TValue>>(_KeyValuePair.Key, _KeyValuePair.Value.AsEnumerable())).GetEnumerator();
            return _IDictionary.GetEnumerator();
        }

        #endregion

        #region GetEnumerator(myFunc)

        public virtual IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
        {
            lock (this)
            {

                var _Enumerator = _IDictionary.GetEnumerator();

                while (_Enumerator.MoveNext())
                {
                    if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(_Enumerator.Current.Key, _Enumerator.Current.Value)))
                        yield return new KeyValuePair<TKey, HashSet<TValue>>(_Enumerator.Current.Key, _Enumerator.Current.Value);
                }

            }
        }

        #endregion

        #endregion

        #region Remove/Clear

        #region Remove(myKey)

        public virtual Boolean Remove(TKey myKey)
        {
            return _IDictionary.Remove(myKey);
        }

        #endregion

        #region Remove(myKey, myValue)

        public virtual Boolean Remove(TKey myKey, TValue myValue)
        {

            var returnValue = false;

            if (_IDictionary.ContainsKey(myKey) && _IDictionary[myKey].Contains(myValue))
            {

                returnValue = _IDictionary[myKey].Remove(myValue);

                // Remove the key if the list of values becomes zero
                if (_IDictionary[myKey].Count == 0)
                    _IDictionary.Remove(myKey);

            }

            return returnValue;

        }

        #endregion

        #region Remove(myFunc)

        public virtual Boolean Remove(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
        {
            lock (this)
            {

                var _Success    = true;
                var _Enumerator = _IDictionary.GetEnumerator();

                while (_Enumerator.MoveNext())
                {
                    if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(_Enumerator.Current.Key, _Enumerator.Current.Value)))
                        _Success &= _IDictionary.Remove(_Enumerator.Current.Key);
                }

                return _Success;

            }
        }

        #endregion

        #region Clear()

        public virtual void Clear()
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
            var kvps = _IDictionary.Where((kv) => (myOrEqual) ? kv.Key.CompareTo(myKey) >= 0 : kv.Key.CompareTo(myKey) > 0);

            KeyValuePair<TKey, IEnumerable<TValue>> tmp;

            foreach (var kvp in kvps)
            {
                if (myFunc != null)
                {
                    tmp = new KeyValuePair<TKey, IEnumerable<TValue>>(kvp.Key, kvp.Value);
                    if (myFunc(tmp))
                    {
                        foreach (var val in kvp.Value)
                        {
                            yield return val;
                        }
                    }
                }
                else
                {
                    foreach (var val in kvp.Value)
                    {
                        yield return val;
                    }
                }
            }
        }

        public IEnumerable<TValue> LowerThan(TKey myKey, bool myOrEqual = true)
        {
            return LowerThan(myKey, null, myOrEqual);
        }

        public IEnumerable<TValue> LowerThan(TKey myKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, bool myOrEqual = true)
        {
            var kvps = _IDictionary.Where((kv) => (myOrEqual) ? kv.Key.CompareTo(myKey) <= 0 : kv.Key.CompareTo(myKey) < 0);

            KeyValuePair<TKey, IEnumerable<TValue>> tmp;

            foreach (var kvp in kvps)
            {
                if (myFunc != null)
                {
                    tmp = new KeyValuePair<TKey, IEnumerable<TValue>>(kvp.Key, kvp.Value);
                    if (myFunc(tmp))
                    {
                        foreach (var val in kvp.Value)
                        {
                            yield return val;
                        }
                    }
                }
                else
                {
                    foreach (var val in kvp.Value)
                    {
                        yield return val;
                    }
                }
            }
        }

        public IEnumerable<TValue> InRange(TKey myFromKey, TKey myToKey, bool myOrEqualFromKey = true, bool myOrEqualToKey = true)
        {
            return InRange(myFromKey, myToKey, null, myOrEqualFromKey, myOrEqualToKey);
        }

        public IEnumerable<TValue> InRange(TKey myFromKey, TKey myToKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, bool myOrEqualFromKey = true, bool myOrEqualToKey = true)
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
                    if (TryGetValue(myFromKey, out resultSet))
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
                //1st return all values between fromKey and most right key in the tree
                foreach (var kvp in _IDictionary.Where((kv) => ((myOrEqualFromKey) ? kv.Key.CompareTo(myFromKey) >= 0 : kv.Key.CompareTo(myFromKey) > 0)))
                {
                    foreach (var val in kvp.Value)
                    {
                        yield return val;
                    }
                }

                //2nd return all values between the most left key in the tree and the toKey
                foreach (var kvp in _IDictionary.Where((kv) => ((myOrEqualToKey) ? kv.Key.CompareTo(myToKey) <= 0 : kv.Key.CompareTo(myToKey) < 0)))
                {
                    foreach (var val in kvp.Value)
                    {
                        yield return val;
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
                    foreach (var val in kvp.Value)
                    {
                        yield return val;
                    }
                }

                #endregion
            }

            #endregion
        }



        #endregion

        #endregion


        public override AFSObject Clone()
        {
            throw new NotImplementedException();
        }



        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _IDictionary.GetEnumerator();
        }

        #endregion


        #region IEstimable Members

        public override ulong GetEstimatedSize()
        {
            return EstimatedSizeConstants.UndefinedObjectSize;
        }

        #endregion
    }

}
