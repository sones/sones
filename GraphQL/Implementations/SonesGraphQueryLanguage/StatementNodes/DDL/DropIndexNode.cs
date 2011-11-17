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
using sones.Library.Commons.Transaction;
using sones.Library.Commons.Security;
using sones.GraphDB;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphQL.Structure.Nodes.Misc;
using sones.GraphQL.GQL.ErrorHandling;
using System.Diagnostics;
using sones.Library.ErrorHandling;
using sones.GraphDB.Request;

namespace sones.GraphQL.StatementNodes.DDL
{
    public sealed class DropIndexNode : AStatement, IAstNodeInit
    {
        #region Data

        String _IndexName = String.Empty;
        String _IndexEdition = null;
        String _TypeName = null;

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            _TypeName = ((ATypeNode)parseNode.ChildNodes[1].AstNode).ReferenceAndType.TypeName;

            _IndexName = parseNode.ChildNodes[4].Token.ValueString;
            
            if (HasChildNodes(parseNode.ChildNodes[5]))
            {
                _IndexEdition = parseNode.ChildNodes[5].ChildNodes[1].Token.ValueString;
            }
        }

        #endregion

        #region AStatement Members

        public override string StatementName
        {
            get { return "DropIndex"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        public override IQueryResult Execute(IGraphDB myGraphDB, IGraphQL myGraphQL, GQLPluginManager myPluginManager, String myQuery, SecurityToken mySecurityToken, Int64 myTransactionToken)
        {

            try
            {
                var stat = myGraphDB.DropIndex(mySecurityToken, myTransactionToken, new RequestDropIndex(_TypeName, _IndexName, _IndexEdition), (stats) => stats);

                return QueryResult.Success(myQuery, SonesGQLConstants.GQL, null, Convert.ToUInt64(stat.ExecutionTime.Milliseconds));
            }
            catch (ASonesException ex)
            {
                return QueryResult.Failure(myQuery, SonesGQLConstants.GQL, ex);
            }
        }

        #endregion

    }
}
