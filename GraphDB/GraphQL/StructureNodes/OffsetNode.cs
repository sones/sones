
#region Usings

using System;

using sones.GraphDB.GraphQL.StructureNodes;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class OffsetNode : AStructureNode, IAstNodeInit
    {

        public UInt64? Count { get; private set; }

        public OffsetNode()
        {
            Count = null;
        }

        private void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            if (parseNode.HasChildNodes())
            {
                if (parseNode.ChildNodes[1] != null)
                {
                    Count = Convert.ToUInt64(parseNode.ChildNodes[1].Token.Value);
                }
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
