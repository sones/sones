using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts
{
    [DataContract(Namespace = sonesRPCServer.Namespace)]
    public class ServiceSecurityToken
    {
        [DataMember]
        public Guid UniqueID;

        public ServiceSecurityToken(Guid myGuid)
        {
            UniqueID = myGuid;
        }
    }
}
