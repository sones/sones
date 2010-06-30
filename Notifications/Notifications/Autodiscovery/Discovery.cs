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


/* PandoraLib - Autodiscovery
 * (c) Daniel Kirstenpfad, 2009
 * 
 * Implements a class which can discover services within
 * a specified Pandora Notification Multicast Group
 * 
 * Lead programmer:
 *      Daniel Kirstenpfad
 * 
 * */
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using sones.Lib;
using sones.Notifications.Autodiscovery.NotificationTypes;
using sones.Notifications.NotificationTypes;
using sones.Notifications.Autodiscovery.Events;
using sones.Lib.DataStructures.Timestamp;
using sones.Lib.DataStructures.UUID;
using sones.Notifications.Messages;

namespace sones.Notifications.Autodiscovery
{
    /// <summary>
    /// A class which can discover services, when you don't need this service anymore you should call the StopDiscovery method.
    /// </summary>
    public class Discovery : INotification  
    {
        public Dictionary<String,DiscoveredService> KnownServices;
        private NotificationDispatcher _NotificationDispatcher;
        private bool shutdown = false;

        #region Events
        public delegate void KnownServicesChangedEventHandler(object sender, KnownServicesChangedArgs e);
        public event KnownServicesChangedEventHandler KnownServicesChanged;
        #endregion


        /// <summary>
        /// the time in timespan after a once discovered service times out
        /// </summary>
        public TimeSpan DiscoveredServiceTimeout = new TimeSpan(0, 0, 5);   // 5 seconds by default

        /// <summary>
        /// This is the constructor of the discovery class. Use the AddDiscoverableService method to
        /// add Services which shall be discovered.
        /// </summary>
        public Discovery()
        {
            Debug.WriteLine("[Discovery] Service Discovery is starting up!");
            KnownServices = new Dictionary<string, DiscoveredService>();
            
            NotificationSettings NotificationSettings = new NotificationSettings();
            NotificationSettings.StartBrigde = true;

            _NotificationDispatcher = new NotificationDispatcher(new UUID("ServiceDiscovery_Discovery"), NotificationSettings);

            // Start the Housekeeper Thread which gets rid of timeouted known services
            Thread Housekeeper_Thread = new Thread(new ThreadStart(HousekeeperThread));
            Housekeeper_Thread.IsBackground = true;
            Housekeeper_Thread.SetApartmentState(ApartmentState.STA);
            Housekeeper_Thread.Start();

            //_NotificationDispatcher.RegisterRecipient(new NSD(), this);
            Debug.WriteLine("[Discovery] Service Discovery is up and running!");
        }

        #region Events

        #region KnownServicesChanged Event
        [STAThread]
        protected virtual void RaiseKnownServicesChanged(KnownServicesChangedAction _Action, DiscoveredService _Service)
        {
            // Raise the event by using the () operator.
            if (KnownServicesChanged != null)
                KnownServicesChanged(this, new KnownServicesChangedArgs(_Action,_Service));
        }
        #endregion

        #endregion

        /// <summary>
        /// this method adds a DiscoverableService to the List of Services to look for, e.g. that can be discovered
        /// </summary>
        /// <param name="Type">the type of the Service</param>
        public void AddDiscoverableService(DiscoverableServiceType Type)
        {
            switch (Type)
            {
                case DiscoverableServiceType.Database:
                    Debug.WriteLine("[Discovery] Added Database Discovery");
                    _NotificationDispatcher.RegisterRecipient(new NSD_Database(), this);
                    break;
                case DiscoverableServiceType.Filesystem:
                    Debug.WriteLine("[Discovery] Added Filesystem Discovery");
                    _NotificationDispatcher.RegisterRecipient(new NSD_Filesystem(), this);
                    break;
                case DiscoverableServiceType.StorageEngine:
                    Debug.WriteLine("[Discovery] Added StorageEngine Discovery");
                    _NotificationDispatcher.RegisterRecipient(new NSD_StorageEngine(), this);
                    break;
            }
        }

