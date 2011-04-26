using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.Networking.Telnet.Events
{
    public class TelnetSubnegotiationEventArgs
    {
        public TelnetOptions TelnetOption;
        public Byte[] ContentData;
    }
}
