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
using sones.GraphQL.Result;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphQL.GQL.Manager.Plugin;
using System.Collections.Generic;
using sones.GraphQL.GQL.Structure.Helper.Definition.Update;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using sones.GraphQL.Structure.Nodes.Misc;
using sones.GraphQL.ErrorHandling;
using sones.GraphQL.Structure.Nodes.DML;
using sones.GraphQL.Structure.Nodes.Expressions;
using sones.GraphDB.TypeSystem;
using sones.GraphDB.Request;
using sones.GraphQL.GQL.Structure.Helper.ExpressionGraph;
using System.Diagnostics;
using sones.Library.PropertyHyperGraph;

namespace sones.GraphQL.StatementNodes.DML
{
    public sealed class InsertOrUpdateNode : AStatement, IAstNodeInit
    {
        #region Data

        private List<AAttributeAssignOrUpdate> _AttributeAssignList;
        private BinaryExpressionDefinition _WhereExpression;
        private String _Type;
        private string _query;
        
        #endregion

        #region Constructor

        public InsertOrUpdateNode()
        { }

        #endregion
        
        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (HasChildNodes(parseNode))
            {

                //get type
                if (parseNode.ChildNodes[1] != null && parseNode.ChildNodes[1].AstNode != null)
                {
                    _Type = ((AstNode)(parseNode.ChildNodes[1].AstNode)).AsString;
                }
                else
                {
                    throw new NotImplementedQLException("");
                }


                if (parseNode.ChildNodes[3] != null && HasChildNodes(parseNode.ChildNodes[3]))
                {

                    _AttributeAssignList = new List<AAttributeAssignOrUpdate>((parseNode.ChildNodes[3].AstNode as AttributeUpdateOrAssignListNode).ListOfUpdate.Select(e => e as AAttributeAssignOrUpdate));

                }

                if (parseNode.ChildNodes[4] != null && ((WhereExpressionNode)parseNode.ChildNodes[4].AstNode).BinaryExpressionDefinition != null)
                {
                    _WhereExpression = ((WhereExpressionNode)parseNode.ChildNodes[4].AstNode).BinaryExpressionDefinition;
                }

            }
        }

        private void NotImplementedQLException(System.Diagnostics.StackTrace stackTrace)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region AStatement Members

        public override string StatementName
        {
            get { return "InsertOrUpdate"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        public override QueryResult Execute(IGraphDB myGraphDB, IGraphQL myGraphQL, GQLPluginManager myPluginManager, String myQuery, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            var sw = Stopwatch.StartNew();

            _query = myQuery;

            IEnumerable<long> myToBeUpdatedVertices = null;
            //prepare
            var vertexType = myGraphDB.GetVertexType<IVertexType>(
                mySecurityToken,
                myTransactionToken,
                new RequestGetVertexType(_Type),
                (stats, vtype) => vtype);

            if (_WhereExpression != null)
            {
                //validate
                _WhereExpression.Validate(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, vertexType);

                //calculate
                var expressionGraph = _WhereExpression.Calculon(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, new CommonUsageGraph(myGraphDB, mySecurityToken, myTransactionToken), false);

                //extract
                myToBeUpdatedVertices = expressionGraph.SelectVertexIDs(new LevelKey(vertexType.ID, myGraphDB, mySecurityToken, myTransactionToken), null, true).ToList();
            }

            if (myToBeUpdatedVertices != null && myToBeUpdatedVertices.Count() > 0)
            {

                //update
                return ProcessUpdate(myToBeUpdatedVertices, myGraphDB, myPluginManager, mySecurityToken, myTransactionToken);

            }
            else
            {
                //insert
                return ProcessInsert(myGraphDB, myPluginManager, mySecurityToken, myTransactionToken);
            }

            sw.Stop();

            return GenerateResult(sw.Elapsed.TotalMilliseconds);
        }

        private QueryResult GenerateResult(double myElapsedTotalMilliseconds)
        {
            return new QueryResult(_query, SonesGQLConstants.GQL, Convert.ToUInt64(myElapsedTotalMilliseconds), ResultType.Successful, new List<IVertexView>());
        }

        private QueryResult ProcessInsert(IGraphDB myGraphDB, GQLPluginManager myPluginManager, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            InsertNode insert = new InsertNode();
            insert.Init(_Type, _AttributeAssignList);
            return insert.Execute(myGraphDB, null, myPluginManager, _query, mySecurityToken, myTransactionToken);
        }

        private QueryResult ProcessUpdate(IEnumerable<long> myVertexIDs, IGraphDB myGraphDB, GQLPluginManager myPluginManager, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            UpdateNode update = new UpdateNode();
            update.Init(_Type, _AttributeAssignList, myVertexIDs);

            return update.Execute(myGraphDB, null, myPluginManager, _query, mySecurityToken, myTransactionToken);
        }

        #endregion
    }
}
