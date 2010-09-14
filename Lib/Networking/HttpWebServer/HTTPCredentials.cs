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
