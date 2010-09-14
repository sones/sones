
#region Usings

using System;
using System.Text;

#endregion

namespace sones.GraphFS.Transactions
{

    /// <summary>
    /// The state of a transaction
    /// </summary>
    public enum TransactionState
    {
        Running,
        NestedTransaction,
        Committing,
        Committed,
        RollingBack,
        RolledBack
    }

}
