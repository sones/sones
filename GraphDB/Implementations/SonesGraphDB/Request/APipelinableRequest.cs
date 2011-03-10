using System;
using sones.GraphDB.Manager;
using sones.Library.ErrorHandling;
using sones.Library.Security;
using sones.Library.Transaction;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// The abstract class for all pipelineable requests
    /// </summary>
    public abstract class APipelinableRequest
    {
        /// <summary>
        /// The id of the pipelineable request
        /// </summary>
        public Guid ID { get; private set; }

        /// <summary>
        /// The security token of the request initiator
        /// </summary>
        protected SecurityToken SecurityToken { get; private set; }

        /// <summary>
        /// The current transaction token
        /// </summary>
        protected TransactionToken TransactionToken { get; private set; }

        /// <summary>
        /// The request statistics
        /// </summary>
        public IRequestStatistics Statistics { get; set; }

        /// <summary>
        /// The exception that might happend
        /// </summary>
        public ASonesException Exception { get; set; }

        #region Constructor

        /// <summary>
        /// Creates a new pipelineable request
        /// </summary>
        /// <param name="mySecurityToken">The security token</param>
        /// <param name="myTransactionToken">The transaction token</param>
        protected APipelinableRequest(SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            ID = Guid.NewGuid();
            SecurityToken = mySecurityToken;
            TransactionToken = myTransactionToken;
        }

        #endregion

        public abstract void Validate(MetaManager myMetaManager);

        public abstract void Execute(MetaManager myMetaManager);

        public abstract IRequest GetRequest();

        public TResult GenerateRequestStatistics<TResult>(Func<IRequestStatistics, TResult> myOutputconverter)
        {
            return myOutputconverter(Statistics);
        }
    }
}