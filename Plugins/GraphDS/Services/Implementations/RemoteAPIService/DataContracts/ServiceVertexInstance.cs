using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using sones.Library.PropertyHyperGraph;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts
{
    [DataContract(Namespace = sonesRPCServer.Namespace)]
    public class ServiceVertexInstance
    {
        public ServiceVertexInstance(IVertex myVertex)
        {
            this.VertexID = myVertex.VertexID;
            this.VertexTypeID = myVertex.VertexTypeID;
            this.Edition =  myVertex.EditionName;
        }
        
        [DataMember]
        public Int64 VertexTypeID;

        [DataMember]
        public Int64 VertexID;
        
        [DataMember]
        public String Edition;
    }
}
