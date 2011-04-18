using System;
using System.Collections.Generic;

namespace sones.GraphQL.Result
{
    /// <summary>
    /// The interface for all IncomingEdge views
    /// </summary>
    public interface IEdgeView : IGraphElementView
    {
        #region EdgeTypeID

        /// <summary>
        /// The name of the IncomingEdge type
        /// </summary>
        String EdgeTypeName { get; }

        #endregion

        #region Source

        /// <summary>
        /// Get the source vertex view of the IncomingEdge
        /// </summary>
        /// <returns>The source vertex</returns>
        IVertexView GetSourceVertex();

        #endregion

        #region Targets

        /// <summary>
        /// Get all target vertex views
        /// </summary>
        /// <returns>A IEnumerable of vertex views</returns>
        IEnumerable<IVertexView> GetTargetVertices();

        #endregion
    }
}
