using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.Result;
using sones.Library.Commons.Transaction;
using sones.Library.Commons.Security;
using sones.GraphDB;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphQL.Structure.Nodes.Misc;
using sones.GraphQL.GQL.ErrorHandling;
using System.Diagnostics;
using sones.Library.ErrorHandling;
using sones.GraphDB.Request.DropIndex;

namespace sones.GraphQL.StatementNodes.DDL
{
    public sealed class DropIndexNode : AStatement, IAstNodeInit
    {
        #region Data

        String _IndexName = String.Empty;
        String _IndexEdition = null;
        String _TypeName = null;

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            _TypeName = ((ATypeNode)parseNode.ChildNodes[1].AstNode).ReferenceAndType.TypeName;

            _IndexName = parseNode.ChildNodes[4].Token.ValueString;
            
            if (HasChildNodes(parseNode.ChildNodes[5]))
            {
                _IndexEdition = parseNode.ChildNodes[5].ChildNodes[1].Token.ValueString;
            }
        }

        #endregion

        #region AStatement Members

        public override string StatementName
        {
            get { return "DropIndex"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        public override QueryResult Execute(IGraphDB myGraphDB, IGraphQL myGraphQL, GQLPluginManager myPluginManager, String myQuery, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            QueryResult qresult = null;
            ASonesException error = null;

            try
            {
                var stat = myGraphDB.DropIndex(mySecurityToken, myTransactionToken, new RequestDropIndex(_TypeName, _IndexName, _IndexEdition), (stats) => stats);

                qresult = new QueryResult(myQuery, "sones.gql", Convert.ToUInt64(stat.ExecutionTime.Milliseconds), ResultType.Successful);
            }
            catch (ASonesException e)
            {
                error = e;
            }

            return new QueryResult(myQuery, "sones.gql", qresult.Duration, qresult.TypeOfResult, qresult.Vertices, error);
        }

        #endregion

    }
}
