/* GraphWebDAV
 * (c) Stefan Licht, 2009
 * 
 * This class holds the TelnetOptionEventHandler
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
    public delegate void TelnetOptionEventHandler(Object mySender, TelnetOptionEventArgs myEventArgs);
}
