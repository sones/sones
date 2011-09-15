using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts
{
    [DataContract(Namespace = sonesRPCServer.Namespace)]
    public class ServiceSecurityToken : IEquatable<ServiceSecurityToken>
    {
        [DataMember]
        public Guid UniqueID;

        public ServiceSecurityToken(Guid myGuid)
        {
            UniqueID = myGuid;
        }


        public bool Equals(ServiceSecurityToken other)
        {
            return (this.UniqueID == other.UniqueID);
        }

        public override bool Equals(object obj)
        {
            if ((obj as ServiceSecurityToken) == null)
            {
                return false;
            }

            return (this.UniqueID == ((ServiceSecurityToken)obj).UniqueID);
        }

        public override int GetHashCode()
        {
            return UniqueID.GetHashCode();
        }
    }
}
