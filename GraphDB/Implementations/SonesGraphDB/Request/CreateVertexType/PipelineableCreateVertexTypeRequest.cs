using System;
using sones.GraphDB.Manager;
using sones.Library.Security;
using sones.Library.Transaction;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// This class is responsible for realizing a create vertex type on the database
    /// </summary>
    public sealed class PipelineableCreateVertexTypeRequest : APipelinableRequest
    {
        #region data

        /// <summary>
        /// The request that contains the todo
        /// </summary>
        private readonly RequestCreateVertexType _request;

        /// <summary>
        /// The vertex type that has been created during execution
        /// </summary>
        private IVertexType _createdVertexType;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new pipelineable create vertex type request
        /// </summary>
        /// <param name="myCreateVertexTypeRequest">The create vertex type request</param>
        /// <param name="mySecurity">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        public PipelineableCreateVertexTypeRequest(RequestCreateVertexType myCreateVertexTypeRequest,
                                                   SecurityToken mySecurity, TransactionToken myTransactionToken)
            : base(mySecurity, myTransactionToken)
        {
            _request = myCreateVertexTypeRequest;
        }

        #endregion

        #region APipelinableRequest Members

        public override void Validate(MetaManager myMetaManager)
        {
            throw new NotImplementedException();
        }

        public override void Execute(MetaManager myMetaManager)
        {
            throw new NotImplementedException();
        }

        public override IRequest GetRequest()
        {
            return _request;
        }

        #endregion

        #region internal methods

        /// <summary>
        /// Generates the result of a create vertex type request
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="myOutputconverter">The output converter that is used to create the TResult</param>
        /// <returns>A TResult</returns>
        internal TResult GenerateRequestResult<TResult>(Converter.CreateVertexTypeResultConverter<TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics, _createdVertexType);
        }

        #endregion
    }
}