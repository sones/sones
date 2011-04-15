using System;
using sones.GraphDB.Request;
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
        #region IGraphDS Members

        public void Shutdown(SecurityToken mySecurityToken)
        {
            throw new NotImplementedException();
        }

        public QueryResult Query(SecurityToken mySecurityToken, TransactionToken myTransactionToken, string myQueryString, string myQueryLanguageName)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IGraphDB Members

        public TResult CreateVertexType<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestCreateVertexTypes myRequestCreateVertexType, Converter.CreateVertexTypeResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult Clear<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestClear myRequestClear, Converter.ClearResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult Insert<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestInsertVertex myRequestInsert, Converter.InsertResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetVertices<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetVertices myRequestGetVertices, Converter.GetVerticesResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetVertexType<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetVertexType myRequestGetVertexType, Converter.GetVertexTypeResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetEdgeType<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetEdgeType myRequestGetEdgeType, Converter.GetEdgeTypeResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public TResult GetVertex<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken, RequestGetVertex myRequestGetVertex, Converter.GetVertexResultConverter<TResult> myOutputconverter)
        {
            throw new NotImplementedException();
        }

        public Guid ID
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region ITransactionable Members

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