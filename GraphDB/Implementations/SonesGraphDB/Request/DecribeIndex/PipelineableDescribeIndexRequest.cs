using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Request.GetIndex;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.Manager;

namespace sones.GraphDB.Request.DecribeIndex
{
    public sealed class PipelineableDescribeIndexRequest : APipelinableRequest
    {
        #region Data

        /// <summary>
        /// The request that contains the todo
        /// </summary>
        private readonly RequestDescribeIndex _request;

        /// <summary>
        /// The edge type that has been fetched from the graphDB
        /// </summary>
        private IEnumerable<IIndexDefinition> _IndexDefinitions;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new pipelineable get edge type request
        /// </summary>
        /// <param name="myGetEdgeTypeRequest">The get edge type request</param>
        /// <param name="mySecurity">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The transaction token</param>
        public PipelineableDescribeIndexRequest(
                                                RequestDescribeIndex myDescribeIndexRequest, 
                                                SecurityToken mySecurity,
                                                TransactionToken myTransactionToken)
            : base(mySecurity, myTransactionToken)
        {
            _request = myDescribeIndexRequest;
        }

        #endregion

        #region APipelinableRequest Members

        public override void Validate(IMetaManager myMetaManager)
        {
            //TODO
        }

        public override void Execute(IMetaManager myMetaManager)
        {
            _IndexDefinitions = myMetaManager.IndexManager.DescribeIndex(_request.TypeName, _request.IndexName, _request.Edition, TransactionToken, SecurityToken);                
        }

        public override IRequest GetRequest()
        {
            return _request;
        }

        #endregion

        #region internal methods

        /// <summary>
        /// Creates the output for a get edge type request
        /// </summary>
        /// <typeparam name="TResult">The type of the myResult</typeparam>
        /// <param name="myOutputconverter">The delegate that is executed uppon output-generation</param>
        /// <returns>A TResult</returns>
        internal TResult GenerateRequestResult<TResult>(Converter.DescribeIndexResultConverter<TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics, _IndexDefinitions);
        }

        #endregion
    }
}
