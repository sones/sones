using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using GraphDSRPC.DataContracts.VertexType;

namespace sones.GraphDS.Services.RemoteAPIService.ServiceContracts
{
    [ServiceContract(Namespace = "http://www.sones.com")]
    public interface IRPCServiceContract
    {
        [OperationContract]
        ServiceVertexType TestGetAVertexType();

    }
}
