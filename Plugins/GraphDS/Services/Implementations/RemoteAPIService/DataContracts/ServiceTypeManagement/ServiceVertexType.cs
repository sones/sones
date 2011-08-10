using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceTypeManagement;
using sones.GraphDB.TypeSystem;


namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.VertexType
{
    [DataContract(Namespace = "http://www.sones.com")]
    public class ServiceVertexType : ServiceBaseType
    {
        public ServiceVertexType(IVertexType myVertexType)
            : base(myVertexType)
        {

        }
    }
}
