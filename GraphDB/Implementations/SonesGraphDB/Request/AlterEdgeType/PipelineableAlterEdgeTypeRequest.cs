using System;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.Manager;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.Request.AlterType
{
    /// <summary>
    /// This class is responsible for realizing a alter type on the database
    /// </summary>
    public sealed class PipelineableAlterEdgeTypeRequest : APipelinableRequest
    {
        #region data

        /// <summary>
        /// The request which is executed
        /// </summary>
        public RequestAlterEdgeType _request;

        private IEdgeType _alteredEdgeType;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new pipelineable alter edge type request
        /// </summary>
        /// <param name="myRequest">The alter edge type request</param>
        /// <param name="mySecurity">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The myOutgoingEdgeVertex transaction token</param>
        public PipelineableAlterEdgeTypeRequest(RequestAlterEdgeType myRequest, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
            :base(mySecurityToken, myTransactionToken)
        {
            _request = myRequest;
        }

        #endregion

        #region APipelineable Members

        /// <summary>
        /// Validates if the given request can be executed
        /// </summary>
        public override void Validate(IMetaManager myMetaManager)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Executes the given request
        /// </summary>
        public override void Execute(IMetaManager myMetaManager)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the alter edge type request
        /// </summary>
        public override IRequest GetRequest()
        {
            return _request;
        }

        #endregion

        #region internal methods

        /// <summary>
        /// Generates the myResult of a alter edge type request
        /// </summary>
        /// <typeparam name="TResult">The type of the myResult</typeparam>
        /// <param name="myOutputconverter">The output converter that is used to create the TResult</param>
        /// <returns>A TResult</returns>
        internal TResult GenerateRequestResult<TResult>(Converter.AlterEdgeTypeResultConverter<TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics, _alteredEdgeType);
        }

        #endregion
    }
}
