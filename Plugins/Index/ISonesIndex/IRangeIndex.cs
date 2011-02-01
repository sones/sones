using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Plugins.Index;

namespace sones.Plugins.Index
{
    /// <summary>
    /// Defines a range index (like B*/+Tree)
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IRangeIndex<TKey, TValue> : IIndex<TKey, TValue>
        where TKey : IComparable
    {
        /// <summary>
        /// Returns all values from keys which are greater than the given key
        /// </summary>
        /// <param name="myKey">the lower bound of the range</param>
        /// <param name="myOrEqual">true if the key shall be included in the range</param>
        /// <returns>values from all keys greater than given key</returns>
        IEnumerable<TValue> GreaterThan(TKey myKey, bool myOrEqual = true);
        /// <summary>
        /// Returns all values from keys which are lower than the given key
        /// </summary>
        /// <param name="myKey">the upper bound of the range</param>
        /// <param name="myOrEqual">true if the key shall be included in the range</param>
        /// <returns>values from all keys lower than given key</returns>
        IEnumerable<TValue> LowerThan(TKey myKey, bool myOrEqual = true);
        /// <summary>
        /// Returns all values from keys in a given range
        /// </summary>
        /// <param name="myFromKey">the lower bound of the range</param>
        /// <param name="myToKey">the upper bound of the range</param>
        /// <param name="myOrEqualFromKey">true if the lower bound shall be included in the range</param>
        /// <param name="myOrEqualToKey">true if the upper bound shall be included in the range</param>
        /// <returns>values from all keys in the given range</returns>
        IEnumerable<TValue> InRange(TKey myFromKey, TKey myToKey, bool myOrEqualFromKey = true, bool myOrEqualToKey = true);
    }
}
