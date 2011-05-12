/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.Result;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using sones.GraphQL.StatementNodes.DML;

namespace sones.GraphQL.Structure.Nodes.DML
{
    public sealed class PartialSelectStmtNode : AStructureNode, IAstNodeInit
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

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            var aSelectNode = (SelectNode)parseNode.ChildNodes[0].AstNode;

            SelectDefinition = new SelectDefinition(aSelectNode.TypeList, aSelectNode.SelectedElements, aSelectNode.WhereExpressionDefinition,
                aSelectNode.GroupByIDs, aSelectNode.Having, aSelectNode.Limit, aSelectNode.Offset, aSelectNode.OrderByDefinition, aSelectNode.ResolutionDepth);

        }

        #endregion
    }
}
