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
