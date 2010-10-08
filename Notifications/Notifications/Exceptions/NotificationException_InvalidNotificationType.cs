using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Notifications.Exceptions
{
    public class NotificationException_InvalidNotificationType : Exception
    {
        public NotificationException_InvalidNotificationType(String message)
            : base(message)
        { }
    }
}
