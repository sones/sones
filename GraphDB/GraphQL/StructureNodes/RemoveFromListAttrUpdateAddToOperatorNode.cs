/* <id name="GraphDB – RemoveFromListAttrUpdateAddToOperatorNode" />
 * <copyright file="RemoveFromListAttrUpdateAddToOperatorNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 */

#region Usings

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class RemoveFromListAttrUpdateAddToOperatorNode : RemoveFromListAttrUpdateNode
    {

        public RemoveFromListAttrUpdateAddToOperatorNode()
        { }

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var idChain = ((IDNode)parseNode.ChildNodes[0].AstNode).IDChainDefinition;
            var AttrName = parseNode.ChildNodes[0].FirstChild.FirstChild.Token.ValueString;
            var tupleDefinition = ((TupleNode)parseNode.ChildNodes[2].AstNode).TupleDefinition;
            AttributeRemoveList = new Managers.Structures.AttributeRemoveList(idChain, AttrName, tupleDefinition);
        }

    }

}
