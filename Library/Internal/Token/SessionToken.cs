using System;
using sones.Library.Internal.Security;

namespace sones.Library.Internal.Token
{
    /// <summary>
    /// A class that containts informations concerning the current session and authentication
    /// </summary>
    public sealed class SessionToken
    {
        #region Data

        /// <summary>
        /// The credentials of the session
        /// </summary>
        public readonly ICredentials Credentials;

        /// <summary>
        /// The Guid of the session
        /// </summary>
        public readonly Guid Guid;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new SessionToken
        /// </summary>
        /// <param name="myCredentials"></param>
        public SessionToken(ICredentials myCredentials)
        {
            Credentials = myCredentials;
            Guid = Guid.NewGuid();
        }

        #endregion

    }
}
