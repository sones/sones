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

///* GraphFS - VersionedIndexObject
// * (c) Achim Friedland, 2009
// * 
// * Lead programmer:
// *      Achim Friedland
// * 
// * */

//#region Usings

//using System;
//using System.Text;
//using System.Collections.Generic;

//using sones.Lib.Cryptography.IntegrityCheck;
//using sones.Lib.Cryptography.SymmetricEncryption;
//using sones.Lib.DataStructures;
//using sones.GraphFS.DataStructures;
//using sones.Lib.DataStructures.Indices;

//#endregion

//namespace sones.GraphFS.Objects
//{

//    public class VersionedIndexObject
//    {
//        public const String Name = "VERSIONEDHASHTABLE";
//    }

//    /// <summary>
//    /// An implementation of a DictionaryObject to store a versioned mapping TKey => HashSet&lt;TValue&gt;.
//    /// The internal IndexValueHistoryList&lt;TValue&gt; datastructure will keep up to $historysize older versions of HashSet&lt;TValue&gt;.
//    /// </summary>
//    /// <typeparam name="TKey">Must implement IComparable</typeparam>

//    public class VersionedIndexObject<TKey, TValue> : AVersionedIndexObject<TKey, TValue>, IVersionedIndexObject<TKey, TValue>
//        where TKey : IComparable
//    {


//        #region Properties

//        #region HistorySize

//        public new UInt64 HistorySize
//        {

//            get
//            {
//                return base.HistorySize;
//            }

//            set
//            {
//                base.HistorySize = value;
//            }

//        }

//        #endregion

//        #endregion


//        #region Constructor

//        #region VersionedIndexObject()

//        /// <summary>
//        /// This will create an empty VersionedIndexObject using a Dictionary&lt;TKey, TValue&gt; for the internal IDictionary&lt;TKey, IndexValueHistoryList&lt;TValue&gt;&gt; object.
//        /// The size of the value history will be set to 3.
//        /// </summary>
//        public VersionedIndexObject()
//            : base()
//        {
//        }

//        #endregion

//        #region VersionedIndexObject(myIDictionaryObject)

//        /// <summary>
//        /// This will create an empty VersionedIndexObject using the given dictionary type for the internal IDictionary&lt;TKey, IndexValueHistoryList&lt;TValue&gt;&gt;.
//        /// The size of the value history will be set to 3.
//        /// </summary>
//        public VersionedIndexObject(IDictionary<TKey, TValue> myIDictionaryObject)
//        //            : base(myIDictionaryObject)
//        {
//        }

//        #endregion

//        #region VersionedIndexObject(myObjectLocation, mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm)

//        /// <summary>
//        /// A constructor used for fast deserializing
//        /// </summary>
//        /// <param name="mySerializedData">A bunch of bytes[] containing the serialized VersionedIndexObject</param>
//        public VersionedIndexObject(ObjectLocation myObjectLocation, Byte[] mySerializedData, IIntegrityCheck myIntegrityCheckAlgorithm, ISymmetricEncryption myEncryptionAlgorithm)
//            : this()
//        {

//            if (mySerializedData == null || mySerializedData.Length == 0)
//                throw new ArgumentNullException("mySerializedData must not be null or its length be zero!");

//            Deserialize(mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm, false);

//        }

//        #endregion

//        #endregion


//        #region Members of AGraphObject

//        #region Clone()

//        public override AFSObject Clone()
//        {

//            var newT = new VersionedIndexObject<TKey, TValue>();
//            newT.Deserialize(Serialize(null, null, false), null, null, this);

//            return newT;

//        }

//        #endregion

//        #endregion


//        #region Members of IIndexObject<TKey, TValue>

//        #region Index handling

//        public string IndexName
//        {
//            get { return VersionedIndexObject.Name; }
//        }

//        public IIndexObject<TKey, TValue> GetNewInstance()
//        {
//            return new BPlusTreeIndexObject<TKey, TValue>();
//        }

//        #endregion

//        #region Add

//        #region Add(myKey, myValue)

//        public new void Add(TKey myKey, TValue myValue)
//        {
//            base.Add(myKey, myValue);
//        }

