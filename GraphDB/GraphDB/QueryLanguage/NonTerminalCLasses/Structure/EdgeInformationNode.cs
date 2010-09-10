/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
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
* 
*/

/* <id name="GraphDB – Attribute Definition astnode" />
 * <copyright file="EdgeTraversalNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This node is requested in case of attribute definition statement.</summary>
 */

#region Usings

using System;
using sones.GraphDB.TypeManagement;
using sones.Lib.Frameworks.Irony.Parsing;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure
{
    /// <summary>
    /// This node is requested in case of an Create Type statement.
    /// </summary>
    public class EdgeInformationNode : AStructureNode
    {
        #region Data

        private String                  _EdgeInformationName = null;
        private SelectionDelimiterNode  _DelimiterNode = null;

        #endregion

        #region constructor

        public EdgeInformationNode()
        {
            
        }

        #endregion

        public void GetContent(CompilerContext myCompilerContext, ParseTreeNode myParseTreeNode)
        {
            _DelimiterNode = (SelectionDelimiterNode)myParseTreeNode.FirstChild.AstNode;
            _EdgeInformationName = myParseTreeNode.ChildNodes[1].Token.ValueString;
        }

        public String                   EdgeInformationName { get { return _EdgeInformationName; } }
        public SelectionDelimiterNode   Delimiter           { get { return _DelimiterNode; } }        
    }
}
