/* <id name="GraphDB – RemoveFromListAttrUpdate Node" />
 * <copyright file="RemoveFromListAttrUpdateNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This node is requested in case of an RemoveFromListAttrUpdate node.</summary>
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
    public class RemoveFromListAttrUpdateNode : AStructureNode, IAstNodeInit
    {

        public AttributeRemoveList AttributeRemoveList { get; protected set; }

        #region constructor

        public RemoveFromListAttrUpdateNode()
        {

        }

        #endregion

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var content = parseNode.FirstChild.AstNode as RemoveFromListAttrUpdateNode;

            AttributeRemoveList = content.AttributeRemoveList;
            base.ParsingResult.PushIExceptional(content.ParsingResult);
        }


        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion

    }

}
