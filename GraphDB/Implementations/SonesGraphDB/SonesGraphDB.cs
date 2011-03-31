using System;
using sones.GraphDB.Manager;
using sones.GraphDB.Request;
using sones.Library.Security;
using sones.Library.Transaction;

namespace sones.GraphDB
{
    /// <summary>
    /// The sones implemention of the graphdb interface
    /// </summary>
    public sealed class SonesGraphDB : IGraphDB
    {
        private readonly MetaManager _metaManager;
        private readonly RequestManager _requestManager;

        #region IGraphDB Members

        #region create VertexType

        public TResult CreateVertexType<TResult>(
            SecurityToken mySecurityToken, 
            TransactionToken myTransactionToken,
            RequestCreateVertexType myRequestCreateVertexType,
            Converter.CreateVertexTypeResultConverter<TResult> myOutputconverter)
        {
            var id =
                _requestManager.RegisterRequest(new PipelineableCreateVertexTypeRequest(myRequestCreateVertexType,
                                                                                        mySecurityToken,
                                                                                        myTransactionToken));

            return ((PipelineableCreateVertexTypeRequest)_requestManager.GetResult(id)).GenerateRequestResult(myOutputconverter);
        }

        #endregion

        #region clear

        public TResult Clear<TResult>(
            SecurityToken mySecurityToken, 
            TransactionToken myTransactionToken,                  
            RequestClear myRequestClear, 
            Converter.ClearResultConverter<TResult> myOutputconverter)
        {
            var id =
                _requestManager.RegisterRequest(new PipelineableClearRequest(myRequestClear, mySecurityToken,
                                                                             myTransactionToken));

            return ((PipelineableClearRequest)_requestManager.GetResult(id)).GenerateRequestResult(myOutputconverter);
        }

        #endregion

        #region Insert

        public TResult Insert<TResult>(
            SecurityToken mySecurityToken, 
            TransactionToken myTransactionToken,
            RequestInsertVertex myRequestInsert,
            Converter.InsertResultConverter<TResult> myOutputconverter)
        {
            var id =
                _requestManager.RegisterRequest(new PipelineableInsertRequest(myRequestInsert, mySecurityToken,
                                                                              myTransactionToken));

            return ((PipelineableInsertRequest)_requestManager.GetResult(id)).GenerateRequestResult(myOutputconverter);
        }

        #endregion

        #region GetVertices

        public TResult GetVertices<TResult>(
            SecurityToken mySecurityToken,
            TransactionToken myTransactionToken,
            RequestGetVertices myRequestGetVertices,
            Converter.GetVerticesResultConverter<TResult> myOutputconverter)
        {
            var id =
                _requestManager.RegisterRequest(new PipelineableGetVerticesRequest(myRequestGetVertices, mySecurityToken, myTransactionToken));

            return ((PipelineableGetVerticesRequest)_requestManager.GetResult(id)).GenerateRequestResult(myOutputconverter);
        }

        #endregion

        #endregion

        #region Transaction

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

        #endregion
    }
}