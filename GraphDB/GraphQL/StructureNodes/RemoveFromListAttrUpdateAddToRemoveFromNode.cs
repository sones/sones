/* <id name="GraphDB – RemoveFromListAttrUpdateAddToRemoveFromNode" />
 * <copyright file="RemoveFromListAttrUpdateAddToRemoveFromNode.cs
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

    /// <summary>
    /// Warnings: Warning_ObsoleteGQL
    /// </summary>
    public class RemoveFromListAttrUpdateAddToRemoveFromNode : RemoveFromListAttrUpdateNode
    {
        
        public RemoveFromListAttrUpdateAddToRemoveFromNode()
        { }

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var idChain = ((IDNode)parseNode.ChildNodes[2].AstNode).IDChainDefinition;
            var tupleDefinition = ((TupleNode)parseNode.ChildNodes[3].AstNode).TupleDefinition;
            var AttrName = parseNode.ChildNodes[2].FirstChild.FirstChild.Token.ValueString;
            AttributeRemoveList = new Managers.Structures.AttributeRemoveList(idChain, AttrName, tupleDefinition);

            base.ParsingResult.PushIWarning(new Warnings.Warning_ObsoleteGQL("REMOVE FROM", "-="));
        }

    }

}
