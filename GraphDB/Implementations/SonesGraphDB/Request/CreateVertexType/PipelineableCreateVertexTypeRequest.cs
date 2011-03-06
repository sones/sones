using System;
using sones.GraphDB.Manager;
using sones.Security;
using sones.Transaction;

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

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new pipelineable create vertex type request
        /// </summary>
        /// <param name="myCreateVertexTypeRequest">The create vertex type request</param>
        /// <param name="mySecurityToken">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        public PipelineableCreateVertexTypeRequest(RequestCreateVertexType myCreateVertexTypeRequest,
                                                   SecurityToken mySecurityToken, TransactionToken myTransactionToken)
            : base(mySecurityToken, myTransactionToken)
        {
            _request = myCreateVertexTypeRequest;
        }

        #endregion

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
    }
}