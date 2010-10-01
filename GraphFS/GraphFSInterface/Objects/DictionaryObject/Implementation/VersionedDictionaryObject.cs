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

/* GraphFS - VersionedDictionaryObject
 * (c) Achim Friedland, 2009
 * 
 * Lead programmer:
 *      Achim Friedland
 * 
 * */

#region Usings

using System;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using sones.Lib.Cryptography.IntegrityCheck;
using sones.Lib.Cryptography.SymmetricEncryption;
using sones.Lib.DataStructures;
using sones.Lib.DataStructures.Timestamp;
using sones.Lib.DataStructures.Indices;
using sones.Lib;

#endregion

namespace sones.GraphFS.Objects
{

    /// <summary>
    /// An implementation of a DictionaryObject to store a versioned mapping TKey => TValue.
    /// The internal DictionaryValueHistory&lt;TValue&gt; datastructure will keep up to $historysize older versions of TValue.
    /// </summary>
    /// <typeparam name="TKey">Must implement IComparable</typeparam>

    

    public class VersionedDictionaryObject<TKey, TValue> : AVersionedDictionaryObject<TKey, TValue>, IVersionedDictionaryObject<TKey, TValue>, IDictionaryObject<TKey, TValue>//, IQueryable<KeyValuePair<TKey, TValue>>
        where TKey : IComparable
        where TValue : IEstimable
    {


        #region Properties

        #region HistorySize

        public new UInt64 HistorySize
        {

            get
            {
                return base.HistorySize;
            }

            set
            {
                base.HistorySize = value;
            }

        }

        #endregion

        #endregion


        #region Constructor

        #region VersionedDictionaryObject()

        /// <summary>
        /// This will create an empty VersionedDictionaryObject using a Dictionary&lt;TKey, TValue&gt; for the internal IDictionary&lt;TKey, DictionaryValueHistory&lt;TValue&gt;&gt; object.
        /// The size of the value history will be set to 3.
        /// </summary>
        public VersionedDictionaryObject()
            : base ()
        {
        }

        #endregion

        #region VersionedDictionaryObject(myIDictionaryObject)

        /// <summary>
        /// This will create an empty VersionedDictionaryObject using the given IDictionary object for the internal IDictionary&lt;TKey, DictionaryValueHistory&lt;TValue&gt;&gt; object.
        /// The size of the value history will be set to 3.
        /// </summary>
        public VersionedDictionaryObject(IDictionary<TKey, TValue> myIDictionaryType)
            : base (myIDictionaryType)
        {
        }

        #endregion

        #region VersionedDictionaryObject(myObjectLocation, mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm)

        /// <summary>
        /// A constructor used for fast deserializing
        /// </summary>
        /// <param name="mySerializedData">A bunch of bytes[] containing the serialized VersionedDictionaryObject</param>
        public VersionedDictionaryObject(String myObjectLocation, Byte[] mySerializedData, IIntegrityCheck myIntegrityCheckAlgorithm, ISymmetricEncryption myEncryptionAlgorithm)
        {

            if (mySerializedData == null || mySerializedData.Length == 0)
                throw new ArgumentNullException("mySerializedData must not be null or its length be zero!");

            Deserialize(mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm, false);

        }

        #endregion

        #endregion


        #region Members of AGraphObject

        #region Clone()

        public override AFSObject Clone()
        {

            var newT = new VersionedDictionaryObject<TKey, TValue>();
            newT.Deserialize(Serialize(null, null, false), null, null, this);

            return newT;

        }

        #endregion

        #endregion


        #region Members of IDictionaryObject<TKey, TValue>

        #region Add - will fail if the key already exists

        #region Add(myKey, myValue)

        /// <summary>
        /// Adds mykey and myValue, but fails if the key already exists.
        /// </summary>
        /// <param name="myKey">the key to access the value</param>
        /// <param name="myValue">the value storing some information</param>
        /// <returns>0 if it failed, 1 for a success</returns>
        public new UInt64 Add(TKey myKey, TValue myValue)
        {
            return base.Add(myKey, myValue);
        }

        #endregion

        #region Add(myKeyValuePair)

        /// <summary>
        /// Adds the given key-value-pair, but fails if the key already exists.
        /// </summary>
        /// <param name="myKeyValuePair">a key-value-pair</param>
        /// <returns>0 if it failed, 1 for a success</returns>
        public new UInt64 Add(KeyValuePair<TKey, TValue> myKeyValuePair)
        {
            return base.Add(myKeyValuePair);
        }

        #endregion

        #region Add(myKeyValuePairs)

        /// <summary>
        /// Adds all given key-value-pairs in one atomic operation.
        /// Will fail if any key already exists.
        /// </summary>
        /// <param name="myKeyValuePairs">An IEnumerable of Key-Value-Pairs</param>
        /// <returns>1 for a success or 0 if any key already exists.</returns>
        public new UInt64 Add(IEnumerable<KeyValuePair<TKey, TValue>> myKeyValuePairs)
        {
            return base.Add(myKeyValuePairs);
        }

        #endregion

        #region Add(myDictionary)

        /// <summary>
        /// Adds all key-value-pairs of the given IDictionary in one atomic operation.
        /// Will fail if any key already exists.
        /// </summary>
        /// <param name="myIDictionary">An IDictionary</param>
        /// <returns>1 for a success or 0 if any key already exists.</returns>
        public new UInt64 Add(IDictionary<TKey, TValue> myIDictionary)
        {
            return base.Add(myIDictionary);
        }

        #endregion

        #endregion

        #region Set - will add a value or overwrite an existing value

        #region Set(myKey, myValue)

        /// <summary>
        /// Adds mykey and myValue. Will overwrite an existing value.
        /// </summary>
        /// <param name="myKey">the key to access the value</param>
        /// <param name="myValue">the value storing some information</param>
        /// <returns>0 if it failed, 1 for a success</returns>
        public new UInt64 Set(TKey myKey, TValue myValue)
        {
            return base.Set(myKey, myValue);
        }

        #endregion

        #region Set(myKeyValuePair)

        /// <summary>
        /// Adds the given key-value-pair. Will overwrite an existing value.
        /// </summary>
        /// <param name="myKeyValuePair">a key-value-pair</param>
        /// <returns>0 if it failed, 1 for a success</returns>
        public new UInt64 Set(KeyValuePair<TKey, TValue> myKeyValuePair)
        {
            return base.Add(myKeyValuePair);
        }

        #endregion

        #region Set(myKeyValuePairs, myIndexSetStrategy)

        public new UInt64 Set(IEnumerable<KeyValuePair<TKey, TValue>> myKeyValuePairs, IndexSetStrategy myIndexSetStrategy)
        {
            return base.Set(myKeyValuePairs, myIndexSetStrategy);
        }

        #endregion

        #region Set(myDictionary, myIndexSetStrategy)

        public new UInt64 Set(Dictionary<TKey, TValue> myDictionary, IndexSetStrategy myIndexSetStrategy)
        {
            return base.Set(myDictionary, myIndexSetStrategy);
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
        public new UInt64 Replace(TKey myKey, TValue myOldValue, TValue myNewValue)
        {
            return base.Replace(myKey, myOldValue, myNewValue);
        }

        #endregion

        #endregion

        #region Contains

        #region ContainsKey(myKey)

        public new Trinary ContainsKey(TKey myKey)
        {
            return base.ContainsKey(myKey);
        }

        #endregion

        #region ContainsValue(myValue)

        public new Trinary ContainsValue(TValue myValue)
        {
            return base.ContainsValue(myValue);
        }

        #endregion

        #region Contains(myKey, myValue)

        public new Trinary Contains(TKey myKey, TValue myValue)
        {
            return base.Contains(myKey, myValue);
        }

        #endregion

        #region Contains(myFunc)

        public new Trinary Contains(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc)
        {
            return base.Contains(myFunc);
        }

        #endregion

        #endregion

        #region Get/Keys/Values/Enumerator

        #region this[myKey]

        public new TValue this[TKey myKey]
        {

            get
            {
                return base[myKey];
            }

            set
            {
                base[myKey] = value;
            }

        }

        #endregion

        #region TryGetValue(myKey, out myValue)

        public new Boolean TryGetValue(TKey myKey, out TValue myValue)
        {
            return base.TryGetValue(myKey, out myValue);
        }

        #endregion


        #region Keys()

        public new IEnumerable<TKey> Keys()
        {
            return base.Keys();
        }

        #endregion

        #region Keys(myFunc)

        public new IEnumerable<TKey> Keys(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc)
        {
            return base.Keys(myFunc);
        }

        #endregion

        #region KeyCount()

        public new UInt64 KeyCount()
        {
            return base.KeyCount();
        }

        #endregion

        #region KeyCount(myFunc)

        public new UInt64 KeyCount(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc)
        {
            return base.KeyCount(myFunc);
        }

        #endregion


        #region Values()

        public new IEnumerable<TValue> Values()
        {
            return base.Values();
        }

        #endregion

        #region Values(myFunc)

        public new IEnumerable<TValue> Values(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc)
        {
            return base.Values(myFunc);
        }

        #endregion

        #region ValueCount()

        public new UInt64 ValueCount()
        {
            return base.ValueCount();
        }

        #endregion

        #region ValueCount(myFunc)

        public new UInt64 ValueCount(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc)
        {
            return base.ValueCount(myFunc);
        }

        #endregion


        #region GetIDictionary()

        public new IDictionary<TKey, TValue> GetIDictionary()
        {
            return base.GetIDictionary();
        }

        #endregion

        #region GetIDictionary(myKeys)

        public new IDictionary<TKey, TValue> GetIDictionary(params TKey[] myKey)
        {
            return base.GetIDictionary(myKey);
        }

        #endregion

        #region GetIDictionary(myFunc)

        public new IDictionary<TKey, TValue> GetIDictionary(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc)
        {
            return base.GetIDictionary(myFunc);
        }

        #endregion


        #region GetEnumerator()

        public new IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return base.GetEnumerator();
        }

        //public new System.Collections.IEnumerator GetEnumerator()
        //{
        //    return base.GetEnumerator();
        //}

        #endregion

        #region GetEnumerator(myFunc)

        public new IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc)
        {
            return base.GetEnumerator(myFunc);
        }

