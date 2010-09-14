using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

using sones.GraphFS.Session;

namespace sones.GraphFS.Administration
{
    public static class FSAdministrationServiceClient
    {
        /// <summary>
        /// Connects to an FS Administration Service Host and returns a 
        /// IGraphFSSession Interface to it.
        /// </summary>
        /// <param name="ServiceURL">the URL to connect, starting with net.tcp://, for example: net.tcp://localhost:8112</param>
        /// <returns>an IGraphFSSession instance</returns>
        public static IGraphFSSession ConnectToAdministrationServiceHost(String ServiceURL)
        {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
            binding.ReceiveTimeout = TimeSpan.MaxValue;
            binding.PortSharingEnabled = false;
            #if(!__MonoCS__)
            binding.ReliableSession.InactivityTimeout = TimeSpan.MaxValue;
            #endif

            #region Security
            //binding.Security.Mode = SecurityMode.TransportWithMessageCredential;
            #endregion

            ChannelFactory<IGraphFSSession> factory = new ChannelFactory<IGraphFSSession>(binding);
            //factory.Credentials.ClientCertificate.SetCertificate(StoreLocation.CurrentUser, StoreName.My, X509FindType.FindByIssuerName, "SONES GmbH - Mail CA");

            return factory.CreateChannel(new EndpointAddress(ServiceURL));
        }
    }
}
