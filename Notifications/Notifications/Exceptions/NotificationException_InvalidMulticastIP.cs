using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Notifications.Exceptions
{
    public class NotificationException_InvalidNotificationPayload : Exception
    {
        public NotificationException_InvalidNotificationPayload(String message)
            : base(message)
        { }
    }
}
