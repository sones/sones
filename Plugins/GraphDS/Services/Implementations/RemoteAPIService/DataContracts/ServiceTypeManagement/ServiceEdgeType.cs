using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceTypeManagement
{
    [DataContract(Namespace = "http://www.sones.com")]
    public class ServiceEdgeType : ServiceBaseType
    {
        public ServiceEdgeType(IEdgeType myEdgeType)
            : base(myEdgeType)
        {

        }
        //Todo: insert EdgeType Member
    }
}
