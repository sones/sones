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


/*
 * ATransaction
 * Achim Friedland, 2010
 */

#region Usings

using System;
using System.Collections.Generic;

using sones.GraphFS.DataStructures;
using sones.GraphFS.Errors;
using sones.GraphFS.Exceptions;

using sones.Lib.ErrorHandling;
using sones.Lib.DataStructures.Timestamp;

#endregion

namespace sones.GraphFS.Transactions
{

    /// <summary>
    /// Abstract implementation of a transaction
    /// </summary>

    public abstract class ATransaction : Exceptional, IDisposable
    {

        #region Properties

        #region UUID

        /// <summary>
        /// The UUID of this transaction
        /// </summary>
        public TransactionUUID      UUID            { get; protected set; }

        #endregion

        #region Created

        /// <summary>
        /// The timestamp this transaction was created
        /// </summary>
        public DateTime             Created         { get; protected set; }

        #endregion

        #region Finished

        /// <summary>
        /// The timestamp this transaction was finished
        /// </summary>
        public DateTime             Finished        { get; protected set; }

        #endregion

        #region Distributed

        /// <summary>
        /// This transaction should synched within the entire cluster.
        /// </summary>
        public Boolean              Distributed     { get; protected set; }

        #endregion

        #region LongRunning

        /// <summary>
        /// This transaction is a long-running transaction.
        /// </summary>
        public Boolean              LongRunning     { get; protected set; }

        #endregion

        #region IsolationLevel

        /// <summary>
        /// The isolation level of this transaction.
        /// </summary>
        public IsolationLevel     IsolationLevel  { get; protected set; }

        #endregion

        #region Name

        /// <summary>
        /// A name or identification for this transaction.
        /// </summary>
        public String               Name            { get; protected set; }

        #endregion

        #region State

        protected TransactionState _State;

        /// <summary>
        /// The current state of this transaction.
        /// </summary>
        public TransactionState State
        {
            get
            {
                switch (_State)
                {
                    
                    case TransactionState.Running :
                        if (HasNestedTransaction)
                            return TransactionState.NestedTransaction;
                        return TransactionState.Running;

                    default :
                        return _State;

                }
            }
        }

        #endregion

        #region HasNestedTransaction

        /// <summary>
        /// Returns true if this transaction contains a nested transaction.
        /// </summary>
        public abstract Boolean HasNestedTransaction { get; }

        #endregion

        #endregion

        #region Constructors

        //#region Transaction()

        ///// <summary>
        ///// Creates a new transaction.
        ///// </summary>
        //public ATransaction()
        //    : this (false, false, IsolationLevel.Full, "")
        //{
        //}

        //#endregion

        #region ATransaction(myIError)

        /// <summary>
        /// This will internal set Failed=True and Status=False
        /// </summary>
        /// <param name="myIError"></param>
        public ATransaction(IError myIError)
        {
            _IErrors    = new Stack<IError>();
            _IErrors.Push(myIError);
            _IWarnings  = new Stack<IWarning>();
        }

        #endregion

        #region ATransaction(myIErrors)

        /// <summary>
        /// This will internal set Failed=True and Status=False
        /// </summary>
        /// <param name="myIErrors"></param>
        public ATransaction(IEnumerable<IError> myIErrors)
        {
            _IErrors    = new Stack<IError>(myIErrors);
            _IWarnings  = new Stack<IWarning>();
        }

        #endregion

        #region Transaction(myDistributed, myLongRunning, myIsolationLevel, myName, myCreated, myTransactionUUID)

        /// <summary>
        /// Creates a new transaction having the given parameters.
        /// </summary>
        /// <param name="myDistributed">Indicates that this transaction should synched within the entire cluster.</param>
        /// <param name="myLongRunning">Indicates that this transaction is a long-running transaction.</param>
        /// <param name="myIsolationLevel">The isolation level of this transaction.</param>
        /// <param name="myName">A name or identification for this transaction.</param>
        /// <param name="myCreated"></param>
        /// <param name="myTransactionUUID"></param>
        public ATransaction(Boolean myDistributed = false, Boolean myLongRunning = false, IsolationLevel myIsolationLevel = IsolationLevel.Serializable, String myName = "", DateTime? myCreated = null, TransactionUUID myTransactionUUID = null)
        {

            UUID            = TransactionUUID.NewUUID;
            _State          = TransactionState.Running;

            if (myCreated.HasValue)
                Created     = myCreated.Value;
            else
                Created     = TimestampNonce.Now;

            IsolationLevel  = myIsolationLevel;
            Distributed     = myDistributed;
            LongRunning     = myLongRunning;
            Name            = myName;

        }

        #endregion

        #region ATransaction(ATransaction)

