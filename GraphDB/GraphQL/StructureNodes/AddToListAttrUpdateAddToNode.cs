/* <id name="GraphDB – AddToListAttrUpdateAddToNode" />
 * <copyright file="AddToListAttrUpdateAddToNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 */

#region Usings

using sones.GraphDB.Managers.Structures;
using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    /// <summary>
    /// Warnings: Warning_ObsoleteGQL
    /// </summary>
    public class AddToListAttrUpdateAddToNode : AddToListAttrUpdateNode
    {

        public AddToListAttrUpdateAddToNode()
        {

        }

        public new void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var _elementsToBeAdded = (CollectionOfDBObjectsNode)parseNode.ChildNodes[3].AstNode;
            var _AttrName = parseNode.ChildNodes[2].FirstChild.FirstChild.Token.ValueString;

            AttributeUpdateList = new AttributeAssignOrUpdateList(_elementsToBeAdded.CollectionDefinition, ((IDNode)parseNode.ChildNodes[2].AstNode).IDChainDefinition, false);

            base.ParsingResult.PushIWarning(new Warnings.Warning_ObsoleteGQL("ADD TO", "+="));
        }

    }

}
