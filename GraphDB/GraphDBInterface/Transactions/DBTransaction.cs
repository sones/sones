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
 * DBTransaction
 * (c) Stefan Licht, 2010
 */

#region Usings

using System;
using System.Collections.Concurrent;

using sones.GraphDB.Errors;
using sones.GraphFS.Session;
using sones.GraphDB.Exceptions;
using sones.GraphFS.Errors;
using sones.GraphFS.Exceptions;
using sones.GraphFS.Transactions;

using sones.Lib.ErrorHandling;
using sones.GraphDB.Context;
using sones.GraphDB.Warnings;

#if(__MonoCS__)
using sones.Lib.DataStructures.ConcurrentDictionary_Mono;
#else
#endif

#endregion

namespace sones.GraphDB.Transactions
{

    /// <summary>
    /// A database transaction
    /// </summary>
    public class DBTransaction : ATransaction
    {

        #region Statics for transaction handling

        #region _SessionTransactions

        [ThreadStatic]        
        #if(__MonoCS__)
            private static MonoConcurrentDictionary<SessionUUID, DBTransaction> _SessionTransactions = new MonoConcurrentDictionary<SessionUUID, DBTransaction>();
        #else
            private static ConcurrentDictionary<SessionUUID, DBTransaction> _SessionTransactions = new ConcurrentDictionary<SessionUUID, DBTransaction>(); // Will not always work!
        #endif

        #endregion

        #region _LockObject

        /// <summary>
        /// This is just a simple object to initialize the SessionTransactions in a threadsafe way
        /// </summary>
        private static Object _LockObject = new object();

        #endregion

        #region GetTransaction(sessionUUID)

        /// <summary>
        /// The main transaction which might contains some nested transactions
        /// </summary>
        public static DBTransaction GetTransaction(SessionUUID sessionUUID)
        {

            #region Check arguments

            if (sessionUUID == null || _SessionTransactions == null)
                return null;

            #endregion

            DBTransaction retVal = null;
            _SessionTransactions.TryGetValue(sessionUUID, out retVal);
            return retVal;

        }

        #endregion

        #region SetTransaction(sessionUUID, myDBTransaction)

        /// <summary>
        /// Set the transaction for the current thread and <paramref name="sessionUUID"/>.
        /// </summary>
        /// <exception cref="Error_ConcurrentTransactionsNotAllowed"></exception>
        /// <param name="sessionUUID"></param>
        /// <param name="dbTransaction"></param>
        public static void SetTransaction(SessionUUID sessionUUID, DBTransaction dbTransaction)
        {

            #region Check arguments

            if (sessionUUID == null)
                throw new GraphDBException(new Error_ArgumentNullOrEmpty("session"));

            if (dbTransaction == null)
                throw new GraphDBException(new Error_ArgumentNullOrEmpty("dbTransaction"));

            #endregion

            #region Initialize the SessionTransactions if needed

            lock (_LockObject)
            {                
                if (_SessionTransactions == null)
                #if(__MonoCS__)
                    _SessionTransactions = new MonoConcurrentDictionary<SessionUUID, DBTransaction>();
                #else
                    _SessionTransactions = new ConcurrentDictionary<SessionUUID, DBTransaction>();                
                #endif

            }

            #endregion

            _SessionTransactions.AddOrUpdate(sessionUUID, dbTransaction, (s, t) =>
            {
                if (t.IsRunning())
                {
                    throw new GraphDBException(new Error_ConcurrentTransactionsNotAllowed());
                }
                return dbTransaction;
            });

        }

        #endregion

        #region RemoveTransaction(sessionUUID)

        /// <summary>
        /// Removes the main transaction.
        /// </summary>
        /// <param name="sessionUUID"></param>
        /// <returns></returns>
        public static Boolean RemoveTransaction(SessionUUID sessionUUID)
        {

            DBTransaction _DBTransaction = null;
            return _SessionTransactions.TryRemove(sessionUUID, out _DBTransaction);

        }

        #endregion

        #region GetLatestTransaction(mySessionUUID)

        /// <summary>
        /// Returns the latest transaction. If the Transaction has running nested transaction it will return the latest nested.
        /// </summary>
        public static DBTransaction GetLatestTransaction(SessionUUID mySessionUUID)
        {

            var _ThreadTransaction = GetTransaction(mySessionUUID);

            while (_ThreadTransaction != null && _ThreadTransaction.HasNestedTransaction)
            {
                _ThreadTransaction = _ThreadTransaction.GetNestedTransaction();
            }

            return _ThreadTransaction;

        }

