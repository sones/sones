using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceRequests;


namespace sones.GraphDS.Services.RemoteAPIService.IncomingEdgeService
{
    [ServiceContract(Namespace = "http://www.sones.com")]
    public interface IIncominEdgeService
    {

        [OperationContract]
        ServiceIncomingEdgePredefinition SetOutgoingEdge(ServiceIncomingEdgePredefinition myPreDef, ServiceVertexTypePredefinition myVertexType, ServiceOutgoingEdgePredefinition myOutgoingEdge);
        
    }
}
