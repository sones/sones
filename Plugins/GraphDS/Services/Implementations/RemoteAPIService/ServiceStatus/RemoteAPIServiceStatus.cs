/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

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
