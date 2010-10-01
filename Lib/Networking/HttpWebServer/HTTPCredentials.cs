/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Lib;

namespace sones.Networking.HTTP
{
    
    public class HTTPCredentials
    {

        public String Username { get; set; }
        public String Password { get; set; }
        public HTTPAuthenticationTypes HTTPCredentialType { get; set; }

        /// <summary>
        /// Create the credentials based on a base64 encoded string which comes from a HTTP header Authentication:
        /// </summary>
        /// <param name="myHTTPHeaderCredential"></param>
        public HTTPCredentials(String myHTTPHeaderCredential)
        {
            var splitted = myHTTPHeaderCredential.Split(new[] { ' ' });

            if (splitted.IsNullOrEmpty())
            {
                throw new ArgumentException("invalid credentials " + myHTTPHeaderCredential);
            }

            if (splitted[0].ToLower() == "basic")
            {
                HTTPCredentialType = HTTPAuthenticationTypes.Basic;
                var usernamePassword = splitted[1].FromBase64().Split(new[] { ':' });

                if (usernamePassword.IsNullOrEmpty())
                    throw new ArgumentException("invalid username/password " + splitted[1].FromBase64());

                Username = usernamePassword[0];
                Password = usernamePassword[1];
            }
            else
            {
                throw new ArgumentException("invalid credentialType " + splitted[0]);
            }

        }

    }
}
