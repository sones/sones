/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
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
*/



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
