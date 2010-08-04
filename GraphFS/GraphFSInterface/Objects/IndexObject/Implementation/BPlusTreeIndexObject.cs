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
 * ABStarTreeIndexObject
 * Achim Friedland, 2009
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
using sones.Lib.DataStructures.BPlusTree;

#endregion

namespace sones.GraphFS.Objects
{

    public class BPlusTreeIndexObject
    {
        public const String Name = "BPlusTree";
    }

    /// <summary>
    /// An abstract implementation of a IndexObject to store a mapping TKey => Hashset&lt;TValue&gt; which may be embedded into other objects.
    /// </summary>
    /// <typeparam name="TKey">Must implement IComparable</typeparam>
    public class BPlusTreeIndexObject<TKey, TValue> : AFSObject, IIndexObject<TKey, TValue>
        where TKey : IComparable
    {


        #region Data

        protected IBPlusTree<TKey, TValue> _BPlusTree;

        #endregion

        
        #region Constructors

        #region BStarTreeIndexObject()

        public BPlusTreeIndexObject()
        {

            // Members of APandoraStructure
            _StructureVersion       = 1;

            // Members of APandoraObject
            _ObjectStream           = FSConstants.DEFAULT_INDEXSTREAM;

            _BPlusTree = new BPlusTree<TKey, TValue>();

        }

        #endregion

        #region BStarTreeIndexObject(myObjectLocation, mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm)

        /// <summary>
        /// A constructor used for fast deserializing
        /// </summary>
        /// <param name="mySerializedData">A bunch of bytes[] containing the serialized IndexObject</param>
        public BPlusTreeIndexObject(String myObjectLocation, Byte[] mySerializedData, IIntegrityCheck myIntegrityCheckAlgorithm, ISymmetricEncryption myEncryptionAlgorithm)
            : this()
        {

            if (mySerializedData == null || mySerializedData.Length == 0)
                throw new ArgumentNullException("mySerializedData must not be null or its length be zero!");

            Deserialize(mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm, false);

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

                mySerializationWriter.WriteUInt64(myNotificationHandling);

                #endregion
                

                #region Write Data

                mySerializationWriter.WriteUInt64((UInt64)_BPlusTree.Count());

                foreach (var keyValuePair in _BPlusTree)
                {
                    mySerializationWriter.WriteObject(keyValuePair.Key);
                    mySerializationWriter.WriteObject(keyValuePair.Value);
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

                UInt64 IndexHashTableNrOfEntries = mySerializationReader.ReadUInt64();
                Object  KeyObject;
                Object  ValueObject;

                #endregion

                #region Read Data

                for (UInt64 i=0; i < IndexHashTableNrOfEntries; i++)
                {
                    
                    KeyObject = mySerializationReader.ReadObject();

                    UInt64 IndexHashTableNrOfValues = mySerializationReader.ReadUInt64();

                    for (UInt64 k=0; k < IndexHashTableNrOfValues; k++)
                    {   
                         ValueObject = mySerializationReader.ReadObject();
                         _BPlusTree.Add((TKey)KeyObject, (TValue)ValueObject);
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

        public String IndexName { get { return BPlusTreeIndexObject.Name; } }

        #endregion

        #region GetNewInstance()

        public IIndexObject<TKey, TValue> GetNewInstance()
        {
            return new BPlusTreeIndexObject<TKey, TValue>();
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
            lock (this)
            {
                _BPlusTree.Set(myKey, myValue, myIndexSetStrategy);
            }
        }

        #endregion

        #region Set(myKey, myValues, myIndexSetStrategy)

        public virtual void Set(TKey myKey, IEnumerable<TValue> myValues, IndexSetStrategy myIndexSetStrategy)
        {
            lock (this)
            {
                _BPlusTree.Set(myKey, myValues, myIndexSetStrategy);
            }
        }

        #endregion

        #region Set(myKeyValuePairs, myIndexSetStrategy)

        public void Set(IEnumerable<KeyValuePair<TKey, TValue>> myKeyValuePairs, IndexSetStrategy myIndexSetStrategy)
        {
            lock (this)
            {
                _BPlusTree.Set(myKeyValuePairs, myIndexSetStrategy);
            }
        }

        #endregion

        #region Set(myKeyValuePair, myIndexSetStrategy)

        public virtual void Set(KeyValuePair<TKey, TValue> myKeyValuePair, IndexSetStrategy myIndexSetStrategy)
        {
            lock (this)
            {
                _BPlusTree.Set(myKeyValuePair, myIndexSetStrategy);
            }
        }

        #endregion

        #region Set(myKeyValuesPair, myIndexSetStrategy)

        public virtual void Set(KeyValuePair<TKey, IEnumerable<TValue>> myKeyValuesPair, IndexSetStrategy myIndexSetStrategy)
        {
            lock (this)
            {
                _BPlusTree.Set(myKeyValuesPair, myIndexSetStrategy);
            }
        }

        #endregion

        #region Set(myDictionary, myIndexSetStrategy)

        public virtual void Set(Dictionary<TKey, TValue> myDictionary, IndexSetStrategy myIndexSetStrategy)
        {
            lock (this)
            {
                _BPlusTree.Set(myDictionary, myIndexSetStrategy);
            }
        }

        #endregion

        #region Set(myMultiValueDictionary, myIndexSetStrategy)

        public virtual void Set(Dictionary<TKey, IEnumerable<TValue>> myMultiValueDictionary, IndexSetStrategy myIndexSetStrategy)
        {
            lock (this)
            {
                _BPlusTree.Set(myMultiValueDictionary, myIndexSetStrategy);
            }
        }

        #endregion

        #endregion

        #region Contains

        #region ContainsKey(myKey)

        public virtual Trinary ContainsKey(TKey myKey)
        {
            return _BPlusTree.ContainsKey(myKey);
        }

        #endregion

        #region ContainsValue(myValue)

        public virtual Trinary ContainsValue(TValue myValue)
        {
            return _BPlusTree.ContainsValue(myValue);
        }

        #endregion

        #region Contains(myKey, myValue)

        public virtual Trinary Contains(TKey myKey, TValue myValue)
        {
            return _BPlusTree.Contains(myKey, myValue);           
        }

        #endregion

        #region Contains(myFunc)

        public virtual Trinary Contains(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
        {
            lock (this)
            {
                return _BPlusTree.Contains(myFunc);
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
                HashSet<TValue> result;

                if (_BPlusTree.TryGetValue(myKey, out result))
                {
                    return result;
                }
                else
                {
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

        public virtual Boolean TryGetValue(TKey myKey, out HashSet<TValue> myValue)
        {
            return _BPlusTree.TryGetValue(myKey, out myValue);
        }

        #endregion


        #region Keys()

        public virtual IEnumerable<TKey> Keys()
        {
            return _BPlusTree.Keys();
        }

        #endregion

        #region Keys(myFunc)

        public virtual IEnumerable<TKey> Keys(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
        {
            lock (this)
            {
                return ((IIndexObject<TKey, TValue>)_BPlusTree).Keys(myFunc);
            }
        }

        #endregion

        #region KeyCount()

        public virtual UInt64 KeyCount()
        {
            return (UInt64)_BPlusTree.KeyCount();
        }

        #endregion

        #region KeyCount(myFunc)

        public virtual UInt64 KeyCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
        {
            lock (this)
            {
                return ((IIndexInterface<TKey, TValue>)_BPlusTree).KeyCount(myFunc);
            }
        }

        #endregion


        #region Values()

        public virtual IEnumerable<HashSet<TValue>> Values()
        {
            return ((IIndexInterface<TKey,TValue>)_BPlusTree).Values();
        }

        #endregion

        #region Values(myFunc)

        public virtual IEnumerable<HashSet<TValue>> Values(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
        {
            lock (this)
            {
                throw new NotImplementedException();
                //var _Enumerator = _BStarTree.GetEnumerator();

                //while (_Enumerator.MoveNext())
                //{
                //    if (myFunc(new KeyValuePair<TKey, IEnumerable<TValue>>(_Enumerator.Current.Key, _Enumerator.Current.Value)))
                //        yield return _Enumerator.Current.Value;
                //}

            }
        }

        #endregion

        public IEnumerable<TValue> GetValues()
        {
            return _BPlusTree.GetValues();
        }

        #region ValueCount()

        public virtual UInt64 ValueCount()
        {
            return (UInt64)_BPlusTree.Count();
        }

        #endregion

        #region ValueCount(myFunc)

        public virtual UInt64 ValueCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
        {
            lock (this)
            {
                return _BPlusTree.ValueCount(myFunc);
            }
        }

        #endregion


        #region GetIDictionary()

        public virtual IDictionary<TKey, HashSet<TValue>> GetIDictionary()
        {
            lock (this)
            {
                return _BPlusTree.GetIDictionary();
            }
        }

        #endregion

        #region GetIDictionary(myFunc)

        public virtual IDictionary<TKey, HashSet<TValue>> GetIDictionary(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
        {
            lock (this)
            {
                return _BPlusTree.GetIDictionary(myFunc);
            }
        }

        #endregion

        #region GetEnumerator()

        public virtual IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator()
        {
            lock (this)
            {
                return _BPlusTree.GetEnumerator();
            }
        }

        #endregion

        #region GetEnumerator(myFunc)

        public virtual IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
        {
            lock (this)
            {
                return _BPlusTree.GetEnumerator(myFunc);
            }
        }

        #endregion

        #endregion

        #region Remove/Clear

        #region Remove(myKey)

        public virtual Boolean Remove(TKey myKey)
        {
            lock (this)
            {
                return _BPlusTree.Remove(myKey);
            }
        }

        #endregion

        #region Remove(myKey, myValue)

        public virtual Boolean Remove(TKey myKey, TValue myValue)
        {
            lock (this)
            {
                return _BPlusTree.Remove(myKey, myValue);
            }
        }

        #endregion

        #region Remove(myFunc)

        public virtual Boolean Remove(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
        {
            lock (this)
            {
                return _BPlusTree.Remove(myFunc);
            }
        }

        #endregion

        #region Clear()

        public virtual void Clear()
        {
            _BPlusTree.Clear();
        }

        #endregion

        #endregion

        #endregion


        public override AFSObject Clone()
        {
            throw new NotImplementedException();
        }


        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (this)
            {
                return _BPlusTree.GetEnumerator();
            }
        }

        #endregion


        public IEnumerable<TValue> GreaterThan(TKey myKey, bool myOrEqual = true)
        {
            lock (this)
            {
                return _BPlusTree.GreaterThan(myKey, myOrEqual);
            }
        }

        public IEnumerable<TValue> GreaterThan(TKey myKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, bool myOrEqual = true)
        {
            lock (this)
            {
                return _BPlusTree.GreaterThan(myKey, myFunc, myOrEqual);
            }
        }

        public IEnumerable<TValue> LowerThan(TKey myKey, bool myOrEqual = true)
        {
            lock (this)
            {
                return _BPlusTree.LowerThan(myKey, myOrEqual);
            }
        }

        public IEnumerable<TValue> LowerThan(TKey myKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, bool myOrEqual = true)
        {
            lock (this)
            {
                return _BPlusTree.LowerThan(myKey, myFunc, myOrEqual);
            }
        }

        public IEnumerable<TValue> InRange(TKey myFromKey, TKey myToKey, bool myOrEqualFromKey = true, bool myOrEqualToKey = true)
        {
            lock (this)
            {
                return _BPlusTree.InRange(myFromKey, myToKey, myOrEqualFromKey, myOrEqualToKey);
            }
        }

        public IEnumerable<TValue> InRange(TKey myFromKey, TKey myToKey, Func<KeyValuePair<TKey, IEnumerable<TValue>>, bool> myFunc, bool myOrEqualFromKey = true, bool myOrEqualToKey = true)
        {
            lock (this)
            {
                return _BPlusTree.InRange(myFromKey, myToKey, myFunc, myOrEqualFromKey, myOrEqualToKey);
            }
        }

    }

}
