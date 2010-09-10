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

/* PandoraFS - FSAdministrationServiceHost
 * (c) Daniel Kirstenpfad, 2009
 * 
 * This is an implementation of a WCF Service Host for the IGraphFSSession Administration Interface
 * 
 * Lead programmer:
 *      Daniel Kirstenpfad
 * 
 */

using System;
using System.ServiceModel;

using sones.GraphFS.Session;
using sones.Notifications.Autodiscovery;

namespace sones.GraphFS.Administration
{
    /// <summary>
    ///  This is a server implementation for the IGraphFSSession Admninistration wcf service interface host.
    /// </summary>
    public class FSAdministrationServiceHost
    {

        #region Definition

        #region Fields
        private ServiceHost _ServiceHost;
        private Uri _BaseAddress;
        private Announcer _AdministrationServiceAnnouncer;
        //private NotificationDispatcher _NotificationDispatcher;
        #endregion

        #region Properties
        /// <summary>
        /// The uri on which the service host is listening
        /// </summary>
        public Uri BaseAddress
        {
            get
            {
                return _BaseAddress;
            }
        }

        /// <summary>
        /// The ServiceHost which handles the net connections
        /// </summary>
        public ServiceHost UnderlyingServiceHost
        {
            get
            {
                return _ServiceHost;
            }
        }
        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Create an administration interface using NetTcpBinding on port 8112
        /// </summary>
        public FSAdministrationServiceHost(/*NotificationDispatcher myNotificationDispatcher*/)
        {
            _BaseAddress = new Uri("net.tcp://localhost:8112");
            //_NotificationDispatcher = myNotificationDispatcher;
        }

       /// <summary>
        /// Create the administration interface for a specified Uri
        /// </summary>
        /// <param name="myBaseAddress">The uri including the protocol (binding), address and port like net.tcp://localhost:8112 </param>
        public FSAdministrationServiceHost(/*NotificationDispatcher myNotificationDispatcher, */Uri myBaseAddress)
        {
            //_NotificationDispatcher = myNotificationDispatcher;
            myBaseAddress = _BaseAddress;
        }

        #endregion

        /// <summary>
        /// Start the interface
        /// </summary>
        /// <param name="myIGraphFSSession">An instance of IGraphFSSession</param>
        public void Start(IGraphFSSession myIGraphFSSession/*, String myEndpointPath*/)
        {
            if (myIGraphFSSession== null)
                throw new ArgumentNullException("IPandoraFS has to be an instance.");

            _ServiceHost = new ServiceHost(myIGraphFSSession, _BaseAddress);

            try
            {
                ServiceBehaviorAttribute serviceBehaviorAttribute = new ServiceBehaviorAttribute();
                if (_ServiceHost.Description.Behaviors[typeof(ServiceBehaviorAttribute)] != null)
                    serviceBehaviorAttribute = (ServiceBehaviorAttribute)_ServiceHost.Description.Behaviors[typeof(ServiceBehaviorAttribute)];
                else
                    _ServiceHost.Description.Behaviors.Add(serviceBehaviorAttribute);

                serviceBehaviorAttribute.InstanceContextMode = InstanceContextMode.Single;
                serviceBehaviorAttribute.IncludeExceptionDetailInFaults = true;

                //_ServiceHost.Description.Behaviors[typeof(ServiceBehaviorAttribute)];
                NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);                
                binding.ReceiveTimeout = TimeSpan.MaxValue;                
                binding.PortSharingEnabled = false;
                #if(!__MonoCS__)
                binding.ReliableSession.InactivityTimeout = TimeSpan.MaxValue;
                #endif

                #region Security
                /*
                binding.Security.Mode = SecurityMode.TransportWithMessageCredential;
                _ServiceHost.Credentials.ServiceCertificate.SetCertificate(StoreLocation.CurrentUser, StoreName.My, X509FindType.FindByIssuerName, "SONES GmbH - Mail CA");
                _ServiceHost.Credentials.ClientCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None;
                */
                #endregion

                _ServiceHost.AddServiceEndpoint(typeof(IGraphFSSession), binding, _BaseAddress);
                _ServiceHost.Open(new TimeSpan(0,1,0));

                _AdministrationServiceAnnouncer = new Announcer(myIGraphFSSession.GetFileSystemUUID().ToString(), _BaseAddress, DiscoverableServiceType.Filesystem);
            }
            catch (CommunicationException ce)
            {
                /*
                 * If you're getting an exception while starting up this the first time you probably want to do
                 * these things:
                 * 
                 * 1. Start this as local Administrator OR Add your User to the SMSvcHost.exe.config file in the
                 *    \Windows\Microsoft.NET\Framework\v3.0\Windows Communication Foundation\ directory
                 * 2. You want to run "sc.exe config NetTcpPortSharing start= demand" as local Administrator to
                 *    enable the net.tcp Port Sharing.
                 *
                 * */
                // TODO: Insert the above todo into the installer/wizard for final customer deployment.

                System.Diagnostics.Debug.WriteLine(ce);
                _ServiceHost.Abort();
                ((IDisposable)_ServiceHost).Dispose();

                throw ce;
            }

        }

        /// <summary>
        /// Stop the bridge and clean up the ressources
        /// </summary>
        public void Stop()
        {
            _AdministrationServiceAnnouncer.StopAnnouncer();
             _ServiceHost.Close();
             ((IDisposable)_ServiceHost).Dispose();
        }

    }
}
