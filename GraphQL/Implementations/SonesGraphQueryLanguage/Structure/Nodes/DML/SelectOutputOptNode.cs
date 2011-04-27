using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Helper.Enums;

namespace sones.GraphQL.Structure.Nodes.DML
{
    public sealed class SelectOutputOptNode : AStructureNode, IAstNodeInit
    {
        private SelectOutputTypes _SelectOutputType;
        public SelectOutputTypes SelectOutputType
        {
            get { return _SelectOutputType; }
        }

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (HasChildNodes(parseNode))
            {
                if (parseNode.ChildNodes[1].Token.ValueString.ToLower() == "graph")
                    _SelectOutputType = SelectOutputTypes.Graph;
                else
                    _SelectOutputType = SelectOutputTypes.Tree;
            }
        }

        #endregion
    }
}
