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

    public class AddToListAttrUpdateOperatorNode : AddToListAttrUpdateNode
    {

        public AddToListAttrUpdateOperatorNode()
        { }

        public new void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            AttributeUpdateList = new AttributeAssignOrUpdateList(((CollectionOfDBObjectsNode)parseNode.ChildNodes[2].AstNode).CollectionDefinition, ((IDNode)parseNode.ChildNodes[0].AstNode).IDChainDefinition, false);
        }
    
    }

}
