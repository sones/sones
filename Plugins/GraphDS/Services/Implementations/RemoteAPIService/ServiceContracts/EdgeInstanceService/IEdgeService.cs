using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace sones.GraphDS.Services.RemoteAPIService.ServiceContracts.EdgeInstanceService
{
    [ServiceContract(Namespace = sonesRPCServer.Namespace)]
    public interface IEdgeService : IGraphElementService
    {

    }
}
