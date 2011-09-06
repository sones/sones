using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceRequests;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.InsertPayload
{
    [DataContract(Namespace="http://www.sones.com")]
    public class ServiceInsertPayload
    {
        [DataMember]
        public List<StructuredProperty> StructuredProperties;

        [DataMember]
        public List<UnstructuredProperty> UnstructuredProperties;

        //[DataMember]
        //public List<ServiceEdgeTypePredefinition> EdgesToAdd;

        [DataMember]
        public Int64 UUID;

        [DataMember]
        public String Comment;

        [DataMember]
        public String Edition;

        [DataMember]
        public List<ServiceEdgePredefinition> Edges;


    }
}
