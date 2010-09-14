using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Notifications.Messages;

namespace sones.Lib.Communication
{
    public class Subscription
    {        
        public Guid SubscriptionID;
        public String FriendlyName;
        public Dictionary<Guid,Client> Subscriber;        
        public Queue<NotificationMessage> SubscriptionMessageQueue;

        public Subscription(String SubscriptionFriendlyName)
        {
            SubscriptionID = Guid.NewGuid();
            Subscriber = new Dictionary<Guid, Client>();
            SubscriptionMessageQueue = new Queue<NotificationMessage>();
        }
    }
}
