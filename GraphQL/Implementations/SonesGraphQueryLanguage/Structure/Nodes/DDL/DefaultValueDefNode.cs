using System;
using Irony.Ast;
using Irony.Parsing;

namespace sones.GraphQL.Structure.Nodes.DDL
{
    /// <summary>
    /// This node just hold the value of a DEFAULT parameter of a list definition.
    /// </summary>
    public sealed class DefaultValueDefNode : AStructureNode, IAstNodeInit
    {
        /// <summary>
        /// The default value.
        /// </summary>
        public Object Value { get; private set; }

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            Value = parseNode.ChildNodes[2].Token.Value;
        }

        #endregion
    }
}
