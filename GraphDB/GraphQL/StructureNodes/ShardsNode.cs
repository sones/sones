
#region Usings

using System;

using sones.GraphDB.GraphQL.StructureNodes;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class ShardsNode : AStructureNode, IAstNodeInit
    {

        private UInt16? _Shards;
        public UInt16? Shards
        {
            get { return _Shards; }
        }

        private void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            _Shards = null;
            if (parseNode.HasChildNodes())
            {
                _Shards = UInt16.Parse(parseNode.ChildNodes[1].Token.ValueString);
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
