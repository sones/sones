using System;
using Irony.Ast;
using Irony.Parsing;

namespace sones.GraphQL.Structure.Nodes.DDL
{
    public sealed class UniqueOptNode : AStructureNode, IAstNodeInit
    {
        private Boolean _IsUnique;
        public Boolean IsUnique
        {
            get { return _IsUnique; }
        }

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (parseNode.ChildNodes !=  null && parseNode.ChildNodes.Count != 0)
                _IsUnique = true;

            else
                _IsUnique = false;
        }

        #endregion
    }
}
