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
        #region data

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

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new pipelineable request
        /// </summary>
        /// <param name="mySecurity">The security token</param>
        /// <param name="myTransactionToken">The transaction token</param>
        protected APipelinableRequest(SecurityToken mySecurity, TransactionToken myTransactionToken)
        {
            ID = Guid.NewGuid();
            SecurityToken = mySecurity;
            TransactionToken = myTransactionToken;
        }

        #endregion

        #region abstract methods

        /// <summary>
        /// Validation of the request
        /// </summary>
        /// <param name="myMetaManager">A manager that contains every other manager</param>
        public abstract void Validate(MetaManager myMetaManager);

        /// <summary>
        /// Execute the request
        /// </summary>
        /// <param name="myMetaManager">A manager that contains every other manager</param>
        public abstract void Execute(MetaManager myMetaManager);

        /// <summary>
        /// Get the request that has been executed
        /// </summary>
        /// <returns>An IRequest</returns>
        public abstract IRequest GetRequest();

        #endregion
    }
}