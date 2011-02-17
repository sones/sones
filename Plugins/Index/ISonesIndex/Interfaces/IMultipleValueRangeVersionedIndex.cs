using System;
using System.Collections.Generic;

namespace sones.Plugins.Index.Interfaces
{
    /// <summary>
    /// Interface defines indices which have a 1:n key-value relationship,
    /// support range queries on index keys and can handle different versions of the index.
    /// </summary>
    /// <typeparam name="TKey">Type of the index-key</typeparam>
    /// <typeparam name="TValue">Type of the index-values</typeparam>
    /// <typeparam name="TVersion">The type of comparable versions</typeparam>
    public interface IMultipleValueRangeVersionedIndex<TKey, TValue, TVersion> :
        IMultipleValueRangeIndex<TKey, TValue>,
        IMultipleValueVersionedIndex<TKey, TValue, TVersion>
        where TKey : IComparable
        where TVersion : IComparable
    {
        #region Range

        /// <summary>
        /// Returns all value-sets from keys which are greater than the given key
        /// </summary>
        /// <param name="myKey">the lower bound of the range</param>
        /// <param name="myVersion">the version of the key</param>
        /// <param name="myOrEqual">true if the key shall be included in the range</param>
        /// <returns>values from all keys greater than given key</returns>
        IEnumerable<IEnumerable<TValue>> GreaterThan(TKey myKey, TVersion myVersion, bool myOrEqual = true);

        /// <summary>
        /// Returns all value-sets from keys which are lower than the given key
        /// </summary>
        /// <param name="myKey">the upper bound of the range</param>
        /// <param name="myVersion">the version of the key</param>
        /// <param name="myOrEqual">true if the key shall be included in the range</param>
        /// <returns>values from all keys lower than given key</returns>
        IEnumerable<IEnumerable<TValue>> LowerThan(TKey myKey, TVersion myVersion, bool myOrEqual = true);

        /// <summary>
        /// Returns all value-sets from keys in a given range
        /// </summary>
        /// <param name="myFromKey">the lower bound of the range</param>
        /// <param name="myToKey">the upper bound of the range</param>
        /// <param name="myVersion">the version of the key</param>
        /// <param name="myOrEqualFromKey">true if the lower bound shall be included in the range</param>
        /// <param name="myOrEqualToKey">true if the upper bound shall be included in the range</param>
        /// <returns>values from all keys in the given range</returns>
        IEnumerable<IEnumerable<TValue>> InRange(TKey myFromKey, TKey myToKey, TVersion myVersion,
                                                 bool myOrEqualFromKey = true, bool myOrEqualToKey = true);

        #endregion
    }
}