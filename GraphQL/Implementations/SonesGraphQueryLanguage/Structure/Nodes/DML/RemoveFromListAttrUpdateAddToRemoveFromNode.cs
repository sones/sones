using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.Structure.Nodes.Misc;
using sones.GraphQL.Structure.Nodes.Expressions;
using sones.GraphQL.GQL.Structure.Helper.Definition.Update;

namespace sones.GraphQL.Structure.Nodes.DML
{
    public sealed class RemoveFromListAttrUpdateAddToRemoveFromNode : RemoveFromListAttrUpdateNode
    {
        public RemoveFromListAttrUpdateAddToRemoveFromNode()
        { }
        
        public void DirectInit(ParsingContext context, ParseTreeNode parseNode)
        {
            var idChain = ((IDNode)parseNode.ChildNodes[2].AstNode).IDChainDefinition;
            var tupleDefinition = ((TupleNode)parseNode.ChildNodes[3].AstNode).TupleDefinition;
            var AttrName = parseNode.ChildNodes[2].FirstChild.FirstChild.Token.ValueString;
            ToBeRemovedList = new AttributeRemoveList(idChain, AttrName, tupleDefinition);
        }
    }
}
