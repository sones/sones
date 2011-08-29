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
using System.Collections.Generic;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphDB;
using sones.GraphDB.Request;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using sones.GraphQL.Result;
using sones.GraphQL.Structure.Nodes.Expressions;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphQL.GQL.Structure.Helper.ExpressionGraph;
using sones.GraphDB.TypeSystem;
using sones.Library.CollectionWrapper;

namespace sones.GraphQL.StatementNodes.DML
{
    public sealed class DeleteNode : AStatement, IAstNodeInit
    {
        #region Data

        private String _query;

        private BinaryExpressionDefinition _WhereExpression;

        private List<String> _toBeDeletedAttributes;

        private String _typeName;

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            _toBeDeletedAttributes = new List<String>();

            _typeName = parseNode.ChildNodes[1].Token.ValueString;

            if (HasChildNodes(parseNode.ChildNodes[3]))
            {
                foreach (var _ParseTreeNode in parseNode.ChildNodes[3].ChildNodes[0].ChildNodes)
                {
                    _toBeDeletedAttributes.Add(_ParseTreeNode.ChildNodes[0].Token.ValueString);
                }
            }

            #region whereClauseOpt

            if (parseNode.ChildNodes[4].ChildNodes != null && parseNode.ChildNodes[4].ChildNodes.Count != 0)
            {
                WhereExpressionNode tempWhereNode = (WhereExpressionNode)parseNode.ChildNodes[4].AstNode;
                _WhereExpression = tempWhereNode.BinaryExpressionDefinition;
            }

            #endregion
        }

        #endregion

        #region AStatement Members

        public override string StatementName
        {
            get { return "Delete"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        public override QueryResult Execute(IGraphDB myGraphDB, IGraphQL myGraphQL, GQLPluginManager myPluginManager, String myQuery, SecurityToken mySecurityToken, Int64 myTransactionToken)
        {
            _query = myQuery;

            var vertexType = myGraphDB.GetVertexType<IVertexType>(
                mySecurityToken,
                myTransactionToken,
                new RequestGetVertexType(_typeName),
                (stats, vType) => vType);

            _WhereExpression.Validate(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, vertexType);

            var expressionGraph = _WhereExpression.Calculon(myPluginManager, myGraphDB, mySecurityToken,
                                                            myTransactionToken,
                                                            new CommonUsageGraph(myGraphDB, mySecurityToken,
                                                                                 myTransactionToken));

            var toBeDeletedVertices =
                expressionGraph.Select(
                    new LevelKey(vertexType.ID, myGraphDB, mySecurityToken, myTransactionToken),
                    null, true);

            //TODO: do sth that is better than that: ew RequestDelete(new RequestGetVertices(_typeName, toBeDeletedVertices.Select(_ => _.VertexID))).
            return myGraphDB.Delete<QueryResult>(
                mySecurityToken,
                myTransactionToken,
                new RequestDelete(new RequestGetVertices(_typeName, toBeDeletedVertices.Select(_ => _.VertexID))).AddAttributes(_toBeDeletedAttributes),
                CreateQueryResult);
        }

        #endregion

        #region private helper

        private QueryResult CreateQueryResult(IRequestStatistics myStats, IEnumerable<IComparable> myDeletedAttributes, IEnumerable<IComparable> myDeletedVertices)
        {
            var view = new List<VertexView>();

            if(myDeletedVertices.Count() > 0)
                view.Add(new VertexView(new Dictionary<String, Object> { { "deleted vertices", new ListCollectionWrapper(myDeletedVertices) } }, null));

            if(myDeletedAttributes.Count() > 0)
                view.Add(new VertexView(new Dictionary<String, Object> { { "deleted attributes", new ListCollectionWrapper(myDeletedAttributes) } }, null));

            return new QueryResult(_query, SonesGQLConstants.GQL, Convert.ToUInt64(myStats.ExecutionTime.TotalMilliseconds), ResultType.Successful, view);
        }

        #endregion
    }
}
