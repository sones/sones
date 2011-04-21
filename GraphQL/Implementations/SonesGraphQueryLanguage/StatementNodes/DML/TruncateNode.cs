using System;
using Irony.Ast;
using Irony.Parsing;
using sones.GraphQL.Result;
using sones.GraphDB;
using sones.Library.ErrorHandling;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;

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
            throw new NotImplementedException();
            
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

        public override QueryResult Execute()
        {
            throw new NotImplementedException();
        }

        public QueryResult Execute(IGraphDB myGraphDB, String myQuery, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            QueryResult qresult = null;
            
            try
            {
                var stat = myGraphDB.Truncate(mySecurityToken, myTransactionToken, new RequestTruncate(_TypeName), (stats) => stats);

                qresult = new QueryResult(myQuery, "GQL", Convert.ToUInt64(stat.ExecutionTime.Milliseconds));
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
