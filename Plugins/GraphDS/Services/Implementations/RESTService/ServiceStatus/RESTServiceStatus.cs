using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Plugins.GraphDS.Services;
using System.Net;

namespace sones.GraphDS.Services.RESTService.ServiceStatus
{
    public class RESTServiceStatus : AServiceStatus
    {
        public RESTServiceStatus(IPAddress myIPAddress,ushort myPort, Boolean isRunning,TimeSpan myRunningTime)
        {
            this.IPAddress = myIPAddress;
            this.Port = myPort;
            this.IsRunning = isRunning;
            this.IsNetService = true;
            this.RunningTime = myRunningTime;
            this.GetOtherStatistically = null;

        }
    }
}
