using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.Structure.Nodes.DML;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using System.Collections.Generic;

namespace sones.GraphQL.Structure.Nodes.Expressions
{
    /// <summary>
    /// This node is requested in case of an Term statement.
    /// </summary>
    public sealed class ExpressionOfAListNode : AStructureNode, IAstNodeInit
    {
        #region Data

        ParseTreeNode _ParseTreeNode = null;
        ParametersNode _ParametersNode = null;

        #endregion

        /// <summary>
        /// A list of parameters which will be passed during an insert operation to the ListEdgeType
        /// Currently only ADBBaseObject is provided
        /// </summary>
        public Dictionary<string, object> Parameters { get; private set; }
        public AExpressionDefinition ExpressionDefinition { get; private set; }

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            _ParseTreeNode = parseNode.ChildNodes[0];

            ExpressionDefinition = GetExpressionDefinition(_ParseTreeNode);

            if (HasChildNodes(parseNode.ChildNodes[1]))
            {
                Parameters = ((ParametersNode)parseNode.ChildNodes[1].AstNode).ParameterValues;
            }
        }

        #endregion
    }
}
