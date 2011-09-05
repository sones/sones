using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceRequests;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.ChangesetObjects
{
    [DataContract(Namespace = sonesRPCServer.Namespace)]
    public class ServiceAlterVertexChangeset
    {
        [DataMember]
        public String NewTypeName;
        
        [DataMember]
        public String Comment;
        
        [DataMember]
        public List<ServicePropertyPredefinition> ToBeAddedProperties;
        
        [DataMember]
        public List<ServiceIncomingEdgePredefinition> ToBeAddedIncomingEdges;

        [DataMember]
        public List<ServiceOutgoingEdgePredefinition> ToBeAddedOutgoingEdges;

        [DataMember]
        public List<ServiceIndexPredefinition> ToBeAddedIndices;

        [DataMember]
        public List<ServiceUniquePredefinition> ToBeAddedUniques;

        [DataMember]
        public List<ServiceMandatoryPredefinition> ToBeAddedMandatories;

        [DataMember]
        public List<ServicePropertyPredefinition> ToBeRemovedProperties;

        [DataMember]
        public List<ServiceIncomingEdgePredefinition> ToBeRemovedIncomingEdges;

        [DataMember]
        public List<ServiceOutgoingEdgePredefinition> ToBeRemovedOutgoingEdges;

        [DataMember]
        public List<ServiceIndexPredefinition> ToBeRemovedIndices;

        [DataMember]
        public List<ServiceUniquePredefinition> ToBeRemovedUniques;

        [DataMember]
        public List<ServiceMandatoryPredefinition> ToBeRemovedMandatories;

        [DataMember]
        public Dictionary<String, String> ToBeRenamedProperties;
    }
}
