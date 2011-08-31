using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;


namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.PayloadObjects
{
    [DataContract(Namespace = "http://www.sones.com")]
    public class ServiceDeletePayload
    {
        [DataMember]
        public List<String> ToBeDeletedAttributes;
               
    }
}
