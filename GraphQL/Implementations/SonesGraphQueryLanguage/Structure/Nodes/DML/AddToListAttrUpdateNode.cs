using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Helper.Definition.Update;

namespace sones.GraphQL.Structure.Nodes.DML
{
    /// <summary>
    /// This node is requested in case of an AddToListAttrUpdate node.
    /// </summary>
    public sealed class AddToListAttrUpdateNode : AStructureNode, IAstNodeInit
    {
        #region properties

        public AAttributeAssignOrUpdate AttributeUpdateList { get; protected set; }

        #endregion

        #region constructor

        public AddToListAttrUpdateNode()
        {

        }

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            var content = parseNode.FirstChild.AstNode as AddToListAttrUpdateNode;

            AttributeUpdateList = content.AttributeUpdateList;
        }

        #endregion
    }
}
