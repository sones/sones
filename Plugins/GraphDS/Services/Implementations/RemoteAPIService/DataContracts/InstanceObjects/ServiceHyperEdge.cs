using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using sones.Library.PropertyHyperGraph;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.InstanceObjects
{
    [DataContract(Namespace = sonesRPCServer.Namespace)]
    public class ServiceHyperEdge : ServiceEdgeInstance
    {
        public ServiceHyperEdge(IHyperEdge myHyperEdge)
            : base(myHyperEdge as IEdge)
        {
            this.TargetVertices = myHyperEdge.GetTargetVertices().Select(x => new ServiceVertexInstance(x)).ToList();
        }

        [DataMember]
        public List<ServiceVertexInstance> TargetVertices;
    }
}
