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
using sones.GraphQL.Structure.Nodes.DML;
using sones.GraphQL.Structure.Nodes.Expressions;
using sones.GraphDB.TypeSystem;
using sones.GraphDB.Request;
using sones.GraphQL.GQL.Structure.Helper.ExpressionGraph;
using sones.GraphQL.ErrorHandling;
using sones.Library.PropertyHyperGraph;

namespace sones.GraphQL.StatementNodes.DML
{
    public sealed class ReplaceNode : AStatement, IAstNodeInit
    {
        #region Data
        
        private BinaryExpressionDefinition _whereExpression;
        private String _TypeName;
        private List<AAttributeAssignOrUpdate> _AttributeAssignList;
        private string _query;

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            if (HasChildNodes(parseNode))
            {
                _TypeName = ((AstNode)(parseNode.ChildNodes[1].AstNode)).AsString;

                if (parseNode.ChildNodes[3] != null && HasChildNodes(parseNode.ChildNodes[3]))
                {
                    _AttributeAssignList = (parseNode.ChildNodes[3].AstNode as AttributeAssignListNode).AttributeAssigns;
                }

                _whereExpression = ((BinaryExpressionNode)parseNode.ChildNodes[5].AstNode).BinaryExpressionDefinition;

                System.Diagnostics.Debug.Assert(_whereExpression != null);
            }
        }

        #endregion

        #region AStatement Members

        public override string StatementName
        {
            get { return "Replace"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        public override IQueryResult Execute(IGraphDB myGraphDB, IGraphQL myGraphQL, GQLPluginManager myPluginManager, String myQuery, SecurityToken mySecurityToken, Int64 myTransactionToken)
        {
            _query = myQuery;

            //prepare
            var vertexType = myGraphDB.GetVertexType<IVertexType>(
                mySecurityToken,
                myTransactionToken,
                new RequestGetVertexType(_TypeName),
                (stats, vtype) => vtype);

            //validate
            _whereExpression.Validate(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, vertexType);

            //calculate
            var expressionGraph = _whereExpression.Calculon(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, new CommonUsageGraph(myGraphDB, mySecurityToken, myTransactionToken), false);

            //extract
            var myToBeUpdatedVertices = expressionGraph.Select(new LevelKey(vertexType.ID, myGraphDB, mySecurityToken, myTransactionToken), null, true).ToList();

            switch (myToBeUpdatedVertices.Count)
            {
                case 0:
                    //insert
                    throw new ReplaceException("There are no vertices that match the where expression, so it's not possible to execute a Replace statement. Try using InsertOrUpdate.");

                case 1:
                    //delete
                    ProcessDelete(myToBeUpdatedVertices[0], myGraphDB, myPluginManager, mySecurityToken, myTransactionToken);

                    //insert
                    return ProcessInsert(myGraphDB, myPluginManager, mySecurityToken, myTransactionToken);

                default:
                    //error
                    throw new NotImplementedQLException("It's currenty not implemented to InsertOrReplace more than one vertex");
            }
        }

        private IQueryResult ProcessInsert(IGraphDB myGraphDB, GQLPluginManager myPluginManager, SecurityToken mySecurityToken, Int64 myTransactionToken)
        {
            InsertNode insert = new InsertNode();
            insert.Init(_TypeName, _AttributeAssignList);
            return insert.Execute(myGraphDB, null, myPluginManager, _query, mySecurityToken, myTransactionToken);
        }

        private void ProcessDelete(IVertex toBeDeletedVertexID, IGraphDB myGraphDB, GQLPluginManager myPluginManager, SecurityToken mySecurityToken, Int64 myTransactionToken)
        {
            myGraphDB.Delete<bool>(
                mySecurityToken,
                myTransactionToken,
                new RequestDelete(new RequestGetVertices(toBeDeletedVertexID.VertexTypeID, new List<long> { toBeDeletedVertexID.VertexID })),
                (stats, attributes, vertices) => true);
        }

        #endregion
    }
}
