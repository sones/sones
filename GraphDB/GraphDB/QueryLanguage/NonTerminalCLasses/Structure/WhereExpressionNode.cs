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

/* <id name="sones GraphDB – WhereExpressionNode" />
 * <copyright file="WhereExpressionNode.cs
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <developer>Daniel Kirstenpfad
 * <summary>This node is requested in case of where clause.</summary>
 */

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using sones.GraphDB.TypeManagement;

using sones.Lib.Frameworks.Irony.Scripting.Runtime;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.Lib.Frameworks.Irony.Scripting.Ast;

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure
{
    /// <summary>
    /// 
    /// </summary>
    public class WhereExpressionNode : AStructureNode
    {
        private BinaryExpressionNode _binExprNode = null;

        public WhereExpressionNode()
        {

        }

        /// <summary>
        /// This handles the Where Expression Node with all the
        /// </summary>
        /// <param name="context"></param>
        /// <param name="parseNode"></param>
        /// <param name="typeManager"></param>
        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            if (parseNode.HasChildNodes())
            {
                _binExprNode = (BinaryExpressionNode)parseNode.ChildNodes[1].AstNode;
            }
        }

        public override string ToString()
        {
            return "whereClauseOpt";
        }

        public BinaryExpressionNode BinExprNode { get { return _binExprNode; } }

    }//class
}//namespace
