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
            throw new NotImplementedException();
        }
        //todo IMplement these data contract
    }
}
