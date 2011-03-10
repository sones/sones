using System;
using System.Collections.Generic;
using sones.Library.PropertyHyperGraph;

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
        /// <param name="myFilterFunc">A function to filter vertices</param>
        /// <returns>A IEnumerable of vertices</returns>
        IEnumerable<IVertex> GetTargetVertices(Func<IVertex, bool> myFilterFunc = null);

        #endregion
    }
}