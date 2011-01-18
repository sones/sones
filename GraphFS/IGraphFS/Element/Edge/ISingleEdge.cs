using System;
using System.Collections.Generic;

namespace sones.GraphFS.Element
{
    /// <summary>
    /// The interface for all single-target edges
    /// </summary>
    public interface ISingleEdge : IEdge
    {
        IVertex GetTargetVertex();
    }
}
