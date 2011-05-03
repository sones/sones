using System;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.Manager;

namespace sones.GraphDB.Request
{
    public sealed class PipelineableTruncateRequest : APipelinableRequest
    {
        #region data

        private readonly RequestTruncate _request;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new pipelineable truncate request
        /// </summary>
        /// <param name="myClearRequest">The truncate request</param>
        /// <param name="mySecurity">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The transaction token</param>
        public PipelineableTruncateRequest(RequestTruncate myTruncateRequest, 
                                            SecurityToken mySecurityToken,
                                            TransactionToken myTransactionToken)
            : base(mySecurityToken, myTransactionToken)
        {
            _request = myTruncateRequest;
        }

        #endregion

        public override void Validate(IMetaManager myMetaManager)
        {
            myMetaManager.VertexTypeManager.CheckManager.GetVertexType(_request.VertexTypeName, TransactionToken, SecurityToken);
        }

        public override void Execute(IMetaManager myMetaManager)
        {
            //1. remove all objects

            //2. reset indices
        }

        public override IRequest GetRequest()
        {
            return _request;
        }

        /// <summary>
        /// Generates the myResult of a truncate request
        /// </summary>
        /// <typeparam name="TResult">The type of the myResult</typeparam>
        /// <param name="myOutputconverter">The output converter that is used to create the TResult</param>
        /// <returns>A TResult</returns>
        internal TResult GenerateRequestResult<TResult>(Converter.TruncateResultConverter<TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics);
        }
    }
}
