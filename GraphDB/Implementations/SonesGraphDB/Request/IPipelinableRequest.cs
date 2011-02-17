using System;
using sones.GraphDB.Manager;
using sones.Security;
using sones.Transaction;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// The interface for all requests that can be run within a pipeline
    /// </summary>
    public interface IPipelinableRequest
    {
        /// <summary>
        /// The id of the request
        /// </summary>
        Guid ID { get; }

        /// <summary>
        /// The definition of the request
        /// </summary>
        IRequest Request { get; }

        /// <summary>
        /// The security token
        /// </summary>
        SecurityToken SecurityToken { get; }

        /// <summary>
        /// The transaction token
        /// </summary>
        TransactionToken TransactionToken { get; }

        /// <summary>
        /// Validates the request
        /// </summary>
        /// <param name="myMetaManager">A manager that contains all necessary managers</param>
        /// <returns>True if the request is valid, otherwise false</returns>
        Boolean Validate(MetaManager myMetaManager);

        /// <summary>
        /// Executes the request
        /// </summary>
        /// <param name="myMetaManager">A manager that contains all necessary managers</param>
        void Execute(MetaManager myMetaManager);
    }
}