        /// <summary>
        /// this method removes a DiscoverableService from the List of Services to look for, e.g. that can be discovered
        /// </summary>
        /// <param name="Type">the type of the Service</param>
        public void RemoveDiscoverableService(DiscoverableServiceType Type)
        {
            switch (Type)
            {
                case DiscoverableServiceType.Database:
                    _NotificationDispatcher.UnRegisterRecipient(new NSD_Database(), this);
                    break;
                case DiscoverableServiceType.Filesystem:
                    _NotificationDispatcher.UnRegisterRecipient(new NSD_Filesystem(), this);
                    break;
                case DiscoverableServiceType.StorageEngine:
                    _NotificationDispatcher.UnRegisterRecipient(new NSD_StorageEngine(), this);
                    break;
            }
        }

        #region Stop Discovery
        /// <summary>
        /// Stops the Discovery of Services
        /// </summary>
        public void StopDiscovery()
        {
            shutdown = true;

            // wait ...
            Thread.Sleep((Int32)DiscoveredServiceTimeout.TotalMilliseconds);

            //_NotificationDispatcher.UnRegisterRecipient(new NSD_Filesystem_Admin(), this);
            _NotificationDispatcher.StopBridge();
        }
        #endregion

        #region Housekeeping / Timeout Management
        ///// <summary>
        ///// the thread that handles the timeout of previously discovered services
        ///// </summary>
        [STAThread]
        private void HousekeeperThread()
        {
            Debug.WriteLine("[Discovery Housekeeper] Service Discovery Housekeeping is up and running!");
            DateTime now;
            List<String> RemoveServices;

            Int32 Milliseconds = (Int32)DiscoveredServiceTimeout.TotalMilliseconds;

            while (!shutdown)
            {
                Thread.Sleep(Milliseconds);

                now = TimestampNonce.Now;
                RemoveServices = new List<string>();

                lock (KnownServices)
                {
                    foreach (DiscoveredService _KnownService in KnownServices.Values)
                    {
                        if (now.Ticks < _KnownService.LastAnnouncementTime.Ticks)
                        {
                            // this is an announcement from the future, don't handle that
                            Debug.WriteLine("[Discovery Housekeeper] Service with Name " + _KnownService.ServiceGlobalUniqueName + " is from the future. Ignoring in Housekeeping.");
                        }
                        else
                        {
                            if ((now.Ticks - _KnownService.LastAnnouncementTime.Ticks) >= DiscoveredServiceTimeout.Ticks)
                            {
                                RemoveServices.Add(_KnownService.ServiceGlobalUniqueName);
                            }
                            else
                            {
                                // do nothing, because it's not yet timed out
                            }
                        }                        
                    }

                    // remove them finally
                    foreach (String _RemoveUniqueName in RemoveServices)
                    {
                        DiscoveredService _service = KnownServices[_RemoveUniqueName];
                        KnownServices.Remove(_RemoveUniqueName);
                        RaiseKnownServicesChanged(KnownServicesChangedAction.Removed, _service);

                        Debug.WriteLine("[Discovery Housekeeper] Removed "+_RemoveUniqueName+" Service from KnownServices because of TimeOut.");
                    }
                    
                    
                }
            }
        }
        #endregion

