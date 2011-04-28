using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Helper.Definition;
using sones.GraphQL.Structure.Nodes.Misc;
using sones.GraphQL.Structure.Nodes.DML;

namespace sones.GraphQL.Structure.Nodes.DDL
{
    /// <summary>
    /// This node is requested in case of an Create Type statement.
    /// </summary>
    public sealed class AttributeDefinitionNode : AStructureNode, IAstNodeInit
    {

        #region constructor

        public AttributeDefinitionNode()
        {

        }

        #endregion

        public AttributeDefinition AttributeDefinition { get; private set; }

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            AttributeDefinition = new AttributeDefinition(((GraphDBTypeNode)parseNode.ChildNodes[0].AstNode).DBTypeDefinition, parseNode.ChildNodes[1].Token.ValueString, ((AttrDefaultValueNode)parseNode.ChildNodes[2].AstNode));
        }

        #endregion
    }
}
