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
using System.Linq;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphDB;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphQL.Result;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.ErrorHandling;

namespace sones.GraphQL.StatementNodes.DML
{
    public sealed class TruncateNode : AStatement, IAstNodeInit
    {
        #region Data

        private String _TypeName = ""; //the name of the type that should be dropped

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            // get Name
            _TypeName = parseNode.ChildNodes.Last().Token.ValueString;
        }

        #endregion

        #region AStatement Members

        public override string StatementName
        {
            get { return "Truncate"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        public override QueryResult Execute(IGraphDB myGraphDB, IGraphQL myGraphQL, GQLPluginManager myPluginManager, String myQuery, SecurityToken mySecurityToken, Int64 myTransactionToken)
        {
            QueryResult result = new QueryResult(myQuery, SonesGQLConstants.GQL, 0L, ResultType.Failed);
            
            try
            {
                var stat = myGraphDB.Truncate(mySecurityToken, myTransactionToken, new RequestTruncate(_TypeName), (stats) => stats);

                result = new QueryResult(myQuery, SonesGQLConstants.GQL, Convert.ToUInt64(stat.ExecutionTime.Milliseconds), ResultType.Successful);
            }
            catch(ASonesException e)
            {
                result = new QueryResult(myQuery, SonesGQLConstants.GQL, 0, ResultType.Failed, null, e);
            }

            return result;
        }

        #endregion

    }
}
