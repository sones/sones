using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Security
{
    /// <summary>
    /// The interface for the user authentication
    /// </summary>
    public interface IUserAuthentication
    {
        /// <summary>
        /// Log on to the system in favour of getting a security token
        /// </summary>
        /// <param name="toBeAuthenticatedCredentials">The credentials that should be authenticated</param>
        /// <returns>A security token</returns>
        SecurityToken LogOn(IUserCredentials toBeAuthenticatedCredentials);

        /// <summary>
        /// Log off
        /// </summary>
        /// <param name="toBeLoggedOfToken">The security token that should be revoked</param>
        void LogOff(SecurityToken toBeLoggedOfToken);
    }
}
