using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Plugins.Index;

namespace ISonesIndex
{
    public interface IMultipleValueIndex<TKey, TValue> : IIndex<TKey, TValue>
        where TKey : IComparable
    {
        #region Add

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
        void Add(KeyValuePair<TKey, IEnumerable<TValue>> myKeyValuesPair, IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.MERGE);
        /// <summary>
        /// Adds a dictionary containing keys and the associated 1-n values to the graph
        /// </summary>
        /// <param name="myDictionary">a dictionary containing keys and the associated 1-n values</param>
        /// <param name="myIndexAddStrategy">defines what to do if the key already exists</param>
        void Add(Dictionary<TKey, IEnumerable<TValue>> myDictionary, IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.MERGE);
        /// <summary>
        /// Gets / Sets the values with the specified key
        /// </summary>
        /// <param name="myKey"></param>
        /// <returns></returns>
        IEnumerable<TValue> this[TKey myKey] { get; set; }

        #endregion

        #region Contains

        /// <summary>
        /// Checks if a key and the associated values exist in the index
        /// </summary>
        /// <param name="myKey">the key</param>
        /// <param name="myValue">the associated value</param>
        /// <returns>true if the key and the associated balue exist, else false</returns>
        bool Contains(TKey myKey, IEnumerable<TValue> myValues);

        #endregion

        #region Values

        /// <summary>
        /// Returns all value set of the index
        /// </summary>
        /// <returns>the value sets of the index</returns>
        IEnumerable<IEnumerable<TValue>> GetValues();

        #endregion
    }
}
