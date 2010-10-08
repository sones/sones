
#region Usings

using sones.GraphDB.GraphQL.StructureNodes;
using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class HavingExpressionNode : AStructureNode, IAstNodeInit
    {

        public BinaryExpressionNode BinExprNode { get; private set; }

        public HavingExpressionNode()
        { }

        /// <summary>
        /// This handles the Where Expression Node with all the
        /// </summary>
        /// <param name="context"></param>
        /// <param name="parseNode"></param>
        /// <param name="typeManager"></param>
        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            if (parseNode.HasChildNodes())
            {
                BinExprNode = (BinaryExpressionNode)parseNode.ChildNodes[1].AstNode;
            }
        }

        public override string ToString()
        {
            return "havingClauseOpt";
        }

        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion

    }

}
