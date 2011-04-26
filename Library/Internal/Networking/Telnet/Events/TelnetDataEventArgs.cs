/* GraphWebDAV
 * (c) Stefan Licht, 2009
 * 
 * This class holds the TelnetDataEventArgs
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
    public class TelnetDataEventArgs
    {
        public Byte[] Data { get; set; }
    }
}
