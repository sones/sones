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

/* <id name="GraphDB – Attribute Definition astnode" />
 * <copyright file="EdgeTraversalNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This node is requested in case of attribute definition statement.</summary>
 */

#region Usings

using System;
using sones.GraphDB.TypeManagement;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure
{
    /// <summary>
    /// This node is requested in case of an Create Type statement.
    /// </summary>
    public class EdgeTraversalNode : AStructureNode
    {
        #region Data

        public String AttributeName { get; private set; }
        public FuncCallNode FuncCall { get; private set; }
        public SelectionDelimiterNode Delimiter { get; private set; }
        
        #endregion

        #region constructor

        public EdgeTraversalNode()
        {
            
        }

        #endregion

        public void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {
            Delimiter = (SelectionDelimiterNode)myParseTreeNode.FirstChild.AstNode;

            if (myParseTreeNode.ChildNodes[1].AstNode == null)
            {
                //AttributeName
                AttributeName = myParseTreeNode.ChildNodes[1].Token.ValueString;
            }
            else
            {
                FuncCall = (FuncCallNode)myParseTreeNode.ChildNodes[1].AstNode;
            }
        }
                
    }
}
