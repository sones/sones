using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceTypeManagement
{
    [DataContract(Namespace = "http://www.sones.com")]
    public class ServiceAttributeDefinition
    {
        public ServiceAttributeDefinition(IAttributeDefinition myAttributeDefinition)
        {
            this.ID = myAttributeDefinition.ID;
            this.Name = myAttributeDefinition.Name;
            this.IsUserDefined = myAttributeDefinition.IsUserDefined;
            this.Kind = (ServiceAttributeType)myAttributeDefinition.Kind;
        }

        [DataMember]
        public Int64 ID;

        [DataMember]
        public String Name;

        [DataMember]
        public Boolean IsUserDefined;

        [DataMember]
        public ServiceAttributeType Kind;
    }
}
