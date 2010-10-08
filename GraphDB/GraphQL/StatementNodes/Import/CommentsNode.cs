
using System;
using System.Collections.Generic;

using sones.GraphDB.Managers.Structures;
using sones.GraphDB.GraphQL.StructureNodes;

using sones.Lib.Frameworks.Irony.Parsing;

namespace sones.GraphDB.GraphQL.StatementNodes.Import
{

    public class CommentsNode : AStructureNode
    {

        public List<String> Comments { get; private set; }

        public CommentsNode()
        {
            Comments = new List<string>();
        }

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            if (parseNode.HasChildNodes())
            {
                foreach (var child in (parseNode.ChildNodes[1].AstNode as TupleNode).TupleDefinition)
                {
                    Comments.Add((child.Value as ValueDefinition).Value.ToString());
                }
            }

        }

    }

}
