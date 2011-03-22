using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Result
{
    /// <summary>
    /// The interface for all edge views
    /// </summary>
    public interface IEdgeView : IGraphElementView
    {
        #region EdgeTypeID

        /// <summary>
        /// The name of the edge type
        /// </summary>
        String EdgeTypeName { get; }

        #endregion

        #region Source

        /// <summary>
        /// Get the source vertex view of the edge
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
