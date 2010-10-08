
#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;

#endregion

namespace sones.GraphDS.Connectors.REST
{

    public class GraphDSREST_Client
    {

        public static IGraphDSREST_Service ConnectToAdministrationServiceHost(String ServiceURL)
        {

            var _WebHttpBinding = new WebHttpBinding { ReceiveTimeout = TimeSpan.MaxValue };

            //binding.PortSharingEnabled = false;
            //binding.ReliableSession.InactivityTimeout = TimeSpan.MaxValue;
            //binding.Security.Mode = SecurityMode.TransportWithMessageCredential;

            // WebChannelFactory! to fix "Manual addressing is enabled on this factory" error
            var _WebChannelFactory = new WebChannelFactory<IGraphDSREST_Service>(_WebHttpBinding, new Uri(ServiceURL));
            //factory.Credentials.ClientCertificate.SetCertificate(StoreLocation.CurrentUser, StoreName.My, X509FindType.FindByIssuerName, "SONES GmbH - Mail CA");

            return _WebChannelFactory.CreateChannel(); //  (new EndpointAddress(ServiceURL));

        }

    }

}
