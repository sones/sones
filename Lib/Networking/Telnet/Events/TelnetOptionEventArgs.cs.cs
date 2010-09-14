/* GraphWebDAV
 * (c) Stefan Licht, 2009
 * 
 * This class holds the TelnetOptionEventArgs
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
    public class TelnetOptionEventArgs
    {
        public TelnetSymbol TelnetSymbol { get; set; }
        /// <summary>
        /// For Do(Dont)Option request a True will result in a WillOption and False in a WontOption
        /// For WillOption request a True will result in a WillOption and False in a WontOption if the Accpted differs from the stored remote option settings
        /// </summary>
        public Boolean Accepted { get; set; }
    }
}
 