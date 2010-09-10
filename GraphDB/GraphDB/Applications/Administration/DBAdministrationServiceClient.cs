/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
* 
*/

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
