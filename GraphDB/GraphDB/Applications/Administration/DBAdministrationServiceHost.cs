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


///* <id name="sones GraphDB – AdministrationServiceHost" />
// * <copyright file="AdministrationServiceHost.cs"
// *            company="sones GmbH">
// * Copyright (c) sones GmbH 2007-2010
// * </copyright>
// * <developer>Daniel Kirstenpfad</developer>
// * <summary>Implements the AdministrationServiceHost</summary>
// */
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.ServiceModel.Description;
//using System.ServiceModel;
//using sones.Lib.Networking.TCPSocket;
//using sones.Notifications;
//using System.Threading;
//using System.Security.Cryptography.X509Certificates;
//using sones.Notifications.Autodiscovery;
//using System.Net;

//namespace sones.GraphDB.Applications.Administration
//{
//    /// <summary>
//    ///  This is a server implementation for the IPandoraDBSession Admninistration wcf service interface host.
//    /// </summary>
//    public class DBAdministrationServiceHost
//    {

//        #region Definition

//        #region Fields
//        private ServiceHost _ServiceHost;
//        private Uri _BaseAddress;
//        //private NotificationDispatcher _NotificationDispatcher;
//        private Announcer _AdministrationServiceAnnouncer;
//        #endregion

//        #region Properties
//        /// <summary>
//        /// The uri on which the service host is listening
//        /// </summary>
//        public Uri BaseAddress
//        {
//            get
//            {
//                return _BaseAddress;
//            }
//        }

//        /// <summary>
//        /// The ServiceHost which handles the net connections
//        /// </summary>
//        public ServiceHost UnderlyingServiceHost
//        {
//            get
//            {
//                return _ServiceHost;
//            }
//        }
//        #endregion

//        #endregion

//        #region Constructors

//        /// <summary>
//        /// Create an administration interface using NetTcpBinding on port 8112
//        /// </summary>
//        public DBAdministrationServiceHost(/*NotificationDispatcher myNotificationDispatcher*/)
//        {
//            _BaseAddress = new Uri("net.tcp://localhost:8113");
//            //_NotificationDispatcher = myNotificationDispatcher;
//        }

//       /// <summary>
//        /// Create the administration interface for a specified Uri
//        /// </summary>
//        /// <param name="myBaseAddress">The uri including the protocol (binding), address and port </param>
//        public DBAdministrationServiceHost(/*NotificationDispatcher myNotificationDispatcher, */Uri myBaseAddress)
//        {
//            //_NotificationDispatcher = myNotificationDispatcher;
//            myBaseAddress = _BaseAddress;
//        }

//        #endregion

//        /// <summary>
//        /// Start the interface
//        /// </summary>
//        /// <param name="myIPandoraDBSession">An instance of IPandoraDBSession</param>
//        public void Start(IPandoraDBSession myIPandoraDBSession/*, String myEndpointPath*/)
//        {
//            if (myIPandoraDBSession== null)
//                throw new ArgumentNullException("IPandoraDBSession has to be an instance.");

//            _ServiceHost = new ServiceHost(myIPandoraDBSession, _BaseAddress);

//            try
//            {
//                ServiceBehaviorAttribute serviceBehaviorAttribute = new ServiceBehaviorAttribute();
//                if (_ServiceHost.Description.Behaviors[typeof(ServiceBehaviorAttribute)] != null)
//                    serviceBehaviorAttribute = (ServiceBehaviorAttribute)_ServiceHost.Description.Behaviors[typeof(ServiceBehaviorAttribute)];
//                else
//                    _ServiceHost.Description.Behaviors.Add(serviceBehaviorAttribute);

//                serviceBehaviorAttribute.InstanceContextMode = InstanceContextMode.Single;
//                serviceBehaviorAttribute.IncludeExceptionDetailInFaults = true;

//                //_ServiceHost.Description.Behaviors[typeof(ServiceBehaviorAttribute)];
//                NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
//                binding.ReceiveTimeout = TimeSpan.MaxValue;
//                binding.PortSharingEnabled = false;
//                #if(!__MonoCS__)
//                binding.ReliableSession.InactivityTimeout = TimeSpan.MaxValue;
//                #endif

//                #region Security
//                /*
//                binding.Security.Mode = SecurityMode.TransportWithMessageCredential;
//                _ServiceHost.Credentials.ServiceCertificate.SetCertificate(StoreLocation.CurrentUser, StoreName.My, X509FindType.FindByIssuerName, "SONES GmbH - Mail CA");
//                _ServiceHost.Credentials.ClientCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None;
//                */
//                #endregion

//                _ServiceHost.AddServiceEndpoint(typeof(IPandoraDBSession), binding, _BaseAddress);
//                _ServiceHost.Open(new TimeSpan(0,1,0));

//                _AdministrationServiceAnnouncer = new Announcer(myIPandoraDBSession.GetDatabaseUniqueID().ToString(), _BaseAddress, DiscoverableServiceType.Database);
//            }
//            catch (CommunicationException ce)
//            {
//                /*
//                 * If you're getting an exception while starting up this the first time you probably want to do
//                 * these things:
//                 * 
//                 * 1. Start this as local Administrator OR Add your User to the SMSvcHost.exe.config file in the
//                 *    \Windows\Microsoft.NET\Framework\v3.0\Windows Communication Foundation\ directory
//                 * 2. You want to run "sc.exe config NetTcpPortSharing start= demand" as local Administrator to
//                 *    enable the net.tcp Port Sharing.
//                 *
//                 * */
//                // TODO: Insert the above todo into the installer/wizard for final customer deployment.

//                System.Diagnostics.Debug.WriteLine(ce);
//                _ServiceHost.Abort();
//                ((IDisposable)_ServiceHost).Dispose();

//                throw ce;
//            }

//        }

//        /// <summary>
//        /// Stop the bridge and clean up the ressources
//        /// </summary>
//        public void Stop()
//        {
//            if (_AdministrationServiceAnnouncer != null)
//                _AdministrationServiceAnnouncer.StopAnnouncer();
//            if (_ServiceHost != null)
//            {
//                _ServiceHost.Close();
//                ((IDisposable)_ServiceHost).Dispose();
//            }
//        }

//    }
//}
