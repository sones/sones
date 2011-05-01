using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.Structure.Nodes.Expressions;

namespace sones.GraphQL.Structure.Nodes.DML
{
    public sealed class HavingExpressionNode : AStructureNode, IAstNodeInit
    {
        public BinaryExpressionNode BinExprNode { get; private set; }

        public HavingExpressionNode()
        { }

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (HasChildNodes(parseNode))
            {
                BinExprNode = (BinaryExpressionNode)parseNode.ChildNodes[1].AstNode;
            }
        }

        #endregion
    }
}
