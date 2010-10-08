/* Graph Library - Messaging System - Local Subscription Dispatcher
 * (c) Daniel Kirstenpfad, 2009
 * 
 * Lead programmer:
 *      Daniel Kirstenpfad
 * 
 * */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Lib.Communication
{
    /// <summary>
    /// this class contains all the methods and functionality to instantiate and manage a Message Subscription Dispatcher
    /// as described in Brainstorming Meeting on 21st April 2009 (see Documentation)
    /// 
    /// It works like this:
    /// 
    /// While maintaining a list of known Clients and a mapping of clients to subscriptions this
    /// dispatcher stores-and-forwards incoming Messages into corresponding SubscriptionQueues.
    /// 
    /// </summary>
    public class SubscriptionDispatcher
    {
        // there are several lists that need to be managed
        
        private Dictionary<Guid, Client> KnownClients;      // the known clients this dispatcher manages

        private List<Subscription> Subscriptions;

        public SubscriptionDispatcher()
        {
            KnownClients = new Dictionary<Guid, Client>();
            Subscriptions = new List<Subscription>();
        }

        #region Subscribe

        public Client Subscribe(String Subscription_FriendlyName)
        {
            lock(Subscriptions)
            {
                foreach (Subscription _Sub in Subscriptions)
                {
                    if (_Sub.FriendlyName == Subscription_FriendlyName)
                    {
                        return Subscribe(_Sub.SubscriptionID);
                    }
                }
            }
            return null;
        }

        public Client Subscribe(Guid SubscriptionID)
        {
            lock (Subscriptions)
            {
                foreach (Subscription _Sub in Subscriptions)
                {
                    if (_Sub.SubscriptionID == SubscriptionID)
                        return Subscribe(_Sub);
                }
            }
            return null;
        }

        public Client Subscribe(Subscription _Subscription)
        {
            lock (_Subscription)
            {
                //_Subscription.
            }
            return null;
        }

        #endregion

    }
}
