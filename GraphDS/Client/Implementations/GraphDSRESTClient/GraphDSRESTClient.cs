using System;
using sones.GraphDB;
using sones.GraphQL.Result;
using sones.Library.Security;
using sones.Library.Transaction;

namespace sones.GraphDSClient
{
    /// <summary>
    /// A GraphDS client that communicates via REST
    /// </summary>
    public sealed class GraphDSRESTClient : IGraphDSClient
    {
        #region IGraphDSClient Members

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