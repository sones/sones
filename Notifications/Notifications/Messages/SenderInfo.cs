using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace sones.Notifications.Messages
{
    public class SenderInfo
    {
        public String Host { get; set; }
        public String DispatcherGuid { get; set; }
        public String SenderID { get; set; }

        public override string ToString()
        {
            return String.Format("Host: {0} Disp: {1} SenderID: {2}", Host, DispatcherGuid, SenderID);
        }

    }


}
