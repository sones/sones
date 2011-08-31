using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.Commons.Security;
using sones.GraphDS.Services.RemoteAPIService.DataContracts;

namespace sones.GraphDS.Services.RemoteAPIService
{
    class SecurityCache
    {
        private Dictionary<String,SecurityToken> SecToken = null;

        public SecurityCache()
        {
            SecToken = new Dictionary<string, SecurityToken>();
        }

        public void AddSecurityToken(SecurityToken mySecurityToken)
        {
            SecToken.Add(mySecurityToken.ToString(), mySecurityToken);
        }

        public SecurityToken GetSecurityToken(ServiceSecurityToken mySerializedSecurityToken)
        {
            return SecToken[mySerializedSecurityToken.GUID];
        }


    }
}
