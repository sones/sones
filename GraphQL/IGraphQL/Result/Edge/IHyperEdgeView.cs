using System.Collections.Generic;

namespace sones.GraphQL.Result
{
    /// <summary>
    /// The interface for all hyper IncomingEdge views
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
