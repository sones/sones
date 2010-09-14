/* GraphLib - NFileSystem
 * (c) Stefan Licht, 2009
 * 
 * This class groups all Notification for the Filesystem.
 * 
 * Lead programmer:
 *      Stefan Licht
 * 
 * */

using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using sones.Notifications.NotificationTypes;
using sones.Notifications;

namespace sones.GraphFS.Notification
{

    /// <summary>
    /// This class groups all Notification for the Filesystem.
    /// </summary>
    public class NFileSystem : ANotificationType
    {

        public NFileSystem()
            : base(IPAddress.Parse("224.10.10.12"), 5000)
        { }

        public override string Description
        {
            get { return "This class groups all Notification for the Filesystem."; }
        }

        public override INotificationArguments GetEmptyArgumentInstance()
        {
            throw new NotImplementedException();
        }

    }

}
