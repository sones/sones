/* GraphLib - Autodiscovery
 * (c) Daniel Kirstenpfad, 2009
 * 
 * Implements the Argument data structure for an KnownServicesChanged Event
 * 
 * Lead programmer:
 *      Daniel Kirstenpfad
 * 
 * */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Notifications.Autodiscovery.Events
{
    public class KnownServicesChangedArgs : EventArgs
    {
        public KnownServicesChangedAction Action;
        public DiscoveredService Service;


        public KnownServicesChangedArgs(KnownServicesChangedAction _Action, DiscoveredService _Service)
        {
            Action = _Action;
            Service = _Service;
        }
    }
}
