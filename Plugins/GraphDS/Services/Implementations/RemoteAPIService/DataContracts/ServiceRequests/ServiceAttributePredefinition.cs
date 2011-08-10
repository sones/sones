using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;


namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceRequests
{
    [DataContract(Namespace = "http://www.sones.com")]
    public abstract class ServiceAttributePredefinition
    {
        [DataMember]
        public String AttributeType;
        [DataMember]
        public String AttributeName;
        [DataMember]
        public String Comment;
               
    }
    
}