//        #endregion

//        #region Add(myKey, myValues)

//        public new void Add(TKey myKey, IEnumerable<TValue> myValues)
//        {
//            base.Add(myKey, myValues);
//        }

//        #endregion

//        #region Add(myKeyValuePair)

//        public new void Add(KeyValuePair<TKey, TValue> myKeyValuePair)
//        {
//            base.Add(myKeyValuePair);
//        }

//        #endregion

//        #region Add(myKeyValuesPair)

//        public new void Add(KeyValuePair<TKey, IEnumerable<TValue>> myKeyValuesPair)
//        {
//            base.Add(myKeyValuesPair);
//        }

//        #endregion

//        #region Add(myDictionary)

//        public new void Add(Dictionary<TKey, TValue> myDictionary)
//        {
//            base.Add(myDictionary);
//        }

//        #endregion

//        #region Add(myMultiValueDictionary)

//        public new void Add(Dictionary<TKey, IEnumerable<TValue>> myMultiValueDictionary)
//        {
//            base.Add(myMultiValueDictionary);
//        }

//        #endregion

//        #endregion

//        #region Set

//        #region Set(myKey, myValue, myIndexSetStrategy)

//        public new void Set(TKey myKey, TValue myValue, IndexSetStrategy myIndexSetStrategy)
//        {
//            base.Set(myKey, myValue, myIndexSetStrategy);
//        }

//        #endregion

//        #region Set(myKey, myValues, myIndexSetStrategy)

//        public new void Set(TKey myKey, IEnumerable<TValue> myValues, IndexSetStrategy myIndexSetStrategy)
//        {
//            base.Set(myKey, myValues, myIndexSetStrategy);
//        }

//        #endregion

//        #region Set(myKeyValuePair, myIndexSetStrategy)

//        public new void Set(KeyValuePair<TKey, TValue> myKeyValuePair, IndexSetStrategy myIndexSetStrategy)
//        {
//            Set(myKeyValuePair.Key, myKeyValuePair.Value, myIndexSetStrategy);
//        }

//        #endregion

//        #region Set(myKeyValuesPair, myIndexSetStrategy)

//        public new void Set(KeyValuePair<TKey, IEnumerable<TValue>> myKeyValuesPair, IndexSetStrategy myIndexSetStrategy)
//        {
//            base.Set(myKeyValuesPair, myIndexSetStrategy);
//        }

//        #endregion

//        #region Set(myKeyValuePairs, myIndexSetStrategy)

//        public new void Set(IEnumerable<KeyValuePair<TKey, TValue>> myKeyValuePairs, IndexSetStrategy myIndexSetStrategy)
//        {
//            base.Set(myKeyValuePairs, myIndexSetStrategy);
//        }

//        #endregion

//        #region Set(myDictionary, myIndexSetStrategy)

//        public new void Set(Dictionary<TKey, TValue> myDictionary, IndexSetStrategy myIndexSetStrategy)
//        {
//            base.Set(myDictionary, myIndexSetStrategy);
//        }

//        #endregion

//        #region Set(myMultiValueDictionary, myIndexSetStrategy)

//        public new void Set(Dictionary<TKey, IEnumerable<TValue>> myMultiValueDictionary, IndexSetStrategy myIndexSetStrategy)
//        {
//            base.Set(myMultiValueDictionary, myIndexSetStrategy);
//        }

//        #endregion

//        #endregion

//        #region Contains

//        #region ContainsKey(myKey)

//        public new Trinary ContainsKey(TKey myKey)
//        {
//            return base.ContainsKey(myKey);
//        }

//        #endregion

//        #region ContainsValue(myValue)

//        public new Trinary ContainsValue(TValue myValue)
//        {
//            return base.ContainsValue(myValue);
//        }

//        #endregion

//        #region Contains(myKey, myValue)

//        public new Trinary Contains(TKey myKey, TValue myValue)
//        {
//            return base.Contains(myKey, myValue);
//        }

//        #endregion

//        #region Contains(myFunc)

//        public new Trinary Contains(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
//        {
//            return base.Contains(myFunc);
//        }

