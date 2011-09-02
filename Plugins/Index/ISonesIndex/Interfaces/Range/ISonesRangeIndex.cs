using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Plugins.Index.Range
{
    /// <summary>
    /// This interfaces defines a default Range Index structure used
    /// by the sones GraphDB.
    /// 
    /// It supports single- and multi-key indexing and manages single or 
    /// multiple values for one key.
    /// 
    /// This interface is non-generic because in the sones GraphDB
    /// every indexable attribute is hidden behing a propertyID
    /// of type Int64.
    /// </summary>
    public interface ISonesRangeIndex : ISonesIndex
    {
        /// <summary>
        /// Returns all vertexIDs that are lower than (or equal to) to the given key.
        /// </summary>
        /// <returns>
        /// All vertexIDs lower than (or equal to) the given key.
        /// </returns>
        /// <param name="myKey">
        /// The upper bound key.
        /// </param>
        /// <param name="myIncludeKey">
        /// True if the values associated with the upper bound shall be returned, too.
        /// </param>
        IEnumerable<Int64> LowerThan(IComparable myKey, bool myIncludeKey = true);

        /// <summary>
        /// Returns all vertexIDs that are greater than (or equal to) to the given key.
        /// </summary>
        /// <returns>
        /// All vertexIDs greater than (or equal to) the given key.
        /// </returns>
        /// <param name="myKey">
        /// The lower bound key.
        /// </param>
        /// <param name="myIncludeKey">
        /// True if the values associated with the lower bound shall be returned, too.
        /// </param>
        IEnumerable<Int64> GreaterThan(IComparable myKey, bool myIncludeKey = true);

        /// <summary>
        /// Returns all vertexID ids that are in a given key range.
        /// </summary>
        /// <param name="myFromKey">
        /// The lower bound of the range.
        /// </param>
        /// <param name="myToKey">
        /// The upper bound of the range.
        /// </param>
        /// <param name="myIncludeFromKey">
        /// True, if the lower bound shall be included in the range.
        /// </param>
        /// <param name='myIncludeToKey'>
        /// True, if the upper bound shall be included in the range.
        /// </param>
        IEnumerable<Int64> Between(IComparable myFromKey,
            IComparable myToKey,
            bool myIncludeFromKey = true,
            bool myIncludeToKey = true);
    }
}
