/* GraphFS MinMaxDictionary
 * (c) Achim Friedland, 2008
 * 
 * This is a generic dictionary which knows the mix and getLatestRevisionTime
 * values of its UInt64 keys.
 * 
 * Lead programmer:
 *      Achim Friedland
 * 
 * */

#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

#endregion

namespace sones.Lib
{

    /// <summary>
    /// This is a generic dictionary which knows the mix and getLatestRevisionTime
    /// values of its UInt64 keys.
    /// </summary>

    

    public class MinMaxDictionary<T>
    {

        #region Data

        protected Dictionary<UInt64, T>  internalDictionary  = null;
        protected UInt64                 minKey              = UInt64.MaxValue;
        protected UInt64                 maxKey              = UInt64.MinValue;

        #endregion

        #region Constructor

        public MinMaxDictionary()
        {
            internalDictionary  = new Dictionary<UInt64, T>();
        }

        #endregion


        #region Internal Methods

        public int Count
        {
            get { return internalDictionary.Count; }
        }


        public Dictionary<UInt64, T>.KeyCollection Keys
        {
            get { return internalDictionary.Keys; }
        }


        public Dictionary<UInt64, T>.ValueCollection Values
        {
            get { return internalDictionary.Values; }
        }


        public T this[UInt64 key]
        {

            get { return internalDictionary[key]; }

            set
            {
                if (key < minKey) minKey = key;
                if (key > maxKey) maxKey = key;
                internalDictionary[key] = value;
            }

        }


        public void Add(UInt64 key, T value)
        {
            if (key < minKey) minKey = key;
            if (key > maxKey) maxKey = key;
            internalDictionary[key] = value;
        }


        public bool ContainsKey(UInt64 key)
        {
            return internalDictionary.ContainsKey(key);
        }


        public bool ContainsValue(T value)
        {
            return internalDictionary.ContainsValue(value);
        }


        public override bool Equals(object obj)
        {
            return internalDictionary.Equals(obj);
        }


        public IEnumerator GetEnumerator()
        {
            return internalDictionary.GetEnumerator();
        }


        public override int GetHashCode()
        {
            return internalDictionary.GetHashCode();
        }


        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            internalDictionary.GetObjectData(info, context);
        }


        public void OnDeserialization(object sender)
        {
            internalDictionary.OnDeserialization(sender);
        }


        public override string ToString()
        {
            return internalDictionary.ToString();
        }


        public bool TryGetValue(UInt64 key, out T value)
        {
            return internalDictionary.TryGetValue(key, out value);
        }


        public void Remove(UInt64 key)
        {
            internalDictionary.Remove(key);
        }


        public void Clear()
        {
            internalDictionary.Clear();
        }

        #endregion


        #region Additional Methods

        public UInt64 MinKey()
        {
            return minKey;
        }


        public UInt64 MaxKey()
        {
            return maxKey;
        }


        public T MinValue()
        {
            return internalDictionary[minKey];
        }


        public T MaxValue()
        {
            return internalDictionary[maxKey];
        }

        #endregion


    }

}
