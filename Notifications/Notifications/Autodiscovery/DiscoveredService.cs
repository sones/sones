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


/* PandoraLib - DiscoveredService
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
