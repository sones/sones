using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphDSRPC.DataContracts.VertexType;
using sones.GraphDB.TypeSystem;
using GraphDSRPC.DataContracts.ServiceTypeManagement;
using sones.GraphDB.Request;
using GraphDSRPC.DataContracts.ServiceRequests;

namespace sones.GraphDS.Services.RemoteAPIService.ServiceConverter
{
    public static class ServiceVertexTypeConverter
    {
        

        public static IEnumerable<ServiceBinaryPropertyDefinition> ConvertAllBinaryPropertiesToService(IEnumerable<IBinaryPropertyDefinition> myBinaryProperties)
        {
            List<ServiceBinaryPropertyDefinition> binaryProperties = new List<ServiceBinaryPropertyDefinition>();
            foreach (var binaryProperty in myBinaryProperties)
            {
                binaryProperties.Add(new ServiceBinaryPropertyDefinition(binaryProperty));
            }
            return binaryProperties;
        }
        
        public static IEnumerable<ServiceOutgoingEdgeDefinition> ConvertAllOutgoingEdgesToService(IEnumerable<IOutgoingEdgeDefinition> myOutgiongEdges)
        {
            List<ServiceOutgoingEdgeDefinition> outgoingEdges = new List<ServiceOutgoingEdgeDefinition>();
            foreach (var outgoingEdge in myOutgiongEdges)
            {
                outgoingEdges.Add(new ServiceOutgoingEdgeDefinition(outgoingEdge));
            }
            return outgoingEdges;
        }
        
        public static IEnumerable<ServiceIncomingEdgeDefinition> ConvertAllIncomingEdgesToService(IEnumerable<IIncomingEdgeDefinition> myIncomingEdges)
        {
            List<ServiceIncomingEdgeDefinition> incomingEdges = new List<ServiceIncomingEdgeDefinition>();
            foreach (var incomingEdge in myIncomingEdges)
            {
                incomingEdges.Add(new ServiceIncomingEdgeDefinition(incomingEdge));
            }
            return incomingEdges;
        }
        
        public static IEnumerable<ServiceIndexDefinition> ConvertAllIndicesToService(IEnumerable<IIndexDefinition> myInIndices)
        {
            List<ServiceIndexDefinition> inIndices = new List<ServiceIndexDefinition>();
            foreach (var Index in myInIndices)
            {
                inIndices.Add(new ServiceIndexDefinition(Index));
            }
            return inIndices;
        }

        public static IEnumerable<ServicePropertyDefinition> ConvertAllPropertiesToService(IEnumerable<IPropertyDefinition> myPropertyList)
        {
            List<ServicePropertyDefinition> propertyList = new List<ServicePropertyDefinition>();
            foreach (var Property in myPropertyList)
            {
                propertyList.Add(new ServicePropertyDefinition(Property));
            }
            return propertyList;
        }
        
        
    }
}
