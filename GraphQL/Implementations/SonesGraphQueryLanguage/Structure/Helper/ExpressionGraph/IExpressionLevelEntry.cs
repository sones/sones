using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphQL.GQL.Structure.Helper.ExpressionGraph
{
    public interface IExpressionLevelEntry
    {
        /// <summary>
        /// All Objects with their ExpressionNode
        /// </summary>
        Dictionary<Int64, IExpressionNode> Nodes { get; }

        /// <summary>
        /// The corresponding Level
        /// </summary>
        LevelKey CorrespondingLevelKey { get; }
    }
}
