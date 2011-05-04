using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.Result;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphQL.GQL.Structure.Nodes.Expressions;
using sones.GraphQL.Structure.Nodes.Misc;
using sones.GraphQL.Structure.Nodes.Expressions;
using System.Diagnostics;
using sones.GraphQL.GQL.ErrorHandling;
using System.Collections.Generic;
using sones.Library.ErrorHandling;
using sones.GraphQL.GQL.Structure.Helper.Definition.Update;
using sones.GraphQL.Structure.Nodes.DML;
using sones.GraphDB.Request;

namespace sones.GraphQL.StatementNodes.DML
{
    public sealed class UpdateNode : AStatement, IAstNodeInit
    {
        #region Data

        private HashSet<AAttributeAssignOrUpdateOrRemove> _listOfUpdates;

        private BinaryExpressionDefinition _WhereExpression;

        private String _TypeName;

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            #region get Type

            _TypeName = (parseNode.ChildNodes[1].AstNode as ATypeNode).ReferenceAndType.TypeName;

            #endregion

            #region get myAttributes

            if (HasChildNodes(parseNode.ChildNodes[3]))
            {
                var AttrUpdateOrAssign = (AttributeUpdateOrAssignListNode)parseNode.ChildNodes[3].AstNode;
                _listOfUpdates = AttrUpdateOrAssign.ListOfUpdate;
            }

            #endregion

            #region whereClauseOpt

            if (parseNode.ChildNodes[4].ChildNodes != null && parseNode.ChildNodes[4].ChildNodes.Count != 0)
            {
                var tempWhereNode = (WhereExpressionNode)parseNode.ChildNodes[4].AstNode;
                _WhereExpression = tempWhereNode.BinaryExpressionDefinition;
            }

            #endregion
        }

        #endregion

        #region AStatement Members

        public override string StatementName
        {
            get { return "Update"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        public override QueryResult Execute(IGraphDB myGraphDB, IGraphQL myGraphQL, GQLPluginManager myPluginManager, String myQuery, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            var sw = Stopwatch.StartNew();

            QueryResult result = null;
            var stat = myGraphDB.Update(mySecurityToken, myTransactionToken, new RequestUpdate(_TypeName), (stats) => stats);

            sw.Stop();

            if (result != null)
                return new QueryResult(myQuery, "GQL", (ulong)sw.ElapsedMilliseconds, result.TypeOfResult, result.Vertices, result.Error);
            else
                return new QueryResult(myQuery, "GQL", (ulong)sw.ElapsedMilliseconds, ResultType.Failed, new List<IVertexView>(), new GQLStatementNodeExecutionException(myQuery, this, "QueryResult is null."));
        }

        #endregion
    }
}
