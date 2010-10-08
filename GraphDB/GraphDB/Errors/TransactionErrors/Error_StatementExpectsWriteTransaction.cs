using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphFS.Transactions;

namespace sones.GraphDB.Errors
{
    public class Error_StatementExpectsWriteTransaction : GraphDBTransactionError
    {
        public String Statement { get; private set; }
        public IsolationLevel IsolationLevel { get; private set; }

        public Error_StatementExpectsWriteTransaction(String statement, IsolationLevel isolationLevel)
        {
            Statement = statement;
            IsolationLevel = isolationLevel;
        }

        public override string ToString()
        {
            return String.Format("The statement \"{0}\" expects an write transaction. Current transaction isolation level is \"{1}\"", Statement, IsolationLevel);
        }
    }
}
