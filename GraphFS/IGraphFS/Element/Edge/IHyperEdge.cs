using System;
using System.Collections.Generic;

namespace sones.GraphFS.Element
{
    /// <summary>
    /// The interface for all hyper edges
    /// </summary>
    public interface IHyperEdge : IEdge
    {
        IEnumerable<ISingleEdge> GetEdges();
    }
}
