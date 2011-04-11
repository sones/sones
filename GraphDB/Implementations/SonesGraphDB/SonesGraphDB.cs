using System;
using sones.GraphDB.Manager;
using sones.GraphDB.Request;
using sones.Library.Security;
using sones.Library.Transaction;
using sones.GraphFS;
using sones.Library.VersionedPluginManager;

namespace sones.GraphDB
{
    /// <summary>
    /// The sones implemention of the graphdb interface
    /// </summary>
    public sealed class SonesGraphDB : IGraphDB
    {
        /// <summary>
        /// The summary of all database relevant manager
        /// </summary>
        private readonly MetaManager _metaManager;
        
        private readonly RequestManager _requestManager;

        private readonly PluginManager _pluginManager;

        private readonly IGraphFS _iGraphFS;

        private readonly Guid _id;

        #region constructor

        public SonesGraphDB(IGraphFS myIGraphFS)
        {
            _iGraphFS = myIGraphFS;
            _id = Guid.NewGuid();

            //manager
            
            //+pluginManager
            //
            //+requestmanager
            //
            //+metamanager
            //++TypeManager
            //++EdgeTypeManager
            //++IndexManager
        }

        #endregion

        #region IGraphDB Members

        #region requests

        #region create VertexType

        public TResult CreateVertexType<TResult>(
            SecurityToken mySecurity, 
            TransactionToken myTransactionToken,
            RequestCreateVertexTypes myRequestCreateVertexType,
            Converter.CreateVertexTypeResultConverter<TResult> myOutputconverter)
        {
            var id =
                _requestManager.RegisterRequest(new PipelineableCreateVertexTypeRequest(myRequestCreateVertexType,
                                                                                        mySecurity,
                                                                                        myTransactionToken));

            return ((PipelineableCreateVertexTypeRequest)_requestManager.GetResult(id)).GenerateRequestResult(myOutputconverter);
        }

        #endregion

        #region clear

        public TResult Clear<TResult>(
            SecurityToken mySecurity, 
            TransactionToken myTransactionToken,                  
            RequestClear myRequestClear, 
            Converter.ClearResultConverter<TResult> myOutputconverter)
        {
            var id =
                _requestManager.RegisterRequest(new PipelineableClearRequest(myRequestClear, mySecurity,
                                                                             myTransactionToken));

            return ((PipelineableClearRequest)_requestManager.GetResult(id)).GenerateRequestResult(myOutputconverter);
        }

        #endregion

        #region Insert

        public TResult Insert<TResult>(
            SecurityToken mySecurity, 
            TransactionToken myTransactionToken,
            RequestInsertVertex myRequestInsert,
            Converter.InsertResultConverter<TResult> myOutputconverter)
        {
            var id =
                _requestManager.RegisterRequest(new PipelineableInsertRequest(myRequestInsert, mySecurity,
                                                                              myTransactionToken));

            return ((PipelineableInsertRequest)_requestManager.GetResult(id)).GenerateRequestResult(myOutputconverter);
        }

        #endregion

        #region GetVertices

        public TResult GetVertices<TResult>(
            SecurityToken mySecurity,
            TransactionToken myTransactionToken,
            RequestGetVertices myRequestGetVertices,
            Converter.GetVerticesResultConverter<TResult> myOutputconverter)
        {
            var id =
                _requestManager.RegisterRequest(new PipelineableGetVerticesRequest(myRequestGetVertices, mySecurity, myTransactionToken));

            return ((PipelineableGetVerticesRequest)_requestManager.GetResult(id)).GenerateRequestResult(myOutputconverter);
        }

        #endregion

        #endregion

        #region misc

        public Guid ID
        {
            get { return _id; }
        }

        #endregion

        #endregion

        #region Transaction

        public TransactionToken Begin(SecurityToken mySecurity, bool myLongrunning = false, IsolationLevel myIsolationLevel = IsolationLevel.Serializable)
        {
            throw new NotImplementedException();
        }

        public void Commit(SecurityToken mySecurity, TransactionToken myTransactionToken)
        {
            throw new NotImplementedException();
        }

        public void Rollback(SecurityToken mySecurity, TransactionToken myTransactionToken)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}