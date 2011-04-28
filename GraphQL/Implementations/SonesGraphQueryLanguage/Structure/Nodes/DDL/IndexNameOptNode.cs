using System;
using Irony.Ast;
using Irony.Parsing;

namespace sones.GraphQL.Structure.Nodes.DDL
{
    public sealed class IndexNameOptNode : AStructureNode, IAstNodeInit
    {
        private String _IndexName;
        public String IndexName
        {
            get { return _IndexName; }
        }

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (HasChildNodes(parseNode))
                _IndexName = parseNode.ChildNodes[0].Token.ValueString;
        }

        #endregion
    }
}
