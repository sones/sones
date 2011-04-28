using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.Manager;

namespace sones.GraphDB.Request.GetVertexType
{
    public sealed class PipelineableGetAllVertexTypesRequest : APipelinableRequest
    {
        #region Data

        /// <summary>
        /// The request that contains the todo
        /// </summary>
        private readonly RequestGetAllVertexTypes _request;

        /// <summary>
        /// The vertex type that has been fetched from the graphDB
        /// </summary>
        private IEnumerable<IVertexType> _fetchedVertexTypes;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new pipelineable get vertex type request
        /// </summary>
        /// <param name="myGetVertexTypeRequest">The get vertex type request</param>
        /// <param name="mySecurity">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The transaction token</param>
        public PipelineableGetAllVertexTypesRequest(
                                                    RequestGetAllVertexTypes myGetAllVertexTypesRequest, 
                                                    SecurityToken mySecurity,
                                                    TransactionToken myTransactionToken)
            : base(mySecurity, myTransactionToken)
        {
            _request = myGetAllVertexTypesRequest;
        }

        #endregion

        #region APipelinableRequest Members

        public override void Validate(IMetaManager myMetaManager)
        {
        }

        public override void Execute(IMetaManager myMetaManager)
        {
            _fetchedVertexTypes = myMetaManager.VertexTypeManager.GetAllVertexTypes(TransactionToken, SecurityToken);   
        }

        public override IRequest GetRequest()
        {
            return _request;
        }

        #endregion

        #region internal methods

        /// <summary>
        /// Creates the output for a get vertex type request
        /// </summary>
        /// <typeparam name="TResult">The type of the myResult</typeparam>
        /// <param name="myOutputconverter">The delegate that is executed uppon output-generation</param>
        /// <returns>A TResult</returns>
        internal TResult GenerateRequestResult<TResult>(Converter.GetAllVertexTypesResultConverter<TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics, _fetchedVertexTypes);
        }

        #endregion
    }
}
