/* GraphLib - NFilesystem
 * (c) Stefan Licht, 2009
 * 
 * This class groups all Notification for the ObjectCache.
 * 
 * Lead programmer:
 *      Stefan Licht
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Notifications.NotificationTypes;
using sones.Notifications;

namespace sones.GraphDB.Notification.NotificationTypes.Database
{
    public class NotificationGroupDatabase : ANotificationType
    {

        public NotificationGroupDatabase()
            : base(System.Net.IPAddress.Parse("224.10.11.10"), 5000)
        { }

        public override string Description
        {
            get { return "Groups all Notification for the Database Instances"; }
        }

        public override INotificationArguments GetEmptyArgumentInstance()
        {
            throw new NotImplementedException();
        }
    }
}
