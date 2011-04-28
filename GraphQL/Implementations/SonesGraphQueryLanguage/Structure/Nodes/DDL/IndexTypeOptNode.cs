using System;
using Irony.Ast;
using Irony.Parsing;

namespace sones.GraphQL.Structure.Nodes.DDL
{
    public sealed class IndexTypeOptNode : AStructureNode, IAstNodeInit
    {
        public String IndexType { get; private set; }

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (HasChildNodes(parseNode))
            {

                IndexType = parseNode.ChildNodes[1].Token.ValueString;

            }
        }

        #endregion
    }
}
