using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.Structure.Helper.Enums;

namespace sones.GraphQL.Structure.Nodes.DML
{
    public sealed class SelectionDelimiterNode : AStructureNode, IAstNodeInit
    {
        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            throw new NotImplementedException();
        }

        #endregion

        internal void SetDelimiter(KindOfDelimiter kindOfDelimiter)
        {
            throw new NotImplementedException();
        }
    }
}
