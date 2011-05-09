using System.Collections.Generic;
using sones.GraphDB.ErrorHandling.Expression;
using sones.GraphDB.Expression;
using sones.GraphDB.Manager;
using sones.Library.PropertyHyperGraph;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.TypeSystem;
using System;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// This class is responsible for realizing a get vertex count on the database
    /// </summary>
    public sealed class PipelineableGetVertexCountRequest : APipelinableRequest
    {
        #region Data

        /// <summary>
        /// The request that contains the todo
        /// </summary>
        private readonly RequestGetVertexCount _request;

        /// <summary>
        /// The vertex count that has been fetched from the graphDB
        /// </summary>
        private UInt64 _vertexCount;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new pipelineable get vertex count request
        /// </summary>
        /// <param name="myGetVertexTypeRequest">The get vertex ciunt request</param>
        /// <param name="mySecurity">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The transaction token</param>
        public PipelineableGetVertexCountRequest(
                                                RequestGetVertexCount myGetVertexTypeRequest, 
                                                SecurityToken mySecurity,
                                                TransactionToken myTransactionToken)
            : base(mySecurity, myTransactionToken)
        {
            _request = myGetVertexTypeRequest;
        }

        #endregion

        #region APipelinableRequest Members

        public override void Validate(IMetaManager myMetaManager)
        {
            if (_request.VertexTypeName != null)
            {
                myMetaManager.VertexTypeManager.CheckManager.GetVertexType(_request.VertexTypeName, TransactionToken, SecurityToken);
            }
            else
            {
                myMetaManager.VertexTypeManager.CheckManager.GetVertexType(_request.VertexTypeID, TransactionToken, SecurityToken);
            }
        }

        public override void Execute(IMetaManager myMetaManager)
        {
            long vertexTypeID;

            if (_request.VertexTypeName != null)
            {
                vertexTypeID = myMetaManager.VertexTypeManager.ExecuteManager.GetVertexType(_request.VertexTypeName, TransactionToken, SecurityToken).ID;
            }
            else
            {
                vertexTypeID = _request.VertexTypeID;
            }

            _vertexCount = myMetaManager.VertexManager.ExecuteManager.VertexStore.GetVertexCount(SecurityToken, TransactionToken, vertexTypeID);
        }

        public override IRequest GetRequest()
        {
            return _request;
        }

        #endregion

        #region internal methods

        /// <summary>
        /// Creates the output for a get vertex count request
        /// </summary>
        /// <typeparam name="TResult">The type of the myResult</typeparam>
        /// <param name="myOutputconverter">The delegate that is executed uppon output-generation</param>
        /// <returns>A TResult</returns>
        internal TResult GenerateRequestResult<TResult>(Converter.GetVertexCountResultConverter<TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics, _vertexCount);
        }

        #endregion

    }
}