using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.InsertPayload
{
    [DataContract(Namespace = "http://www.sones.com")]
    public class StructuredProperty
    {
        [DataMember]
        public String PropertyName;

        [DataMember]
        public object PropertyValue;

    }
}
