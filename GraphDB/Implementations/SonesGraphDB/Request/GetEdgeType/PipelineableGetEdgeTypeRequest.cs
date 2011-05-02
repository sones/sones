using System.Collections.Generic;
using sones.GraphDB.ErrorHandling.Expression;
using sones.GraphDB.Expression;
using sones.GraphDB.Manager;
using sones.Library.PropertyHyperGraph;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.TypeSystem;
using sones.GraphDB.Manager.TypeManagement;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// This class is responsible for realizing a get edge type on the database
    /// </summary>
    public sealed class PipelineableGetEdgeTypeRequest : APipelinableRequest
    {
        #region Data

        /// <summary>
        /// The request that contains the todo
        /// </summary>
        private readonly RequestGetEdgeType _request;

        /// <summary>
        /// The edge type that has been fetched from the graphDB
        /// </summary>
        private IEdgeType _fetchedEdgeType;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new pipelineable get edge type request
        /// </summary>
        /// <param name="myGetEdgeTypeRequest">The get edge type request</param>
        /// <param name="mySecurity">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The transaction token</param>
        public PipelineableGetEdgeTypeRequest(
                                                RequestGetEdgeType myGetEdgeTypeRequest, 
                                                SecurityToken mySecurity,
                                                TransactionToken myTransactionToken)
            : base(mySecurity, myTransactionToken)
        {
            _request = myGetEdgeTypeRequest;
        }

        #endregion

        #region APipelinableRequest Members

        public override void Validate(IMetaManager myMetaManager)
        {
            DoExecute(myMetaManager.EdgeTypeManager.CheckManager);
        }

        public override void Execute(IMetaManager myMetaManager)
        {
            _fetchedEdgeType = DoExecute(myMetaManager.EdgeTypeManager.ExecuteManager);
        }

        private IEdgeType DoExecute(IEdgeTypeHandler myManager)
        {
            if (_request.EdgeTypeName == null)
            {
                return myManager.GetEdgeType(_request.EdgeTypeID, TransactionToken, SecurityToken);
            }
            else
            {
                return myManager.GetEdgeType(_request.EdgeTypeName, TransactionToken, SecurityToken);
            }
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
        internal TResult GenerateRequestResult<TResult>(Converter.GetEdgeTypeResultConverter<TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics, _fetchedEdgeType);
        }

        #endregion

    }
}