using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Notifications.Exceptions
{
    public class NotificationException_InvalidMulticastIP : Exception
    {
        public NotificationException_InvalidMulticastIP(String message)
            : base(message)
        { }
    }
}
