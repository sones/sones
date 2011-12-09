using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Plugins.Index.Helper;
using sones.Library.PropertyHyperGraph;

namespace sones.Plugins.Index.Versioned
{
    /// <summary>
    /// Interface represents a versioned index where keys / values
    /// can be retrieved by a given timestamp.
    /// </summary>
    public interface ISonesVersionedIndex : ISonesIndex
    {
        #region Counts

        /// <summary>
        /// Returns the number of stored keys at a given version
        /// </summary>
        /// <param name="myVersion">The version</param>
        /// <returns>Number of keys associated with a given version.</returns>
        Int64 KeyCount(Int64 myVersion);

        /// <summary>
        /// Returns the number of values at a given version
        /// </summary>
        /// <param name="myVersion">The version</param>
        /// <returns>Number of values associated with a given version.</returns>
        Int64 ValueCount(Int64 myVersion);

        #endregion

        #region Keys

        /// <summary>
        /// Returns all stored keys at a given version
        /// </summary>
        /// <returns>Search keys associated with a given version.</returns>
        ICloseableEnumerable<IComparable> Keys(Int64 myVersion);

        #endregion

        #region TryGetValue

        /// <summary>
        /// Selects all values of the index for the given key at a given version
        /// 
        /// The key has to be of the same type as the indexed property.
        /// </summary>
        /// <returns>
        /// True if the index contains the given key.
        /// </returns>
        /// <param name="myKey">
        /// The key to search for.
        /// </param>
        /// <param name="myVertexIDs">
        /// Stores the vertexIDs if the key has been found.
        /// </param>
        /// <param name="myVersion">The version</param>
        bool TryGetValues(IComparable myKey, 
            out ICloseableEnumerable<Int64> myVertexIDs,
            Int64 myVersion);

        #endregion

        #region this

        /// <summary>
        /// Returns all values associated to the given key at a given version.
        /// 
        /// If the key doesn't exist a 
        /// <see cref="sones.Plugins.Index.ErrorHandling.IndexKeyNotFoundException"/>
        /// will be thrown.
        /// </summary>
        /// <param name="myKey">The search key</param>
        /// <param name="myVersion">The version</param>
        /// <returns>Values associated with the key at a given version</returns>
        ICloseableEnumerable<Int64> this[IComparable myKey, Int64 myVersion] { get; }

        #endregion

        #region ContainsKey

        /// <summary>
        /// Checks if the given key exists in the index at a given version.
        /// 
        /// The type of the key has to be the type of the indexed property.
        /// </summary>
        /// <returns>
        /// True if the index contains the given key.
        /// </returns>
        /// <param name="myKey">
        /// The key to search for.
        /// </param>
        /// <param name="myVersion">The version</param>
        bool ContainsKey(IComparable myKey, Int64 myVersion);

        #endregion

        #region ClearHistory

        /// <summary>
        /// Removes all versioned data except the
        /// actual from the index.
        /// </summary>
        void ClearHistory();

        /// <summary>
        /// Removes all data from index which is older
        /// than the specified version.
        /// </summary>
        void Clear(Int64 myOldestVersion);

        #endregion
    }
}
