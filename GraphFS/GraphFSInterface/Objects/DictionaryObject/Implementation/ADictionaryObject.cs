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


/* PandoraFS - ADictionaryObject
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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using sones.Lib;
using sones.Lib.BTree;
using sones.Lib.Serializer;

using sones.Lib.DataStructures;
using sones.Lib.NewFastSerializer;
using sones.Lib.DataStructures.Indices;

#endregion

namespace sones.GraphFS.Objects
{

    /// <summary>
    /// An abstract implementation of a DictionaryObject to store a mapping TKey => TValue which may be embedded into other objects.
    /// </summary>
    /// <typeparam name="TKey">Must implement IComparable</typeparam>

    

    public abstract class ADictionaryObject<TKey, TValue> : AFSObject
        where TKey : IComparable
    {


        #region Data

        protected IDictionary<TKey, TValue> _IDictionary;

        #endregion


        #region Constructor

        #region ADictionaryObject()

        /// <summary>
        /// This will create an empty ADictionaryObject using a Dictionary&lt;TKey, TValue&gt; for the internal IDictionary&lt;TKey, TValue&gt; object.
        /// </summary>
        protected ADictionaryObject()
            : this (new Dictionary<TKey, TValue>())
        {
        }

        #endregion

        #region ADictionaryObject(myIDictionary)

        /// <summary>
        /// This will create an empty ADictionaryObject using the given IDictionary object for the internal IDictionary&lt;TKey, TValue&gt; object.
        /// </summary>
        protected ADictionaryObject(IDictionary<TKey, TValue> myIDictionary)
        {
            _IDictionary        = myIDictionary;
        }

        #endregion

        #region ADictionaryObject(myObjectLocation, mySerializedData)

        /// <summary>
        /// A constructor used for fast deserializing
        /// </summary>
        /// <param name="mySerializedData">A bunch of bytes[] containing the serialized ADictionaryObject</param>
        protected ADictionaryObject(String myObjectLocation, Byte[] mySerializedData)
            : this()
        {

            if (mySerializedData == null || mySerializedData.Length == 0)
                throw new ArgumentNullException("mySerializedData must not be null or its length be zero!");

            Deserialize(mySerializedData);

        }

        #endregion

        #endregion

        
        #region Members of APandoraStructure

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

                mySerializationWriter.WriteObject(myNotificationHandling);

                #endregion

                #region Write KeyValuePairs

                mySerializationWriter.WriteObject((UInt64) _IDictionary.Count);

                foreach (var _KeyValuePair in _IDictionary)
                {                                        
                    // Write TKey...
                    mySerializationWriter.WriteObject(_KeyValuePair.Key);

                    // Write TValue...                    
                    mySerializationWriter.WriteObject(_KeyValuePair.Value);

                }

                #endregion

            }

            catch (SerializationException e)
            {
                throw new SerializationException("ADictionaryObject_HashTable serialization error: " + e.Message, e);
            }

            catch (Exception e)
            {
                throw new Exception("ADictionaryObject_HashTable serialization error: " + e.Message, e);
            }

        }

        #endregion

        #region DeserializeInnerObject(ref mySerializationReader)

        public override void Deserialize(ref SerializationReader mySerializationReader)
        {

            try
            {
                
                #region NotificationHandling

                UInt64 _NotificationHandling = (UInt64)mySerializationReader.ReadObject();

                #endregion

                #region Read KeyValuePairs

                UInt64 IndexHashTableNrOfEntries = (UInt64) mySerializationReader.ReadObject();
                _IDictionary = new Dictionary<TKey, TValue>();

                if (IndexHashTableNrOfEntries > 0)
                {

                    TKey   KeyObject;
                    TValue ValueObject;

                    for (UInt64 i = 0; i < IndexHashTableNrOfEntries; i++)
                    {                                                
                        
                        KeyObject = (TKey)mySerializationReader.ReadObject();
                        
                        ValueObject = (TValue) mySerializationReader.ReadObject();

                        // -- Add both to the internal dictionary ---------
                        _IDictionary.Add(KeyObject, ValueObject);

                    }

                }

                #endregion

            }

            catch (SerializationException e)
            {
                throw new SerializationException("ADictionaryObject_HashTable deserialization error: " + e.Message, e);
            }

            catch (Exception e)
            {
                throw new Exception("ADictionaryObject_HashTable deserialization error: " + e.Message, e);
            }

        }

        #endregion

        #endregion


        #region Object-specific Methods

        #region Add - will fail if the key already exists

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
                if (!_IDictionary.ContainsKey(myKey))
                {
                    _IDictionary.Add(myKey, myValue);
                    isDirty = true;
                    return 1;
                }
            }

            return 0;

        }

        #endregion

        #region Add(myKeyValuePair)

        /// <summary>
        /// Adds the given key-value-pair, but fails if the key already exists.
        /// </summary>
        /// <param name="myKeyValuePair">a key-value-pair</param>
        /// <returns>0 if it failed, 1 for a success</returns>
        protected virtual UInt64 Add(KeyValuePair<TKey, TValue> myKeyValuePair)
        {

            lock (this)
            {
                if (!_IDictionary.ContainsKey(myKeyValuePair.Key))
                {
                    _IDictionary.Add(myKeyValuePair.Key, myKeyValuePair.Value);
                    isDirty = true;
                    return 1;
                }
            }

            return 0;

        }

        #endregion

        #region Add(myKeyValuePairs)

        /// <summary>
        /// Adds all given key-value-pairs in one atomic operation.
        /// Will fail if any key already exists.
        /// </summary>
        /// <param name="myKeyValuePairs">An IEnumerable of Key-Value-Pairs</param>
        /// <returns>1 for a success or 0 if any key already exists.</returns>
        protected virtual UInt64 Add(IEnumerable<KeyValuePair<TKey, TValue>> myKeyValuePairs)
        {
            lock (this)
            {

                foreach (var _KeyValuePair in myKeyValuePairs)
                    if (_IDictionary.ContainsKey(_KeyValuePair.Key))
                        return 0;

                foreach (var _KeyValuePair in myKeyValuePairs)
                    _IDictionary.Add(_KeyValuePair.Key, _KeyValuePair.Value);

                isDirty = true;

                return 1;

            }
        }

        #endregion

        #region Add(myDictionary)

        /// <summary>
        /// Adds all key-value-pairs of the given IDictionary in one atomic operation.
        /// Will fail if any key already exists.
        /// </summary>
        /// <param name="myIDictionary">An IDictionary</param>
        /// <returns>1 for a success or 0 if any key already exists.</returns>
        protected virtual UInt64 Add(IDictionary<TKey, TValue> myIDictionary)
        {
            lock (this)
            {

                foreach (var _KeyValuePair in myIDictionary)
                    if (_IDictionary.ContainsKey(_KeyValuePair.Key))
                        return 0;

                foreach (var _KeyValuePair in myIDictionary)
                    _IDictionary.Add(_KeyValuePair.Key, _KeyValuePair.Value);

                isDirty = true;

                return 1;

            }
        }

        #endregion

        #endregion

        #region Set

        #region Set(myKey, myValue)

        /// <summary>
        /// Set a value of type T to the index using a key of type TKey.
        /// This will add the value if it does not exist, or overwritte an
        /// existing value.
        /// </summary>
        /// <param name="myKey">the key to access the value</param>
        /// <param name="myValue">the value storing some information</param>
        protected virtual UInt64 Set(TKey myKey, TValue myValue)
        {

            lock (this)
            {

                TValue _Value;

                if (_IDictionary.TryGetValue(myKey, out _Value))
                    _Value = myValue;

                else
                    _IDictionary.Add(myKey, myValue);

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

                return 1;

            }
        }

        #endregion

        #endregion

        #region Replace - will fail if the key does not exist or the given value != actual value

        #region Replace(myKey, myValue)

        /// <summary>
        /// Replaces the value indexed by myKey with myNewValue as logn as the given myOldValue matched the actual value.
        /// Will fail if the key is not existent or the actual value is not equals myOldValue due to concurrency conflicts.
        /// </summary>
        /// <param name="myKey">the key to access the value</param>
        /// <param name="myOldValue">the old value</param>
        /// <param name="myNewValue">the new value</param>
        /// <returns>0 if it failed, 1 for a success</returns>
        protected virtual UInt64 Replace(TKey myKey, TValue myOldValue, TValue myNewValue)
        {

            lock (this)
            {

                TValue _OldValue;

                if (_IDictionary.TryGetValue(myKey, out _OldValue))
                {
                    if (myOldValue.Equals(_OldValue))
                    {
                        _IDictionary[myKey] = myNewValue;
                        isDirty = true;
                        return 1;
                    }
                }

                return 0;

            }

        }

        #endregion

        #endregion

        #region Contains

        #region ContainsKey(myKey)

        /// <summary>
        /// Checks if the given key exists within the index.
        /// </summary>
        /// <param name="myKey">the key to search for</param>
        /// <returns>true|false</returns>
        protected virtual Trinary ContainsKey(TKey myKey)
        {
            lock (this)
            {

                if (_IDictionary.ContainsKey(myKey))
                    return Trinary.TRUE;

                return Trinary.FALSE;

            }
        }

        #endregion

        #region ContainsValue(myValue)

        /// <summary>
        /// Checks if the given value exists within the index.
        /// </summary>
        /// <param name="myValue">the value to search for</param>
        /// <returns>true|false</returns>
        protected virtual Trinary ContainsValue(TValue myValue)
        {
            lock (this)
            {

                var _ReturnValue = (from item in _IDictionary where myValue.Equals(item.Value) select myValue).First<TValue>();

                if (_ReturnValue == null)
                    return Trinary.FALSE;

                return Trinary.TRUE;

            }
        }

        #endregion

        #region Contains(myKey, myValue)

        protected virtual Trinary Contains(TKey myKey, TValue myValue)
        {
            lock (this)
            {

                TValue _Value;

                var _Success = _IDictionary.TryGetValue(myKey, out _Value);

                if (_Success && myValue.Equals(_Value))
                    return Trinary.TRUE;

                return Trinary.FALSE;

            }
        }

        #endregion

        #region Contains(myFunc)

        protected virtual Trinary Contains(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc)
        {
            lock (this)
            {

                var _Enumerator = _IDictionary.GetEnumerator();

                while (_Enumerator.MoveNext())
                {
                    if (myFunc(_Enumerator.Current))
                        return Trinary.TRUE;
                }

                return Trinary.FALSE;

            }
        }

        #endregion

        #endregion

        #region Get/Keys/Values/Enumerator

        #region this[myKey]

        protected virtual TValue this[TKey myKey]
        {

            get
            {
                lock (this)
                {
                    return _IDictionary[myKey];
                }
            }

            set
            {
                lock (this)
                {
                    _IDictionary[myKey] = value;
                }
            }

        }

        #endregion

        #region TryGetValue(myKey, out myValue)

        protected virtual Boolean TryGetValue(TKey myKey, out TValue myValue)
        {
            lock (this)
            {
                return _IDictionary.TryGetValue(myKey, out myValue);
            }
        }

        #endregion


        #region Keys()

        protected virtual ICollection<TKey> Keys()
        {
            lock (this)
            {
                return _IDictionary.Keys;
            }
        }

        #endregion

        #region Keys(myFunc)

        protected virtual ICollection<TKey> Keys(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc)
        {
            lock (this)
            {

                var _HashSet    = new HashSet<TKey>();
                var _Enumerator = _IDictionary.GetEnumerator();

                while (_Enumerator.MoveNext())
                {
                    if (myFunc(_Enumerator.Current))
                        _HashSet.Add(_Enumerator.Current.Key);
                }

                return _HashSet;

            }
        }

        #endregion

        #region KeyCount()

        protected virtual UInt64 KeyCount()
        {
            lock (this)
            {
                return (UInt64) _IDictionary.Keys.LongCount();
            }
        }

        #endregion

        #region KeyCount(myFunc)

        protected virtual UInt64 KeyCount(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc)
        {
            lock (this)
            {

                var _LongCount  = 0UL;
                var _Enumerator = _IDictionary.GetEnumerator();

                while (_Enumerator.MoveNext())
                {
                    if (myFunc(_Enumerator.Current))
                        _LongCount++;
                }

                return _LongCount;

            }
        }

        #endregion


        #region Values()

        protected virtual ICollection<TValue> Values()
        {
            lock (this)
            {
                return _IDictionary.Values;
            }
        }

        #endregion

        #region Values(myFunc)

        protected virtual ICollection<TValue> Values(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc)
        {
            lock (this)
            {

                var _HashSet    = new HashSet<TValue>();
                var _Enumerator = _IDictionary.GetEnumerator();

                while (_Enumerator.MoveNext())
                {
                    if (myFunc(_Enumerator.Current))
                        _HashSet.Add(_Enumerator.Current.Value);
                }

                return _HashSet;

            }
        }

        #endregion

        #region ValueCount()

        protected virtual UInt64 ValueCount()
        {
            lock (this)
            {
                return (UInt64)_IDictionary.LongCount();
            }
        }

        #endregion

        #region ValueCount(myFunc)

        protected virtual UInt64 ValueCount(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc)
        {
            lock (this)
            {

                var _LongCount  = 0UL;
                var _Enumerator = _IDictionary.GetEnumerator();

                while (_Enumerator.MoveNext())
                {
                    if (myFunc(_Enumerator.Current))
                        _LongCount++;
                }

                return _LongCount;

            }
        }

        #endregion


        #region GetIDictionary()

        protected virtual IDictionary<TKey, TValue> GetIDictionary()
        {
            lock (this)
            {
                return (IDictionary<TKey, TValue>) _IDictionary;
            }
        }

        #endregion

        #region GetIDictionary(myKeys)

        protected virtual IDictionary<TKey, TValue> GetIDictionary(params TKey[] myKeys)
        {
            lock (this)
            {

                TValue _Value;

                var _newIDictionary = new Dictionary<TKey, TValue>();

                foreach (var _Key in myKeys)
                {
                    if (_IDictionary.TryGetValue(_Key, out _Value))
                        _newIDictionary.Add(_Key, _Value);
                }

                return _newIDictionary;

            }
        }

        #endregion

        #region GetIDictionary(myFunc)

        protected virtual IDictionary<TKey, TValue> GetIDictionary(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc)
        {
            lock (this)
            {

                var _IDictionary = new Dictionary<TKey, TValue>();
                var _Enumerator  = _IDictionary.GetEnumerator();

                while (_Enumerator.MoveNext())
                {
                    if (myFunc(_Enumerator.Current))
                        _IDictionary.Add(_Enumerator.Current.Key, _Enumerator.Current.Value);
                }

                return _IDictionary;

            }
        }

        #endregion


        #region GetEnumerator()

        protected virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            lock (this)
            {
                return _IDictionary.GetEnumerator();
            }
        }

        //protected override System.Collections.IEnumerator GetEnumerator()
        //{
        //    lock (this)
        //    {
        //        return _IndexHashTable.GetEnumerator();
        //    }
        //}        

        #endregion

        #region GetEnumerator(myFunc)

        protected virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc)
        {
            lock (this)
            {

                var _Enumerator = _IDictionary.GetEnumerator();

                while (_Enumerator.MoveNext())
                {
                    if (myFunc(_Enumerator.Current))
                        yield return _Enumerator.Current;
                }

            }
        }

        #endregion

        #endregion

        #region Remove/Clear

        #region Remove(myKey)

        protected virtual Boolean Remove(TKey myKey)
        {
            lock (this)
            {
                return _IDictionary.Remove(myKey);
            }
        }

        #endregion

        #region Remove(myKey, myValue)

        protected virtual Boolean Remove(TKey myKey, TValue myValue)
        {
            lock (this)
            {

                TValue _Value;

                var Success = _IDictionary.TryGetValue(myKey, out _Value);

                if (Success && myValue.Equals(_Value))
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

                var _Success    = true;
                var _Enumerator = _IDictionary.GetEnumerator();

                while (_Enumerator.MoveNext())
                {
                    if (myFunc(_Enumerator.Current))
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


    }

}