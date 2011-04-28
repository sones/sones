using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.Result;
using sones.GraphDB;
using sones.Library.ErrorHandling;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using System.Linq;
using sones.GraphQL.GQL.Manager.Plugin;

namespace sones.GraphQL.StatementNodes.DML
{
    public sealed class TruncateNode : AStatement, IAstNodeInit
    {
        #region Data

        private String _TypeName = ""; //the name of the type that should be dropped

        #endregion

        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            var grammar = (SonesGQLGrammar)context.Language.Grammar;

            // get Name
            _TypeName = parseNode.ChildNodes.Last().Token.ValueString;
        }

        #endregion

        #region AStatement Members

        public override string StatementName
        {
            get { return "Truncate"; }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { return TypesOfStatements.ReadWrite; }
        }

        public override QueryResult Execute(IGraphDB myGraphDB, IGraphQL myGraphQL, GQLPluginManager myPluginManager, String myQuery, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            QueryResult qresult = null;
            
            try
            {
                var stat = myGraphDB.Truncate(mySecurityToken, myTransactionToken, new RequestTruncate(_TypeName), (stats) => stats);

                qresult = new QueryResult(myQuery, "sones.gql", Convert.ToUInt64(stat.ExecutionTime.Milliseconds),ResultType.Successful);
            }
            catch(ASonesException e)
            {
                qresult.Error = e;
            }

            return qresult;
        }

        #endregion

    }
}
