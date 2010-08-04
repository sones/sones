/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
*/

/* <id name="sones GraphDB – Term node" />
 * <copyright file="TermNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Stefan Licht</developer>
 * <summary>This node is requested in case of Term statement.</summary>
 */

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Frameworks.Irony.Scripting.Ast;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;
using sones.GraphDB.Managers.Structures;
using sones.GraphDB.TypeManagement.PandoraTypes;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure
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
