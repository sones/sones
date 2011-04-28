using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.ErrorHandling;

namespace sones.GraphQL.Structure.Nodes.DML
{
    /// <summary>
    /// Contains the default value for an attribute.
    /// </summary>
    public sealed class AttrDefaultValueNode : AStructureNode, IAstNodeInit
    {

        #region Properties

        public string Value { get; private set; }

        #endregion

        #region constructors

        public AttrDefaultValueNode()
        { }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (HasChildNodes(parseNode))
            {
                Value = parseNode.ChildNodes[1].Token.ValueString;
            }
            else
            {
                Value = null;
            }
        }

        #endregion
    }
}
