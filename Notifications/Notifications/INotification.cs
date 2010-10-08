/* GraphLib - INotification
 * (c) Stefan Licht, 2009
 * 
 * The interface for all Graph Notification systems. Each client who subscribe to
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
    /// The interface for all Graph file systems
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