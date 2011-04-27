using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.Structure.Nodes.Expressions;

namespace sones.GraphQL.Structure.Nodes.DML
{
    /// <summary>
    /// This node is requested in case of an Create Type statement.
    /// </summary>
    public sealed class EdgeTraversalNode : AStructureNode, IAstNodeInit
    {
        #region Data

        public String AttributeName { get; private set; }
        public FuncCallNode FuncCall { get; private set; }
        public SelectionDelimiterNode Delimiter { get; private set; }

        #endregion

        #region constructor

        public EdgeTraversalNode()
        {

        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            Delimiter = (SelectionDelimiterNode)parseNode.FirstChild.AstNode;

            if (parseNode.ChildNodes[1].AstNode == null)
            {
                //AttributeName
                AttributeName = parseNode.ChildNodes[1].Token.ValueString;
            }

            else
            {
                FuncCall = (FuncCallNode)parseNode.ChildNodes[1].AstNode;
            }
        }

        #endregion
    }
}
