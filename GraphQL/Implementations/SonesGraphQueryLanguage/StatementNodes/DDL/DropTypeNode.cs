using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.Result;
using sones.GraphDB;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphQL.GQL.Manager.Plugin;
using sones.GraphDB.Request.DropType;
using sones.Library.ErrorHandling;

namespace sones.GraphQL.StatementNodes.DDL
{
    public sealed class DropTypeNode : AStatement, IAstNodeInit
    {
        #region Data

        //the name of the type that should be dropped
        String _TypeName = ""; 

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            #region get Name

            _TypeName = parseNode.ChildNodes[2].Token.ValueString;

            #endregion
        }

        #endregion

        #region AStatement Members

        public override string StatementName
        {
            get { return "DropType"; }
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
                var stat = myGraphDB.DropType(mySecurityToken, myTransactionToken, new RequestDropType(_TypeName), (stats) => stats);

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
