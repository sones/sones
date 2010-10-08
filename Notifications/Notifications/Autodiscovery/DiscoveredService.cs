/* GraphLib - DiscoveredService
 * (c) Daniel Kirstenpfad, 2009
 * 
 * Implements a structure which holds all the known information about
 * a Service which is discovered through auto discovery.
 * 
 * Lead programmer:
 *      Daniel Kirstenpfad
 * 
 * */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using sones.Lib.DataStructures.Timestamp;

namespace sones.Notifications.Autodiscovery
{
    public class DiscoveredService
    {
        public String ServiceGlobalUniqueName  { get; set; }
        public Uri ServiceUri  { get; set; }
        public DiscoverableServiceType ServiceType { get; set; }
        public DateTime LastAnnouncementTime { get; set; }

        public String ServiceUriString
        {
            get
            {
                if (ServiceUri != null)
                    return ServiceUri.ToString();
                else
                    return "";
            }
        }

        public DiscoveredService(String _ServiceGlobalUniqueName, Uri _ServiceURI, DiscoverableServiceType _ServiceType)
        {
            ServiceGlobalUniqueName = _ServiceGlobalUniqueName;
            ServiceUri = _ServiceURI;
            ServiceType = _ServiceType;
            LastAnnouncementTime = TimestampNonce.Now;
        }

        public DiscoveredService(String _ServiceGlobalUniqueName, Uri _ServiceURI, DiscoverableServiceType _ServiceType, DateTime _LastAnnouncementTime)
        {
            ServiceGlobalUniqueName = _ServiceGlobalUniqueName;
            ServiceUri = _ServiceURI;
            ServiceType = _ServiceType;
            LastAnnouncementTime = _LastAnnouncementTime;
        }

        public void UpdateAnnouncementTime()
        {
            LastAnnouncementTime = TimestampNonce.Now;
        }
    }
}
