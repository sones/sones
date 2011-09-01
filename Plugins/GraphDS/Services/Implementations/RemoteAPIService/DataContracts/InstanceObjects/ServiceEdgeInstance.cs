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
        public ServiceEdgeInstance(IEdge myEdge) : base(myEdge.EdgeTypeID)
        {
            this.Comment = myEdge.Comment;
            var SourceVertex = myEdge.GetSourceVertex();
            this.SourceVertexID = SourceVertex.VertexID;
            this.SourceVertexTypeID = SourceVertex.VertexRevisionID;
        }
       
        [DataMember]
        public Int64 EdgeID;

        [DataMember]
        public Int64 SourceVertexTypeID;

        [DataMember]
        public Int64 SourceVertexID;
        
        [DataMember]
        public String Comment;
        
    }
}
