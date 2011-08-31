using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceRequests
{
    [DataContract(Namespace = "http://www.sones.com")]
    public class ServiceMandatoryPredefinition
    {
        [DataMember]
        public String PropertyName;

        [DataMember]
        public object DefaultValue;

        public MandatoryPredefinition ToMandatoryPredefinition()
        {
            return new MandatoryPredefinition(this.PropertyName, this.DefaultValue);
        }
    }
}
