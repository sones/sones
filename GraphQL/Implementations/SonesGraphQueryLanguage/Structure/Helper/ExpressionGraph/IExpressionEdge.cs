using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.GQL.Structure.Helper.ExpressionGraph.Helper;

namespace sones.GraphQL.GQL.Structure.Helper.ExpressionGraph
{
    /// <summary>
    /// Interface for the expression edge.
    /// </summary>
    public interface IExpressionEdge
    {
        EdgeKey Direction { get; }
        Int64 Destination { get; }
    }
}
