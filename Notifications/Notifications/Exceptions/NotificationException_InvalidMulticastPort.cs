using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Notifications.Exceptions
{
    public class NotificationException_InvalidMulticastPort : Exception
    {
        public NotificationException_InvalidMulticastPort(String message)
            : base(message)
        { }
    }
}
