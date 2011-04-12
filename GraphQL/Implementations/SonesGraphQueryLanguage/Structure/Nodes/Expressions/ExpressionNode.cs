using System;
using Irony.Ast;
using Irony.Parsing;

namespace sones.GraphQL.Structure.Nodes.Expressions
{
    public sealed class ExpressionNode : AStructureNode, IAstNodeInit
    {
        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
