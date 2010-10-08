/* GraphLib - NFilesystem
 * (c) Stefan Licht, 2009
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

namespace sones.StorageEngines.Notification
{

    /// <summary>
    /// This class groups all Notification for the StorageEngines.
    /// </summary>

    public class NStorageEngine : ANotificationType
    {

        public NStorageEngine()
            : base(IPAddress.Parse("224.10.10.11"), 5000)
        { }

        public override string Description
        {
            get { return "Groups all Notification for the StorageEngine."; }
        }


        public override INotificationArguments GetEmptyArgumentInstance()
        {
            throw new NotImplementedException();
        }

    }

}
