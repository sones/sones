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


/* PandoraLib - ANotificationType
 * (c) Stefan Licht, 2009
 * 
 * Each NotificationType must derive this abstract class.
 * 
 * It might add some special properties to Validate the Notification. The
 * Validate method will be invoked from the Dispatcher before the Message will be 
 * created. You should use the properties defined in the class and set by the client and 
 * the Arguments which will be set from the notifying part. 
 * Check out this example: sones.Graph.Notification.NotificationTypes.NObjectCache_ObjectLocationChanged
 * 
 * It should override the Arguments struct to specify any special arguments
 * which will be returned to the subscribed client. If you do not need any arguments
 * just leave the arguments as they are.
 * 
 * Lead programmer:
 *      Stefan Licht
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace sones.Notifications.NotificationTypes
{

    public abstract class ANotificationType
    {

        #region Nested Arguments class

        public abstract class Arguments : INotificationArguments
        {

            public abstract Byte[] Serialize();

            public abstract void Deserialize(Byte[] mySerializedBytes);

            public new abstract String ToString();

        };

        #endregion


        /// <summary>
        /// multicast groups is Class D - 224.0.0.0 to 239.255.255.255
        /// </summary>
        public IPAddress MulticastIPAddress { get; set; }   // sahould be private!
        public Int32     MulticastPort      { get; set; }

        public abstract String Description  { get; }


        /// <summary>
        /// This set the Multicast IPAddress and Port for this Group and all derived NotificationTypes
        /// </summary>
        /// <param name="myMulticastIPAddress">Valid multicast IPAddress is Class D - 224.0.0.0 to 239.255.255.255</param>
        /// <param name="myMulticastPort">A valid Port</param>
        public ANotificationType(IPAddress myMulticastIPAddress, Int32 myMulticastPort)
        {
            MulticastIPAddress = myMulticastIPAddress;
            MulticastPort      = myMulticastPort;
        }
        

        public virtual Boolean Validate(INotificationArguments myNotificationArguments)
        {
            return true;
        }

        public abstract INotificationArguments GetEmptyArgumentInstance();


    }

}
