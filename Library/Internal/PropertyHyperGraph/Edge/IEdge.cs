using System;
using System.Collections.Generic;

namespace sones.Library.PropertyHyperGraph
{
    /// <summary>
    /// The interface for all edge species
    /// </summary>
    public interface IEdge : IGraphElement, IEdgeProperties
    {
        #region Source

        /// <summary>
        /// Get the source vertex of the edge
        /// </summary>
        /// <returns>The source vertex</returns>
        IVertex GetSourceVertex();

        #endregion

        #region Targets

        /// <summary>
        /// Get all target vertices
        /// </summary>
        /// <param name="myFilter">A function to filter vertices</param>
        /// <returns>A IEnumerable of vertices</returns>
        IEnumerable<IVertex> GetTargetVertices(Filter.TargetVertexFilter myFilter = null);

        #endregion
    }
}