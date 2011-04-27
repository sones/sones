using System;
using Irony.Ast;
using Irony.Parsing;

namespace sones.GraphQL.Structure.Nodes.DML
{
    public sealed class LimitNode : AStructureNode, IAstNodeInit
    {
        public UInt64? Count { get; private set; }

        public LimitNode()
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
