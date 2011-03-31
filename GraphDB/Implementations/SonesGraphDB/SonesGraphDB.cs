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

        public TResult CreateVertexType<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken,
                                                 RequestCreateVertexType myRequestCreateVertexType,
                                                 Converter.CreateVertexTypeResultConverter<TResult> myOutputconverter)
        {
            var id =
                _requestManager.RegisterRequest(new PipelineableCreateVertexTypeRequest(myRequestCreateVertexType,
                                                                                        mySecurityToken,
                                                                                        myTransactionToken));

            return ((PipelineableCreateVertexTypeRequest)_requestManager.GetResult(id)).GenerateRequestResult(myOutputconverter);
        }

        public TResult Clear<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken,
                                      RequestClear myRequestClear, Converter.ClearResultConverter<TResult> myOutputconverter)
        {
            var id =
                _requestManager.RegisterRequest(new PipelineableClearRequest(myRequestClear, mySecurityToken,
                                                                             myTransactionToken));

            return ((PipelineableClearRequest)_requestManager.GetResult(id)).GenerateRequestResult(myOutputconverter);
        }

        public TResult Insert<TResult>(SecurityToken mySecurityToken, TransactionToken myTransactionToken,
                                       RequestInsertVertex myRequestInsert,
                                       Converter.InsertResultConverter<TResult> myOutputconverter)
        {
            var id =
                _requestManager.RegisterRequest(new PipelineableInsertRequest(myRequestInsert, mySecurityToken,
                                                                              myTransactionToken));

            return ((PipelineableInsertRequest)_requestManager.GetResult(id)).GenerateRequestResult(myOutputconverter);
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