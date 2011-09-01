using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDS.Services.RemoteAPIService.ServiceContracts.VertexInstanceService;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.InstanceObjects;

namespace sones.GraphDS.Services.RemoteAPIService.ServiceContractImplementation
{
    public partial class RPCServiceContract : IVertexService
    {

        public bool HasIncomingVertices(ServiceVertexInstance myVertex, long myVertexTypeID, long myEdgePropertyID)
        {
            throw new NotImplementedException();
        }

        public List<Tuple<long, long, List<ServiceVertexInstance>>> GetAllIncomingVertices(ServiceVertexInstance myVertex)
        {
            throw new NotImplementedException();
        }

        public List<ServiceVertexInstance> GetIncomingVertices(ServiceVertexInstance myVertex, long myVertexTypeID, long myEdgePropertyID)
        {
            throw new NotImplementedException();
        }

        public bool HasOutgoingEdge(ServiceVertexInstance myVertex, long myEdgePropertyID)
        {
            throw new NotImplementedException();
        }

        public List<Tuple<long, ServiceEdgeInstance>> GetAllOutgoingEdges(ServiceVertexInstance myVertex)
        {
            throw new NotImplementedException();
        }

        public List<Tuple<long, ServiceHyperEdgeInstance>> GetAllOutgoingHyperEdges(ServiceVertexInstance myVertex)
        {
            throw new NotImplementedException();
        }

        public List<Tuple<long, ServiceSingleEdgeInstance>> GetAllOutgoingSingleEdges(ServiceVertexInstance myVertex)
        {
            throw new NotImplementedException();
        }

        public ServiceEdgeInstance GetOutgoingEdge(ServiceVertexInstance myVertex, long myEdgePropertyID)
        {
            throw new NotImplementedException();
        }

        public ServiceEdgeInstance GetOutgoingHyperEdge(ServiceVertexInstance myVertex, long myEdgePropertyID)
        {
            throw new NotImplementedException();
        }

        public ServiceEdgeInstance GetOutgoingSingleEdge(ServiceVertexInstance myVertex, long myEdgePropertyID)
        {
            throw new NotImplementedException();
        }

        public System.IO.Stream GetBinaryProperty(ServiceVertexInstance myVertex, long myPropertyID)
        {
            throw new NotImplementedException();
        }

        public List<Tuple<long, System.IO.Stream>> GetAllBinaryProperties(ServiceVertexInstance myVertex)
        {
            throw new NotImplementedException();
        }

        public object GetProperty(ServiceVertexInstance myVertex, long myPropertyID)
        {
            throw new NotImplementedException();
        }

        public bool HasProperty(ServiceVertexInstance myVertex, long myPropertyID)
        {
            throw new NotImplementedException();
        }

        public int GetCountOfProperties(ServiceVertexInstance myVertex)
        {
            throw new NotImplementedException();
        }

        public List<Tuple<long, object>> GetAllProperties(ServiceVertexInstance myVertex)
        {
            throw new NotImplementedException();
        }

        public string GetPropertyAsString(ServiceVertexInstance myVertex, long myPropertyID)
        {
            throw new NotImplementedException();
        }

        public object GetUnstructuredProperty(ServiceVertexInstance myVertex, string myPropertyName)
        {
            throw new NotImplementedException();
        }

        public bool HasUnstructuredProperty(ServiceVertexInstance myVertex, string myPropertyName)
        {
            throw new NotImplementedException();
        }

        public int GetCountOfUnstructuredProperties(ServiceVertexInstance myVertex)
        {
            throw new NotImplementedException();
        }

        public List<Tuple<string, object>> GetAllUnstructuredProperties(ServiceVertexInstance myVertex)
        {
            throw new NotImplementedException();
        }

        public string GetUnstructuredPropertyAsString(ServiceVertexInstance myVertex, string myPropertyName)
        {
            throw new NotImplementedException();
        }

        public string Comment(ServiceVertexInstance myVertex)
        {
            throw new NotImplementedException();
        }

        public long CreationDate(ServiceVertexInstance myVertex)
        {
            throw new NotImplementedException();
        }

        public long ModificationDate(ServiceVertexInstance myVertex)
        {
            throw new NotImplementedException();
        }
    }
}