        #endregion

        #endregion

        #region Remove/Clear

        #region Remove(TKey)

        public new Boolean Remove(TKey myKey)
        {
            return base.Remove(myKey);
        }

        #endregion

        #region Remove(myKey, myValue)

        public new Boolean Remove(TKey myKey, TValue myValue)
        {
            return base.Remove(myKey, myValue);
        }

        #endregion

        #region Remove(myFunc)

        public new Boolean Remove(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc)
        {
            return base.Remove(myFunc);
        }

        #endregion

        #region Clear()

        public new void Clear()
        {
            base.Clear();
        }

        #endregion

        #endregion

        #endregion


        #region Members of IVersionedDictionaryObject<TKey, TValue>

        #region Replace - will fail if the key does not exist or the given timestamp != actual timestamp

        /// <summary>
        /// Replaces the value indexed by myKey with myNewValue as long as the given timestamp matches the actual timestamp.
        /// Will fail if the key is not existent or the actual timestamp is not equals myTimestamp due to concurrency conflicts.
        /// </summary>
        /// <param name="myKey">the key to access the value</param>
        /// <param name="myValue">the value storing some information</param>
        /// <returns>0 if it failed or the timestamp of the replace operation</returns>
        public new UInt64 ReplaceByTimestamp(TKey myKey, UInt64 myTimestamp, TValue myNewValue)
        {
            return base.ReplaceByTimestamp(myKey, myTimestamp, myNewValue);
        }

