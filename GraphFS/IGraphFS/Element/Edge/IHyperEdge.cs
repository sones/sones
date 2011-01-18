using System;
using System.Collections.Generic;

namespace sones.GraphFS.Element
{
    /// <summary>
    /// The interface for all hyper edges
    /// </summary>
    public interface IHyperEdge : IEdge
    {
        /// <summary>
        /// Gets all contained edges
        /// </summary>
        /// <param name="myFilterFunction">A function to filter those edges</param>
        /// <returns>An IEnumerable of edges</returns>
        IEnumerable<ISingleEdge> GetEdges(Func<ISingleEdge, bool> myFilterFunction = null);
    }
}
