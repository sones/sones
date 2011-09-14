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
using sones.GraphDS.Services.RemoteAPIService.ErrorHandling;

namespace sones.GraphDS.Services.RemoteAPIService.ServiceContractImplementation
{
    public partial class RPCServiceContract : IVertexService
    {
        public bool HasIncomingVertices(ServiceSecurityToken mySecToken, Int64 myTransToken,
            long myVertexTypeID, ServiceVertexInstance myVertex, long myEdgePropertyID)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertexTypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.HasIncomingVertices(myVertexTypeID, myEdgePropertyID);
        }

        public List<Tuple<long, long, List<ServiceVertexInstance>>> GetAllIncomingVertices(ServiceSecurityToken mySecToken, Int64 myTransToken,
            ServiceVertexInstance myVertex)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.GetAllIncomingVertices().Select(x => new Tuple<long, long, List<ServiceVertexInstance>>(x.Item1, x.Item2, x.Item3.Select(y => new ServiceVertexInstance(y)).ToList())).ToList();
        }

        public List<ServiceVertexInstance> GetIncomingVertices(ServiceSecurityToken mySecToken, Int64 myTransToken,
            long myVertexTypeID, ServiceVertexInstance myVertex, long myEdgePropertyID)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.GetIncomingVertices(myVertexTypeID, myEdgePropertyID).Select(x => new ServiceVertexInstance(x)).ToList();
        }

        public bool HasOutgoingEdge(ServiceSecurityToken mySecToken, Int64 myTransToken,
            ServiceVertexInstance myVertex, long myEdgePropertyID)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.HasOutgoingEdge(myEdgePropertyID);
        }

        public List<Tuple<long, ServiceEdgeInstance>> GetAllOutgoingEdges(ServiceSecurityToken mySecToken, Int64 myTransToken,
            ServiceVertexInstance myVertex)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.GetAllOutgoingEdges().Select(x =>
            {
                if (x.Item2 is ISingleEdge)
                    return new Tuple<long, ServiceEdgeInstance>(x.Item1, new ServiceSingleEdgeInstance(x.Item2 as ISingleEdge, x.Item1));
                else
                    return new Tuple<long, ServiceEdgeInstance>(x.Item1, new ServiceHyperEdgeInstance(x.Item2 as IHyperEdge, x.Item1));
            }).ToList();
        }

        public List<Tuple<long, ServiceHyperEdgeInstance>> GetAllOutgoingHyperEdges(ServiceSecurityToken mySecToken, Int64 myTransToken,
            ServiceVertexInstance myVertex)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.GetAllOutgoingHyperEdges().Select(x => new Tuple<long, ServiceHyperEdgeInstance>(x.Item1, new ServiceHyperEdgeInstance(x.Item2, x.Item1))).ToList();
        }

        public List<Tuple<long, ServiceSingleEdgeInstance>> GetAllOutgoingSingleEdges(ServiceSecurityToken mySecToken, Int64 myTransToken,
            ServiceVertexInstance myVertex)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.GetAllOutgoingSingleEdges().Select(x => new Tuple<long, ServiceSingleEdgeInstance>(x.Item1, new ServiceSingleEdgeInstance(x.Item2, x.Item1))).ToList();
        }

        public ServiceEdgeInstance GetOutgoingEdge(ServiceSecurityToken mySecToken, Int64 myTransToken,
            ServiceVertexInstance myVertex, long myEdgePropertyID)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            var Edge = Response.GetOutgoingEdge(myEdgePropertyID);
            if(Edge is ISingleEdge)
                return new ServiceSingleEdgeInstance(Edge as ISingleEdge, myEdgePropertyID);
            else
                return new ServiceHyperEdgeInstance(Edge as IHyperEdge, myEdgePropertyID);
        }

        public ServiceHyperEdgeInstance GetOutgoingHyperEdge(ServiceSecurityToken mySecToken, Int64 myTransToken,
            ServiceVertexInstance myVertex, long myEdgePropertyID)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            var value = Response.GetOutgoingHyperEdge(myEdgePropertyID);
            if (value != null)
            {
                return new ServiceHyperEdgeInstance(Response.GetOutgoingHyperEdge(myEdgePropertyID), myEdgePropertyID);
            }
            return null;
            
        }

        public ServiceSingleEdgeInstance GetOutgoingSingleEdge(ServiceSecurityToken mySecToken, Int64 myTransToken,
            ServiceVertexInstance myVertex, long myEdgePropertyID)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            var value = Response.GetOutgoingSingleEdge(myEdgePropertyID);
            if (value != null)
            {
                return new ServiceSingleEdgeInstance(value, myEdgePropertyID);
            }
            return null;                            
        }

        public System.IO.Stream GetBinaryProperty(ServiceSecurityToken mySecToken, Int64 myTransToken,
            ServiceVertexInstance myVertex, long myPropertyID)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            throw new NotImplementedException();
        }

        public List<Tuple<long, System.IO.Stream>> GetAllBinaryProperties(ServiceSecurityToken mySecToken, Int64 myTransToken,
            ServiceVertexInstance myVertex)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            throw new NotImplementedException();
        }

        public object GetProperty(ServiceSecurityToken mySecToken, Int64 myTransToken,
            ServiceVertexInstance myVertex, long myPropertyID)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.GetProperty(myPropertyID);
        }

        public bool HasProperty(ServiceSecurityToken mySecToken, Int64 myTransToken,
            ServiceVertexInstance myVertex, long myPropertyID)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.HasProperty(myPropertyID);
        }

        public int GetCountOfProperties(ServiceSecurityToken mySecToken, Int64 myTransToken,
            ServiceVertexInstance myVertex)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.GetCountOfProperties();
        }

        public List<Tuple<long, object>> GetAllProperties(ServiceSecurityToken mySecToken, Int64 myTransToken,
            ServiceVertexInstance myVertex)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.GetAllProperties().Select(x => new Tuple<long, object>(x.Item1, (object)x.Item2)).ToList();
        }

        public string GetPropertyAsString(ServiceSecurityToken mySecToken, Int64 myTransToken,
            ServiceVertexInstance myVertex, long myPropertyID)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.GetPropertyAsString(myPropertyID);
        }

        public object GetUnstructuredProperty(ServiceSecurityToken mySecToken, Int64 myTransToken,
            ServiceVertexInstance myVertex, string myPropertyName)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.GetUnstructuredProperty<object>(myPropertyName);
        }

        public bool HasUnstructuredProperty(ServiceSecurityToken mySecToken, Int64 myTransToken,
            ServiceVertexInstance myVertex, string myPropertyName)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.HasUnstructuredProperty(myPropertyName);
        }

        public int GetCountOfUnstructuredProperties(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexInstance myVertex)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.GetCountOfUnstructuredProperties();
        }

        public List<Tuple<string, object>> GetAllUnstructuredProperties(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexInstance myVertex)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.GetAllUnstructuredProperties().ToList();
        }

        public string GetUnstructuredPropertyAsString(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexInstance myVertex, string myPropertyName)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.GetUnstructuredPropertyAsString(myPropertyName);
        }

        public string Comment(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexInstance myVertex)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.Comment;
        }

        public long CreationDate(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexInstance myVertex)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.CreationDate;
        }

        public long ModificationDate(ServiceSecurityToken mySecToken, Int64 myTransToken, ServiceVertexInstance myVertex)
        {
            SecurityToken myDBSecToken;
            if (!SecurityTokenMap.TryGetValue(mySecToken, out myDBSecToken))
                throw new SecurityException("The givin ServiceSecurityToken was violated! The request was not executed.");
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myVertex.TypeID, myVertex.VertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(myDBSecToken, myTransToken, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.ModificationDate;
        }
    }
}
