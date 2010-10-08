/* TelnetCommandEventHandler
 * (c) Stefan Licht, 2009
 * 
 * This class holds the TelnetCommandEventHandler
 * 
 * Lead programmer:
 *      Stefan Licht
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Networking.Telnet.Events
{
    public delegate void TelnetCommandEventHandler(Object mySender, TelnetCommandEventArgs myEventArgs);
}
