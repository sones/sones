
#region Usings

using System;

#endregion

namespace sones.GraphFS.Transactions
{

    /// <summary>
    /// Extensions for the transaction classes
    /// </summary>
    public static class TransactionExtensions
    {

        //#region StateIs(this myTransaction, myTransactionState)

        ///// <summary>
        ///// Checks if a transaction has the given state
        ///// </summary>
        ///// <param name="myATransaction">A ATransaction object</param>
        ///// <param name="myTransactionState">The requested state of the transaction</param>
        ///// <returns>true|false</returns>
        //public static Boolean StateIs(this ATransaction myATransaction, TransactionState myTransactionState)
        //{

        //    if (myATransaction == null)
        //        return false;

        //    return myATransaction.State == myTransactionState;

        //}

        //#endregion

        #region IsRunning(this myATransaction)

        /// <summary>
        /// True if the transaction is in running state or false if the transaction is null or not in running state
        /// </summary>
        /// <param name="myATransaction">A ATransaction object</param>
        /// <returns>True for a running transaction</returns>
        public static Boolean IsRunning(this ATransaction myATransaction)
        {
            return (myATransaction != null && (myATransaction.State == TransactionState.Running || myATransaction.State == TransactionState.NestedTransaction));
        }

        #endregion

        #region IsReadonly

        public static Boolean IsReadonly(this ATransaction transaction)
        {
            if (transaction != null && (transaction.IsolationLevel == IsolationLevel.ReadCommitted || transaction.IsolationLevel == IsolationLevel.ReadUncommitted))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        #endregion

    }

}
