/* GraphFS - NAccessControl
 * (c) Henning Rauch, 2009
 * 
 * This class groups all Notification concerning the Access Control Management.
 * 
 * Lead programmer:
 *      Henning Rauch
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Notifications.NotificationTypes;
using sones.Notifications;
using System.Net;

namespace sones.GraphFS.Notification
{
    /// <summary>
    /// This class groups all Notification concerning the Access Control Management.
    /// </summary>
    public class NAccessControl : ANotificationType
    {

        public NAccessControl()
            : base(IPAddress.Parse("224.10.10.13"), 5000)
        { }

        public override string Description
        {
            get { return "This class groups all Notification for the Access Control Management."; }
        }

        public override INotificationArguments GetEmptyArgumentInstance()
        {
            throw new NotImplementedException();
        }
    }
}
