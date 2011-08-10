using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using sones.GraphDB.TypeSystem;
using sones.GraphDB.Request;
using sones.GraphDS.Services.RemoteAPIService.ServiceContracts.VertexTypeServices;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.VertexType;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceTypeManagement;


namespace sones.GraphDS.Services.RemoteAPIService.ServiceContractImplementation
{
    
    public partial class RPCServiceContract : AbstractBaseTypeService, IVertexTypeService
    {
        
        public bool HasBinaryProperty(ServiceVertexType myServiceVertexType, string myEdgeName)
        {
            throw new NotImplementedException();
        }

        public ServiceBinaryPropertyDefinition GetBinaryPropertyDefinition(ServiceVertexType myServiceVertexType, string myEdgeName)
        {
            throw new NotImplementedException();
        }

        public bool HasBinaryProperties(ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ServiceBinaryPropertyDefinition> GetBinaryProperties(ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        public bool HasIncomingEdge(ServiceVertexType myServiceVertexType, string myEdgeName)
        {
            throw new NotImplementedException();
        }

        public ServiceIncomingEdgeDefinition GetIncomingEdgeDefinition(ServiceVertexType myServiceVertexType, string myEdgeName)
        {
            throw new NotImplementedException();
        }

        public bool HasIncomingEdges(ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ServiceIncomingEdgeDefinition> GetIncomingEdgeDefinitions(ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        public bool HasOutgoingEdge(ServiceVertexType myServiceVertexType, string myEdgeName)
        {
            throw new NotImplementedException();
        }

        public ServiceOutgoingEdgeDefinition GetOutgoingEdgeDefinition(ServiceVertexType myServiceVertexType, string myEdgeName)
        {
            throw new NotImplementedException();
        }

        public bool HasOutgoingEdges(ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ServiceOutgoingEdgeDefinition> GetOutgoingEdgeDefinitions(ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        public bool HasIndexDefinitions(ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ServiceIndexDefinition> GetIndexDefinitions(ServiceVertexType myServiceVertexType, bool myIncludeAncestorDefinitions)
        {
            throw new NotImplementedException();
        }
    }
}
