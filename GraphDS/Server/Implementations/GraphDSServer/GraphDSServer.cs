using System;
using System.Net;
using sones.GraphDB;
using sones.GraphQL.Result;
using sones.Library.Security;
using sones.Library.Transaction;

namespace sones.GraphDSServer
{
    public sealed class GraphDSServer : IGraphDSServer
    {
        #region IGraphDSServer Members

        public void StartRESTService(string myServiceID, ushort myPort, IPAddress myIPAddress)
        {
            throw new NotImplementedException();
        }

        public bool StopRESTService(string myServiceID)
        {
            throw new NotImplementedException();
        }

        public IGraphDB GraphDB
        {
            get { throw new NotImplementedException(); }
        }

        public void Shutdown(SecurityToken mySecurityToken)
        {
            throw new NotImplementedException();
        }

        public QueryResult Query(SecurityToken mySecurityToken, TransactionToken myTransactionToken,
                                 string myQueryString, string myQueryLanguageName)
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