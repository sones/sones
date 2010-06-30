/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
*/



#region Usings

using System;
using sones.GraphFS.Transactions;

#endregion

namespace GraphFSInterface.Transactions
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
