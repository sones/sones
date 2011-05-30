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
using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using sones.GraphQL.GQL.Structure.Helper.Definition.Update;
using System.Collections.Generic;
using sones.GraphQL.Structure.Nodes.Misc;
using sones.GraphQL.ErrorHandling;
using sones.GraphQL.Structure.Nodes.DML;
using sones.GraphQL.Structure.Nodes.Expressions;
using sones.GraphDB.TypeSystem;
using sones.GraphDB.Request;
using sones.GraphQL.GQL.Structure.Helper.ExpressionGraph;
using System.Diagnostics;

namespace sones.GraphQL.StatementNodes.DML
{
    public sealed class InsertOrReplaceNode : AStatement, IAstNodeInit
    {
        #region data
		
        private BinaryExpressionDefinition _whereExpression;
        private List<AAttributeAssignOrUpdate> _attributeAssignList;
        private String _type;
        private String _query;

	    #endregion

        #region Constructor

        public InsertOrReplaceNode()
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
                    _type = ((AstNode)(parseNode.ChildNodes[1].AstNode)).AsString;
                }
                else
                {
                    throw new NotImplementedQLException("");
                }

                if (parseNode.ChildNodes[3] != null && HasChildNodes(parseNode.ChildNodes[3]))
                {

                    _attributeAssignList = (parseNode.ChildNodes[3].AstNode as AttributeAssignListNode).AttributeAssigns;

                }

                if (parseNode.ChildNodes[4] != null && ((WhereExpressionNode)parseNode.ChildNodes[4].AstNode).BinaryExpressionDefinition != null)
                {
                    _whereExpression = ((WhereExpressionNode)parseNode.ChildNodes[4].AstNode).BinaryExpressionDefinition;

                }
            }   
        }

        #endregion

        #region AStatement Members

        public override string StatementName
        {
            get { return "InsertOrReplace"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        public override QueryResult Execute(IGraphDB myGraphDB, IGraphQL myGraphQL, GQLPluginManager myPluginManager, String myQuery, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            Stopwatch sw = Stopwatch.StartNew();

            _query = myQuery;
            QueryResult result = null;
            String myAction = "";
            List<long> myToBeUpdatedVertices = new List<long>();
            
            //prepare
            var vertexType = myGraphDB.GetVertexType<IVertexType>(
                mySecurityToken,
                myTransactionToken,
                new RequestGetVertexType(_type),
                (stats, vtype) => vtype);

            if (_whereExpression != null)
            {
                //validate
                _whereExpression.Validate(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, vertexType);

                //calculate
                var expressionGraph = _whereExpression.Calculon(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, new CommonUsageGraph(myGraphDB, mySecurityToken, myTransactionToken), false);

                //extract
                myToBeUpdatedVertices = expressionGraph.SelectVertexIDs(new LevelKey(vertexType.ID, myGraphDB, mySecurityToken, myTransactionToken), null, true).ToList();
            }

            switch (myToBeUpdatedVertices.Count)
            {
                case 0:
                    
                    //insert
                    result = ProcessInsert(myGraphDB, myPluginManager, mySecurityToken, myTransactionToken);

                    myAction = "Inserted";

                    break;

                case 1:
                
                    //delete
                    ProcessDelete(myToBeUpdatedVertices[0], myGraphDB, myPluginManager, mySecurityToken, myTransactionToken);

                    //insert
                    result = ProcessInsert(myGraphDB, myPluginManager, mySecurityToken, myTransactionToken);

                    myAction = "Replaced";

                    break;

                default:
                    //error
                    throw new NotImplementedQLException("It's currenty not implemented to InsertOrReplace more than one vertex");
            }

            if (result.Error != null)
                throw result.Error;

            return GenerateResult(sw.ElapsedMilliseconds, result, myAction);
        }

        private QueryResult ProcessInsert(IGraphDB myGraphDB, GQLPluginManager myPluginManager, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            InsertNode insert = new InsertNode();

            insert.Init(_type, _attributeAssignList);

            return insert.Execute(myGraphDB, null, myPluginManager, _query, mySecurityToken, myTransactionToken);
        }

        private void ProcessDelete(long toBeDeletedVertexID, IGraphDB myGraphDB, GQLPluginManager myPluginManager, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            var stat = myGraphDB.Delete(mySecurityToken,
                                         myTransactionToken,
                                         new RequestDelete(new RequestGetVertices(_type, new List<long> {toBeDeletedVertexID})),
                                         (stats) => stats);
        }

        private QueryResult GenerateResult(double myElapsedTotalMilliseconds, QueryResult myResult, String myAction)
        {
            List<IVertexView> view = new List<IVertexView>();

            if (myResult != null)
            {
                foreach (var item in myResult.Vertices)
                {
                    var dict = new Dictionary<string, object>();

                    if (item.HasProperty("VertexID"))
                        dict.Add("VertexID", item.GetProperty<IComparable>("VertexID"));

                    if (item.HasProperty("VertexTypeID"))
                        dict.Add("VertexTypeID", item.GetProperty<IComparable>("VertexTypeID"));

                    dict.Add("Action", myAction);

                    view.Add(new VertexView(dict, null));
                }
            }

            return new QueryResult(_query, SonesGQLConstants.GQL, Convert.ToUInt64(myElapsedTotalMilliseconds),
                                   ResultType.Successful, view);
        }

        #endregion
    }
}
