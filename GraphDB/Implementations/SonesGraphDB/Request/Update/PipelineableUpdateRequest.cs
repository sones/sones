using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.Request.Update
{
    public sealed class PipelineableUpdateRequest : APipelinableRequest
    {
        #region Data

        /// <summary>
        /// The request that contains the todo
        /// </summary>
        private readonly RequestUpdate _request;

        private IVertexType updatedVertexType;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new pipelineable get vertex request
        /// </summary>
        /// <param name="myGetEdgeTypeRequest">The get vertex request</param>
        /// <param name="mySecurity">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The transaction token</param>
        public PipelineableUpdateRequest(   RequestUpdate myUpdateRequest, 
                                            SecurityToken mySecurity,
                                            TransactionToken myTransactionToken)
            : base(mySecurity, myTransactionToken)
        {
            _request = myUpdateRequest;
        }

        #endregion

        /// <summary>
        /// Validates the given request.
        /// </summary>
        public override void Validate(Manager.IMetaManager myMetaManager)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Executes the given request.
        /// </summary>
        public override void Execute(Manager.IMetaManager myMetaManager)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the update request.
        /// </summary>
        public override IRequest GetRequest()
        {
            return _request;
        }

        /// <summary>
        /// Generates the myResult of a update request
        /// </summary>
        /// <typeparam name="TResult">The type of the myResult</typeparam>
        /// <param name="myOutputconverter">The output converter that is used to create the TResult</param>
        /// <returns>A TResult</returns>
        internal TResult GenerateRequestResult<TResult>(Converter.UpdateResultConverter<TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics, updatedVertexType);
        }
    }
}
