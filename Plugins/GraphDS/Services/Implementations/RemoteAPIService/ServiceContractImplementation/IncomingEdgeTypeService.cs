using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDS.Services.RemoteAPIService.IncomingEdgeService;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceRequests;

namespace sones.GraphDS.Services.RemoteAPIService.ServiceContractImplementation
{
    public partial class RPCServiceContract : IIncominEdgeService
    {
        public const char TypeSeparator = '.';

        
        public DataContracts.ServiceRequests.ServiceIncomingEdgePredefinition SetOutgoingEdge(ServiceIncomingEdgePredefinition myPreDef, ServiceVertexTypePredefinition myVertexType, ServiceOutgoingEdgePredefinition myOutgoingEdge)
        {
            if (myVertexType != null && myOutgoingEdge != null)
                myPreDef.AttributeType = Combine(myVertexType.VertexTypeName, myOutgoingEdge.AttributeName);

            return myPreDef;
        }

        public string Combine(string myTargetType, string myTargetEdgeName)
        {
            return String.Join(TypeSeparator.ToString(), myTargetType, myTargetEdgeName);
        }

        
    }
}
