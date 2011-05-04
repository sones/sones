using System;
using Irony.Ast;
using Irony.Parsing;
using System.Collections.Generic;
using sones.GraphQL.GQL.Structure.Helper.Definition.Update;

namespace sones.GraphQL.Structure.Nodes.DML
{
    /// <summary>
    /// This node is requested in case of an AttrRemove node.
    /// </summary>
    public sealed class AttrRemoveNode : AStructureNode, IAstNodeInit
    {
        #region properties

        public AttributeRemove AttributeRemove { get; private set; }

        #endregion

        #region constructor

        public AttrRemoveNode()
        {

        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            var _toBeRemovedAttributes = new List<string>();

            foreach (ParseTreeNode aParseTreeNode in parseNode.ChildNodes[2].ChildNodes)
            {
                _toBeRemovedAttributes.Add(aParseTreeNode.Token.ValueString);
            }

            AttributeRemove = new AttributeRemove(_toBeRemovedAttributes);
        }

        #endregion
    }
}
