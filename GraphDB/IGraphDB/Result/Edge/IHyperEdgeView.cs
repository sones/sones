using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Result
{
    /// <summary>
    /// The interface for all hyper edge views
    /// </summary>
    public interface IHyperEdgeView : IEdgeView
    {
        /// <summary>
        /// Gets all contained edges
        /// </summary>
        /// <returns>An IEnumerable of edges</returns>
        IEnumerable<ISingleEdgeView> GetEdges();
    }
}
