using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Plugins.Index.Helper;

namespace sones.Plugins.Index.Spatial
{
    /// <summary>
    /// Interface defines a 2-dimensional Spatial Index
    /// used by the sones GraphDB.
    /// 
    /// Supports
    ///     - Exact-(Point)-Match
    ///     - Range-(Within)-Match
    ///     - Near-(Neighborhood)-Match
    /// </summary>
    public interface ISonesSpatialIndex : ISonesIndex
    {
        #region Add

        /// <summary>
        /// Adds a vertexID at the given point.
        /// </summary>
        /// <param name="myPoint">Spatial position of the vertex.</param>
        /// <param name="myVertexID">The VertexID</param>
        /// <param name="myIndexAddStrategy">Define what happens if a vertexID exists at the given point.</param>
        void Add(IPoint myPoint, Int64 myVertexID,
            IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE);

        /// <summary>
        /// Adds a collection of Point-VertexID pairs.
        /// </summary>
        /// <param name="myKeyValuePairs">A collection of Point-VertexID pairs</param>
        /// <param name="myIndexAddStrategy">Define what happens if a vertexID exists at the given point.</param>
        void AddRange(IEnumerable<KeyValuePair<IPoint, Int64>> myKeyValuePairs,
            IndexAddStrategy myIndexAddStrategy = IndexAddStrategy.UNIQUE);

        #endregion

        #region Retrieval

        #region Exact and Region match (Returns all values at a given point or in a given space.)

        /// <summary>
        /// Returns all vertexIDs associated to the given geometry.
        /// </summary>
        /// <param name="myGeometry">Search space</param>
        /// <param name="myVertexIDs">Stores found vertexIDs</param>
        /// <returns>True, if at least one object has been found at the given point or in the space.</returns>
        bool TryGetValues(IGeometry myGeometry, out IEnumerable<Int64> myVertexIDs);

        #endregion

        #region Neighborhood match (Returns k-nearest neighbours with optional maximum distance.)

        /// <summary>
        /// Returns k nearest neighbours of the given point.
        /// </summary>
        /// <param name="myCentralPoint">GeoHash of central point</param>
        /// <param name="myK">Maximum number of neighbours to find</param>
        /// <param name="myVertexIDs">Stores found vertexIDs</param>
        /// <param name="myMaximumDistance">Defines an optional search boundary</param>
        /// <returns>True, if at least one neighbour has been found.</returns>
        bool TryGetValuesNear(IPoint myCentralPoint, Int32 myK, out IEnumerable<Int64> myVertexIDs, double? myMaximumDistance);

        #endregion

        #endregion

        #region Remove

        /// <summary>
        /// Removes all vertexIDs stored in the given space.
        /// </summary>
        /// <param name="myGeometry">Search space</param>
        /// <returns>True, if the stored values where deleted.</returns>
        bool Remove(IGeometry myGeometry);

        #endregion
    }
}
