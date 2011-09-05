using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceRequests;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.InstanceObjects;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.ChangesetObjects
{
    [DataContract(Namespace = sonesRPCServer.Namespace)]
    public class ServiceUpdateChangeset
    {
        [DataMember]
        public String Comment;

        [DataMember]
        public String Edition;

        [DataMember]
        public Dictionary<String, IComparable> ToBeUpdatedUnstructuredProperties;

        [DataMember]
        public Dictionary<String, IComparable> ToBeUpdatedStructuredProperties;

        [DataMember]
        public List<ServiceEdgeInstance> ToBeUpdatedEdges;
    }
}
