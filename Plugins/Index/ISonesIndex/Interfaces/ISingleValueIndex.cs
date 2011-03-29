using System;
using System.Collections.Generic;
using sones.Plugins.Index.Helper;

namespace sones.Plugins.Index.Interfaces
{
    /// <summary>
    /// Interface defines indices which have a 1:1 key-value relationship.
    /// </summary>
    /// <typeparam name="TKey">Type of the index-key</typeparam>
    /// <typeparam name="TValue">Type of the values</typeparam>
    public interface ISingleValueIndex<TKey, TValue> :
        IIndex<TKey, TValue>,
        IEnumerable<KeyValuePair<TKey, TValue>>
        where TKey : IComparable
    {
        #region Add

        /// <summary>
        /// Gets / Sets the associated value of the key
        /// </summary>
        /// <param name="myKey"></param>
        /// <returns></returns>
        TValue this[TKey myKey] { get; set; }

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
        void Add(KeyValuePair<TKey, TValue> myKeyValuePair,
                 IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE);

        /// <summary>
        /// Adds a dictionary containing keys and the associated value to the graph
        /// </summary>
        /// <param name="myDictionary">a dictionary containing keys and the associated value</param>
        /// <param name="myIndexAddStrategy">defines what to do if the key already exists</param>
        void Add(IDictionary<TKey, TValue> myDictionary, IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE);

        #endregion

        #region Values

        /// <summary>
        /// Returns all values of the index
        /// </summary>
        /// <returns></returns>
        ISet<TValue> Values();

        #endregion
    }
}