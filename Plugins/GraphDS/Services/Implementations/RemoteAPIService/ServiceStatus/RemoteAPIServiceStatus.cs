using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Plugins.GraphDS.Services;
using System.Net;

namespace sones.GraphDS.Services.RemoteAPIService.ServiceStatus
{
    public class RemoteAPIServiceStatus : AServiceStatus
    {
        public RemoteAPIServiceStatus(IPAddress myIPAddress, ushort myPort,Boolean isSecure, Boolean isRunning, TimeSpan myRunningTime)
        {
            this.IPAddress = myIPAddress;
            this.Port = myPort;
            this.IsRunning = isRunning;
            this.IsNetService = true;
            this.RunningTime = myRunningTime;
            this.OtherStatistically = new Dictionary<string, object>();
            

            String Description = "This Service starts a SOAP - based 'Remote API Server'." + Environment.NewLine + 
                "      * You can get the WSDL file at " + (isSecure == true ? "https" : "http") + ":" + myIPAddress.Address.ToString() + ":" + myPort.ToString() + 
                "/rpc?wsdl" + Environment.NewLine +
                "      * You can also use the already built client wrapper with a better handling.";

            OtherStatistically.Add("Description", Description);
            OtherStatistically.Add("IsSecure", isSecure);
        }

        
    }
}