//        #endregion

//        #endregion

//        #region Get/Keys/Values/Enumerator

//        #region this[myKey]

//        public new HashSet<TValue> this[TKey myKey]
//        {

//            get
//            {
//                return base[myKey];
//            }

//            set
//            {
//                base[myKey] = value;
//            }

//        }

//        #endregion

//        #region TryGetValue(myKey, out myValue)

//        public new Boolean TryGetValue(TKey myKey, out HashSet<TValue> myValue)
//        {
//            return base.TryGetValue(myKey, out myValue);
//        }

//        #endregion


//        #region Keys()

//        public new IEnumerable<TKey> Keys()
//        {
//            return base.Keys();
//        }

//        #endregion

//        #region Keys(myFunc)

//        public new IEnumerable<TKey> Keys(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
//        {
//            return base.Keys(myFunc);
//        }

//        #endregion

//        #region KeyCount()

//        public new UInt64 KeyCount()
//        {
//            return base.KeyCount();
//        }

//        #endregion

//        #region KeyCount(myFunc)

//        public new UInt64 KeyCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
//        {
//            return base.KeyCount(myFunc);
//        }

//        #endregion


//        #region Values()

//        public new IEnumerable<HashSet<TValue>> Values()
//        {
//            return base.Values();
//        }

//        #endregion

//        #region Values(myFunc)

//        public new IEnumerable<HashSet<TValue>> Values(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
//        {
//            return base.Values(myFunc);
//        }

//        #endregion

//        #region ValueCount()

//        public new UInt64 ValueCount()
//        {
//            return base.ValueCount();
//        }

//        #endregion

//        #region ValueCount(myFunc)

//        public new UInt64 ValueCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
//        {
//            return base.ValueCount(myFunc);
//        }

//        #endregion

//        #region GetValues()

//        public IEnumerable<TValue> GetValues()
//        {
//            foreach (var hashset in base.Values())
//            {
//                foreach (var val in hashset)
//                {
//                    yield return val;
//                }
//            }
//        }

//        #endregion

//        #region GetIDictionary()

//        public new IDictionary<TKey, HashSet<TValue>> GetIDictionary()
//        {
//            return base.GetIDictionary();
//        }

//        #endregion

//        #region GetIDictionary(myFunc)

//        public new IDictionary<TKey, HashSet<TValue>> GetIDictionary(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
//        {
//            return base.GetIDictionary(myFunc);
//        }

//        #endregion


//        #region GetEnumerator()

//        public new IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator()
//        {
//            return base.GetEnumerator();
//        }

//        #endregion

//        #region GetEnumerator(myFunc)

//        public new IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
//        {
//            return base.GetEnumerator(myFunc);
//        }

//        #endregion

//        #endregion

//        #region Remove/Clear

//        #region Remove(myKey)

//        public new Boolean Remove(TKey myKey)
//        {
//            return base.Remove(myKey);
//        }

//        #endregion

//        #region Remove(myKey, myValue)

//        public new Boolean Remove(TKey myKey, TValue myValue)
//        {
//            return base.Remove(myKey, myValue);
//        }

//        #endregion

//        #region Remove(myFunc)

//        public new Boolean Remove(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc)
//        {
//            return base.Remove(myFunc);
//        }

//        #endregion

//        #region Clear()

//        public new void Clear()
//        {
//            base.Clear();
//        }

//        #endregion

//        #endregion

//        #endregion


//        #region IVersionedIndexObject<TKey,TValue> Members

//        #region Contains

//        #region ContainsKey(myKey, myVersion)

//        public new Trinary ContainsKey(TKey myKey, Int64 myVersion)
//        {
//            return base.ContainsKey(myKey, myVersion);
//        }

//        #endregion

//        #region ContainsValue(myValue, myVersion)

//        public new Trinary ContainsValue(TValue myValue, Int64 myVersion)
//        {
//            return base.ContainsValue(myValue, myVersion);
//        }

//        #endregion

//        #region Contains(myKey, myValue, myVersion)

//        public new Trinary Contains(TKey myKey, TValue myValue, Int64 myVersion)
//        {
//            return base.Contains(myKey, myValue, myVersion);
//        }

