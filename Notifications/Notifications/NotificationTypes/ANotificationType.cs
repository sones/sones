/* GraphLib - ANotificationType
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
