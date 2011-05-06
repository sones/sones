using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Manager;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.ErrorHandling;

namespace sones.GraphDB.Request.DropIndex
{
    public sealed class PipelineableDropIndexRequest : APipelinableRequest
    {
        #region data

        private readonly RequestDropIndex _request;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new pipelineable get vertex request
        /// </summary>
        /// <param name="myGetEdgeTypeRequest">The get vertex request</param>
        /// <param name="mySecurity">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The transaction token</param>
        public PipelineableDropIndexRequest(RequestDropIndex myDropTypeRequest,
                                            SecurityToken mySecurity,
                                            TransactionToken myTransactionToken)
            : base(mySecurity, myTransactionToken)
        {
            _request = myDropTypeRequest;
        }

        #endregion

        public override void Validate(IMetaManager myMetaManager)
        {
            myMetaManager.VertexTypeManager.CheckManager.GetVertexType(_request.TypeName, TransactionToken, SecurityToken);
        }

        public override void Execute(IMetaManager myMetaManager)
        {
            myMetaManager.IndexManager.DropIndex(_request, TransactionToken, SecurityToken);
        }

        public override IRequest GetRequest()
        {
            return _request;
        }

        internal TResult GenerateRequestResult<TResult>(Converter.DropIndexResultConverter<TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics);
        }
    }
}
