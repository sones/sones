using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.Internal.Token;

namespace sones.GraphDB.Transaction
{
    public interface ITransactionManager
    {
        TransactionToken BeginTransaction(SessionToken mySessionToken, 
            Boolean         myLongrunning       = false, 
            IsolationLevel  myIsolationLevel    = IsolationLevel.Serializable);

        void CommitTransaction(SessionToken mySessionToken, 
            TransactionToken myTransactionToken);

        void RollbackTransaction(SessionToken mySessionToken,
            TransactionToken myTransactionToken);
    }
}
