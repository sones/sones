using System;
using sones.GraphDB.Request;
using sones.Library.VersionedPluginManager;

namespace sones.GraphDB.Manager
{
    #region IRequestSchedulerVersionCompatibility

    /// <summary>
    /// A static implementation of the compatible IRequestScheduler plugin versions. 
    /// Defines the min and max version for all IRequestScheduler implementations which will be activated
    /// </summary>
    public static class IRequestSchedulerVersionCompatibility
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
    /// The interface for all request scheduler
    /// </summary>
    public interface IRequestScheduler : IPluginable
    {
        /// <summary>
        /// Decides whether a request can be executed in parallel or not
        /// </summary>
        /// <param name="myRequest">The request which is analyzed</param>
        /// <returns>True for parallel execution, otherwise false</returns>
        bool ExecuteRequestInParallel(IRequest myRequest);
    }
}