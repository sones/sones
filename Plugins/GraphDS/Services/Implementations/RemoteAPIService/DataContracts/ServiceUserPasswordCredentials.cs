using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using sones.Library.Commons.Security;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts
{
    [DataContract(Namespace = sonesRPCServer.Namespace)]
    public class ServiceUserPasswordCredentials : IUserCredentials
    {
        #region data

        /// <summary>
        /// The hashed password
        /// </summary>
        [DataMember]
        private int _passwordHash;

        /// <summary>
        /// The login string
        /// </summary>
        [DataMember]
        private String _login;

        #endregion

        #region Constructor

        /// <summary>
        /// Create new credentials
        /// </summary>
        /// <param name="myLogin">The login string</param>
        /// <param name="myPassword">The password</param>
        public ServiceUserPasswordCredentials(String myLogin, String myPassword)
        {
            _login = myLogin;
            _passwordHash = myPassword.GetHashCode();
        }

        #endregion

        #region overrides

        public override string ToString()
        {
            return String.Format("Login: {0}, PW-Hash: {1}", _login, _passwordHash);
        }

        #endregion

        #region IUserCredentials Members

        public string Login
        {
            get { return _login; }
        }

        #endregion

        public int PasswordHash
        {
            get { return _passwordHash; }
        }
    }
}
