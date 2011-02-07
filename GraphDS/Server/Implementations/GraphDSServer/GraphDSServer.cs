using System;
using System.Net;
using sones.GraphDB.Request;
using sones.GraphDS;
using sones.GraphQL.Result;
using sones.Security;
using sones.Transaction;

namespace sones.GraphDSServer
{
    public sealed class GraphDSServer : IGraphDSServer, IGraphDS
    {

        public void StartRESTService(string myServiceID, ushort myPort, IPAddress myIPAddress)
        {
            throw new NotImplementedException();
        }

        public bool StopRESTService(string myServiceID)
        {
            throw new NotImplementedException();
        }

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

        public TResult Insert<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestInsertVertex<TResult> myRequestInsert)
        {
            throw new NotImplementedException();
        }

        public TransactionToken Begin(SecurityToken mySecurityToken, bool myLongrunning = false, IsolationLevel myIsolationLevel = IsolationLevel.Serializable)
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

        public SecurityToken LogOn(IUserCredentials toBeAuthenticatedCredentials)
        {
            throw new NotImplementedException();
        }

        public void LogOff(SecurityToken toBeLoggedOfToken)
        {
            throw new NotImplementedException();
        }
    }
}
