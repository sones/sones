using System;

namespace sones.Library.Internal.Security
{
    /// <summary>
    /// Used for authentication
    /// 
    /// there might be implementations for some kind of base authentication or via public key cryptography
    /// </summary>
    public interface ICredentials
    {
        /// <summary>
        /// Get the login
        /// </summary>
        /// <returns></returns>
        String GetLogin();
    }
}
