using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.IdentityModel.Selectors;

namespace sones.Networking.HTTP
{

    public class HTTPSecurity
    {

        private HttpClientCredentialType _CredentialType;
        public HttpClientCredentialType CredentialType
        {
            get { return _CredentialType; }
            set { _CredentialType = value; }
        }

        private UserNamePasswordValidator _UserNamePasswordValidator;
        public UserNamePasswordValidator UserNamePasswordValidator
        {
            get { return _UserNamePasswordValidator; }
            set { _UserNamePasswordValidator = value; }
        }


        //UserNamePasswordValidator UserNamePasswordValidator

    }
}
