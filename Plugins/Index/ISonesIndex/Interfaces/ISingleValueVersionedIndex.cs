using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Plugins.Index.Helper;

namespace sones.Plugins.Index.Interfaces
{
    /// <summary>
    /// Interface defines indices which have a 1:1 key-value relationship
    ///  and can handle different versions of the index.
    /// </summary>
    /// <typeparam name="TKey">Type of the index-key</typeparam>
    /// <typeparam name="TValue">Type of the index-values</typeparam>
    /// <typeparam name="TVersion">The type of comparable versions</typeparam>
    public interface ISingleValueVersionedIndex<TKey, TValue, TVersion> :
        ISingleValueIndex<TKey, TValue>,
        IVersionedIndex<TKey, TValue, TVersion>
        where TKey : IComparable
        where TVersion : IComparable
    {
        #region Add

        /// <summary>
        /// Adds a key and an associated value to the index
        /// </summary>
        /// <param name="myKey">the key to be inserted</param>
        /// <param name="myValue">the associated value</param>
        /// <param name="myVersion">the version of the index</param>
        /// <param name="myIndexAddStrategy">defines what to do if the key already exists</param>
        void Add(TKey myKey, TValue myValue, TVersion myVersion, IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE);

        /// <summary>
        /// Adds a KeyValuePair to the index
        /// </summary>
        /// <param name="myKeyValuePair">KeyValuePair</param>
        /// <param name="myVersion">the version of the index</param>
        /// <param name="myIndexAddStrategy">defines what to do if the key already exists</param>
        void Add(KeyValuePair<TKey, TValue> myKeyValuePair, TVersion myVersion, IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE);

        /// <summary>
        /// Adds a dictionary containing keys and the associated value to the graph
        /// </summary>
        /// <param name="myDictionary">a dictionary containing keys and the associated value</param>
        /// <param name="myVersion">the version of the index</param>
        /// <param name="myIndexAddStrategy">defines what to do if the key already exists</param>
        void Add(Dictionary<TKey, TValue> myDictionary, TVersion myVersion, IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE);

        /// <summary>
        /// Gets / Sets the associated value of the key
        /// </summary>
        /// <param name="myKey">the index key</param>
        /// <param name="myVersion">the version of the index</param>
        /// <returns>set returns the value (if it exists)</returns>
        TValue this[TKey myKey, TVersion myVersion] { get; set; }

        #endregion

        #region Values

        /// <summary>
        /// Returns all values of a given version
        /// </summary>
        /// <param name="myVersion">the version</param>
        /// <returns>all values of the given version</returns>
        IEnumerable<TValue> Values(TVersion myVersion);

        #endregion
    }
}
