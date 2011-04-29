using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;

namespace sones.GraphQL.Structure.Nodes.Expressions
{
    public sealed class BinaryExpressionNode : AStructureNode, IAstNodeInit
    {
        #region Data

        private String _OperatorSymbol;
        private AExpressionDefinition _left = null;
        private AExpressionDefinition _right = null;
        private String OriginalString = String.Empty;

        #endregion

        #region Properties

        public BinaryExpressionDefinition BinaryExpressionDefinition { get; private set; }

        #endregion

        #region constructor

        public BinaryExpressionNode()
        {

        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            #region set type of binary expression

            _OperatorSymbol = parseNode.ChildNodes[1].FirstChild.Token.ValueString;
            _left = GetExpressionDefinition(parseNode.ChildNodes[0]);
            _right = GetExpressionDefinition(parseNode.ChildNodes[2]);

            #endregion

            BinaryExpressionDefinition = new BinaryExpressionDefinition(_OperatorSymbol, _left, _right);

            OriginalString += _left.ToString() + " ";
            OriginalString += _OperatorSymbol + " ";
            OriginalString += _right.ToString();
        }

        #endregion
    }
}
