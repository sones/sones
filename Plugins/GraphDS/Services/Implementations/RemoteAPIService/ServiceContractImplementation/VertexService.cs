using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;
using sones.GraphDS.Services.RemoteAPIService.ServiceContracts.VertexInstanceService;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.InstanceObjects;
using sones.GraphDS.Services.RemoteAPIService.ServiceConverter;
using sones.Library.PropertyHyperGraph;
using sones.Library.Commons.Security;
using sones.GraphDS.Services.RemoteAPIService.DataContracts;

namespace sones.GraphDS.Services.RemoteAPIService.ServiceContractImplementation
{
    public partial class RPCServiceContract : IVertexService
    {
        #region IVertexService

        public bool HasIncomingVertices(SecurityToken mySecToken, ServiceTransactionToken myTransToken,
            long myVertexTypeID, ServiceVertexInstance myVertex, long myEdgePropertyID)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertexTypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.HasIncomingVertices(myVertexTypeID, myEdgePropertyID);
        }

        public List<Tuple<long, long, List<ServiceVertexInstance>>> GetAllIncomingVertices(SecurityToken mySecToken, ServiceTransactionToken myTransToken,
            ServiceVertexInstance myVertex)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.GetAllIncomingVertices().Select(x => new Tuple<long, long, List<ServiceVertexInstance>>(x.Item1, x.Item2, x.Item3.Select(y => new ServiceVertexInstance(y)).ToList())).ToList();
        }

        public List<ServiceVertexInstance> GetIncomingVertices(SecurityToken mySecToken, ServiceTransactionToken myTransToken,
            long myVertexTypeID, ServiceVertexInstance myVertex, long myEdgePropertyID)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.GetIncomingVertices(myVertexTypeID, myEdgePropertyID).Select(x => new ServiceVertexInstance(x)).ToList();
        }

        public bool HasOutgoingEdge(SecurityToken mySecToken, ServiceTransactionToken myTransToken,
            ServiceVertexInstance myVertex, long myEdgePropertyID)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.HasOutgoingEdge(myEdgePropertyID);
        }

        public List<Tuple<long, ServiceEdgeInstance>> GetAllOutgoingEdges(SecurityToken mySecToken, ServiceTransactionToken myTransToken,
            ServiceVertexInstance myVertex)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.GetAllOutgoingEdges().Select(x => new Tuple<long, ServiceEdgeInstance>(x.Item1, new ServiceEdgeInstance(x.Item2))).ToList();
        }

        public List<Tuple<long, ServiceHyperEdgeInstance>> GetAllOutgoingHyperEdges(SecurityToken mySecToken, ServiceTransactionToken myTransToken,
            ServiceVertexInstance myVertex)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.GetAllOutgoingHyperEdges
        }

        public List<Tuple<long, ServiceSingleEdgeInstance>> GetAllOutgoingSingleEdges(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexInstance myVertex)
        {
            throw new NotImplementedException();
        }

        public ServiceEdgeInstance GetOutgoingEdge(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexInstance myVertex, long myEdgePropertyID)
        {
            throw new NotImplementedException();
        }

        public ServiceEdgeInstance GetOutgoingHyperEdge(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexInstance myVertex, long myEdgePropertyID)
        {
            throw new NotImplementedException();
        }

        public ServiceEdgeInstance GetOutgoingSingleEdge(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexInstance myVertex, long myEdgePropertyID)
        {
            throw new NotImplementedException();
        }

        public System.IO.Stream GetBinaryProperty(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexInstance myVertex, long myPropertyID)
        {
            throw new NotImplementedException();
        }

        public List<Tuple<long, System.IO.Stream>> GetAllBinaryProperties(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexInstance myVertex)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IGraphElementService

        public object GetProperty(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexInstance myVertex, long myPropertyID)
        {
            throw new NotImplementedException();
        }

        public bool HasProperty(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexInstance myVertex, long myPropertyID)
        {
            throw new NotImplementedException();
        }

        public int GetCountOfProperties(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexInstance myVertex)
        {
            throw new NotImplementedException();
        }

        public List<Tuple<long, object>> GetAllProperties(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexInstance myVertex)
        {
            throw new NotImplementedException();
        }

        public string GetPropertyAsString(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexInstance myVertex, long myPropertyID)
        {
            throw new NotImplementedException();
        }

        public object GetUnstructuredProperty(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexInstance myVertex, string myPropertyName)
        {
            throw new NotImplementedException();
        }

        public bool HasUnstructuredProperty(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexInstance myVertex, string myPropertyName)
        {
            throw new NotImplementedException();
        }

        public int GetCountOfUnstructuredProperties(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexInstance myVertex)
        {
            throw new NotImplementedException();
        }

        public List<Tuple<string, object>> GetAllUnstructuredProperties(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexInstance myVertex)
        {
            throw new NotImplementedException();
        }

        public string GetUnstructuredPropertyAsString(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexInstance myVertex, string myPropertyName)
        {
            throw new NotImplementedException();
        }

        public string Comment(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexInstance myVertex)
        {
            throw new NotImplementedException();
        }

        public long CreationDate(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexInstance myVertex)
        {
            throw new NotImplementedException();
        }

        public long ModificationDate(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexInstance myVertex)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
