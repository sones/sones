using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.Structure.Nodes.Misc;
using sones.GraphQL.GQL.Structure.Helper.Definition.Update;

namespace sones.GraphQL.Structure.Nodes.DML
{
    public sealed class RemoveFromListAttrUpdateAddToOperatorNode : RemoveFromListAttrUpdateNode
    {
        public RemoveFromListAttrUpdateAddToOperatorNode()
        { }

        public void DirectInit(ParsingContext context, ParseTreeNode parseNode)
        {
            var idChain = ((IDNode)parseNode.ChildNodes[0].AstNode).IDChainDefinition;
            var AttrName = parseNode.ChildNodes[0].FirstChild.FirstChild.Token.ValueString;
            var definition = ((RemoveFromListAttrUpdateScopeNode)parseNode.ChildNodes[1].AstNode).TupleDefinition;
            ToBeRemovedList = new AttributeRemoveList(idChain, AttrName, definition);
        }

    }
}
