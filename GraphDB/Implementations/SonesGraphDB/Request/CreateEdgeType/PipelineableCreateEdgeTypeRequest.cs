using System.Collections.Generic;
using sones.GraphDB.Manager;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using System.Linq;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// This class is responsible for realizing a create edge type on the database
    /// </summary>
    public sealed class PipelineableCreateEdgeTypeRequest : APipelinableRequest
    {
        #region data

        /// <summary>
        /// The request that contains the todo
        /// </summary>
        private readonly RequestCreateEdgeType _request;

        /// <summary>
        /// The parentVertex type that has been created during execution
        /// </summary>
        private IEdgeType _createdEdgeType;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new pipelineable create edge type request
        /// </summary>
        /// <param name="myCreateEdgeTypeRequest">The create edge type request</param>
        /// <param name="mySecurity">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The myOutgoingEdgeVertex transaction token</param>
        public PipelineableCreateEdgeTypeRequest(RequestCreateEdgeType myCreateEdgeTypeRequest,
                                                   SecurityToken mySecurity, TransactionToken myTransactionToken)
            : base(mySecurity, myTransactionToken)
        {
            _request = myCreateEdgeTypeRequest;
        }

        #endregion

        #region APipelinableRequest Members

        public override void Validate(IMetaManager myMetaManager)
        {
        }

        public override void Execute(IMetaManager myMetaManager)
        {
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
        internal TResult GenerateRequestResult<TResult>(Converter.CreateEdgeTypeResultConverter<TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics, _createdEdgeType);
        }

        #endregion
    }
}