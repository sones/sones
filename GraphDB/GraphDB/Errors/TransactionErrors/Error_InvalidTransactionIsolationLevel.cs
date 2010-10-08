using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors.Transactions
{
    public class Error_InvalidTransactionIsolationLevel : GraphDBTransactionError
    {
        public String IsolationLevel { get; private set; }

        public Error_InvalidTransactionIsolationLevel(String isolationLevel)
        {
            IsolationLevel = isolationLevel;
        }

        public override string ToString()
        {
            return IsolationLevel;
        }
    }
}
