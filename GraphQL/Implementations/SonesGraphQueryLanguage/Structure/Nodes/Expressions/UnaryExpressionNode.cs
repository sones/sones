using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using sones.GraphQL.ErrorHandling;

namespace sones.GraphQL.Structure.Nodes.Expressions
{
    public sealed class UnaryExpressionNode : AStructureNode, IAstNodeInit
    {
        #region Data

        private String _OperatorSymbol;
        private Object _Term;

        #endregion

        public UnaryExpressionDefinition UnaryExpressionDefinition { get; private set; }

        #region Accessor

        public String OperatorSymbol { get; private set; }
        public AExpressionDefinition Expression { get; private set; }

        #endregion

        #region Constructor

        public UnaryExpressionNode()
        { }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (HasChildNodes(parseNode))
            {

                if (parseNode.ChildNodes[1].AstNode == null)
                {
                    throw new NotImplementedQLException("");
                }

                _OperatorSymbol = parseNode.ChildNodes[0].Token.Text;
                Expression = GetExpressionDefinition(parseNode.ChildNodes[1]);

            }

            System.Diagnostics.Debug.Assert(Expression != null);

            UnaryExpressionDefinition = new UnaryExpressionDefinition(_OperatorSymbol, Expression);
        }

        #endregion
    }
}
