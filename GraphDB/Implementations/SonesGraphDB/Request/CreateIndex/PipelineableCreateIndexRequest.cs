using System;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.ErrorHandling;
using sones.GraphDB.Manager;

namespace sones.GraphDB.Request.CreateIndex
{
    public sealed class PipelineableCreateIndexRequest : APipelinableRequest
    {
        #region data

        private readonly RequestCreateIndex _request;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new pipelineable clear request
        /// </summary>
        /// <param name="myClearRequest">The clear request</param>
        /// <param name="mySecurity">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The myOutgoingEdgeVertex transaction token</param>
        public PipelineableCreateIndexRequest(RequestCreateIndex myCreateIndexRequest, SecurityToken mySecurity, TransactionToken myTransactionToken)
            : base(mySecurity, myTransactionToken)
        {
            _request = myCreateIndexRequest;
        }

        #endregion

        public override void Validate(IMetaManager myMetaManager)
        {
            if(string.IsNullOrWhiteSpace(_request.IndexDefinition.VertexTypeName))
            {
                throw new InvalidIndexAttributeException(_request.IndexDefinition.VertexTypeName, "");
            }
            else if(string.IsNullOrWhiteSpace(_request.IndexDefinition.Name) )
            {
                throw new InvalidIndexAttributeException(_request.IndexDefinition.Name, "");
            }
            else if (string.IsNullOrWhiteSpace(_request.IndexDefinition.Name))
            {
                throw new InvalidIndexAttributeException(_request.IndexDefinition.TypeName, "");
            }
        }

        public override void Execute(IMetaManager myMetaManager)
        {
            var indexDef = myMetaManager.IndexManager.CreateIndex(_request.IndexDefinition, SecurityToken, TransactionToken);

            if (indexDef == null)
            {
                throw new IndexCreationException(_request.IndexDefinition, "");
            }
        }

        public override IRequest GetRequest()
        {
            return _request;
        }

        internal TResult GenerateRequestResult<TResult>(Converter.CreateIndexResultConverter<TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics);
        }
    }
}
