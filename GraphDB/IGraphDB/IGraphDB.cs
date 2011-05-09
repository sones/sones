using System;
using sones.GraphDB.Request;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;

namespace sones.GraphDB
{
    #region IGraphDBVersionCompatibility

    /// <summary>
    /// A static implementation of the compatible IGraphDB plugin versions. 
    /// Defines the min and max version for all IGraphDB implementations which will be activated used this IGraphDB.
    /// </summary>
    internal static class IGraphDBVersionCompatibility
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
    /// The interface for all graphdb implementations
    /// </summary>
    public interface IGraphDB : ITransactionable, IUserAuthentication, IReadWriteGraphDB
    {
        #region misc

        /// <summary>
        /// The id of the graph database
        /// </summary>
        Guid ID { get; }

        /// <summary>
        /// Shutdown the graphdb
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        void Shutdown(SecurityToken mySecurityToken);

        #endregion
    }
}
