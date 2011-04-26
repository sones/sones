/* GraphWebDAV
 * (c) Stefan Licht, 2009
 * 
 * This class holds the TelnetKeyEventArgs
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
    public class TelnetKeyEventArgs
    {
        public ConsoleKeyInfo KeyInfo { get; set; }
    }
}
