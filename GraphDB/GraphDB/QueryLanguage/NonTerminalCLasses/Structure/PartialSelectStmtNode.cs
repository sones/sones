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

/* <id name="sones GraphDB – Partial select statement node" />
 * <copyright file="PartialSelectStmtNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH 2007-2010
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This node is requested in case of a Partial select statement.</summary>
 */

#region Usings

using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using sones.GraphDB.TypeManagement;
using sones.GraphDB.QueryLanguage.Result;
using sones.GraphDB.QueryLanguage.NonTerminalClasses.Statements;

using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.ObjectManagement;
using sones.Lib.DataStructures;
using sones.GraphDB.QueryLanguage.NonTerminalCLasses.Statements.Select;

#endregion

namespace sones.GraphDB.QueryLanguage.NonTerminalClasses.Structure
{
    /// <summary>
    /// This node is requested in case of a Partial select statement.
    /// </summary>
    class PartialSelectStmtNode : AStructureNode
    {
        #region Data

        QueryResult _queryResult = null;

        #endregion

        #region constructor

        public PartialSelectStmtNode()
        {
            
        }

        #endregion

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {
            var dbContext = context.IContext as DBContext;
            var typeManager = dbContext.DBTypeManager;

            var aSelectNode = (SelectNode)parseNode.ChildNodes[0].AstNode;

            var dbObjectCache = dbContext.DBObjectCache;

            _queryResult = aSelectNode.Execute(null , dbContext);

           dbObjectCache = null;
        }

        public QueryResult QueryResult { get { return _queryResult; } }
    }
}
