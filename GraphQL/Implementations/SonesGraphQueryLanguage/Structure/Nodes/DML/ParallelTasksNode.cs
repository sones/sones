using System;
using Irony.Ast;
using Irony.Parsing;

namespace sones.GraphQL.Structure.Nodes.DML
{
    public sealed class ParallelTasksNode : AStructureNode, IAstNodeInit
    {
        public UInt32 ParallelTasks { get; private set; }

        public ParallelTasksNode()
        {
            ParallelTasks = 1;
        }

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (parseNode.ChildNodes != null && parseNode.ChildNodes.Count != 0)
            {
                ParallelTasks = Convert.ToUInt32(parseNode.ChildNodes[1].Token.Value);
            }
        }

        #endregion
    }
}