        #region INotification Members
        /// <summary>
        /// Handles a new incoming Notification
        /// </summary>
        /// <param name="NotificationMessage">this should be an NServiceDiscovery_Announcement message</param>
        /// <returns>true if everything was successful, false if something went wrong</returns>
        public bool HandleNotification(NotificationMessage NotificationMessage)
        {
            /*
             * DISCLAIMER:
             * 
             * I DO KNOW THAT THIS IS UGLY BUT IT WORKS FOR NOW. I WILL REWRITE THIS PORTION ASAP, 
             * ESPECIALLY THIS BIG IF CLAUSE DOWN THERE!
             *  
             * */

            if (NotificationMessage.NotificationType == typeof(NSD_Filesystem).FullBaseName<ANotificationType>())
            {
                // we got the right messagetype
                NSD_Filesystem.Arguments Args = ((NSD_Filesystem.Arguments)NotificationMessage.Arguments);
                
                #region NSD_Filesystem
                lock (KnownServices)
                {
                    if (KnownServices.ContainsKey(Args.ServiceGlobalUniqueName))
                    {
                        DiscoveredService _Service = KnownServices[Args.ServiceGlobalUniqueName];

                        // you can change everything through a new announcement message but not the global unique name of the service, keep
                        // that in mind!
                        _Service.ServiceUri = Args.ServiceUri;
                        _Service.ServiceType = Args.ServiceType;
                        
                        _Service.UpdateAnnouncementTime();
                        Debug.WriteLine("[Discovery] Updating Announcement time for Service "+_Service.ServiceGlobalUniqueName);
                        RaiseKnownServicesChanged(KnownServicesChangedAction.Modified, _Service);
                    }
                    else
                    {
                        DiscoveredService newService = new DiscoveredService(Args.ServiceGlobalUniqueName, Args.ServiceUri, Args.ServiceType);
                        Debug.WriteLine("[Discovery] Discovered new Service " + newService.ServiceGlobalUniqueName);
                        KnownServices.Add(Args.ServiceGlobalUniqueName, newService);
                        RaiseKnownServicesChanged(KnownServicesChangedAction.Added, newService);
                    }
                }
                #endregion
                return true;
            }
            else
                    if (NotificationMessage.NotificationType == typeof(NSD_StorageEngine).FullBaseName<ANotificationType>())
                    {
                        // we got the right messagetype
                        NSD_StorageEngine.Arguments Args = ((NSD_StorageEngine.Arguments)NotificationMessage.Arguments);

                        #region NSD_StorageEngine
                        lock (KnownServices)
                        {
                            if (KnownServices.ContainsKey(Args.ServiceGlobalUniqueName))
                            {
                                DiscoveredService _Service = KnownServices[Args.ServiceGlobalUniqueName];

                                // you can change everything through a new announcement message but not the global unique name of the service, keep
                                // that in mind!
                                _Service.ServiceUri = Args.ServiceUri;
                                _Service.ServiceType = Args.ServiceType;

                                _Service.UpdateAnnouncementTime();
                                Debug.WriteLine("[Discovery] Updating Announcement time for Service " + _Service.ServiceGlobalUniqueName);
                                RaiseKnownServicesChanged(KnownServicesChangedAction.Modified, _Service);

                            }
                            else
                            {
                                DiscoveredService newService = new DiscoveredService(Args.ServiceGlobalUniqueName, Args.ServiceUri, Args.ServiceType);
                                Debug.WriteLine("[Discovery] Discovered new Service " + newService.ServiceGlobalUniqueName);
                                KnownServices.Add(Args.ServiceGlobalUniqueName, newService);
                                RaiseKnownServicesChanged(KnownServicesChangedAction.Added, newService);
                            }
                        }
                        #endregion
                        return true;
                    }
                    else
                            if (NotificationMessage.NotificationType == typeof(NSD_Database).FullBaseName<ANotificationType>())
                            {
                                // we got the right messagetype
                                NSD_Database.Arguments Args = ((NSD_Database.Arguments)NotificationMessage.Arguments);

                                #region NSD_Database
                                lock (KnownServices)
                                {
                                    if (KnownServices.ContainsKey(Args.ServiceGlobalUniqueName))
                                    {
                                        DiscoveredService _Service = KnownServices[Args.ServiceGlobalUniqueName];

                                        // you can change everything through a new announcement message but not the global unique name of the service, keep
                                        // that in mind!
                                        _Service.ServiceUri = Args.ServiceUri;
                                        _Service.ServiceType = Args.ServiceType;

                                        _Service.UpdateAnnouncementTime();
                                        Debug.WriteLine("[Discovery] Updating Announcement time for Service " + _Service.ServiceGlobalUniqueName);
                                        RaiseKnownServicesChanged(KnownServicesChangedAction.Modified, _Service);

                                    }
                                    else
                                    {
                                        DiscoveredService newService = new DiscoveredService(Args.ServiceGlobalUniqueName, Args.ServiceUri, Args.ServiceType);
                                        Debug.WriteLine("[Discovery] Discovered new Service " + newService.ServiceGlobalUniqueName);
                                        KnownServices.Add(Args.ServiceGlobalUniqueName, newService);
                                        RaiseKnownServicesChanged(KnownServicesChangedAction.Added, newService);
                                    }
                                }
                                #endregion
                                return true;
                            }
            else
            {
                return false;
            }

        }
        #endregion
    }
}
