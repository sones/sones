using System.Collections.Generic;
using sones.GraphDB.ErrorHandling.Expression;
using sones.GraphDB.Expression;
using sones.GraphDB.Manager;
using sones.Library.PropertyHyperGraph;
using sones.Library.Security;
using sones.Library.Transaction;
using sones.GraphDB.TypeSystem;
using System;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// This class is responsible for realizing a get vertex on the database
    /// </summary>
    public sealed class PipelineableGetVertexRequest : APipelinableRequest
    {
        #region Data

        /// <summary>
        /// The request that contains the todo
        /// </summary>
        private readonly RequestGetVertex _request;

        /// <summary>
        /// The vertex that has been fetched from the graphDB
        /// </summary>
        private IVertex _fetchedVertex;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new pipelineable get vertex request
        /// </summary>
        /// <param name="myGetEdgeTypeRequest">The get vertex request</param>
        /// <param name="mySecurity">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The transaction token</param>
        public PipelineableGetVertexRequest(
                                                RequestGetVertex myGetVertexRequest, 
                                                SecurityToken mySecurity,
                                                TransactionToken myTransactionToken)
            : base(mySecurity, myTransactionToken)
        {
            _request = myGetVertexRequest;
        }

        #endregion

        #region APipelinableRequest Members

        public override void Validate(IMetaManager myMetaManager)
        {
            myMetaManager.VertexManager.CanGetVertex(
                _request.GetVertexDefinition.VertexTypeName,
                _request.GetVertexDefinition.VertexID,
                _request.GetVertexDefinition.Edition,
                _request.GetVertexDefinition.Timespan,
                TransactionToken, SecurityToken);
        }

        public override void Execute(IMetaManager myMetaManager)
        {
            _fetchedVertex = myMetaManager.VertexManager.GetVertex(
                _request.GetVertexDefinition.VertexTypeName, 
                _request.GetVertexDefinition.VertexID, 
                _request.GetVertexDefinition.Edition, 
                _request.GetVertexDefinition.Timespan, 
                TransactionToken, SecurityToken);
        }

        public override IRequest GetRequest()
        {
            return _request;
        }

        #endregion

        #region internal methods

        /// <summary>
        /// Creates the output for a get vertex request
        /// </summary>
        /// <typeparam name="TResult">The type of the myResult</typeparam>
        /// <param name="myOutputconverter">The delegate that is executed uppon output-generation</param>
        /// <returns>A TResult</returns>
        internal TResult GenerateRequestResult<TResult>(Converter.GetVertexResultConverter<TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics, _fetchedVertex);
        }

        #endregion

    }
}