using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Plugins.Index
{
    #region IIndexVersionCompatibility

    /// <summary>
    /// A static implementation of the compatible IIndex plugin versions. 
    /// Defines the min and max version for all IIndex implementations which will be activated used this IIndex.
    /// </summary>
    public static class IIndexVersionCompatibility
    {
        public static Version MinVersion
        {
            get
            {
                return new Version("1.0.0.1");
            }
        }
        public static Version MaxVersion
        {
            get
            {
                return new Version("1.0.0.1");
            }
        }
    }

    #endregion

    /// <summary>
    /// The interfaces for all indices
    /// </summary>
    public interface IIndex<TKey, TValue> : IEnumerable<TValue>
        where TKey : IComparable
    {
        /// <summary>
        /// Is this a persistent index?
        /// </summary>
        Boolean IsPersistent { get; }

        /// <summary>
        /// Returns the name of the index
        /// </summary>
        String Name { get; }

        #region Add

        /// <summary>
        /// Adds a key and an associated value to the index
        /// </summary>
        /// <param name="myKey">the key to be inserted</param>
        /// <param name="myValue">the associated value</param>
        /// <param name="myIndexAddStrategy">defines what to do if the key already exists</param>
        void Add(TKey myKey, TValue myValue, IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE);
        /// <summary>
        /// Adds a KeyValuePair to the index
        /// </summary>
        /// <param name="myKeyValuePair">KeyValuePair</param>
        /// <param name="myIndexAddStrategy">defines what to do if the key already exists</param>
        void Add(KeyValuePair<TKey, TValue> myKeyValuePair, IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE);
        /// <summary>
        /// Adds a dictionary containing keys and the associated value to the graph
        /// </summary>
        /// <param name="myDictionary">a dictionary containing keys and the associated value</param>
        /// <param name="myIndexAddStrategy">defines what to do if the key already exists</param>
        void Add(Dictionary<TKey, TValue> myDictionary, IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE);
        /// <summary>
        /// Gets / Sets the associated value of the key
        /// </summary>
        /// <param name="myKey"></param>
        /// <returns></returns>
        TValue this[TKey myKey] { get; set; }

        #endregion

        #region Contains

        /// <summary>
        /// Checks if a key exists in the index
        /// </summary>
        /// <param name="myKey">the key to be checked</param>
        /// <returns>true if the key exists, else false</returns>
        bool ContainsKey(TKey myKey);
        /// <summary>
        /// Checks if a value exists in the index
        /// </summary>
        /// <param name="myValue">the value to be checked</param>
        /// <returns>true if the value exists, else false</returns>
        bool ContainsValue(TValue myValue);
        /// <summary>
        /// Checks if a key and an associated value exists in the index
        /// </summary>
        /// <param name="myKey">the key</param>
        /// <param name="myValue">the associated value</param>
        /// <returns>true if the key and the associated balue exist, else false</returns>
        bool Contains(TKey myKey, TValue myValue);

        #endregion

        #region Remove

        /// <summary>
        /// Removes a given key and its associated value from the index
        /// </summary>
        /// <param name="myKey">the key to be removed</param>
        /// <returns></returns>
        bool Remove(TKey myKey);

        #endregion

        #region Clear

        /// <summary>
        /// Clears the index, removes all elements
        /// </summary>
        void Clear();

        #endregion

        #region Keys

        /// <summary>
        /// Returns all keys of the index
        /// </summary>
        IEnumerable<TKey> Keys { get; }

        #endregion

        #region Values

        /// <summary>
        /// Returns all values of the index
        /// </summary>
        /// <returns></returns>
        IEnumerable<TValue> GetValues();

        #endregion

        #region Counts

        /// <summary>
        /// Returns the number of keys
        /// </summary>
        Int64 KeyCount { get; }
        /// <summary>
        /// Returns the number of values
        /// </summary>
        /// <returns></returns>
        Int64 GetValueCount();

        #endregion

    }
}
