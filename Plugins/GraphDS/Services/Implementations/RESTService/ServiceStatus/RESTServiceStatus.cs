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
            this.OtherStatistically = new Dictionary<string, object>();
            String Description = "* REST Service is started at http://localhost:" + myPort + Environment.NewLine +
                "      * access it directly by passing the GraphQL query using the"+ Environment.NewLine +
                "        REST interface or a client library. (see documentation)"+ Environment.NewLine +
                "* we recommend to use the AJAX WebShell. " + Environment.NewLine +
                "        Browse to http://localhost:" + myPort + "/WebShell and use" + Environment.NewLine +
                "        the username  'test' and password 'test'";



            OtherStatistically.Add("Description", Description);

        }
    }
}
