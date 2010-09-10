/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
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
* 
*/

/* 
 * FSTransaction
 * (c) Achim Friedland, 2010
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.GraphFS.Errors;
using sones.GraphFS.Exceptions;
using sones.Lib.ErrorHandling;

#endregion

namespace sones.GraphFS.Transactions
{

    /// <summary>
    /// A file system transaction
    /// </summary>
    public class FSTransaction : ATransaction
    {

        #region Statics

        /// <summary>
        /// The main transaction object, might contains some nested transactions
        /// </summary>
        [ThreadStatic]
        public static FSTransaction Transaction;

        #endregion

        #region Data

        /// <summary>
        /// Subscribe to this event to get informed if the transaction was closed unexpected.
        /// The delegate must clean up all ressources of the disposed transaction (PreAllcoated tickets etc)
        /// </summary>
        public event TransactionDisposedHandler OnDispose;

        private IGraphFS        _TransactionFS;

        private FSTransaction   _NestedTransaction;

        #endregion

        #region Properties

        #region HasNestedTransaction

        /// <summary>
        /// Returns true if this transaction contains nested transactions
        /// </summary>
        public override Boolean HasNestedTransaction
        {

            get
            {

                if (_NestedTransaction == null)
                    return false;

                switch (_NestedTransaction._State)
                {

                    case TransactionState.Running :
                        return true;

                    case TransactionState.NestedTransaction :
                        return true;

                    case TransactionState.Committed :
                        return false;

                    case TransactionState.RolledBack :
                        return false;

                }

                return false;

            }

        }

        #endregion

        #endregion


        #region Constructors

        //#region FSTransaction()

        ///// <summary>
        ///// Creates a new transaction
        ///// </summary>
        //public FSTransaction()
        //    : this (false, false, IsolationLevel.Full, "", null)
        //{
        //}

        //#endregion

        #region FSTransaction(myIError)

        /// <summary>
        /// This will internal set Failed=True and Status=False
        /// </summary>
        /// <param name="myIError"></param>
        public FSTransaction(IError myIError)
            : base (myIError)
        { }

        #endregion

        #region FSTransaction(myIErrors)

        /// <summary>
        /// This will internal set Failed=True and Status=False
        /// </summary>
        /// <param name="myIErrors"></param>
        public FSTransaction(IEnumerable<IError> myIErrors)
            : base(myIErrors)
        { }

        #endregion

        #region FSTransaction(myDistributed, myLongRunning, myIsolationLevel, myName)

        /// <summary>
        /// Creates a new transaction having the given parameters.
        /// </summary>
        /// <param name="myDistributed">Indicates that this transaction should synched within the entire cluster.</param>
        /// <param name="myLongRunning">Indicates that this transaction is a long-running transaction.</param>
        /// <param name="myIsolationLevel">The isolation level of this transaction.</param>
        /// <param name="myName">A name or identification for this transaction.</param>
        /// <param name="myTimeStamp"></param>
        public FSTransaction(Boolean myDistributed = false, Boolean myLongRunning = false, IsolationLevel myIsolationLevel = IsolationLevel.Serializable, String myName = "", DateTime? myTimeStamp = null)
            : base (myDistributed, myLongRunning, myIsolationLevel, myName, myTimeStamp)
        {
            _NestedTransaction = null;
        }

        #endregion

        #endregion


        #region BeginNestedTransaction()

        /// <summary>
        /// Creates a nested transaction.
        /// </summary>
        public FSTransaction BeginNestedTransaction()
        {
            return BeginNestedTransaction(false, false, IsolationLevel.Serializable, "");
        }

        #endregion

        #region BeginNestedTransaction(myIsolationLevel)

        /// <summary>
        /// Creates a nested transaction having the given isolation level.
        /// </summary>
        /// <param name="myIsolationLevel">The isolation level of the nested transaction</param>
        public FSTransaction BeginNestedTransaction(IsolationLevel myIsolationLevel)
        {
            return BeginNestedTransaction(false, false, myIsolationLevel, "");
        }

        #endregion

        #region BeginNestedTransaction(myDistributed, myLongRunning, myIsolationLevel, myName)

        /// <summary>
        /// Creates a nested transaction having the given parameters.
        /// </summary>
        /// <param name="myDistributed">Indicates that the nested transaction should synched within the entire cluster.</param>
        /// <param name="myLongRunning">Indicates that the nested transaction is a long-running transaction.</param>
        /// <param name="myIsolationLevel">The isolation level of the nested transaction.</param>
        /// <param name="myName">A name or identification for the nested transaction.</param>
        public FSTransaction BeginNestedTransaction(Boolean myDistributed, Boolean myLongRunning, IsolationLevel myIsolationLevel, String myName, DateTime? myTimeStamp = null)
        {

            switch (State)
            {

                // Running => Rolledback
                case TransactionState.Running:
                    _NestedTransaction = new FSTransaction(myDistributed, myLongRunning, myIsolationLevel, myName, myTimeStamp);
                    return _NestedTransaction;


                // NestedTransactions => Error!
                // At the moment do not allow to auto-commit the nested transactions!
                case TransactionState.NestedTransaction:
                    return new FSTransaction(new GraphFSError_TransactionAlreadyRolledback());


                // Committing => Error!
                case TransactionState.Committing:
                    return new FSTransaction(new GraphFSError_TransactionCurrentlyCommitting());

                // Commited => Error!
                case TransactionState.Committed:
                    return new FSTransaction(new GraphFSError_TransactionAlreadyCommited());


                // RollingBack => Error!
                case TransactionState.RollingBack:
                    return new FSTransaction(new GraphFSError_TransactionCurrentlyRollingBack());

                // RolledBack => Error!
                case TransactionState.RolledBack:
                    return new FSTransaction(new GraphFSError_TransactionAlreadyRolledback());


                default:
                    throw new GraphFSException("FSTransaction.BeginNestedTransaction() is invalid!");

            }

        }

        #endregion

        #region GetNestedTransaction

        public FSTransaction GetNestedTransaction()
        {
            return _NestedTransaction;
        }

        #endregion

    }

}
