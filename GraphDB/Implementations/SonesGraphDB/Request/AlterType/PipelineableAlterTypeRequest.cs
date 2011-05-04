using System;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.Manager;

namespace sones.GraphDB.Request.AlterType
{
    public sealed class PipelineableAlterTypeRequest : APipelinableRequest
    {
        #region data

        /// <summary>
        /// The request which is executet
        /// </summary>
        public RequestAlterVertexType _request;

        //some more data, which henning won't say me

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new pipelineable alter type request
        /// </summary>
        /// <param name="myRequest">The alter type request</param>
        /// <param name="mySecurity">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The myOutgoingEdgeVertex transaction token</param>
        public PipelineableAlterTypeRequest(RequestAlterVertexType myRequest, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
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
        internal TResult GenerateRequestResult<TResult>(Converter.AlterTypeResultConverter<TResult> myOutputconverter)
        {
            //put some more data in the output ???
            return myOutputconverter(Statistics);
        }

        #endregion
    }
}
