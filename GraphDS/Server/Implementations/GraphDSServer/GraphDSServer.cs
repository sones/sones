using System;
using System.Net;
using sones.GraphDB.Request;
using sones.GraphDS;
using sones.GraphQL.Result;
using sones.Security;
using sones.Transaction;

namespace sones.GraphDSServer
{
    public sealed class GraphDSServer : IGraphDSServer
    {
        #region IGraphDSREST Members

        public void StartRESTService(string myServiceID, ushort myPort, IPAddress myIPAddress)
        {
            throw new NotImplementedException();
        }

        public bool StopRESTService(string myServiceID)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IGraphDS Members

        public GraphDB.IGraphDB GraphDB
        {
            get { throw new NotImplementedException(); }
        }

        public void Shutdown(SecurityToken mySecurityToken)
        {
            throw new NotImplementedException();
        }

        public QueryResult Query(SecurityToken mySecurityToken, TransactionToken myTransactionToken, string myQueryString, string myQueryLanguageName)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IUserAuthentication Members

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
