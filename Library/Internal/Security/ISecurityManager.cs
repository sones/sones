using System;
using sones.Library.VersionedPluginManager;
using sones.Library.VertexStore;

namespace sones.Library.Security
{
    #region ISecurityManagerVersionCompatibility

    /// <summary>
    /// A static implementation of the compatible ISecurityManager plugin versions. 
    /// Defines the min and max version for all ISecurityManager implementations which will be activated
    /// </summary>
    public static class ISecurityManagerVersionCompatibility
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
    /// The interface for all security managers
    /// Authentication & integrity & encryption
    /// </summary>
    public interface ISecurityManager : IAuthentication, IIntegrity, IEncryption, IVertexStore, IPluginable
    {
        /// <summary>
        /// Is a certain token allowd to create a vertex type
        /// </summary>
        /// <param name="mySecuritytoken">The token</param>
        /// <returns>True or false</returns>
        bool AllowedToCreateVertexType(SecurityToken mySecuritytoken);


        //... to be continued
    }
}