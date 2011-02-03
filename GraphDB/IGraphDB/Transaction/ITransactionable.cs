using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Security;

namespace sones.GraphDB.Transaction
{
    public interface ITransactionable
    {
        /// <summary>
        /// Starts a new transaction
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myLongrunning">Is this a long running transaction</param>
        /// <param name="myIsolationLevel">The isolation level</param>
        /// <returns>A transaction token</returns>
        TransactionToken BeginTransaction(SecurityToken mySecurityToken, 
            Boolean         myLongrunning       = false, 
            IsolationLevel  myIsolationLevel    = IsolationLevel.Serializable);

        /// <summary>
        /// Commits a transaction
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The transaction token that identifies the transaction that shoulb be commited</param>
        void CommitTransaction(SecurityToken mySecurityToken, 
            TransactionToken myTransactionToken);

        /// <summary>
        /// Rollback of a transaction
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The transaction token that identifies the transaction that should be rolled back</param>
        void RollbackTransaction(SecurityToken mySecurityToken,
            TransactionToken myTransactionToken);
    }
}