        #endregion

        #endregion


        #region Data

        private DBTransaction _NestedTransaction;

        /// <summary>
        /// The DBContext from the Session. On commit of the main transaction, this context will get the changes.
        /// As soon as we have ALWAYS a transaction, we can remove this.
        /// </summary>
        private IDBContext _SessionDBContext;
        private IDBContext _TransactionalDBContext;
        private FSTransaction _FSTransaction;
        private SessionUUID _Session;

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

                    case TransactionState.Running:
                        return true;

                    case TransactionState.NestedTransaction:
                        return true;

                    case TransactionState.Committed:
                        return false;

                    case TransactionState.RolledBack:
                        return false;

                }

                return false;

            }

        }

        #endregion

        #endregion


        #region Constructors
        
        #region DBTransaction(myIError)

        /// <summary>
        /// This will internal set Failed=True and Status=False
        /// </summary>
        /// <param name="myIError"></param>
        public DBTransaction(IError myIError)
            : base (myIError)
        { }

        #endregion

        #region DBTransaction(myDistributed, myLongRunning, myIsolationLevel, myName)

        /// <summary>
        /// Creates a new transaction having the given parameters.
        /// </summary>
        /// <param name="myDistributed">Indicates that this transaction should synched within the entire cluster.</param>
        /// <param name="myLongRunning">Indicates that this transaction is a long-running transaction.</param>
        /// <param name="myIsolationLevel">The isolation level of this transaction.</param>
        /// <param name="myName">A name or identification for this transaction.</param>
        public DBTransaction(SessionUUID session, Boolean myDistributed = false, Boolean myLongRunning = false, IsolationLevel myIsolationLevel = IsolationLevel.Serializable, String myName = "", DateTime? timestamp = null)
            : base(myDistributed, myLongRunning, myIsolationLevel, myName, timestamp)
        {

            //_SessionDBContext = dbContext;
            _TransactionalDBContext = GetDBContext().CopyMe();
            _Session = session;
        }

        public DBTransaction(IDBContext dbContext, SessionUUID session, FSTransaction fsTransaction)
            :base(fsTransaction)
        {
            _FSTransaction = fsTransaction;

            _SessionDBContext = dbContext;
            _TransactionalDBContext = dbContext.CopyMe();
            _Session = session;
        }

        #endregion

        #endregion


        #region GetDBContext()

        public IDBContext GetDBContext()
        {

            if (HasNestedTransaction)
                return _NestedTransaction.GetDBContext();

            return _TransactionalDBContext;

        }

        #endregion

        #region Rollback

        public override Exceptional Rollback(bool async = false)
        {

            _TransactionalDBContext = null;

            var result = _FSTransaction.Rollback(async);
            result.PushIExceptional(base.Rollback(async));

            if (DBTransaction.GetTransaction(_Session) == this)
            {
                DBTransaction.RemoveTransaction(_Session);
            }

            return result;

        }

        #endregion

        #region Commit

        public override Exceptional Commit(bool async = false)
        {

            #region Check for nested transaction

            if ((_NestedTransaction).IsRunning())
            {
                return new Exceptional(new Error_ExistingNestedTransaction("Could not commit transaction due to existing nested transaction \"" + _NestedTransaction.Name + "\""));
            }

            #endregion

            #region Commit FS transaction

            var result = _FSTransaction.Commit(async);
            
            #endregion

            #region Commit base transaction to set state etc.

            result.PushIExceptional(base.Commit(async));

            #endregion

            if (DBTransaction.GetTransaction(_Session) == this)
            {
                #region Remove the main transaction and copy context to the main context

                DBTransaction.RemoveTransaction(_Session);
                _TransactionalDBContext.CopyTo(_SessionDBContext);

                #endregion
            }
            else
            {
                #region Copy context to the parent context

                var dbContext = DBTransaction.GetLatestTransaction(_Session).GetDBContext();
                _TransactionalDBContext.CopyTo(dbContext);

                #endregion
            }

            return result;

            //TODO: How could we reflect the changes to the origin dbContext?
            //_DBContext = _GraphDBSessionReference.GetDBContext();
            //_TransactionalDBContext.CopyTo(_DBContext);
            /////////////////////////////////////////////////////////////////

        }

        #endregion


        #region Nested Transactions

        #region BeginNestedTransaction(FSTransaction transaction)

        /// <summary>
        /// Starts a new nested transaction using all parameters from a <paramref name="fsTransaction"/>
        /// </summary>
        /// <param name="fsTransaction"></param>
        /// <returns>The DBTransaction</returns>
        public DBTransaction BeginNestedTransaction(FSTransaction fsTransaction)
        {

            var dbTransaction = new DBTransaction(DBTransaction.GetLatestTransaction(_Session).GetDBContext(), _Session, fsTransaction);
            return BeginNestedTransaction(dbTransaction);

        }

        #endregion

        #region BeginNestedTransaction(myDistributed, myLongRunning, myIsolationLevel, myName, timestamp)

        /// <summary>
        /// Creates a nested transaction having the given parameters.
        /// </summary>
        /// <param name="myDistributed">Indicates that the nested transaction should synched within the entire cluster.</param>
        /// <param name="myLongRunning">Indicates that the nested transaction is a long-running transaction.</param>
        /// <param name="myIsolationLevel">The isolation level of the nested transaction.</param>
        /// <param name="myName">A name or identification for the nested transaction.</param>
        public DBTransaction BeginNestedTransaction(Boolean myDistributed = false, Boolean myLongRunning = false,
            IsolationLevel myIsolationLevel = IsolationLevel.Serializable, String myName = "", DateTime? timestamp = null)
        {

            DBTransaction dbTransaction;

            if (_FSTransaction != null)
            {
                var fsNestedTransaction = _FSTransaction.BeginNestedTransaction(myDistributed, myLongRunning, myIsolationLevel, myName, timestamp);
                dbTransaction = new DBTransaction(DBTransaction.GetLatestTransaction(_Session).GetDBContext(), _Session, fsNestedTransaction);
            }
            else
            {
                dbTransaction = new DBTransaction(_Session, myDistributed, myLongRunning, myIsolationLevel, myName, timestamp);
                dbTransaction.PushIWarning(new Warning_NoFSTransaction());
            }

            return BeginNestedTransaction(dbTransaction);

        }

        #endregion

        #region BeginNestedTransaction(DBTransaction transaction)

        /// <summary>
        /// In favour for one method which checks the states, we first create the DBTransaction and then check the state with this method
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private DBTransaction BeginNestedTransaction(DBTransaction transaction)
        {
            switch (State)
            {

                // Running => Rolledback
                case TransactionState.Running:
                    _NestedTransaction = transaction;
                    return _NestedTransaction;


                // NestedTransactions => Error!
                // At the moment do not allow to auto-commit the nested transactions!
                case TransactionState.NestedTransaction:
                    return new DBTransaction(new GraphFSError_TransactionAlreadyRolledback());


                // Committing => Error!
                case TransactionState.Committing:
                    return new DBTransaction(new GraphFSError_TransactionCurrentlyCommitting());

                // Commited => Error!
                case TransactionState.Committed:
                    return new DBTransaction(new GraphFSError_TransactionAlreadyCommited());


                // RollingBack => Error!
                case TransactionState.RollingBack:
                    return new DBTransaction(new GraphFSError_TransactionCurrentlyRollingBack());

                // RolledBack => Error!
                case TransactionState.RolledBack:
                    return new DBTransaction(new GraphFSError_TransactionAlreadyRolledback());


                default:
                    throw new GraphFSException("DBTransaction.BeginNestedTransaction() is invalid!");

            }
        }

        #endregion

        #region GetNestedTransaction

        /// <summary>
        /// Returns the current nested transaction.
        /// </summary>
        /// <returns></returns>
        public DBTransaction GetNestedTransaction()
        {
            return _NestedTransaction;
        }

        #endregion

        #endregion


        #region Equals

        public override bool Equals(object obj)
        {

            if (obj == null)
                return false;

            if (obj is DBTransaction)
            {
                DBTransaction DBTransObj = (DBTransaction)obj;

                if (DBTransObj.UUID == this.UUID)
                    return true;
            }
            else
                return false;
            
            return base.Equals(obj);

        }

        #endregion

        #region GetHashCode

        public override int GetHashCode()
        {
            return UUID.GetHashCode();
        }

        #endregion

        #region Dispose()

        public override void Dispose()
        {

            if (_FSTransaction != null)
                _FSTransaction.Dispose();

            base.Dispose();

        }

        #endregion

    }

}
