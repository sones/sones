using System;
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

        public override QueryResult Execute(IGraphDB myGraphDB, IGraphQL myGraphQL, GQLPluginManager myPluginManager, String myQuery, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            var expressionGraph = _WhereExpression.Calculon(myPluginManager, myGraphDB, mySecurityToken,
                                                            myTransactionToken,
                                                            new CommonUsageGraph(myGraphDB, mySecurityToken,
                                                                                 myTransactionToken));

            var vertexType = myGraphDB.GetVertexType<IVertexType>(
                mySecurityToken,
                myTransactionToken,
                new RequestGetVertexType(_typeName),
                (stats, vType) => vType);

            var toBeDeletedVertices =
                expressionGraph.SelectVertexIDs(
                    new LevelKey(vertexType.ID, myGraphDB, mySecurityToken, myTransactionToken),
                    null, true);

            return myGraphDB.Delete<QueryResult>(
                mySecurityToken,
                myTransactionToken,
                new RequestDelete(new RequestGetVertices(_typeName, toBeDeletedVertices), _toBeDeletedAttributes),
                CreateQueryResult);
        }

        #endregion

        #region private helper

        private QueryResult CreateQueryResult(IRequestStatistics myStats)
        {
            return new QueryResult(_query, SonesGQLConstants.GQL, Convert.ToUInt64(myStats.ExecutionTime.TotalMilliseconds), ResultType.Successful, new List<IVertexView>());
        }

        #endregion
    }
}
