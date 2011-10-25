using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.IO;

namespace sones.GraphDS.Services.RemoteAPIService.ServiceContracts.MonoMEX
{
    [ServiceContract(Namespace = sonesRPCServer.Namespace)]
    public interface IMonoMEX
    { 
        [OperationContract]
        [WebGet(UriTemplate = "/wsdl", BodyStyle = WebMessageBodyStyle.Bare)]
        Stream GetWSDL();
    }
}
