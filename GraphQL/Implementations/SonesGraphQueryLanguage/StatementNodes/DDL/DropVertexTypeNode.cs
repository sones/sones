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
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using System.Linq;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.Library.ErrorHandling;
using sones.GraphDB.Request;
using System.Collections.Generic;

namespace sones.GraphQL.StatementNodes.DDL
{
    public sealed class DropVertexTypeNode : AStatement, IAstNodeInit
    {
        #region Data

        //the name of the type that should be dropped
        String _TypeName = String.Empty;

        String _query;

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            #region get Name

            _TypeName = parseNode.ChildNodes[3].Token.ValueString;

            #endregion
        }

        #endregion

        #region AStatement Members

        public override string StatementName
        {
            get { return "DropVertexType"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        public override QueryResult Execute(IGraphDB myGraphDB, 
                                            IGraphQL myGraphQL, 
                                            GQLPluginManager myPluginManager, 
                                            String myQuery, 
                                            SecurityToken mySecurityToken, 
                                            TransactionToken myTransactionToken)
        {
            _query = myQuery;

            return myGraphDB.DropVertexType(mySecurityToken,
                                            myTransactionToken, 
                                            new RequestDropVertexType(_TypeName), 
                                            GenerateOutput);
        }

        #endregion

        #region helper

        private QueryResult GenerateOutput(IRequestStatistics myStats, 
                                            Dictionary<Int64, String> myDeletedTypeIDs)
        {
            var temp = new Dictionary<String, object>();

            foreach(var item in myDeletedTypeIDs)
            {
                temp.Add("RemovedTypeID", item.Key);
                temp.Add("RemovedTypeName", item.Value);
            }

            return new QueryResult(_query, 
                                    SonesGQLConstants.GQL, 
                                    Convert.ToUInt64(myStats.ExecutionTime.Milliseconds), 
                                    ResultType.Successful, 
                                    new List<IVertexView> 
                                    { 
                                        new VertexView( temp, null )
                                    });
        }

        #endregion
    }
}
