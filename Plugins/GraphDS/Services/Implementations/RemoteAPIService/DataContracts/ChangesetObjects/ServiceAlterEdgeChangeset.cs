using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceRequests;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.ChangesetObjects
{
    [DataContract(Namespace = sonesRPCServer.Namespace)]
    public class ServiceAlterEdgeChangeset
    {
        [DataMember]
        public String NewTypeName;
        
        [DataMember]
        public String Comment;
        
        [DataMember]
        public List<ServicePropertyPredefinition> ToBeAddedProperties;
                
        [DataMember]
        public List<ServicePropertyPredefinition> ToBeRemovedProperties;
                
        [DataMember]
        public Dictionary<String, String> ToBeRenamedProperties;
    }
}
