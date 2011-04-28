using System;
using Irony.Ast;
using Irony.Parsing;

namespace sones.GraphQL.Structure.Nodes.DDL
{
    public sealed class EditionOptNode : AStructureNode, IAstNodeInit
    {
        public String IndexEdition { get; private set; }

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (HasChildNodes(parseNode))
                IndexEdition = parseNode.ChildNodes[1].Token.ValueString;
        }

        #endregion
    }
}
