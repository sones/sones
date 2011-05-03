using System;
using sones.GraphDB.Manager;
using sones.Library.PropertyHyperGraph;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.Commons.VertexStore.Definitions;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// This class is responsible for realizing an insert on the database
    /// </summary>
    public sealed class PipelineableInsertRequest : APipelinableRequest
    {
        #region Data

        /// <summary>
        /// The request that contains the todo
        /// </summary>
        internal readonly RequestInsertVertex _request;

        /// <summary>
        /// The vertex that has been created... 
        /// it is used for generating the output
        /// </summary>
        private IVertex _createdVertex;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new pipelineable insert request
        /// </summary>
        /// <param name="myInsertVertexRequest">The insert parentVertex type request</param>
        /// <param name="mySecurity">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The myOutgoingEdgeVertex transaction token</param>
        public PipelineableInsertRequest(RequestInsertVertex myInsertVertexRequest, SecurityToken mySecurity,
                                         TransactionToken myTransactionToken)
            : base(mySecurity, myTransactionToken)
        {
            _request = myInsertVertexRequest;
        }

        #endregion

        #region APipelinableRequest Members

        public override void Validate(IMetaManager myMetaManager)
        {
            myMetaManager.VertexManager.CheckManager.AddVertex(_request, TransactionToken, SecurityToken);
        }

        public override void Execute(IMetaManager myMetaManager)
        {
            _createdVertex = myMetaManager.VertexManager.ExecuteManager.AddVertex(_request, TransactionToken, SecurityToken);
        }

        public override IRequest GetRequest()
        {
            return _request;
        }

        #endregion

        #region internal methods

        /// <summary>
        /// Creates the output for an insert statement
        /// </summary>
        /// <typeparam name="TResult">The type of the myResult</typeparam>
        /// <param name="myOutputconverter">The delegate that is executed uppon output-generation</param>
        /// <returns>A TResult</returns>
        internal TResult GenerateRequestResult<TResult>(Converter.InsertResultConverter<TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics, _createdVertex);
        }

        #endregion
    }
}