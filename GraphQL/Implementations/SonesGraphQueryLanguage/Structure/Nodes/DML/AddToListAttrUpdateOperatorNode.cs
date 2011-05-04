using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.GQL.Structure.Helper.Definition;
using sones.GraphQL.Structure.Nodes.Misc;

namespace sones.GraphQL.Structure.Nodes.DML
{
    public sealed class AddToListAttrUpdateOperatorNode : AddToListAttrUpdateNode, IAstNodeInit
    {
        public AddToListAttrUpdateOperatorNode()
        { }

        public new void DirectInit(ParsingContext context, ParseTreeNode parseNode)
        {
            AttributeUpdateList = new AttributeAssignOrUpdateList(((CollectionOfDBObjectsNode)parseNode.ChildNodes[2].AstNode).CollectionDefinition, ((IDNode)parseNode.ChildNodes[0].AstNode).IDChainDefinition, false);
        }
    }
}
