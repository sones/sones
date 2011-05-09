using System;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.Manager;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.Request.AlterType
{
    public sealed class PipelineableAlterVertexTypeRequest : APipelinableRequest
    {
        #region data

        /// <summary>
        /// The request which is executet
        /// </summary>
        public RequestAlterVertexType _request;

        private IVertexType _alteredVertexType;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new pipelineable alter type request
        /// </summary>
        /// <param name="myRequest">The alter type request</param>
        /// <param name="mySecurity">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The myOutgoingEdgeVertex transaction token</param>
        public PipelineableAlterVertexTypeRequest(RequestAlterVertexType myRequest, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
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
            myMetaManager.VertexTypeManager.CheckManager.AlterVertexType(_request, SecurityToken, TransactionToken);
        }

        /// <summary>
        /// Executes the given request
        /// </summary>
        public override void Execute(IMetaManager myMetaManager)
        {
            _alteredVertexType = myMetaManager.VertexTypeManager.ExecuteManager.AlterVertexType(_request, SecurityToken, TransactionToken);
        }

        /// <summary>
        /// Returns the alter type request
        /// </summary>
        public override IRequest GetRequest()
        {
            return _request;
        }

        #endregion

        #region internal methods

        /// <summary>
        /// Generates the myResult of a alter type request
        /// </summary>
        /// <typeparam name="TResult">The type of the myResult</typeparam>
        /// <param name="myOutputconverter">The output converter that is used to create the TResult</param>
        /// <returns>A TResult</returns>
        internal TResult GenerateRequestResult<TResult>(Converter.AlterVertexTypeResultConverter<TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics, _alteredVertexType);
        }

        #endregion
    }
}
