using System;
using sones.GraphDB.Manager;
using sones.Library.Security;
using sones.Library.Transaction;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// This class is responsible for realizing a clear on the database
    /// </summary>
    public sealed class PipelineableClearRequest : APipelinableRequest
    {
        #region data

        private readonly RequestClear _request;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new pipelineable clear request
        /// </summary>
        /// <param name="myClearRequest">The clear request</param>
        /// <param name="mySecurityToken">The security token of the request initiator</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        public PipelineableClearRequest(RequestClear myClearRequest, SecurityToken mySecurityToken,
                                        TransactionToken myTransactionToken)
            : base(mySecurityToken, myTransactionToken)
        {
            _request = myClearRequest;
        }

        #endregion

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

        /// <summary>
        /// Generates the result of a clear request
        /// </summary>
        /// <typeparam name="TResult">The type of the result</typeparam>
        /// <param name="myOutputconverter">The output converter that is used to create the TResult</param>
        /// <returns>A TResult</returns>
        internal TResult GenerateRequestResult<TResult>(Converter.ClearResultConverter<TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics);
        }
    }
}