        /// <summary>
        /// This will internal set Failed=True and Status=False
        /// </summary>
        /// <param name="myIError"></param>
        public ATransaction(ATransaction transaction)
            :this(transaction.Distributed, transaction.LongRunning, transaction.IsolationLevel, transaction.Name, transaction.Created, transaction.UUID)
        { }

        #endregion

        #endregion


        #region Commit()

        public virtual Exceptional Commit()
        {
            return Commit(false);
        }

        #endregion

        #region Commit(myAsync)

        /// <summary>
        /// Mark this transaction as committed. Will not invoke the OnDispose event to clean up the ressources
        /// </summary>
        /// <param name="myAsync">if true commit will be async; default: false</param>
        public virtual Exceptional Commit(Boolean myAsync)
        {

            switch (State)
            {

                // Running => Committed
                case TransactionState.Running:
                    _State   = TransactionState.Committing;
                    // Do actual some work!
                    Finished = TimestampNonce.Now;
                    _State   = TransactionState.Committed;
                    return Exceptional.OK;


                // NestedTransactions => Error!
                // At the moment do not allow to auto-commit the nested transaction!
                case TransactionState.NestedTransaction:
                    return new Exceptional(new GraphFSError_CouldNotCommitNestedTransaction());


                // Committing => OK!
                case TransactionState.Committing:
                    return Exceptional.OK;

                // Commited => Error!
                case TransactionState.Committed:
                    return new Exceptional(new GraphFSError_TransactionAlreadyCommited());


                // RollingBack => Error!
                case TransactionState.RollingBack:
                    return new Exceptional(new GraphFSError_TransactionAlreadyRolledback());

                // Rolledback => Error!
                case TransactionState.RolledBack:
                    return new Exceptional(new GraphFSError_TransactionAlreadyRolledback());


                default:
                    throw new GraphFSException("Transaction.Commit() is invalid!");

            }

        }

        #endregion

        #region Rollback()

        public virtual Exceptional Rollback()
        {
            return Rollback(false);
        }

        #endregion

        #region Rollback(myAsync)

        /// <summary>
        /// Mark this transaction as rolledback. Will invoke the event OnDispose to clean up ressources.
        /// </summary>
        /// <param name="myAsync">if true rollback will be async; default: false</param>
        public virtual Exceptional Rollback(Boolean myAsync)
        {

            switch (State)
            {

                // Running => RollingBack => Rolledback
                case TransactionState.Running :
                    _State   = TransactionState.RollingBack;
                    // Do actual some work!
                    //        //if (OnDispose != null)
                    //        //    OnDispose(this, new TransactionDisposedEventArgs(this, _SessionTokenReference));
                    Finished = TimestampNonce.Now;
                    _State   = TransactionState.RolledBack;
                    return Exceptional.OK;


                // NestedTransactions => Error!
                // At the moment do not allow to auto-rollback the nested transaction!
                case TransactionState.NestedTransaction :
                    return new Exceptional(new GraphFSError_CouldNotRolleBackNestedTransaction());

            
                // Committing => Error!
                case TransactionState.Committing :
                    return new Exceptional(new GraphFSError_TransactionAlreadyCommited());

                // Commited => Error!
                case TransactionState.Committed :
                    return new Exceptional(new GraphFSError_TransactionAlreadyCommited());


                // RollingBack => OK!
                case TransactionState.RollingBack :
                    return Exceptional.OK;

                // RolledBack => Error!
                case TransactionState.RolledBack :
                    return new Exceptional(new GraphFSError_TransactionAlreadyRolledback());


                default :
                    throw new GraphFSException("Transaction.Rollback() is invalid!");

            }

        }

        #endregion


        #region Operator overloading

        #region Operator == (myATransaction, myTransactionState)

        public static Boolean operator == (ATransaction myATransaction, TransactionState myTransactionState)
        {

            if (myATransaction == null)
                return false;

            return myATransaction.State == myTransactionState;

        }

        #endregion

        #region Operator != (myATransaction, myTransactionState)

        public static Boolean operator != (ATransaction myATransaction, TransactionState myTransactionState)
        {
            return !(myATransaction == myTransactionState);
        }

        #endregion

        #endregion


        #region ToString()

        public override String ToString()
        {

            var _Nested      = "";
            var _Distributed = "";
            var _LongRunning = "";

            if (HasNestedTransaction)
                _Nested      = " nested";

            if (Distributed)
                _Distributed = " distributed";

            if (LongRunning)
                _LongRunning = " long-running";

            var _ReturnValue = String.Format("{0}{1}{2}{3}{4}, Lifetime: {5} => {6}, UUID {7}", Name, State, _Nested, _Distributed, _LongRunning, Created, Finished, UUID);

            return _ReturnValue;

        }

        #endregion

        #region IDisposable Members

        public override void Dispose()
        {
            if (State != TransactionState.Committed)
                Rollback();
        }

        #endregion        

    }

}
