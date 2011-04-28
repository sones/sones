using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Helper.Enums;

namespace sones.GraphQL.Structure.Nodes.DDL
{
    public sealed class EdgeType_SortedNode : AStructureNode, IAstNodeInit
    {
        public Boolean IsSorted
        {
            get;
            private set;
        }

        public SortDirection SortDirection
        {
            get;
            private set;
        }

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (!HasChildNodes(parseNode))
                return;

            IsSorted = true;
            if (parseNode.ChildNodes.Count == 3 && parseNode.ChildNodes[2].Token.Text.ToUpper() == SonesGQLGrammar.TERMINAL_DESC)
                SortDirection = SortDirection.Desc;
            else
                SortDirection = SortDirection.Asc;
        }

        #endregion
    }
}
