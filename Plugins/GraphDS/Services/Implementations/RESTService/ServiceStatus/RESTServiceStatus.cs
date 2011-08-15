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

            String MyIPAdressString = myIPAddress.ToString();

            if (MyIPAdressString == "0.0.0.0")
                MyIPAdressString = "localhost";

            String Description = "   * REST Service is started at http://" + MyIPAdressString + ":" + myPort + Environment.NewLine +
                                 "      * access it directly by passing the GraphQL query using the"+ Environment.NewLine +
                                 "        REST interface or a client library. (see documentation)"+ Environment.NewLine +
                                 "      * if you want JSON Output add ACCEPT: application/json "+ Environment.NewLine +
                                 "        to the client request header (or application/xml or"+ Environment.NewLine +
                                 "        application/text)"+ Environment.NewLine +
                                 "   * for first steps we recommend to use the AJAX WebShell. "+ Environment.NewLine +
                                 "     Browse to http://" + MyIPAdressString + ":" + myPort + "/WebShell" + Environment.NewLine +
                                 "     (default username and passwort: test / test)";

            OtherStatistically.Add("Description", Description);

        }
    }
}
