
using System;

using sones.GraphDB.GraphQL.StructureNodes;

using sones.Lib.Frameworks.Irony.Parsing;

namespace sones.GraphDB.GraphQL.StatementNodes.Import
{

    public class ParallelTasksNode : AStructureNode
    {

        public UInt32 ParallelTasks { get; private set; }

        public ParallelTasksNode()
        {
            ParallelTasks = 1;
        }

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            if (parseNode.HasChildNodes())
            {
                ParallelTasks = Convert.ToUInt32(parseNode.ChildNodes[1].Token.Value);
            }

        }

    }

}
