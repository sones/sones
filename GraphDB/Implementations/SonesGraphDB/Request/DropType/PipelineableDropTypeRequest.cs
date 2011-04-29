using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.Manager;
using sones.GraphDB.TypeSystem;
using sones.Library.ErrorHandling;
using sones.GraphDB.ErrorHandling;

namespace sones.GraphDB.Request.DropType
{
    public sealed class PipelineableDropTypeRequest : APipelinableRequest
    {
        #region Data

        /// <summary>
        /// The request that contains the todo
        /// </summary>
        private readonly RequestDropType _request;

        #endregion
        
        #region constructor

        /// <summary>
        /// Creates a new pipelineable get vertex request
        /// </summary>
        /// <param name="myGetEdgeTypeRequest">The get vertex request</param>
        /// <param name="mySecurity">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The transaction token</param>
        public PipelineableDropTypeRequest( RequestDropType myDropTypeRequest, 
                                            SecurityToken mySecurity,
                                            TransactionToken myTransactionToken)
            : base(mySecurity, myTransactionToken)
        {
            _request = myDropTypeRequest;
        }

        #endregion
        
        public override void Validate(IMetaManager myMetaManager)
        {
            myMetaManager.VertexTypeManager.CanGetVertexType(_request.TypeName, TransactionToken, SecurityToken);
        }

        public override void Execute(IMetaManager myMetaManager)
        {
            IVertexType graphDBType = myMetaManager.VertexTypeManager.GetVertexType(_request.TypeName, TransactionToken, SecurityToken);

            if (graphDBType == null)
            {
                throw new VertexTypeDoesNotExistException(_request.TypeName);
            }

            myMetaManager.VertexTypeManager.RemoveVertexType(new List<IVertexType> {graphDBType}, TransactionToken, SecurityToken);
        }

        public override IRequest GetRequest()
        {
            return _request;
        }

        #region internal methods

        /// <summary>
        /// Creates the output for a get vertex request
        /// </summary>
        /// <typeparam name="TResult">The type of the myResult</typeparam>
        /// <param name="myOutputconverter">The delegate that is executed uppon output-generation</param>
        /// <returns>A TResult</returns>
        internal TResult GenerateRequestResult<TResult>(Converter.DropTypeResultConverter<TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics);
        }

        #endregion 
    }
}
