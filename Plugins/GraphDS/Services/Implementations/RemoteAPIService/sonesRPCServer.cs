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
using sones.GraphDS.Services.RemoteAPIService.ServiceContractImplementation;
using System.ServiceModel.Description;
using sones.GraphDS.Services.RemoteAPIService.ServiceContracts.VertexTypeServices;
using sones.GraphDS.Services.RemoteAPIService.ServiceContracts;
using sones.GraphDS.Services.RemoteAPIService.API_Services;
using WCFExtras.Wsdl;
using sones.GraphDS.Services.RemoteAPIService.EdgeTypeService;
using sones.GraphDS.Services.RemoteAPIService.ServiceContracts.VertexInstanceService;
using sones.GraphDS.Services.RemoteAPIService.ServiceContracts.EdgeInstanceService;
using sones.GraphDS.Services.RemoteAPIService.ServiceContracts.MonoMEX;

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
        public IPAddress ListeningIPAdress { get; private set; }

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

        /// <summary>
        /// The current used Namespace
        /// </summary>
        public const String Namespace = "http://www.sones.com";
       
       
        /// <summary>
        /// The complete URI of the service
        /// </summary>
        public Uri URI { get; private set; }

        /// <summary>
        /// The WCF Service Host
        /// </summary>
        private ServiceHost _ServiceHost;

        #endregion

        #region C'tor
       
        public sonesRPCServer(IGraphDS myGraphDS, IPAddress myIPAdress, ushort myPort, String myURI, Boolean myIsSecure, Boolean myAutoStart = false)
        {
            this._GraphDS = myGraphDS;
            this.IsSecure = myIsSecure;
            this.ListeningIPAdress = myIPAdress;
            this.ListeningPort = myPort;
            String CompleteUri = (myIsSecure == true ? "https://" : "http://") + myIPAdress.ToString() + ":" + myPort + "/" + myURI;
            this.URI = new Uri(CompleteUri);

            if (!this.URI.IsWellFormedOriginalString())
                throw new Exception("The URI Pattern is not well formed!");

            InitializeServer();

            if (myAutoStart)
                _ServiceHost.Open();

        }

        #endregion

        #region Initialize WCF Service
 
        private void InitializeServer()
        {
            BasicHttpBinding BasicBinding = new BasicHttpBinding();
            
            BasicBinding.Name = "sonesBasic";
            BasicBinding.Namespace = Namespace;
            BasicBinding.MessageEncoding = WSMessageEncoding.Text;
            BasicBinding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;

            if (IsSecure)
            {
                BasicBinding.Security.Mode = BasicHttpSecurityMode.Transport;
            }
            
            
            RPCServiceContract ContractInstance = new RPCServiceContract(_GraphDS);
            
          
            _ServiceHost = new ServiceHost(ContractInstance, this.URI);
            _ServiceHost.Description.Namespace = Namespace;


            #region Global Service Interface

            ContractDescription RPCServiceContract = ContractDescription.GetContract(typeof(IRPCServiceContract));
            RPCServiceContract.Namespace = Namespace;
            ServiceEndpoint RPCServiceService = new ServiceEndpoint(RPCServiceContract, BasicBinding, new EndpointAddress(this.URI.AbsoluteUri));
            _ServiceHost.AddServiceEndpoint(RPCServiceService);

            #endregion

            #region GraphDS API Contract

            ContractDescription APIContract = ContractDescription.GetContract(typeof(IGraphDS_API));
            APIContract.Namespace = Namespace;
            ServiceEndpoint APIService = new ServiceEndpoint(APIContract, BasicBinding, new EndpointAddress(this.URI.ToString()));
            _ServiceHost.AddServiceEndpoint(APIService);

            #endregion

            #region Type Services

            #region VertexTypeService

            ContractDescription VertexTypeServiceContract = ContractDescription.GetContract(typeof(IVertexTypeService));
            VertexTypeServiceContract.Namespace = Namespace;
            ServiceEndpoint VertexTypeService = new ServiceEndpoint(VertexTypeServiceContract, BasicBinding, new EndpointAddress(this.URI.ToString()));
            _ServiceHost.AddServiceEndpoint(VertexTypeService);

            #endregion

            #region VertexInstanceService

            ContractDescription VertexServiceContract = ContractDescription.GetContract(typeof(IVertexService));
            VertexServiceContract.Namespace = Namespace;
            ServiceEndpoint VertexService = new ServiceEndpoint(VertexServiceContract, BasicBinding, new EndpointAddress(this.URI.ToString()));
            _ServiceHost.AddServiceEndpoint(VertexService);

            #endregion
            
            #region EdgeTypeService

            ContractDescription EdgeTypeServiceContract = ContractDescription.GetContract(typeof(IEdgeTypeService));
            EdgeTypeServiceContract.Namespace = Namespace;
            ServiceEndpoint EdgeTypeService = new ServiceEndpoint(EdgeTypeServiceContract, BasicBinding, new EndpointAddress(this.URI.ToString()));
            _ServiceHost.AddServiceEndpoint(EdgeTypeService);

            #endregion

            #region EdgeInstanceService

            ContractDescription EdgeInstanceServiceContract = ContractDescription.GetContract(typeof(IEdgeService));
            EdgeInstanceServiceContract.Namespace = Namespace;
            ServiceEndpoint EdgeInstanceService = new ServiceEndpoint(EdgeInstanceServiceContract, BasicBinding, new EndpointAddress(this.URI.ToString()));
            _ServiceHost.AddServiceEndpoint(EdgeInstanceService);

            #endregion
            
            #endregion
            
            #region Metadata Exchange

            // mono can't export automatic generated WSDL. Because of that, we must do that explicit
                        
            WebHttpBinding WebBinding = new WebHttpBinding();
            WebBinding.Namespace = Namespace;
            var rpc = this.URI.Segments.Last();
            var monoURI = this.URI.ToString().Replace(rpc, "");

            var _ServiceHost2 = new ServiceHost(typeof(MonoMEX), new Uri(monoURI));
            _ServiceHost2.Description.Namespace = Namespace;

            ContractDescription MonoMEX = ContractDescription.GetContract(typeof(IMonoMEX));


            ServiceEndpoint MonoMEXeService = new ServiceEndpoint(MonoMEX, WebBinding, new EndpointAddress(new Uri(monoURI)));

            _ServiceHost2.AddServiceEndpoint(MonoMEXeService);
            _ServiceHost2.Description.Endpoints[0].Behaviors.Add(new System.ServiceModel.Description.WebHttpBehavior());
            _ServiceHost2.Open();


            //the on-the-fly generation of the WSDL leads to several errors on client side (and crashes the server) - so a simple output is necessary

            #region Automatic WCF WSDL Generator

            //ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            //smb.HttpGetEnabled = true;


            //_ServiceHost.Description.Behaviors.Add(smb);

            //// Add MEX endpoint

            //_ServiceHost.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexHttpBinding(), "mex");
            //foreach (ServiceEndpoint endpoint in _ServiceHost.Description.Endpoints)
            //{
            //    //export just one file
            //    endpoint.Behaviors.Add(new WsdlExtensions(new WsdlExtensionsConfig() { SingleFile = true }));

            //} 

            #endregion





            #endregion

        }



        #endregion

        #region Service Host Control

        public void StartServiceHost()
        {
            if(!IsRunning)
            {
                _ServiceHost.Open();
            }
            
        }

        public void StopServiceHost()
        {
            if (IsRunning)
            {
                _ServiceHost.Close();
            }
        }


        #endregion


    }
}
