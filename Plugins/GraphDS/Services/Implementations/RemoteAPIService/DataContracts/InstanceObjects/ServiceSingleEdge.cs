using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using sones.Library.PropertyHyperGraph;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.InstanceObjects
{
    [DataContract(Namespace = sonesRPCServer.Namespace)]
    public class ServiceSingleEdge : ServiceEdgeInstance
    {
        public ServiceSingleEdge(ISingleEdge mySingleEdge): base(mySingleEdge as IEdge)
        {
           this.TargetVertex = new ServiceVertexInstance(mySingleEdge.GetTargetVertex());
        }

        [DataMember]
        public ServiceVertexInstance TargetVertex;
    }
}
