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
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphQL.GQL.Structure.Helper.Definition;
using sones.GraphQL.GQL.Structure.Nodes.DML;
using System.Diagnostics;
using sones.Library.ErrorHandling;
using System.Collections.Generic;

namespace sones.GraphQL.StatementNodes.DML
{
    public sealed class DescribeNode : AStatement, IAstNodeInit
    {
        #region Data

        private ADescribeDefinition _DescribeDefinition;

        #endregion

        #region Constructor

        public DescribeNode()
        {
        }

        #endregion        

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            _DescribeDefinition = ((ADescrNode)parseNode.ChildNodes[1].AstNode).DescribeDefinition;
        }

        #endregion

        #region AStatement Members

        public override string StatementName
        {
            get { return "Describe"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.Readonly; }
        }

        public override IQueryResult Execute(IGraphDB myGraphDB, 
                                            IGraphQL myGraphQL, 
                                            GQLPluginManager myPluginManager, 
                                            String myQuery, 
                                            SecurityToken mySecurityToken, 
                                            Int64 myTransactionToken)
        {
            var sw = Stopwatch.StartNew();

            try
            {
                var result = _DescribeDefinition.GetResult(myPluginManager,
                                                        myGraphDB, 
                                                        mySecurityToken, 
                                                        myTransactionToken);
                return QueryResult.Success(myQuery, SonesGQLConstants.GQL, result, Convert.ToUInt64(sw.ElapsedMilliseconds));
            }
            catch (ASonesException ex)
            {
                return QueryResult.Failure(myQuery, SonesGQLConstants.GQL, ex);
            }
        }

        #endregion
    }
}
