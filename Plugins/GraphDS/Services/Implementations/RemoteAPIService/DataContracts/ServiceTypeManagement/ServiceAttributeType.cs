using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceTypeManagement
{
    [DataContract(Namespace = "http://www.sones.com")]
    public enum ServiceAttributeType
    {
        [EnumMember]
        Property,

        [EnumMember]
        IncomingEdge,

        [EnumMember]
        OutgoingEdge,

        [EnumMember]
        BinaryProperty

    }
}
