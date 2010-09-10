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

/* PandoraFS - NServiceDiscovery_Filesystem_Admin
 * (c) Daniel Kirstenpfad, 2009
 * 
 * This class groups all Notification concerning the Automatic Service Discovery
 * 
 * Lead programmer:
 *      Daniel Kirstenpfad
 * 
 * */

using System;
using sones.Notifications.NotificationTypes;

namespace sones.Notifications.Autodiscovery.NotificationTypes
{
    /// <summary>
    /// This class groups all Filesystem Admin Notifications from the Automatic Service Discovery
    /// </summary>
    public class NServiceDiscovery_StorageEngine : ANotificationType
    {
        public NServiceDiscovery_StorageEngine()
            : base(System.Net.IPAddress.Parse("224.10.10.19"), 5000)
        { }

        public override string Description
        {
            get { return "This class groups all StorageEngine Service Announcements (Automatic Service Discovery)."; }
        }

        public override INotificationArguments GetEmptyArgumentInstance()
        {
            throw new NotImplementedException();
        }
    }
}
