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
using sones.GraphDS.Services.RemoteAPIService.ServiceContracts.MEX;
using sones.GraphDS.Services.RemoteAPIService.ServiceContracts.StreamedService;
using System.Xml;
using System.ServiceModel.Channels;

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
        /// Indicates wether the MEX service is delivering a single file
        /// </summary>
        public Boolean IsSingleFile { get; internal set; }

        /// <summary>
        /// The current used Namespace
        /// </summary>
        public const String Namespace = "http://www.sones.com";
       
       
        /// <summary>
        /// The complete URI of the service
        /// </summary>
        public Uri URI { get; private set; }

        /// <summary>
        /// The complete URI of the mex service
        /// </summary>
        public Uri MexUri { get; private set; }

        /// <summary>
        /// The WCF Service Host
        /// </summary>
        private ServiceHost _ServiceHost;

        /// <summary>
        /// The Metadata Exchange Service Host
        /// </summary>
        private ServiceHost _MexServiceHost;

        #endregion

        #region C'tor
       
        public sonesRPCServer(IGraphDS myGraphDS, IPAddress myIPAdress, ushort myPort, String myURI, Boolean myIsSecure, Boolean myAutoStart = false, Boolean myIsSingleFile = false)
        {
            this._GraphDS = myGraphDS;
            this.IsSecure = myIsSecure;
            this.ListeningIPAdress = myIPAdress;
            this.ListeningPort = myPort;
            this.IsSingleFile = myIsSingleFile;
            this.URI = new Uri((myIsSecure == true ? "https://" : "http://") + myIPAdress.ToString() + ":" + myPort + "/" + myURI);
            this.MexUri = new Uri("http://" + myIPAdress.ToString() + ":" + (myPort + 1) + "/" + myURI);

            if (!this.URI.IsWellFormedOriginalString())
                throw new Exception("The URI Pattern is not well formed!");

            InitializeServer();

            if (myAutoStart)
            {
                StartServiceHost();
            }
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
            BasicBinding.MaxBufferSize = 268435456;
            BasicBinding.MaxReceivedMessageSize = 268435456;
            BasicBinding.SendTimeout = new TimeSpan(1, 0, 0);
            BasicBinding.ReceiveTimeout = new TimeSpan(1, 0, 0);
            XmlDictionaryReaderQuotas readerQuotas = new XmlDictionaryReaderQuotas();
            readerQuotas.MaxDepth = 2147483647;
            readerQuotas.MaxStringContentLength = 2147483647;
            readerQuotas.MaxBytesPerRead = 2147483647;
            readerQuotas.MaxNameTableCharCount = 2147483647;
            readerQuotas.MaxArrayLength = 2147483647;
            BasicBinding.ReaderQuotas = readerQuotas;

            BasicHttpBinding StreamedBinding = new BasicHttpBinding();
            StreamedBinding.Name = "sonesStreamed";
            StreamedBinding.Namespace = Namespace;
            StreamedBinding.MessageEncoding = WSMessageEncoding.Text;
            StreamedBinding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
            StreamedBinding.TransferMode = TransferMode.Streamed;
            StreamedBinding.MaxReceivedMessageSize = 2147483648;
            StreamedBinding.MaxBufferSize = 4096;
            StreamedBinding.SendTimeout = new TimeSpan(1, 0, 0, 0);
            StreamedBinding.ReceiveTimeout = new TimeSpan(1, 0, 0, 0);

            WebHttpBinding WebBinding = new WebHttpBinding();
            WebBinding.Name = "sonesWeb";
            WebBinding.Namespace = Namespace;
            WebBinding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
            
            

            if (IsSecure)
            {
                BasicBinding.Security.Mode = BasicHttpSecurityMode.Transport;               
                StreamedBinding.Security.Mode = BasicHttpSecurityMode.Transport;
            }
            
            
            RPCServiceContract ContractInstance = new RPCServiceContract(_GraphDS);
            
          
            _ServiceHost = new ServiceHost(ContractInstance, this.URI);
            _ServiceHost.Description.Namespace = Namespace;


            #region Global Service Interface

            //ContractDescription RPCServiceContract = ContractDescription.GetContract(typeof(IRPCServiceContract));
            //RPCServiceContract.Namespace = Namespace;
            //ServiceEndpoint RPCServiceService = new ServiceEndpoint(RPCServiceContract, BasicBinding, new EndpointAddress(this.URI.AbsoluteUri));
            //_ServiceHost.AddServiceEndpoint(RPCServiceService);

            #endregion


            #region Streamed Contract

            ContractDescription StreamedContract = ContractDescription.GetContract(typeof(IStreamedService));
            StreamedContract.Namespace = Namespace;
            ServiceEndpoint StreamedService = new ServiceEndpoint(StreamedContract, StreamedBinding, new EndpointAddress(this.URI.ToString() + "/streamed"));
            _ServiceHost.AddServiceEndpoint(StreamedService);

            #endregion


            #region GraphDS API Contract

            ContractDescription APIContract = ContractDescription.GetContract(typeof(IGraphDS_API));
            APIContract.Namespace = Namespace;

            #region SOAP
            ServiceEndpoint APIService = new ServiceEndpoint(APIContract, BasicBinding, new EndpointAddress(this.URI.ToString()));
            _ServiceHost.AddServiceEndpoint(APIService);
            #endregion
            
            #region json
            //ServiceEndpoint APIServiceJson = new ServiceEndpoint(APIContract, WebBinding, new EndpointAddress(this.URI.ToString() + "/json"));
            //APIServiceJson.Behaviors.Add(new WebScriptEnablingBehavior());
            //_ServiceHost.AddServiceEndpoint(APIServiceJson);
            #endregion

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

            /*
             * When using automatic wsdl generation, there are errors when not using C# Visual Studio:
             *   - (server side) Mono does not yet support wsdl generation
             *   - Java Stub generation using Axis causes the server to hang up
             *   - Some importers may not support multiple markup files
             *   
             * Therefore, a static wsdl file is provided to the client.
             */

            _MexServiceHost = new ServiceHost(typeof(MonoMEX), MexUri);
            _MexServiceHost.Description.Namespace = Namespace;
            Binding MexBinding = new WebHttpBinding();
            MexBinding.Namespace = Namespace;

            ContractDescription MexContract = ContractDescription.GetContract(typeof(IMonoMEX));
            ServiceEndpoint MexService = new ServiceEndpoint(MexContract, MexBinding, new EndpointAddress(MexUri.ToString()));
            _MexServiceHost.AddServiceEndpoint(MexService);
            _MexServiceHost.Description.Endpoints[0].Behaviors.Add(new System.ServiceModel.Description.WebHttpBehavior());


            //// auto generation
            //ServiceMetadataBehavior metadataBehavior = new ServiceMetadataBehavior();
            //metadataBehavior.HttpGetEnabled = true;
            //Binding mexBinding = MetadataExchangeBindings.CreateMexHttpBinding();
            //mexBinding.Namespace = Namespace;            
            //_ServiceHost.Description.Behaviors.Add(metadataBehavior);
            //ServiceEndpoint mexEndpoint = _ServiceHost.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, mexBinding, "mex");
            //foreach (ServiceEndpoint endpoint in _ServiceHost.Description.Endpoints)
            //{
            //    //export just one file
            //    endpoint.Behaviors.Add(new WsdlExtensions(new WsdlExtensionsConfig() { SingleFile = true }));
            //} 

            #endregion
        }

        #endregion

        #region Service Host Control

        public void StartServiceHost()
        {
            if(!IsRunning)
            {
                try
                {
                    _ServiceHost.Open();
                    _MexServiceHost.Open();
                    IsRunning = true;
                }
                catch (Exception e)
                {
                    if (_ServiceHost != null)
                    {
                        _ServiceHost.Close();
                    }
                    if (_MexServiceHost != null)
                    {
                        _MexServiceHost.Close();
                    }
                    throw e;
                }
            }
        }

        public void StopServiceHost()
        {
            if (IsRunning)
            {
                _ServiceHost.Close();
                _MexServiceHost.Close();
                IsRunning = false;
            }
        }

        #endregion
    }
}
