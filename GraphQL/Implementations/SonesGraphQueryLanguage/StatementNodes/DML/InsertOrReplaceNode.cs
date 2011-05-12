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
		
        private BinaryExpressionDefinition _WhereExpression;
        private List<AAttributeAssignOrUpdate> _AttributeAssignList;
        private String _Type;
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
                    _Type = ((ATypeNode)(parseNode.ChildNodes[1].AstNode)).ReferenceAndType.TypeName;
                }
                else
                {
                    throw new NotImplementedQLException("");
                }

                if (parseNode.ChildNodes[3] != null && HasChildNodes(parseNode.ChildNodes[3]))
                {

                    _AttributeAssignList = (parseNode.ChildNodes[3].AstNode as AttributeAssignListNode).AttributeAssigns;

                }

                if (parseNode.ChildNodes[4] != null && ((WhereExpressionNode)parseNode.ChildNodes[4].AstNode).BinaryExpressionDefinition != null)
                {
                    _WhereExpression = ((WhereExpressionNode)parseNode.ChildNodes[4].AstNode).BinaryExpressionDefinition;

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
            _query = myQuery;

            //prepare
            var vertexType = myGraphDB.GetVertexType<IVertexType>(
                mySecurityToken,
                myTransactionToken,
                new RequestGetVertexType(_Type),
                (stats, vtype) => vtype);

            //validate
            _WhereExpression.Validate(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, vertexType);

            //calculate
            var expressionGraph = _WhereExpression.Calculon(myPluginManager, myGraphDB, mySecurityToken, myTransactionToken, new CommonUsageGraph(myGraphDB, mySecurityToken, myTransactionToken), false);

            //extract
            var myToBeUpdatedVertices = expressionGraph.SelectVertexIDs(new LevelKey(vertexType.ID, myGraphDB, mySecurityToken, myTransactionToken), null, true).ToList();

            switch (myToBeUpdatedVertices.Count)
            {
                case 0:
                    //insert
                    return ProcessInsert(myGraphDB, myPluginManager, mySecurityToken, myTransactionToken);

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

        private QueryResult ProcessInsert(IGraphDB myGraphDB, GQLPluginManager myPluginManager, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            InsertNode insert = new InsertNode();
            insert.Init(_Type, _AttributeAssignList);
            return insert.Execute(myGraphDB, null, myPluginManager, _query, mySecurityToken, myTransactionToken);
        }

        private void ProcessDelete(long toBeDeletedVertexID, IGraphDB myGraphDB, GQLPluginManager myPluginManager, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            myGraphDB.Delete<bool>(
                mySecurityToken,
                myTransactionToken, 
                new RequestDelete(new RequestGetVertices(_Type, new List<long>{toBeDeletedVertexID})),
                (stats) => true);
        }

        #endregion
    }
}
