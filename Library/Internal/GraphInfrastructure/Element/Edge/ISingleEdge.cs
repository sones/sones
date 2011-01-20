using System;
using System.Collections.Generic;

namespace sones.GraphInfrastructure.Element
{
    /// <summary>
    /// The interface for all single-target edges
    /// </summary>
    public interface ISingleEdge : IEdge
    {
        /// <summary>
        /// Returns the target vertex
        /// </summary>
        /// <returns>The target vertex</returns>
        IVertex GetTargetVertex();
    }
}
