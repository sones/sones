using System;
using System.Threading;
using sones.GraphDB.Request;
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
        APipelinableRequest SynchronExecution(APipelinableRequest myToBeExecutedRequest);
    }
}
