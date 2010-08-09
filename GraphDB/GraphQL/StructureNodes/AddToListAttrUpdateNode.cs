/* <id name="GraphDB – AddToListAttrUpdate Node" />
 * <copyright file="AddToListAttrUpdateNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This node is requested in case of an AddToListAttrUpdate node.</summary>
 */

#region usings

using sones.GraphDB.Managers.Structures;
using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    public class AddToListAttrUpdateNode : AStructureNode, IAstNodeInit
    {
        
        #region properties

        public AAttributeAssignOrUpdate AttributeUpdateList { get; protected set; }

        #endregion

        #region constructor

        public AddToListAttrUpdateNode()
        {

        }

        #endregion

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var content = parseNode.FirstChild.AstNode as AddToListAttrUpdateNode;

            AttributeUpdateList = content.AttributeUpdateList;

        }

        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion

    }

}
