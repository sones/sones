using sones.Library.PropertyHyperGraph;
using System.Collections.Generic;

namespace sones.GraphDB.Expression.QueryPlan
{
    /// <summary>
    /// The interface for all query plans
    /// </summary>
    public interface IQueryPlan
    {
        /// <summary>
        /// Executes the query plan an returns the desired vertices
        /// </summary>
        /// <returns>An enumerable of vertices</returns>
        IEnumerable<IVertex> Execute();
    }
}
