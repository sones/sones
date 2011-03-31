using System;
using sones.GraphDB.Manager;
using sones.Library.Security;
using sones.Library.Transaction;
using sones.Library.PropertyHyperGraph;
using System.Collections.Generic;

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
        /// The vertex that have been fetched by the Graphdb
        /// it is used for generating the output
        /// </summary>
        private IEnumerable<IVertex> _fetchedIVertices;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new pipelineable get vertices request
        /// </summary>
        /// <param name="myGetVerticesRequest">The get vertices type request</param>
        /// <param name="mySecurityToken">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        public PipelineableGetVerticesRequest(
                                                RequestGetVertices myGetVerticesRequest, 
                                                SecurityToken mySecurityToken,
                                                TransactionToken myTransactionToken)
            : base(mySecurityToken, myTransactionToken)
        {
            _request = myGetVerticesRequest;
        }

        #endregion

        #region APipelinableRequest Members

        public override void Validate(MetaManager myMetaManager)
        {
            throw new NotImplementedException();
        }

        public override void Execute(MetaManager myMetaManager)
        {
            throw new NotImplementedException();
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
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="myOutputconverter">The delegate that is executed uppon output-generation</param>
        /// <returns>A TResult</returns>
        internal TResult GenerateRequestResult<TResult>(Converter.GetVerticesResultConverter<TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics, _fetchedIVertices);
        }

        #endregion
    }
}