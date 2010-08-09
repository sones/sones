
#region Usings

using System;

using sones.GraphDB.GraphQL.StructureNodes;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class IndexNameOptNode : AStructureNode, IAstNodeInit
    {

        private String _IndexName;
        public String IndexName
        {
            get { return _IndexName; }
        }

        private void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            if (parseNode.HasChildNodes())
                _IndexName = parseNode.ChildNodes[0].Token.ValueString;
        }

        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion

    }

}
