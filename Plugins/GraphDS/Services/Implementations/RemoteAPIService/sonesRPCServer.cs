/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.ServiceModel;

namespace sones.GraphDS.Services.RemoteAPIService
{
    public class sonesRPCServer
    {
        #region Data

        /// <summary>
        /// The current IGraphDS instance
        /// </summary>
        private IGraphDS _GraphDS;

        /// <summary>
        /// The current listening ipaddress
        /// </summary>
        private IPAddress ListeningIPAdress { get; private set; }

        /// <summary>
        /// The current listening port
        /// </summary>
        public ushort ListeningPort { get; private set; }

        /// <summary>
        /// Indicates wether the Server uses SSL 
        /// </summary>
        public Boolean IsSecure { get; private set; }
        
        /// <summary>
        /// Indicates wether the Server is running
        /// </summary>
        public Boolean IsRunning { get; private set; }

        #endregion

        #region C'tor

        public sonesRPCServer(IGraphDS myGraphDS, IPAddress myIPAdress, ushort myPort, Boolean myIsSecure)
        {
            this._GraphDS = myGraphDS;
            this.IsSecure = myIsSecure;
            this.ListeningIPAdress = myIPAdress;
            this.ListeningPort = myPort;

        }

        #endregion

        #region Initialize WCF Service
 
        private void InitializeServer()
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.Name = "sonesBasic";
            binding.Namespace = "http://www.sones.com";
            binding.MessageEncoding = WSMessageEncoding.Text;


            binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;

            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;




            binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;



            Uri address = new Uri("http://localhost:9970/rpc");

            RPCServiceContract myContract = new RPCServiceContract(myGraphDSServer);


            // Create a ServiceHost for the CalculatorService type and provide the base address.
            ServiceHost serviceHost = new ServiceHost(myContract, address);



            serviceHost.Description.Namespace = "http://www.sones.com";

            ContractDescription VertexTypeContract = ContractDescription.GetContract(typeof(IVertexTypeService));
            ContractDescription RPCServiceContract = ContractDescription.GetContract(typeof(IRPCServiceContract));
            ContractDescription APIContract = ContractDescription.GetContract(typeof(IGraphDS_API));
            ContractDescription IncomingEdgeContract = ContractDescription.GetContract(typeof(IIncominEdgeService));

            ServiceEndpoint APIService = new ServiceEndpoint(APIContract, binding, new EndpointAddress("http://localhost:9970/rpc"));
            ServiceEndpoint IncomingEdgeService = new ServiceEndpoint(IncomingEdgeContract, binding, new EndpointAddress("http://localhost:9970/rpc"));

            ServiceEndpoint VertexTypeService = new ServiceEndpoint(VertexTypeContract, binding, new EndpointAddress("http://localhost:9970/rpc"));
            ServiceEndpoint RPCServiceService = new ServiceEndpoint(RPCServiceContract, binding, new EndpointAddress("http://localhost:9970/rpc"));



            serviceHost.AddServiceEndpoint(RPCServiceService);
            serviceHost.AddServiceEndpoint(VertexTypeService);
            serviceHost.AddServiceEndpoint(APIService);
            serviceHost.AddServiceEndpoint(IncomingEdgeService);







            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();

            smb.HttpGetEnabled = true;




            //smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
            serviceHost.Description.Behaviors.Add(smb);
            // Add MEX endpoint

            serviceHost.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexHttpBinding(), "mex");
            foreach (ServiceEndpoint endpoint in serviceHost.Description.Endpoints)
            {
                endpoint.Behaviors.Add(new WsdlExtensions(new WsdlExtensionsConfig() { SingleFile = true }));

            }

            serviceHost.Open();
        
        }



        #endregion


    }
}
