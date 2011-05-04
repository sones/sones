using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Helper.Definition;
using sones.GraphQL.Structure.Nodes.Misc;

namespace sones.GraphQL.Structure.Nodes.DML
{
    public sealed class AddToListAttributeUpdateAddToNode : AddToListAttrUpdateNode, IAstNodeInit
    {
        public AddToListAttributeUpdateAddToNode()
        {

        }

        public void DirectInit(ParsingContext context, ParseTreeNode parseNode)
        {
            var _elementsToBeAdded = (CollectionOfDBObjectsNode)parseNode.ChildNodes[3].AstNode;
            var _AttrName = parseNode.ChildNodes[2].FirstChild.FirstChild.Token.ValueString;

            AttributeUpdateList = new AttributeAssignOrUpdateList(_elementsToBeAdded.CollectionDefinition, ((IDNode)parseNode.ChildNodes[2].AstNode).IDChainDefinition, false);
        }
    }
}
