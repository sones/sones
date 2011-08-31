using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using sones.Library.Commons.Security;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts
{
    [DataContract(Namespace = "http://www.sones.com")]
    public class ServiceSecurityToken
    {
        //currently unused
        public ServiceSecurityToken(SecurityToken mySecurityToken)
        {
            this.GUID = mySecurityToken.ToString(); 
        }


        [DataMember]
        public String GUID;
                
    }
}
