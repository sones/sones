using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Irony.Ast;
using sones.GraphQL.Result;
using Irony.Parsing;

namespace sones.GraphQL.StatementNodes.Transactions
{
    public sealed class CommitRollbackTransactionNode : AStatement, IAstNodeInit
    {
        #region IAstNodeInit Members

        public void Init(ParsingContext context, ParseTreeNode parseNode)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region AStatement Members

        public override string StatementName
        {
            get { throw new NotImplementedException(); }
        }

        public override TypesOfStatements TypeOfStatement
        {
            get { throw new NotImplementedException(); }
        }

        public override QueryResult Execute()
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
