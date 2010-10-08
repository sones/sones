using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Notifications.Messages;

namespace sones.Lib.Communication
{
    public class Client
    {
        public Guid Client_ID;
        public DateTime Last_Action;
        public Queue<NotificationMessage> MessageQueue;
    }
}
