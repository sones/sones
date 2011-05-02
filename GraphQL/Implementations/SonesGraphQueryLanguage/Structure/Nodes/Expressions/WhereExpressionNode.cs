using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;

namespace sones.GraphQL.Structure.Nodes.Expressions
{
    /// <summary>
    /// This node is requested in case of where clause.
    /// </summary>
    public sealed class WhereExpressionNode : AStructureNode, IAstNodeInit
    {
        public BinaryExpressionDefinition BinaryExpressionDefinition { get; private set; }

        public WhereExpressionNode()
        { }

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (HasChildNodes(parseNode))
            {

                if (parseNode.ChildNodes[1].AstNode is TupleNode && (parseNode.ChildNodes[1].AstNode as TupleNode).TupleDefinition.TupleElements.Count == 1)
                {
                    var tuple = (parseNode.ChildNodes[1].AstNode as TupleNode).TupleDefinition.Simplyfy();
                    BinaryExpressionDefinition = (tuple.TupleElements[0].Value as BinaryExpressionDefinition);
                }
                else if (parseNode.ChildNodes[1].AstNode is BinaryExpressionNode)
                {
                    BinaryExpressionDefinition = ((BinaryExpressionNode)parseNode.ChildNodes[1].AstNode).BinaryExpressionDefinition;
                }
            }
        }

        #endregion
    }
}
