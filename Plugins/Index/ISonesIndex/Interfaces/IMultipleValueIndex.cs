using System;
using System.Collections.Generic;
using sones.Plugins.Index.Helper;

namespace sones.Plugins.Index.Interfaces
{
    /// <summary>
    /// Interface defines indices which have a 1:n key-value relationship.
    /// </summary>
    /// <typeparam name="TKey">Type of the index-key</typeparam>
    /// <typeparam name="TValue">Type of the values</typeparam>
    public interface IMultipleValueIndex<TKey, TValue> :
        IIndex<TKey, TValue>,
        IEnumerable<KeyValuePair<TKey, IEnumerable<TValue>>>
        where TKey : IComparable
    {
        #region Add

        /// <summary>
        /// Gets / Sets the values with the specified key
        /// </summary>
        /// <param name="myKey"></param>
        /// <returns></returns>
        IEnumerable<TValue> this[TKey myKey] { get; set; }

        /// <summary>
        /// Adds a key and 1-n values assigned values to the index
        /// </summary>
        /// <param name="myKey">the key to be inserted</param>
        /// <param name="myValues">the associated 1-n values</param>
        /// <param name="myIndexAddStrategy">defines what to do if the key already exists</param>
        void Add(TKey myKey, IEnumerable<TValue> myValues, IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.MERGE);

        /// <summary>
        /// Adds a keyvaluepair containing one key and 1-n values to the index
        /// </summary>
        /// <param name="myKeyValuesPair">KeyValuePair with one key and 1-n values</param>
        /// <param name="myIndexAddStrategy">defines what to do if the key already exists</param>
        void Add(KeyValuePair<TKey, IEnumerable<TValue>> myKeyValuesPair,
                 IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.MERGE);

        /// <summary>
        /// Adds a dictionary containing keys and the associated 1-n values to the graph
        /// </summary>
        /// <param name="myDictionary">a dictionary containing keys and the associated 1-n values</param>
        /// <param name="myIndexAddStrategy">defines what to do if the key already exists</param>
        void Add(Dictionary<TKey, IEnumerable<TValue>> myDictionary,
                 IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.MERGE);

        #endregion

        #region Contains

        /// <summary>
        /// Checks if a key and the associated values exist in the index
        /// </summary>
        /// <param name="myKey">the key</param>
        /// <param name="myValues">the associated values</param>
        /// <returns>true if the key and the associated balue exist, else false</returns>
        bool Contains(TKey myKey, IEnumerable<TValue> myValues);

        #endregion

        #region Values

        /// <summary>
        /// Returns all value set of the index
        /// </summary>
        /// <returns>the value sets of the index</returns>
        IEnumerable<IEnumerable<TValue>> Values();

        #endregion
    }
}