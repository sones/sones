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
    }
}
