using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceTypeManagement
{
    [DataContract(Namespace = "http://www.sones.com")]
    public class ServiceBinaryPropertyDefinition : ServiceAttributeDefinition
    {
        public ServiceBinaryPropertyDefinition(IBinaryPropertyDefinition myBinaryPropertyDefinition)
            : base(myBinaryPropertyDefinition)
        {

        }
    }
}
