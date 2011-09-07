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
            return Response.GetAllOutgoingEdges().Select(x =>
            {
                if (x.Item2 is ISingleEdge)
                    return new Tuple<long, ServiceEdgeInstance>(x.Item1, new ServiceSingleEdgeInstance(x.Item2 as ISingleEdge, x.Item1));
                else
                    return new Tuple<long, ServiceEdgeInstance>(x.Item1, new ServiceHyperEdgeInstance(x.Item2 as IHyperEdge, x.Item1));
            }).ToList();
        }

        public List<Tuple<long, ServiceHyperEdgeInstance>> GetAllOutgoingHyperEdges(SecurityToken mySecToken, ServiceTransactionToken myTransToken,
            ServiceVertexInstance myVertex)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.GetAllOutgoingHyperEdges().Select(x => new Tuple<long, ServiceHyperEdgeInstance>(x.Item1, new ServiceHyperEdgeInstance(x.Item2, x.Item1))).ToList();
        }

        public List<Tuple<long, ServiceSingleEdgeInstance>> GetAllOutgoingSingleEdges(SecurityToken mySecToken, ServiceTransactionToken myTransToken,
            ServiceVertexInstance myVertex)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.GetAllOutgoingSingleEdges().Select(x => new Tuple<long, ServiceSingleEdgeInstance>(x.Item1, new ServiceSingleEdgeInstance(x.Item2, x.Item1))).ToList();
        }

        public ServiceEdgeInstance GetOutgoingEdge(SecurityToken mySecToken, ServiceTransactionToken myTransToken,
            ServiceVertexInstance myVertex, long myEdgePropertyID)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            var Edge = Response.GetOutgoingEdge(myEdgePropertyID);
            if(Edge is ISingleEdge)
                return new ServiceSingleEdgeInstance(Edge as ISingleEdge, myEdgePropertyID);
            else
                return new ServiceHyperEdgeInstance(Edge as IHyperEdge, myEdgePropertyID);
        }

        public ServiceHyperEdgeInstance GetOutgoingHyperEdge(SecurityToken mySecToken, ServiceTransactionToken myTransToken,
            ServiceVertexInstance myVertex, long myEdgePropertyID)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            var value = Response.GetOutgoingHyperEdge(myEdgePropertyID);
            if (value != null)
            {
                return new ServiceHyperEdgeInstance(Response.GetOutgoingHyperEdge(myEdgePropertyID), myEdgePropertyID);
            }
            return null;
            
        }

        public ServiceSingleEdgeInstance GetOutgoingSingleEdge(SecurityToken mySecToken, ServiceTransactionToken myTransToken,
            ServiceVertexInstance myVertex, long myEdgePropertyID)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            var value = Response.GetOutgoingSingleEdge(myEdgePropertyID);
            if (value != null)
            {
                return new ServiceSingleEdgeInstance(value, myEdgePropertyID);
            }
            return null;                            
        }

        public System.IO.Stream GetBinaryProperty(SecurityToken mySecToken, ServiceTransactionToken myTransToken,
            ServiceVertexInstance myVertex, long myPropertyID)
        {
            throw new NotImplementedException();
        }

        public List<Tuple<long, System.IO.Stream>> GetAllBinaryProperties(SecurityToken mySecToken, ServiceTransactionToken myTransToken,
            ServiceVertexInstance myVertex)
        {
            throw new NotImplementedException();
        }

        public object GetProperty(SecurityToken mySecToken, ServiceTransactionToken myTransToken,
            ServiceVertexInstance myVertex, long myPropertyID)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.GetProperty(myPropertyID);
        }

        public bool HasProperty(SecurityToken mySecToken, ServiceTransactionToken myTransToken,
            ServiceVertexInstance myVertex, long myPropertyID)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.HasProperty(myPropertyID);
        }

        public int GetCountOfProperties(SecurityToken mySecToken, ServiceTransactionToken myTransToken,
            ServiceVertexInstance myVertex)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.GetCountOfProperties();
        }

        public List<Tuple<long, object>> GetAllProperties(SecurityToken mySecToken, ServiceTransactionToken myTransToken,
            ServiceVertexInstance myVertex)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.GetAllProperties().Select(x => new Tuple<long, object>(x.Item1, (object)x.Item2)).ToList();
        }

        public string GetPropertyAsString(SecurityToken mySecToken, ServiceTransactionToken myTransToken,
            ServiceVertexInstance myVertex, long myPropertyID)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.GetPropertyAsString(myPropertyID);
        }

        public object GetUnstructuredProperty(SecurityToken mySecToken, ServiceTransactionToken myTransToken,
            ServiceVertexInstance myVertex, string myPropertyName)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.GetUnstructuredProperty<object>(myPropertyName);
        }

        public bool HasUnstructuredProperty(SecurityToken mySecToken, ServiceTransactionToken myTransToken,
            ServiceVertexInstance myVertex, string myPropertyName)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.HasUnstructuredProperty(myPropertyName);
        }

        public int GetCountOfUnstructuredProperties(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexInstance myVertex)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.GetCountOfUnstructuredProperties();
        }

        public List<Tuple<string, object>> GetAllUnstructuredProperties(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexInstance myVertex)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.GetAllUnstructuredProperties().ToList();
        }

        public string GetUnstructuredPropertyAsString(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexInstance myVertex, string myPropertyName)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.GetUnstructuredPropertyAsString(myPropertyName);
        }

        public string Comment(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexInstance myVertex)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.Comment;
        }

        public long CreationDate(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexInstance myVertex)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.CreationDate;
        }

        public long ModificationDate(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceVertexInstance myVertex)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.ModificationDate;
        }
    }
}
