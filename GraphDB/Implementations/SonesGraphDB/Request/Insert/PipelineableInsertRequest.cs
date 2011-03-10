using System;
using sones.GraphDB.Manager;
using sones.Library.Transaction;
using sones.Security;

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
        private readonly RequestInsertVertex _request;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new pipelineable insert request
        /// </summary>
        /// <param name="myInsertVertexRequest">The insert vertex type request</param>
        /// <param name="mySecurityToken">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        public PipelineableInsertRequest(RequestInsertVertex myInsertVertexRequest, SecurityToken mySecurityToken,
                                         TransactionToken myTransactionToken)
            : base(mySecurityToken, myTransactionToken)
        {
            _request = myInsertVertexRequest;
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