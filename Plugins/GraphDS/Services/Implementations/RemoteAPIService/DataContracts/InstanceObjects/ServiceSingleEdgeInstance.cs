using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.InstanceObjects
{
    [DataContract(Namespace = sonesRPCServer.Namespace)]
    public class ServiceSingleEdgeInstance : ServiceEdgeInstance
    {
        [DataMember]
        public ServiceVertexInstance TargetVertex;

    }
}
