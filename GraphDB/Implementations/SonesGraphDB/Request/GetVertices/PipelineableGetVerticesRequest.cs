using System.Collections.Generic;
using sones.GraphDB.ErrorHandling.Expression;
using sones.GraphDB.Expression;
using sones.GraphDB.Manager;
using sones.Library.PropertyHyperGraph;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.Expression.QueryPlan;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// This class is responsible for realizing a get vertices on the database
    /// </summary>
    public sealed class PipelineableGetVerticesRequest : APipelinableRequest
    {
        #region Data

        /// <summary>
        /// The request that contains the todo
        /// </summary>
        private readonly RequestGetVertices _request;

        /// <summary>
        /// The vertices that have been fetched by the Graphdb
        /// it is used for generating the output
        /// </summary>
        private IEnumerable<IVertex> _fetchedIVertices;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new pipelineable get vertices request
        /// </summary>
        /// <param name="myGetVerticesRequest">The get vertices type request</param>
        /// <param name="mySecurity">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The myOutgoingEdgeVertex transaction token</param>
        public PipelineableGetVerticesRequest(
                                                RequestGetVertices myGetVerticesRequest, 
                                                SecurityToken mySecurity,
                                                TransactionToken myTransactionToken)
            : base(mySecurity, myTransactionToken)
        {
            _request = myGetVerticesRequest;
        }

        #endregion

        #region APipelinableRequest Members

        public override void Validate(IMetaManager myMetaManager)
        {
            #region case 1 - Expression

            if (_request.Expression != null)
            {
                if (!myMetaManager.QueryPlanManager.IsValidExpression(_request.Expression))
                {
                    throw new InvalidExpressionException(_request.Expression);
                }
            }
            
            #endregion

            #region case 2 - No Expression

            else if (_request.VertexTypeName != null)
            {
                //2.1 typeName as string
                myMetaManager.VertexTypeManager.CheckManager.GetVertexType(_request.VertexTypeName, TransactionToken, SecurityToken);
            }
            else
            {
                //2.2 type as id
                myMetaManager.VertexTypeManager.CheckManager.GetVertexType(_request.VertexTypeID, TransactionToken, SecurityToken);
            }

            #endregion
        }

        public override void Execute(IMetaManager myMetaManager)
        {
            #region case 1 - Expression

            if (_request.Expression != null)
            {
                _fetchedIVertices = myMetaManager.VertexManager.GetVertices(_request.Expression, _request.IsLongrunning, TransactionToken, SecurityToken);
            }

            #endregion

            #region case 2 - No Expression

            else if (_request.VertexTypeName != null)
            {
                //2.1 typeName as string
                if (_request.VertexIDs != null)
                {
                    //2.1.1 vertex ids
                    List<IVertex> fetchedVertices = new List<IVertex>();

                    foreach (var item in _request.VertexIDs)
                    {
                        fetchedVertices.Add(myMetaManager.VertexManager.GetVertex(_request.VertexTypeName, item, null, null, TransactionToken, SecurityToken));
                    }

                    _fetchedIVertices = fetchedVertices;
                }
                else
                {
                    //2.1.2 no vertex ids ... take all
                    _fetchedIVertices = myMetaManager.VertexManager.GetVertices(_request.VertexTypeName, TransactionToken, SecurityToken);
                }
            }
            else
            {
                //2.2 type as id
                if (_request.VertexIDs != null)
                {
                    //2.2.1 vertex ids
                    List<IVertex> fetchedVertices = new List<IVertex>();

                    foreach (var item in _request.VertexIDs)
                    {
                        fetchedVertices.Add(myMetaManager.VertexManager.GetVertex(_request.VertexTypeID, item, null, null, TransactionToken, SecurityToken));
                    }

                    _fetchedIVertices = fetchedVertices;
                }
                else
                {
                    //2.2.2 no vertex ids ... take all
                    _fetchedIVertices = myMetaManager.VertexManager.GetVertices(_request.VertexTypeID, TransactionToken, SecurityToken);
                }
            }

            #endregion
        }

        public override IRequest GetRequest()
        {
            return _request;
        }

        #endregion

        #region internal methods

        /// <summary>
        /// Creates the output for a get vertices request
        /// </summary>
        /// <typeparam name="TResult">The type of the myResult</typeparam>
        /// <param name="myOutputconverter">The delegate that is executed uppon output-generation</param>
        /// <returns>A TResult</returns>
        internal TResult GenerateRequestResult<TResult>(Converter.GetVerticesResultConverter<TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics, _fetchedIVertices);
        }

        #endregion

    }
}