using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.Manager;
using sones.Library.PropertyHyperGraph;

namespace sones.GraphDB.Request.Delete
{
    public sealed class PipelineableDeleteRequest : APipelinableRequest
    {
        #region data

        private readonly RequestDelete _request;

        private IEnumerable<IVertex> _verticesToDelete;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new pipelineable clear request
        /// </summary>
        /// <param name="myClearRequest">The clear request</param>
        /// <param name="mySecurity">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The myOutgoingEdgeVertex transaction token</param>
        public PipelineableDeleteRequest(RequestDelete myDeleteRequest, SecurityToken mySecurity,
                                        TransactionToken myTransactionToken)
            : base(mySecurity, myTransactionToken)
        {
            _request = myDeleteRequest;
        }

        #endregion

        public override void Validate(IMetaManager myMetaManager)
        {
            //_request.
            //myMetaManager.VertexTypeManager.CanGetVertexType(_request.TypeName, TransactionToken, SecurityToken);
        }

        public override void Execute(IMetaManager myMetaManager)
        {
            throw new NotImplementedException();
        }

        public override IRequest GetRequest()
        {
            return _request;
        }

        internal TResult GenerateRequestResult<TResult>(Converter.DeleteResultConverter<TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics);
        }
    }
}
