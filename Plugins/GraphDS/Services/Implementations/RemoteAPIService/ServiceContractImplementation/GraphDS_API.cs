using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using sones.GraphDB.TypeSystem;
using sones.GraphDB.Request;
using sones.GraphDS.Services.RemoteAPIService.API_Services;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.VertexType;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceRequests;


namespace sones.GraphDS.Services.RemoteAPIService.ServiceContractImplementation
{
    
    public partial class RPCServiceContract : IGraphDS_API
    {


        public ServiceVertexType CreateVertexType(ServiceVertexTypePredefinition myVertexTypePreDef)
        {
            VertexTypePredefinition Predefinition = myVertexTypePreDef.ToVertexTypePredefinition();
            return new ServiceVertexType(GraphDS.CreateVertexType<IVertexType>(null, null, new RequestCreateVertexType(Predefinition), (Statistics, VertexType) => VertexType));
        }
    }
}
