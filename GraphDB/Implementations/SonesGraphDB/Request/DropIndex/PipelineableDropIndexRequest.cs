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
            IVertexType graphDBType = myMetaManager.VertexTypeManager.ExecuteManager.GetVertexType(_request.TypeName, TransactionToken, SecurityToken);
            IVertexType indexType = null;

            if (graphDBType == null)
            {
                throw new VertexTypeDoesNotExistException(_request.TypeName);
            }

            var indices = graphDBType.GetIndexDefinitions(true);

            if (indices != null)
            {
                foreach (var index in indices)
                {
                    //name of found index is not null or empty and equals searched index name
                    if (!string.IsNullOrWhiteSpace(index.Name) && index.Name.Equals(_request.IndexName))
                    {
                        //edition of searched index is null or empty
                        if (string.IsNullOrWhiteSpace(_request.Edition))
                        {
                            indexType = myMetaManager.VertexTypeManager.ExecuteManager.GetVertexType(index.IndexTypeName, TransactionToken, SecurityToken);
                        }
                        else
                        {
                            if (index.Edition.Equals(_request.Edition))
                            {
                                indexType = myMetaManager.VertexTypeManager.ExecuteManager.GetVertexType(index.IndexTypeName, TransactionToken, SecurityToken);
                            }
                        }

                    }
                }
            }

            if (indexType == null)
            {
                throw new IndexTypeDoesNotExistException(_request.TypeName, _request.IndexName);
            }
            
            myMetaManager.VertexTypeManager.ExecuteManager.RemoveVertexTypes(new List<IVertexType> { indexType }, TransactionToken, SecurityToken);
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
