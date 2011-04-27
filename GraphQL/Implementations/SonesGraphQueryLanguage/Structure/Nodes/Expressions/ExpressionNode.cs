using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;

namespace sones.GraphQL.Structure.Nodes.Expressions
{
    /// <summary>
    /// This node is requested in case of an Term statement.
    /// </summary>
    public sealed class ExpressionNode : AStructureNode, IAstNodeInit
    {
        public AExpressionDefinition ExpressionDefinition { get; private set; }

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            ExpressionDefinition = GetExpressionDefinition(parseNode.ChildNodes[0]);
        }

        #endregion
    }
}
