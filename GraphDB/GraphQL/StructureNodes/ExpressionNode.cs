/* <id name="PandoraDB – Term node" />
 * <copyright file="TermNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This node is requested in case of Term statement.</summary>
 */

#region Usings

using sones.GraphDB.Managers.Structures;
using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    /// <summary>
    /// This node is requested in case of an Term statement.
    /// </summary>
    class ExpressionNode : AStructureNode, IAstNodeInit
    {

        public AExpressionDefinition ExpressionDefinition { get; private set; }

        private void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            ExpressionDefinition = GetExpressionDefinition(parseNode.ChildNodes[0]);
        }

        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion
    
    }

}
