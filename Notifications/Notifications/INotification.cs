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

/* PandoraLib - INotification
 * (c) Stefan Licht, 2009
 * 
 * The interface for all Pandora Notification systems. Each client who subscribe to
 * any type of Notification must implement this interface. If the client subscribes 
 * to more than one NotificationType, the HandleNotification must switch the correct 
 * NotificationType before handles it.
 * e.g: if (NotificationMessage.NotificationType.GetType() == typeof(NObjectCache_ObjectLocationChanged))
 *          ....
 * 
 * Lead programmer:
 *      Stefan Licht
 * 
 * */

#region Usings

using System;
using System.Text;
using sones.Notifications.Messages;
using sones.Notifications.NotificationTypes;

#endregion

namespace sones.Notifications
{

    /// <summary>
    /// The interface for all Pandora file systems
    /// </summary>
    public interface INotification
    {

        /// <summary>
        /// Returns the description of the notification
        /// </summary>
        //String Description { get; }

        Boolean HandleNotification(NotificationMessage NotificationMessage);

    }

}