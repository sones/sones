/* <id name="PandoraDB – Term node" />
 * <copyright file="TermNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Stefan Licht</developer>
 * <summary>This node is requested in case of Term statement.</summary>
 */

#region Usings

using System.Collections.Generic;

using sones.GraphDB.Managers.Structures;
using sones.GraphDB.GraphQL.StructureNodes;
using sones.GraphDB.TypeManagement.BasicTypes;

using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    /// <summary>
    /// This node is requested in case of an Term statement.
    /// </summary>
    class ExpressionOfAListNode : AStructureNode, IAstNodeInit
    {

        #region Data

        ParseTreeNode _ParseTreeNode = null;
        ParametersNode _ParametersNode = null;

        #endregion

        /// <summary>
        /// A list of parameters which will be passed during an insert operation to the ListEdgeType
        /// Currently only ADBBaseObject is provided
        /// </summary>
        public List<ADBBaseObject> Parameters { get; private set; }
        public AExpressionDefinition ExpressionDefinition { get; private set; }

        private void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            _ParseTreeNode = parseNode.ChildNodes[0];

            ExpressionDefinition = GetExpressionDefinition(_ParseTreeNode);

            if (parseNode.ChildNodes[1].HasChildNodes())
            {
                Parameters = ((ParametersNode)parseNode.ChildNodes[1].AstNode).ParameterValues;
            }
        }

        #region IAstNodeInit Members

        public void Init(CompilerContext context, ParseTreeNode parseNode)
        {
            GetContent(context, parseNode);
        }

        #endregion

    }

}
