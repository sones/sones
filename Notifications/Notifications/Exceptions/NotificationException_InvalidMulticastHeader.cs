using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Notifications.Exceptions
{
    public class NotificationException_InvalidMulticastHeader : Exception
    {
        public NotificationException_InvalidMulticastHeader(String message) 
            : base(message)
        {
        }

        public NotificationException_InvalidMulticastHeader(String message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
