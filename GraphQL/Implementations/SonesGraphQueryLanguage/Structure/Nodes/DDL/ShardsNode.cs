using System;
using Irony.Ast;
using Irony.Parsing;

namespace sones.GraphQL.Structure.Nodes.DDL
{
    public sealed class ShardsNode : AStructureNode, IAstNodeInit
    {
        private UInt16? _Shards;
        public UInt16? Shards
        {
            get { return _Shards; }
        }

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            _Shards = null;
            if (parseNode.ChildNodes != null && parseNode.ChildNodes.Count != 0)
            {
                _Shards = UInt16.Parse(parseNode.ChildNodes[1].Token.ValueString);
            }
        }

        #endregion
    }
}
