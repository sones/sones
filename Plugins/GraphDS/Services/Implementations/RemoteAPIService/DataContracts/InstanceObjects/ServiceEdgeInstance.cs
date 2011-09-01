using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using sones.Library.PropertyHyperGraph;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.InstanceObjects
{
    [DataContract(Namespace = sonesRPCServer.Namespace)]
    public class ServiceEdgeInstance
    {
        public ServiceEdgeInstance(IEdge myEdge)
        {
            this.EdgeTypeID = myEdge.EdgeTypeID;
            
        }
        
        [DataMember]
        public Int64 EdgeTypeID;

    }
}
