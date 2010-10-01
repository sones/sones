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

/* <id name="GraphDB – Partial select statement node" />
 * <copyright file="PartialSelectStmtNode.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Henning Rauch</developer>
 * <summary>This node is requested in case of a Partial select statement.</summary>
 */

#region Usings

using sones.GraphDB.GraphQL.StatementNodes.Select;


using sones.Lib.Frameworks.Irony.Parsing;
using sones.GraphDB.Managers.Structures;
using System.Collections.Generic;
using System;
using sones.GraphDB.Managers.Select;
using sones.GraphDB.Result;

#endregion

namespace sones.GraphDB.GraphQL.StructureNodes
{

    /// <summary>
    /// This node is requested in case of a Partial select statement.
    /// </summary>
    public class PartialSelectStmtNode : AStructureNode
    {

        #region Data

        QueryResult _queryResult = null;

        #endregion

        #region Properties
        public SelectDefinition SelectDefinition { get; private set; }

        #endregion

        #region constructor

        public PartialSelectStmtNode()
        {
            
        }

        #endregion

        public void GetContent(CompilerContext context, ParseTreeNode parseNode)
        {

            var aSelectNode = (SelectNode)parseNode.ChildNodes[0].AstNode;

            SelectDefinition = new Managers.Structures.SelectDefinition(aSelectNode.TypeList, aSelectNode.SelectedElements, aSelectNode.WhereExpressionDefinition, 
                aSelectNode.GroupByIDs, aSelectNode.Having, aSelectNode.Limit, aSelectNode.Offset, aSelectNode.OrderByDefinition, aSelectNode.ResolutionDepth);

        }

        public QueryResult ExecuteQuery(IGraphDBSession myGraphDBSession)
        {
            return myGraphDBSession.Select(SelectDefinition.SelectedElements, SelectDefinition.TypeList, SelectDefinition.WhereExpressionDefinition, SelectDefinition.GroupByIDs, SelectDefinition.Having, SelectDefinition.OrderByDefinition, SelectDefinition.Limit, SelectDefinition.Offset, SelectDefinition.ResolutionDepth);
        }

    }

}
