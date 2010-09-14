///* <id name="GraphDB – AdministrationServiceClient" />
// * <copyright file="AdministrationServiceClient.cs"
// *            company="sones GmbH">
// * Copyright (c) sones GmbH. All rights reserved.
// * </copyright>
// * <developer>Daniel Kirstenpfad</developer>
// * <summary>Implements the AdministrationServiceClient used by the Administration GUI</summary>
// */

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.ServiceModel;

//namespace sones.GraphDB.Applications.Administration
//{
//    public static class DBAdministrationServiceClient
//    {
//        /// <summary>
//        /// Connects to an DB Administration Service Host and returns a 
//        /// IGraphDBSession Interface to it.
//        /// </summary>
//        /// <param name="ServiceURL">the URL to connect, starting with net.tcp://, for example: net.tcp://localhost:8112</param>
//        /// <returns>an IGraphDBSession</returns>
//        public static IGraphDBSession ConnectToAdministrationServiceHost(String ServiceURL)
//        {
//            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
//            binding.ReceiveTimeout = TimeSpan.MaxValue;
//            binding.PortSharingEnabled = false;
//            #if (!__MonoCS__)
//            binding.ReliableSession.InactivityTimeout = TimeSpan.MaxValue;
//            #endif

//            #region Security
//            //binding.Security.Mode = SecurityMode.TransportWithMessageCredential;
//            #endregion

//            ChannelFactory<IGraphDBSession> factory = new ChannelFactory<IGraphDBSession>(binding);
//            //factory.Credentials.ClientCertificate.SetCertificate(StoreLocation.CurrentUser, StoreName.My, X509FindType.FindByIssuerName, "SONES GmbH - Mail CA");

//            return factory.CreateChannel(new EndpointAddress(ServiceURL));
//        }
//    }
//}
