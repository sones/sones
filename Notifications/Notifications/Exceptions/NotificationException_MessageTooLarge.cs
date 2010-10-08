using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Notifications.Exceptions
{
    public class NotificationException_MessageTooLarge : Exception
    {
        public NotificationException_MessageTooLarge(String message)
            : base(message)
        {
        }
    }
}
