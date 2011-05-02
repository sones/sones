using System;
using System.Collections.Generic;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using sones.GraphQL.Structure.Nodes.Expressions;

namespace sones.GraphQL.Structure.Nodes.DML
{
    public sealed class CommentsNode : AStructureNode, IAstNodeInit
    {
        public List<String> Comments { get; private set; }

        public CommentsNode()
        {
            Comments = new List<string>();
        }

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (HasChildNodes(parseNode))
            {
                foreach (var child in (parseNode.ChildNodes[1].AstNode as TupleNode).TupleDefinition)
                {
                    Comments.Add((child.Value as ValueDefinition).Value.ToString());
                }
            }
        }

        #endregion
    }
}
