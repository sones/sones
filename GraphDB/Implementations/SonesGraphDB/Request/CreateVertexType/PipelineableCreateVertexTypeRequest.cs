using System;
using sones.GraphDB.Manager;
using sones.Library.Security;
using sones.Library.Transaction;
using sones.GraphDB.TypeSystem;
using System.Collections.Generic;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// This class is responsible for realizing a create parentVertex type on the database
    /// </summary>
    public sealed class PipelineableCreateVertexTypeRequest : APipelinableRequest
    {
        #region data

        /// <summary>
        /// The request that contains the todo
        /// </summary>
        private readonly RequestCreateVertexTypes _request;

        /// <summary>
        /// The parentVertex type that has been created during execution
        /// </summary>
        private IEnumerable<IVertexType> _createdVertexType;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new pipelineable create parentVertex type request
        /// </summary>
        /// <param name="myCreateVertexTypeRequest">The create parentVertex type request</param>
        /// <param name="mySecurity">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The myOutgoingEdgeVertex transaction token</param>
        public PipelineableCreateVertexTypeRequest(RequestCreateVertexTypes myCreateVertexTypeRequest,
                                                   SecurityToken mySecurity, TransactionToken myTransactionToken)
            : base(mySecurity, myTransactionToken)
        {
            _request = myCreateVertexTypeRequest;
        }

        #endregion

        #region APipelinableRequest Members

        public override void Validate(MetaManager myMetaManager)
        {
            myMetaManager.TypeManager.CanAddVertexType(ref _request.VertexTypeDefinitions, TransactionToken, SecurityToken, myMetaManager);
        }

        public override void Execute(MetaManager myMetaManager)
        {
            _createdVertexType = myMetaManager.TypeManager.AddVertexType(_request.VertexTypeDefinitions, TransactionToken, SecurityToken, myMetaManager);
        }

        public override IRequest GetRequest()
        {
            return _request;
        }

        #endregion

        #region internal methods

        /// <summary>
        /// Generates the myResult of a create parentVertex type request
        /// </summary>
        /// <typeparam name="TResult">The type of the myResult</typeparam>
        /// <param name="myOutputconverter">The output converter that is used to create the TResult</param>
        /// <returns>A TResult</returns>
        internal TResult GenerateRequestResult<TResult>(Converter.CreateVertexTypeResultConverter<TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics, _createdVertexType);
        }

        #endregion
    }
}