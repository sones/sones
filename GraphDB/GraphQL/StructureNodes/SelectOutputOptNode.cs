
#region Usings

using sones.GraphDB.GraphQL.StructureNodes;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public enum SelectOutputTypes
    {
        Tree,
        Graph
    }

    public class SelectOutputOptNode : AStructureNode, IAstNodeInit
    {

        private SelectOutputTypes _SelectOutputType;
        public SelectOutputTypes SelectOutputType
        {
            get { return _SelectOutputType; }
        }

        private void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            if (parseNode.HasChildNodes())
            {
                if (parseNode.ChildNodes[1].Token.ValueString.ToLower() == "graph")
                    _SelectOutputType = SelectOutputTypes.Graph;
                else
                    _SelectOutputType = SelectOutputTypes.Tree;
            }

        }

        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion

    }

}