//        #endregion

//        #region Contains(myFunc, myVersion)

//        public new Trinary Contains(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion)
//        {
//            return base.Contains(myFunc, myVersion);
//        }

//        #endregion

//        #endregion

//        #region Get/Keys/Values/Enumerator

//        #region this[myKey, myVersion]  // Int64

//        public new HashSet<TValue> this[TKey myKey, Int64 myVersion]
//        {

//            get
//            {
//                return base[myKey, myVersion];
//            }

//        }

//        #endregion

//        #region this[myKey, myVersion]  // UInt64

//        public new HashSet<TValue> this[TKey myKey, UInt64 myVersion]
//        {

//            get
//            {
//                return base[myKey, myVersion];
//            }

//        }

//        #endregion

//        #region TryGetValue(myKey, out myValue, myVersion)

//        public new Boolean TryGetValue(TKey myKey, out HashSet<TValue> myValue, Int64 myVersion)
//        {
//            return base.TryGetValue(myKey, out myValue, myVersion);
//        }

//        #endregion


//        #region Keys(myVersion)

//        public new IEnumerable<TKey> Keys(Int64 myVersion)
//        {
//            return base.Keys(myVersion);
//        }

//        #endregion

//        #region Keys(myFunc, myVersion)

//        public new IEnumerable<TKey> Keys(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion)
//        {
//            return base.Keys(myFunc, myVersion);
//        }

//        #endregion

//        #region KeyCount(myVersion)

//        public new UInt64 KeyCount(Int64 myVersion)
//        {
//            return base.KeyCount(myVersion);
//        }

//        #endregion

//        #region KeyCount(myFunc, myVersion)

//        public new UInt64 KeyCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion)
//        {
//            return base.KeyCount(myFunc, myVersion);
//        }

//        #endregion


//        #region Values(myVersion)

//        public new IEnumerable<HashSet<TValue>> Values(Int64 myVersion)
//        {
//            return base.Values(myVersion);
//        }

//        #endregion

//        #region Values(myFunc, myVersion)

//        public new IEnumerable<HashSet<TValue>> Values(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion)
//        {
//            return base.Values(myFunc, myVersion);
//        }

//        #endregion

//        #region ValueCount(myVersion)

//        public new UInt64 ValueCount(Int64 myVersion)
//        {
//            return base.ValueCount(myVersion);
//        }

//        #endregion

//        #region ValueCount(myFunc, myVersion)

//        public new UInt64 ValueCount(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion)
//        {
//            return base.ValueCount(myFunc, myVersion);
//        }

//        #endregion


//        #region GetIDictionary(myVersion)

//        public new IDictionary<TKey, HashSet<TValue>> GetIDictionary(Int64 myVersion)
//        {
//            return base.GetIDictionary(myVersion);
//        }

//        #endregion

//        #region GetIDictionary(myFunc, myVersion)

//        public new IDictionary<TKey, HashSet<TValue>> GetIDictionary(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion)
//        {
//            return base.GetIDictionary(myFunc, myVersion);
//        }

//        #endregion


//        #region GetEnumerator(myVersion)

//        public new IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator(Int64 myVersion)
//        {
//            return base.GetEnumerator(myVersion);
//        }

//        #endregion

//        #region GetEnumerator(myFunc, myVersion)

//        public new IEnumerator<KeyValuePair<TKey, HashSet<TValue>>> GetEnumerator(Func<KeyValuePair<TKey, IEnumerable<TValue>>, Boolean> myFunc, Int64 myVersion)
//        {
//            return base.GetEnumerator(myFunc, myVersion);
//        }

//        #endregion

//        #endregion

//        #region Additional methods

//        #region VersionCount(myKey)

//        public new UInt64 VersionCount(TKey myKey)
//        {
//            return base.VersionCount(myKey);
//        }

//        #endregion

//        #region ClearHistory(myKey)

//        public new void ClearHistory(TKey myKey)
//        {
//            base.ClearHistory(myKey);
//        }

//        #endregion

//        #endregion

//        #endregion

//    }

//}
