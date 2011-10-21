using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Plugins.Index.Helper;

namespace sones.Plugins.Index.Compound
{
    /// <summary>
    /// This interfaces defines a default Compound Index structure used
    /// by the sones GraphDB.
    /// 
    /// It supports single- and multi-key (multi-dimensional) indexing 
    /// and manages single or multiple values for one key. Furthermore, 
    /// it is possible to use exact match or partial match queries.
    /// 
    /// This interface is non-generic because in the sones GraphDB
    /// every indexable attribute is hidden behind a propertyID
    /// of type Int64.
    /// </summary>
    public interface ISonesCompoundIndex : ISonesIndex
    {
        #region Counts

        /// <summary>
        /// Returns the number of stored keys associated
        /// with the given propertyID
        /// </summary>
        Int64 KeyCount(Int64 myPropertyID);

        #endregion

        #region Keys

        /// <summary>
        /// Returns all keys associated with the given propertyID.
        /// </summary>
        /// <param name="myPropertyID">The propertyID of the keys to be returned</param>
        /// <returns>All keys associated with the given propertyID.</returns>
        IEnumerable<IComparable> Keys(Int64 myPropertyID);

        #endregion

        #region Init

        /// <summary>
        /// Inits the index and tells it which
        /// propertyIDs to use for indexing.
        /// 
        /// The order of the list defines the internal
        /// key order.
        /// </summary>
        /// <param name="myPropertyIDs">
        /// A collection of propertyIDs which will
        /// be used by the compound index.
        /// </param>
        void Init(IEnumerable<Int64> myPropertyIDs);

        #endregion

        #region GetKeyTypes

        /// <summary>
        /// Gets the key types.
        /// </summary>
        /// <returns>
        /// A dictionary which contains a mapping between the stored property and its type.
        /// </returns>
        IDictionary<Int64, Type> GetKeyTypes();

        #endregion

        #region Add

        /// <summary>
        /// Adds a vertexID associated to a collection of keys to the index.
        /// </summary>
        /// <param name="myKeys">Collection of compound index keys</param>
        /// <param name="myVertexID">VertexID</param>
        /// <param name="myIndexAddStrategy">Define what happens if one of the keys already exists.</param>
        void Add(IEnumerable<ICompoundIndexKey> myKeys, Int64 myVertexID,
            IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE);

        /// <summary>
        /// Adds a collection of Keys-Value-Pairs to the index.
        /// </summary>
        /// <param name="myKeysValuePairs">Collection of Key-Value-Pairs of CompundIndexKeys and associated vertexIDs</param>
        /// <param name="myIndexAddStrategy">Define what happens if one of the keys already exists.</param>
        void AddRange(IEnumerable<KeyValuePair<IEnumerable<ICompoundIndexKey>, Int64>> myKeysValuePairs,
            IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE);

        #endregion

        #region TryGetValues (Exact match)

        /// <summary>
        ///  Selects all values of the index for the given key collection.
        /// </summary>
        /// <returns>
        ///  True, if the index contains n of the given n keys. 
        /// </returns>
        /// <param name="myKeys">
        ///  A collection of keys to search for.
        /// </param>
        /// <param name="myVertexIDs">
        ///  Stores the vertexIDs if a matching for all keys was found.
        /// </param>
        bool TryGetValues(IEnumerable<ICompoundIndexKey> myKeys, out IEnumerable<Int64> myVertexIDs);

        #endregion

        #region TryGetValuesPartial (Partial Match)

        /// <summary>
        /// Returns all vertexIDs where the keys match 1-n of the n given keys.
        /// </summary>
        /// <returns>
        /// True, if the index contains 1-n of the n given keys.
        /// </returns>
        /// <param name="myKeys">
        /// A collection of search keys consisting of tuples of propertyID and key.
        /// </param>
        /// <param name="myVertexIDs">
        /// Stores the vertexIDs if a matching for 1-n keys was found.
        /// </param>
        bool TryGetValuesPartial(IEnumerable<ICompoundIndexKey> myKeys, out IEnumerable<Int64> myVertexIDs);

        #endregion

        #region this

        /// <summary>
        /// Returns all vertexIDs associated with the 
        /// given key collection.
        /// </summary>
        /// <param name="myKeys">A collection of search keys</param>
        /// <returns>Associated values</returns>
        IEnumerable<Int64> this[IEnumerable<ICompoundIndexKey> myKeys] { get; }

        #endregion
    }
}
