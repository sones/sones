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
using System.Net;
using sones.Notifications;

namespace sones.GraphFS.Notification
{
    public class NObjectCache : ANotificationType
    {

        public NObjectCache()
            : base(IPAddress.Parse("224.10.10.10"), 5000)
        { }

        public override string Description
        {
            get { return "Groups all Notification for the ObjectCache"; }
        }

        public override INotificationArguments GetEmptyArgumentInstance()
        {
            throw new NotImplementedException();
        }
    }
}
