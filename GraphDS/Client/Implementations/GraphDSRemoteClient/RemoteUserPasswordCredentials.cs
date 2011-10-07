/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.Commons.Security;

namespace sones.GraphDS.GraphDSRemoteClient
{
    /// <summary>
    /// Used for authentication via RemoteAPI
    /// </summary>
    [Serializable()]
    public class RemoteUserPasswordCredentials : IUserCredentials
    {
        #region data

        /// <summary>
        /// The hashed password
        /// </summary>
        private int _passwordHash;

        /// <summary>
        /// The login string
        /// </summary>
        private String _login;

        #endregion

        #region Constructor

        /// <summary>
        /// Create new credentials
        /// </summary>
        /// <param name="myLogin">The login string</param>
        /// <param name="myPassword">The password</param>
        public RemoteUserPasswordCredentials(String myLogin, String myPassword)
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
