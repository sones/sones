using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using sones.GraphDB.TypeSystem;

namespace sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceTypeManagement
{
    [DataContract(Namespace = sonesRPCServer.Namespace)]
    public class ServiceUniqueDefinition
    {
        public ServiceUniqueDefinition(IUniqueDefinition myUniqueDefinition)
        {
            this.DefiningVertexType = new ServiceVertexType(myUniqueDefinition.DefiningVertexType);
            this.UniquePropertyDefinition = myUniqueDefinition.UniquePropertyDefinitions.Select(x => new ServicePropertyDefinition(x)).ToList();
        }

        [DataMember]
        public ServiceVertexType DefiningVertexType;

        [DataMember]
        public List<ServicePropertyDefinition> UniquePropertyDefinition;
    }
}
