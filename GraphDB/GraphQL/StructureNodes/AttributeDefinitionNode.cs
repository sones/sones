/* <id name="GraphDB – Attribute Definition astnode" />
 * <copyright file="AttributeDefinitionNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This node is requested in case of attribute definition statement.</summary>
 */

#region Usings

using sones.GraphDB.Managers.Structures;
using sones.GraphDB.GraphQL.StructureNodes;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    /// <summary>
    /// This node is requested in case of an Create Type statement.
    /// </summary>
    public class AttributeDefinitionNode : AStructureNode
    {

        #region constructor

        public AttributeDefinitionNode()
        {
            
        }

        #endregion

        public AttributeDefinition AttributeDefinition { get; private set; }

        public void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {
            AttributeDefinition = new AttributeDefinition(((GraphDBTypeNode)myParseTreeNode.ChildNodes[0].AstNode).DBTypeDefinition, myParseTreeNode.ChildNodes[1].Token.ValueString, ((AttrDefaultValueNode)myParseTreeNode.ChildNodes[2].AstNode).Value);
        }
        
    }

}
