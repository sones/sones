using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Helper.Definition.Update;

namespace sones.GraphQL.Structure.Nodes.DML
{
    /// <summary>
    /// This node is requested in case of an RemoveFromListAttrUpdate node.
    /// </summary>
    public sealed class RemoveFromListAttrUpdateNode : AStructureNode, IAstNodeInit
    {
        public AttributeRemoveList AttributeRemoveList { get; protected set; }

        #region constructor

        public RemoveFromListAttrUpdateNode()
        {

        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            var content = parseNode.FirstChild.AstNode as RemoveFromListAttrUpdateNode;

            AttributeRemoveList = content.AttributeRemoveList;
        }

        #endregion
    }
}