        #endregion

        #region Contains

        #region ContainsKey(myKey, myVersion)

        public new Trinary ContainsKey(TKey myKey, Int64 myVersion)
        {
            return base.ContainsKey(myKey, myVersion);
        }

        #endregion

        #region ContainsValue(myValue, myVersion)

        public new Trinary ContainsValue(TValue myValue, Int64 myVersion)
        {
            return base.ContainsValue(myValue, myVersion);
        }

        #endregion

        #region Contains(myKey, myValue, myVersion)

        public new Trinary Contains(TKey myKey, TValue myValue, Int64 myVersion)
        {
            return base.Contains(myKey, myValue, myVersion);
        }

        #endregion

        #region Contains(myFunc, myVersion)

        public new Trinary Contains(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc, Int64 myVersion)
        {
            return base.Contains(myFunc, myVersion);
        }

        #endregion

        #endregion

        #region Get/Keys/Values/Enumerator

        #region this[myKey, myVersion]  // Int64

        public new TValue this[TKey myKey, Int64 myVersion]
        {

            get
            {
                return base[myKey, myVersion];
            }

            set
            {
                base[myKey] = value;
            }

        }

        #endregion

        #region this[myKey, myVersion]  // UInt64

        public new TValue this[TKey myKey, UInt64 myVersion]
        {

            get
            {
                return base[myKey, myVersion];
            }

            set
            {
                base[myKey] = value;
            }

        }

