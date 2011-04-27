using System;
using Irony.Ast;
using Irony.Parsing;

namespace sones.GraphQL.Structure.Nodes.DML
{
    /// <summary>
    /// This node is requested in case of an Create Type statement.
    /// </summary>
    public sealed class EdgeInformationNode : AStructureNode, IAstNodeInit
    {
        #region Data

        public String EdgeInformationName { get; private set; }
        public SelectionDelimiterNode Delimiter { get; private set; }

        #endregion

        #region constructor

        public EdgeInformationNode()
        {

        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            Delimiter = (SelectionDelimiterNode)parseNode.FirstChild.AstNode;
            EdgeInformationName = parseNode.ChildNodes[1].Token.ValueString;
        }

        #endregion
    }
}
