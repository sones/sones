using System;
using sones.GraphDB.Manager;
using sones.GraphDB.Request;
using sones.Security;
using sones.Transaction;

namespace sones.GraphDB
{
    /// <summary>
    /// The sones implemention of the graphdb interface
    /// </summary>
    public sealed class SonesGraphDB : IGraphDB
    {
        private MetaManager _metaManager;

        #region IGraphDB Members

        public TResult CreateVertexType<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken,
                                                 RequestCreateVertexType myRequestCreateVertexType,
                                                 Func<IRequestStatistics, TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult Clear<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken,
                                      RequestClear myRequestClear, Func<IRequestStatistics, TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult Insert<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken,
                                       RequestInsertVertex myRequestInsert,
                                       Func<IRequestStatistics, TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TransactionToken Begin(SecurityToken mySecurityToken, bool myLongrunning = false,
                                      IsolationLevel myIsolationLevel = IsolationLevel.Serializable)
        {
            throw new NotImplementedException();
        }

        public void Commit(SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            throw new NotImplementedException();
        }

        public void Rollback(SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}