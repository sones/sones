using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDS;
using sones.Library.Internal.Security;
using sones.GraphDB.Transaction;
using sones.GraphDB.Request;
using sones.GraphQL.Result;
using System.Net;

namespace sones.GraphDSServer
{
    public sealed class GraphDSServer : IGraphDSServer, IGraphDS
    {
        #region IGraphDS members

        public void Shutdown(SecurityToken mySecurityToken)
        {
            throw new NotImplementedException();
        }

        public QueryResult Query(SecurityToken mySecurityToken, TransactionToken myTransactionToken, string myQueryString, string myQueryLanguageName)
        {
            throw new NotImplementedException();
        }

        public TResult CreateVertexType<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestCreateVertexType<TResult> myRequestCreateVertexType)
        {
            throw new NotImplementedException();
        }

        public TResult Clear<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestClear<TResult> myRequestClear)
        {
            throw new NotImplementedException();
        }

        public TransactionToken BeginTransaction(SecurityToken mySecurityToken, bool myLongrunning = false, IsolationLevel myIsolationLevel = IsolationLevel.Serializable)
        {
            throw new NotImplementedException();
        }

        public void CommitTransaction(SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            throw new NotImplementedException();
        }

        public void RollbackTransaction(SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            throw new NotImplementedException();
        }

        public SecurityToken LogOn(IUserCredentials toBeAuthenticatedCredentials)
        {
            throw new NotImplementedException();
        }

        public void LogOff(SecurityToken toBeLoggedOfToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region REST

        public void StartRESTService(string myServiceID, ushort myPort, IPAddress myIPAddress)
        {
            throw new NotImplementedException();
        }

        public bool StopRESTService(string myServiceID)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
