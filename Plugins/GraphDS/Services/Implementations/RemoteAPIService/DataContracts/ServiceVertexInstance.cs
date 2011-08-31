using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using sones.Library.PropertyHyperGraph;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts
{
    [DataContract(Namespace = "http://www.sones.com")]
    public class ServiceVertexInstance
    {
        public ServiceVertexInstance(IVertex myVertex)
        {
            this.VertexID = myVertex.VertexID;
            this.VertexTypeID = myVertex.VertexTypeID;
            this.Edition =  myVertex.EditionName;
            this.myModificationDate = myVertex.ModificationDate;
            this.myCreationDate = myVertex.CreationDate;
            this.Comment = myVertex.Comment;
        }

        [DataMember]
        public String VertexTypeName;

        [DataMember]
        public Int64 VertexTypeID;

        [DataMember]
        public Int64 VertexID;
        
        [DataMember]
        public String Edition;

        [DataMember]
        public String Comment;
        
        [DataMember]
        public Int64 myCreationDate;

        [DataMember]
        public Int64 myModificationDate;

    }
}
