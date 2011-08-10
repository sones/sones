using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using sones.GraphDB.TypeSystem;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.VertexType;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceTypeManagement
{
    [DataContract(Namespace = "http://www.sones.com")]
    public class ServiceOutgoingEdgeDefinition : ServiceAttributeDefinition
    {
        public ServiceOutgoingEdgeDefinition(IOutgoingEdgeDefinition myOutgoingEdgeDefinition)
            : base(myOutgoingEdgeDefinition)
        {
            this.EdgeType = new ServiceEdgeType(myOutgoingEdgeDefinition.EdgeType);
            this.InnerEdgeType = new ServiceEdgeType(myOutgoingEdgeDefinition.InnerEdgeType);
            this.SourceVertexType = new ServiceVertexType(myOutgoingEdgeDefinition.SourceVertexType);
            this.TargetVertexType = new ServiceVertexType(myOutgoingEdgeDefinition.TargetVertexType);
            this.Multiplicity = (ServiceEdgeMultiplicity)myOutgoingEdgeDefinition.Multiplicity;
        }

        [DataMember]
        public ServiceEdgeType EdgeType;

        [DataMember]
        public ServiceEdgeType InnerEdgeType;

        [DataMember]
        public ServiceVertexType SourceVertexType;

        [DataMember]
        public ServiceVertexType TargetVertexType;

        [DataMember]
        public ServiceEdgeMultiplicity Multiplicity;

    }
}
