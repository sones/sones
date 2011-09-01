using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using sones.Library.PropertyHyperGraph;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.InstanceObjects
{
    [DataContract(Namespace = sonesRPCServer.Namespace)]
    public class ServiceEdgeInstance : AGraphElement
    {
        public ServiceEdgeInstance(ISingleEdge myEdge) : base(myEdge.EdgeTypeID)
        {
            this.Comment = myEdge.Comment;
            var SourceVertex = myEdge.GetSourceVertex();
            this.SourceVertexID = SourceVertex.VertexID;
            this.SourceVertexTypeID = SourceVertex.VertexRevisionID;
            this.TargetVertices = myEdge.GetTargetVertices().Select(x => new ServiceVertexInstance(x)).ToList();
        }
       
        [DataMember]
        public Int64 EdgeID;

        [DataMember]
        public Int64 SourceVertexTypeID;

        [DataMember]
        public Int64 SourceVertexID;

        [DataMember]
        public List<ServiceVertexInstance> TargetVertices;

        [DataMember]
        public String Comment;
        
    }
}
