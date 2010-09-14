/* GraphLib - Autodiscovery
 * (c) Daniel Kirstenpfad, 2009
 * 
 * Implements a class which can announce services within
 * a specified Graph Notification Multicast Group
 * 
 * Lead programmer:
 *      Daniel Kirstenpfad
 * 
 * */

using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using sones.Notifications.Autodiscovery.NotificationTypes;
using sones.Lib.DataStructures.UUID;

namespace sones.Notifications.Autodiscovery
{
    public class Announcer
    {
        // the internally used NotificationDispatcher
        private NotificationDispatcher _NotificationDispatcher;
        
        private String _internalServiceGlobalUniqueName = "";
        public String ServiceGlobalUniqueName
        {
            get
            { 
                return _internalServiceGlobalUniqueName; 
            }
        }
        
        public Uri ServiceUri;
        public DiscoverableServiceType ServiceType;
        public DateTime LastAnnouncementTime;

        // the Shutdown Flag for the TimingThread
        private bool shutdown = false;

        // timespan to wait until another announcement is sent out
        public TimeSpan SendAnnouncementTimeSpan = new TimeSpan(0,0,2); // default 2 seconds 

        /// <summary>
        /// the constructor, you can have as much Announcers as you need to announce all the services you have
        /// </summary>
        public Announcer(String _ServiceGlobalUniqueName, Uri _ServiceUri, DiscoverableServiceType _ServiceType)
        {
            Debug.WriteLine("[Announcer] Service Announcer is starting up for Service "+_ServiceGlobalUniqueName+" !");

            _internalServiceGlobalUniqueName = _ServiceGlobalUniqueName;
            ServiceUri = _ServiceUri;
            ServiceType = _ServiceType;

            NotificationSettings NotificationSettings = new NotificationSettings();
            NotificationSettings.StartBrigde = true;
            _NotificationDispatcher = new NotificationDispatcher(new UUID("ServiceDiscovery_Announcement"), NotificationSettings);

            //// Start the Housekeeper Thread which gets rid of timeouted known services
            Thread Announcer_Thread = new Thread(new ThreadStart(AnnouncerThread));
            Announcer_Thread.Name = "Announcer.AnnouncerThread()";
            Announcer_Thread.Start();
            Debug.WriteLine("[Announcer] Service Announcer is up and running for Service " + _ServiceGlobalUniqueName + " !");            
        }

        #region Stop Announcer
        /// <summary>
        /// Stops the Announcement of Services
        /// </summary>
        public void StopAnnouncer()
        {
            shutdown = true;

            // wait ...
            Thread.Sleep((Int32)SendAnnouncementTimeSpan.TotalMilliseconds);
            
            _NotificationDispatcher.StopBridge();
        }
        #endregion

        #region Announcement of Service
        ///// <summary>
        ///// the thread that handles the timeout of previously discovered services
        ///// </summary>
        private void AnnouncerThread()
        {
            Debug.WriteLine("[Announcer] Announcer Thread is up and running for Service " + ServiceGlobalUniqueName + " !");            

            while (!shutdown)
            {
                Thread.Sleep((Int32)SendAnnouncementTimeSpan.TotalMilliseconds);

                //

                switch (ServiceType)
                {
                    case DiscoverableServiceType.Database:
                        _NotificationDispatcher.SendNotification(typeof(NSD_Database), new NSD_Database.Arguments(ServiceGlobalUniqueName, ServiceUri, ServiceType));
                        break;                    
                    case DiscoverableServiceType.Filesystem:
                        _NotificationDispatcher.SendNotification(typeof(NSD_Filesystem), new NSD_Filesystem.Arguments(ServiceGlobalUniqueName, ServiceUri, ServiceType));
                        break;
                    case DiscoverableServiceType.StorageEngine:
                        _NotificationDispatcher.SendNotification(typeof(NSD_StorageEngine), new NSD_StorageEngine.Arguments(ServiceGlobalUniqueName, ServiceUri, ServiceType));
                        break;
                }
            }
        }
        #endregion
    }
}
