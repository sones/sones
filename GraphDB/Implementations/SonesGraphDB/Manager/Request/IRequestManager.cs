using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Request;
using System.Threading;
using sones.Library.VersionedPluginManager;

namespace sones.GraphDB.Manager
{
    #region IRequestManagerFSVersionCompatibility

    /// <summary>
    /// A static implementation of the compatible IRequestManager plugin versions. 
    /// Defines the min and max version for all IRequestManager implementations which will be activated
    /// </summary>
    internal static class IRequestManagerVersionCompatibility
    {
        public static Version MinVersion
        {
            get
            {
                return new Version("2.0.0.0");
            }
        }
        public static Version MaxVersion
        {
            get
            {
                return new Version("2.0.0.0");
            }
        }
    }

    #endregion


    /// <summary>
    /// The interface for all request managers
    /// </summary>
    public interface IRequestManager : IPluginable
    {
        /// <summary>
        /// Gets the myResult
        /// 
        /// If there was an error during validation or execution, the corresponding exception is thrown
        /// </summary>
        /// <param name="myInterestingResult">The id of the pipelineable request</param>
        /// <returns>The myResult of the request</returns>
        APipelinableRequest GetResult(Guid myInterestingResult);

        /// <summary>
        /// Initializes the request manager
        /// </summary>
        /// <param name="executionTaskCount">The number ob tasks that work in parallel on the execution queue</param>
        /// <param name="cts">The cancellation token source</param>
        void Init(int executionTaskCount, CancellationTokenSource cts);

        /// <summary>
        /// Registeres a new request
        /// </summary>
        /// <param name="myToBeAddedRequest">The request that should be registered</param>
        /// <returns>The unique id of the request</returns>
        Guid RegisterRequest(APipelinableRequest myToBeAddedRequest);

        /// <summary>
        /// gracefully shutdown of the requestmanager
        /// </summary>
        /// <param name="myIsGracefulshutdown">If true, the RequestManager does not accept any more Requests and processes the remaining ones. Otherwise the remaining requests are canceled asap.</param>
        void Shutdown(Boolean myIsGracefulshutdown = true);
    }
}
