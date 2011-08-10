using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.VertexType;
using sones.GraphDS.Services.RemoteAPIService.DataContracts.ServiceRequests;


namespace sones.GraphDS.Services.RemoteAPIService.API_Services
{
    [ServiceContract(Namespace = "http://www.sones.com", Name = "IGraphDS")]
    public interface IGraphDS_API
    {
        [OperationContract]
        ServiceVertexType CreateVertexType(ServiceVertexTypePredefinition myVertexTypePreDef); 
                                                                    
    }
}
