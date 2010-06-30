/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
*/


/* Pandora Library - Messaging System - Local Subscription Dispatcher
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
