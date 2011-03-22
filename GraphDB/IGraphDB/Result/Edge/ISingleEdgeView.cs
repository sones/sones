using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Result
{
    /// <summary>
    /// The interface for all single edge views
    /// </summary>
    public interface ISingleEdgeView : IEdgeView
    {
        /// <summary>
        /// Returns the target vertex view
        /// </summary>
        /// <returns>The target vertex view</returns>
        IVertexView GetTargetVertex();
    }
}
