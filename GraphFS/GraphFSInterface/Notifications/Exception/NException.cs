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
using System.Text;
using System.Linq;
using System.Collections.Generic;

using sones.Notifications;
using sones.Notifications.NotificationTypes;

namespace sones.GraphFS.Notification
{
    /// <summary>
    /// This class groups all Notification for the Filesystem.
    /// </summary>
    public class NException : ANotificationType
    {

        public NException()
            : base(IPAddress.Parse("224.10.10.13"), 5000)
        { }

        public override string Description
        {
            get { return "This class groups all Notification for any kind of exception."; }
        }


        public override INotificationArguments GetEmptyArgumentInstance()
        {
            throw new NotImplementedException();
        }
    }
}
