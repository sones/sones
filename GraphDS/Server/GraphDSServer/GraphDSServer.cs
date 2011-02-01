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
    public sealed class GraphDSServer : IGraphDS
    {
        #region REST

        /// <summary>
        /// Starts a new REST service
        /// </summary>
        /// <param name="myServiceID">The unique identifier of the service</param>
        /// <param name="myPort">The used port</param>
        /// <param name="myIPAddress">The used ip-address</param>
        void StartRESTService(String myServiceID, UInt16 myPort, IPAddress myIPAddress)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stops a REST service
        /// </summary>
        /// <param name="myServiceID">The unique identifier of the REST service that is going to be stopped</param>
        /// <returns>True for successful stop, otherwise false</returns>
        bool StopRESTService(String myServiceID)
        {
            throw new NotImplementedException();
        }

        #endregion

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
    }
}
