using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Notifications.Exceptions
{
    public class NotificationException_BridgeAlreadyConnected : Exception
    {
        public NotificationException_BridgeAlreadyConnected(String message)
            : base(message)
        {
        }
    }
}
