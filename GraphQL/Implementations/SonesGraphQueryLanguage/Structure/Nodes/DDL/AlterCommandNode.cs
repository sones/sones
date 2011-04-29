using System;
using Irony.Ast;
using Irony.Parsing;

namespace sones.GraphQL.Structure.Nodes.DDL
{
    public sealed class AlterCommandNode : AStructureNode, IAstNodeInit
    {
        #region Data

        //public AAlterTypeCommand AlterTypeCommand { get; set; }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
