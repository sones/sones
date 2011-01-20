using System;

namespace sones.Library.Internal.Security
{
    /// <summary>
    /// Used for session authentication
    /// </summary>
    public sealed class Credentials : ICredentials
    {
        #region data

        /// <summary>
        /// The login string
        /// </summary>
        public readonly String Login;

        /// <summary>
        /// The hashed password
        /// </summary>
        public readonly int PasswordHash;

        #endregion

        #region Constructor

        /// <summary>
        /// Create new credentials
        /// </summary>
        /// <param name="myLogin">The login string</param>
        /// <param name="myPassword">The password</param>
        public Credentials(String myLogin, String myPassword)
        {
            Login = myLogin;
            PasswordHash = myPassword.GetHashCode();
        }

        #endregion

        #region overrides

        public override string ToString()
        {
            return String.Format("Login: {0}, PW-Hash: {1}", Login, PasswordHash);
        }

        #endregion

        #region ICredentials

        public string GetLogin()
        {
            return Login;
        }

        #endregion

    }
}
