using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Transaction
{
    /// <summary>
    /// The state of a transaction
    /// </summary>
    public enum TransactionState
    {
        /// <summary>
        /// The transaction is running currently
        /// </summary>
        Running,
        
        /// <summary>
        /// The transaction is nested within another one
        /// </summary>
        NestedTransaction,
        
        /// <summary>
        /// The transaction commits currently
        /// </summary>
        Committing,
        
        /// <summary>
        /// The transaction is commited
        /// </summary>
        Committed,
        
        /// <summary>
        /// The transaction is rolling back currently
        /// </summary>
        RollingBack,
        
        /// <summary>
        /// The transaction has been rolled back
        /// </summary>
        RolledBack
    }
}
