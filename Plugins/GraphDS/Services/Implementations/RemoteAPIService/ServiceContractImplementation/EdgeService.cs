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
using sones.GraphDS.Services.RemoteAPIService.ServiceContracts.EdgeInstanceService;

namespace sones.GraphDS.Services.RemoteAPIService.ServiceContractImplementation
{
    public partial class RPCServiceContract : IEdgeService
    {
        
        #region IGraphElementService
                        
        public object GetProperty(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeInstance myEdge, long myPropertyID)
        {
            var Request = ServiceRequestFactory.MakeRequestGetVertex(myEdge.SourceVertexTypeID, myEdge.SourceVertexID);
            var Response = this.GraphDS.GetVertex<IVertex>(mySecToken, myTransToken.TransactionID, Request, ServiceReturnConverter.ConvertOnlyVertexInstance);
            return Response.GetProperty(myPropertyID);
        }

        public bool HasProperty(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeInstance myEdge long myPropertyID)
        {
            throw new NotImplementedException();
        }

        public int GetCountOfProperties(SecurityToken mySecToken, ServiceTransactionToken myTransToken, AGraphElement myGraphElement)
        {
            throw new NotImplementedException();
        }

        public List<Tuple<long, object>> GetAllProperties(SecurityToken mySecToken, ServiceTransactionToken myTransToken, AGraphElement myGraphElement)
        {
            throw new NotImplementedException();
        }

        public string GetPropertyAsString(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeInstance myEdge long myPropertyID)
        {
            throw new NotImplementedException();
        }

        public object GetUnstructuredProperty(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeInstance myEdge string myPropertyName)
        {
            throw new NotImplementedException();
        }

        public bool HasUnstructuredProperty(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeInstance myEdge string myPropertyName)
        {
            throw new NotImplementedException();
        }

        public int GetCountOfUnstructuredProperties(SecurityToken mySecToken, ServiceTransactionToken myTransToken, AGraphElement myGraphElement)
        {
            throw new NotImplementedException();
        }

        public List<Tuple<string, object>> GetAllUnstructuredProperties(SecurityToken mySecToken, ServiceTransactionToken myTransToken, AGraphElement myGraphElement)
        {
            throw new NotImplementedException();
        }

        public string GetUnstructuredPropertyAsString(SecurityToken mySecToken, ServiceTransactionToken myTransToken, ServiceEdgeInstance myEdge string myPropertyName)
        {
            throw new NotImplementedException();
        }

        public string Comment(SecurityToken mySecToken, ServiceTransactionToken myTransToken, AGraphElement myGraphElement)
        {
            throw new NotImplementedException();
        }

        public long CreationDate(SecurityToken mySecToken, ServiceTransactionToken myTransToken, AGraphElement myGraphElement)
        {
            throw new NotImplementedException();
        }

        public long ModificationDate(SecurityToken mySecToken, ServiceTransactionToken myTransToken, AGraphElement myGraphElement)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