        #endregion

        #region TryGetValue(myKey, out myValue, myVersion)

        public new Boolean TryGetValue(TKey myKey, out TValue myValue, Int64 myVersion)
        {
            return base.TryGetValue(myKey, out myValue, myVersion);
        }

        #endregion


        #region Keys(myVersion)

        public new IEnumerable<TKey> Keys(Int64 myVersion)
        {
            return base.Keys(myVersion);
        }

        #endregion

        #region Keys(myFunc, myVersion)

        public new IEnumerable<TKey> Keys(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc, Int64 myVersion)
        {
            return base.Keys(myFunc, myVersion);
        }

        #endregion

        #region KeyCount(myVersion)

        public new UInt64 KeyCount(Int64 myVersion)
        {
            return base.KeyCount(myVersion);
        }

        #endregion

        #region KeyCount(myFunc, myVersion)

        public new UInt64 KeyCount(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc, Int64 myVersion)
        {
            return base.KeyCount(myFunc, myVersion);
        }

        #endregion


        #region Values(myVersion)

        public new IEnumerable<TValue> Values(Int64 myVersion)
        {
            return base.Values(myVersion);
        }

        #endregion

        #region Values(myFunc, myVersion)

        public new IEnumerable<TValue> Values(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc, Int64 myVersion)
        {
            return base.Values(myFunc, myVersion);
        }

        #endregion

        #region ValueCount(myVersion)

        public new UInt64 ValueCount(Int64 myVersion)
        {
            return base.ValueCount(myVersion);
        }

        #endregion

        #region ValueCount(myFunc, myVersion)

        public new UInt64 ValueCount(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc, Int64 myVersion)
        {
            return base.ValueCount(myFunc, myVersion);
        }

        #endregion


        #region GetIDictionary(myVersion)

        public new IDictionary<TKey, TValue> GetIDictionary(Int64 myVersion)
        {
            return base.GetIDictionary(myVersion);
        }

        #endregion

        #region GetIDictionary(myVersion, myKeys)

        public new IDictionary<TKey, TValue> GetIDictionary(Int64 myVersion, params TKey[] myKey)
        {
            return base.GetIDictionary(myVersion, myKey);
        }

        #endregion

        #region GetIDictionary(myFunc, myVersion)

        public new IDictionary<TKey, TValue> GetIDictionary(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc, Int64 myVersion)
        {
            return base.GetIDictionary(myFunc, myVersion);
        }

        #endregion


        #region GetEnumerator(myVersion)

        public new IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator(Int64 myVersion)
        {
            return base.GetEnumerator(myVersion);
        }

        #endregion

        #region GetEnumerator(myFunc, myVersion)

        public new IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator(Func<KeyValuePair<TKey, TValue>, Boolean> myFunc, Int64 myVersion)
        {
            return base.GetEnumerator(myFunc, myVersion);
        }

        #endregion

        #endregion

        #region Additional methods

        #region GetTimestampValuePair(myKey)

        public new TimestampValuePair<TValue> GetTimestampValuePair(TKey myKey)
        {
            return base.GetTimestampValuePair(myKey);
        }

        #endregion

        #region VersionCount(myKey)

        public new UInt64 VersionCount(TKey myKey)
        {
            return base.VersionCount(myKey);
        }

        #endregion

        #region ClearHistory(myKey)

        /// <summary>
        /// Clears the history information of the given key
        /// </summary>
        public new void ClearHistory(TKey myKey)
        {
            base.ClearHistory(myKey);
        }

        #endregion

        #endregion

        #endregion

        
        //#region IQueryable Members

        //public Type ElementType
        //{
        //    get
        //    {
        //        return typeof(KeyValuePair<TKey, TValue>);
        //    }
        //}

        //public System.Linq.Expressions.Expression Expression
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public IQueryProvider Provider
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //#endregion


        //#region IEnumerable Members

        //System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        //{
        //    throw new NotImplementedException();
        //}

        //#endregion

        #region IEstimable Members

        public override ulong GetEstimatedSize()
        {
            return EstimatedSizeConstants.UndefinedObjectSize;
        }

        #endregion

    }

}
