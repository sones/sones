/* GraphFS - DictionaryObject
 * (c) Achim Friedland, 2009
 * 
 * Lead programmer:
 *      Achim Friedland
 * 
 * */

#region Usings

using System;
using System.Text;
using System.Collections.Generic;

using sones.Lib.Cryptography.IntegrityCheck;
using sones.Lib.Cryptography.SymmetricEncryption;
using sones.Lib.DataStructures;
using sones.GraphFS.DataStructures;
using sones.Lib.DataStructures.Indices;

#endregion

namespace sones.GraphFS.Objects
{

    /// <summary>
    /// An implementation of a DictionaryObject to store a mapping TKey => TValue.
    /// </summary>
    /// <typeparam name="TKey">Must implement IComparable</typeparam>

    

    public class DictionaryObject<TKey, TValue> : ADictionaryObject<TKey, TValue>, IDictionaryObject<TKey, TValue>
        where TKey : IComparable
    {


        #region Constructor

        #region DictionaryObject()

        /// <summary>
        /// This will create an empty DictionaryObject using a Dictionary&lt;TKey, TValue&gt; for the internal IDictionary&lt;TKey, TValue&gt; object.
        /// </summary>
        public DictionaryObject()
            : this(new Dictionary<TKey, TValue>())
        {
        }

        #endregion

        #region DictionaryObject(myIDictionary)

        /// <summary>
        /// This will create an empty DictionaryObject using the given IDictionary object for the internal IDictionary&lt;TKey, TValue&gt; object.
        /// </summary>
        public DictionaryObject(IDictionary<TKey, TValue> myIDictionary)
        {

            // Members of AGraphStructure
            _StructureVersion   = 1;

            // Members of AGraphObject
            _ObjectStream       = FSConstants.DEFAULT_INDEXSTREAM;

            // Object specific data...
            _IDictionary        = myIDictionary;

        }

        #endregion

        #region DictionaryObject(myObjectLocation, mySerializedData, myIntegrityCheckAlgorithm, myEncryptionAlgorithm)

        /// <summary>
        /// A constructor used for fast deserializing
        /// </summary>
        /// <param name="mySerializedData">A bunch of bytes[] containing the serialized DictionaryObject</param>
        public DictionaryObject(String myObjectLocation, Byte[] mySerializedData, IIntegrityCheck myIntegrityCheckAlgorithm, ISymmetricEncryption myEncryptionAlgorithm)
            : this()
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

            var newT = new DictionaryObject<TKey, TValue>();
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

        #region Set

        #region Set(myKey, myValue)

        public new UInt64 Set(TKey myKey, TValue myValue)
        {
            return base.Set(myKey, myValue);
        }

        #endregion

        #region Set(myKeyValuePair)

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


    }

}
