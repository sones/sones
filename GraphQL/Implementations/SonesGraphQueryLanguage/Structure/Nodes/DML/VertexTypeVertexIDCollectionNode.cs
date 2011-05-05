using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Helper.Definition;
using sones.GraphQL.Structure.Nodes.Misc;
using System.Collections.Generic;

namespace sones.GraphQL.Structure.Nodes.DML
{
    public sealed class VertexTypeVertexIDCollectionNode : AStructureNode, IAstNodeInit
    {
        public List<VertexTypeVertexElementNode> Elements { get; private set; }

        public VertexTypeVertexIDCollectionNode()
        {
            Elements = new List<VertexTypeVertexElementNode>();
        }

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            foreach (var aChildnode in parseNode.ChildNodes)
            {
                Elements.Add((VertexTypeVertexElementNode)aChildnode.AstNode);
            }

        }

        #endregion
    }
}
