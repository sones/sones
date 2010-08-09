
#region Usings

using System;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.GraphQL.StructureNodes;
using sones.Lib.DataStructures;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class EdgeType_SortedNode : AStructureNode, IAstNodeInit
    {

        public Boolean IsSorted
        {
            get; private set;
        }

        public SortDirection SortDirection
        {
            get; private set;
        }

        private void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            if (!parseNode.HasChildNodes())
                return;

            IsSorted = true;
            if (parseNode.ChildNodes.Count == 3 && parseNode.ChildNodes[2].Token.Text.ToUpper() == GraphQL.GraphQueryLanguage.TERMINAL_DESC)
                SortDirection = SortDirection.Desc;
            else
                SortDirection = SortDirection.Asc;
        }

        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion
    
    }

}
