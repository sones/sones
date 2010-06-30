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

/* <id name="sones GraphDB – DescrSettNode" />
 * <copyright file="DescrSettNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Dirk Bludau</developer>
 * <summary></summary>
 */

#region Usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.TypeManagement;

using sones.GraphDB.ObjectManagement;
using sones.Lib.ErrorHandling;
using sones.GraphDB.Settings;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.Exceptions;
using sones.GraphDB.Errors;
#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalCLasses.Structure
{
    public class DescribeSettingNode : ADescrNode
    {
        #region Data
        private List<SelectionResultSet>    _SettingValues;
        #endregion

        #region contstructor
        public DescribeSettingNode()
        {            
        }
        #endregion

        #region AStructureNode

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            _SettingValues = new List<SelectionResultSet>();

            try
            {
                if (parseNode.HasChildNodes())
                {
                    ADescrNode Stmt = null;

                    if (parseNode.ChildNodes[1].AstNode is DescribeSettItemNode)
                        Stmt = (DescribeSettItemNode)parseNode.ChildNodes[1].AstNode;

                    if (parseNode.ChildNodes[1].AstNode is DescribeSettingsItemsNode)
                        Stmt = (DescribeSettingsItemsNode)parseNode.ChildNodes[1].AstNode;
                        
                    _SettingValues = Stmt.Result;
                }
            }
            catch(Exception e)
            {
                new GraphDBException(new Error_UnknownDBError(e));
            }
        }        
        #endregion

        #region Accessor
        public override List<SelectionResultSet> Result
        { get { return _SettingValues; } }
        #endregion

    }
}
