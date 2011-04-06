using System;


namespace sones.Plugins.Index
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
        /// The exception that might happend
        /// </summary>
        public Exception Exception { get; set; }

        public RequestType TypeOfRequest { get; protected set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new pipelineable request
        /// </summary>        
        protected APipelinableRequest()
        {
            ID = Guid.NewGuid();            
        }

        #endregion

        #region abstract methods

        /// <summary>
        /// Execute the request
        /// </summary>        
        public abstract void Execute();

        /// <summary>
        /// Get the request that has been executed
        /// </summary>
        public abstract Object GetRequest();

        #endregion
    }
}