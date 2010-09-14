using System.Collections.Generic;
using sones.GraphFS.DataStructures;

namespace sones.GraphDB.Structures.ExpressionGraph
{

    public interface IExpressionLevelEntry
    {
        /// <summary>
        /// All Objects with their ExpressionNode
        /// </summary>
        Dictionary<ObjectUUID, IExpressionNode> Nodes { get; }

        /// <summary>
        /// The corresponding Level
        /// </summary>
        LevelKey CorrespondingLevelKey { get; }
    }

}
