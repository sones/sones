using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;

namespace sones.Library.Commons.Security
{
    
    internal class UsernamePasswordValidator : UserNamePasswordValidator
    {
        private readonly String _username;
        private readonly String _password;

        public UsernamePasswordValidator(String myUsername, String myPassword)
        {
            _username = myUsername;
            _password = myPassword;
        }
        
        public override void Validate(string myUserName, string myPassword)
        {

            if (!(myUserName == _username && myPassword == _password))
            {
                throw new SecurityTokenException("Unknown Username or Password");
            }
        }
    }
    
    
    
    /// <summary>
    /// Used for authentication
    /// </summary>
    public sealed class UserPasswordCredentials : IUserCredentials
    {
        #region data

        /// <summary>
        /// The hashed password
        /// </summary>
        public readonly int PasswordHash;

        /// <summary>
        /// The login string
        /// </summary>
        private readonly String _login;
        private readonly UsernamePasswordValidator _validator;

        #endregion

        #region Constructor

        /// <summary>
        /// Create new credentials
        /// </summary>
        /// <param name="myLogin">The login string</param>
        /// <param name="myPassword">The password</param>
        public UserPasswordCredentials(String myLogin, String myPassword)
        {
            _validator = new UsernamePasswordValidator(myLogin, myPassword);
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

        #region IUserCredentials Members

        public string Login
        {
            get { return _login; }
        }

        #endregion


        public System.IdentityModel.Selectors.UserNamePasswordValidator CreateHttpCredentials()
        {
            return _validator;
        }

        
    }
}