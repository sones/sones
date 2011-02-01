using System;

namespace sones.Library.Internal.Security
{
    /// <summary>
    /// Used for authentication
    /// </summary>
    public sealed class UserPasswordCredentials : IUserCredentials
    {
        #region data

        /// <summary>
        /// The login string
        /// </summary>
        private readonly String _login;

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
        public UserPasswordCredentials(String myLogin, String myPassword)
        {
            _login = myLogin;
            PasswordHash = myPassword.GetHashCode();
        }

        #endregion

        #region overrides

        public override string ToString()
        {
            return String.Format("Login: {0}, PW-Hash: {1}", _login, PasswordHash);
        }

        #endregion

        #region ICredentials Members

        public string Login
        {
            get { return _login; }
        }

        #endregion
    }
}
