using System.Collections.Generic;
using sones.GraphDB.Manager;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;

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

        public override void Validate(IMetaManager myMetaManager)
        {
            myMetaManager.VertexTypeManager.CanAddVertexType(_request.VertexTypeDefinitions, TransactionToken, SecurityToken);
        }

        public override void Execute(IMetaManager myMetaManager)
        {
            _createdVertexType = myMetaManager.VertexTypeManager.AddVertexType(_request.VertexTypeDefinitions, TransactionToken, SecurityToken);
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
        internal TResult GenerateRequestResult<TResult>(Converter.CreateVertexTypesResultConverter<TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics, _createdVertexType);
        }

        #endregion
    }
}