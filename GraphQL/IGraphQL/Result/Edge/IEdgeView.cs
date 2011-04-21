using System;
using System.Collections.Generic;

namespace sones.GraphQL.Result
{
    /// <summary>
    /// The interface for all IncomingEdge views
    /// </summary>
    public interface IEdgeView : IGraphElementView
    {
        
        #region Targets

        /// <summary>
        /// Get all target vertex views
        /// </summary>
        /// <returns>A IEnumerable of vertex views</returns>
        IEnumerable<IVertexView> GetTargetVertices();

        #endregion
    }
}
