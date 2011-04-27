using System;
using Irony.Ast;
using Irony.Parsing;

namespace sones.GraphQL.Structure.Nodes.DML
{
    public sealed class OffsetNode : AStructureNode, IAstNodeInit
    {
        public UInt64? Count { get; private set; }

        public OffsetNode()
        {
            Count = null;
        }

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (HasChildNodes(parseNode))
            {
                if (parseNode.ChildNodes[1] != null)
                {
                    Count = Convert.ToUInt64(parseNode.ChildNodes[1].Token.Value);
                }
            }
        }

        #endregion
    }
}
