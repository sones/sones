/* GraphLib - Autodiscovery
 * (c) Daniel Kirstenpfad, 2009
 * 
 * Implements an enumerator to hold the info what happened within an KnownServicesChanged Event
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
    public enum KnownServicesChangedAction : byte
    {
        Added = 1,
        Removed = 2,
        Modified = 3,
    }
}
