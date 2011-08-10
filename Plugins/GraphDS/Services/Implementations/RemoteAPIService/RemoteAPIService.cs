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
using sones.Library.VersionedPluginManager;
using sones.GraphDS.Services.RemoteAPIService.ServiceStatus;
using System.Diagnostics;
using System.Net;

namespace sones.GraphDS.Services.RemoteAPIService
{
    public class RemoteAPIService : IService, IPluginable
    {
        #region Data

        private Boolean _IsSecure;

        private IGraphDS _GraphDS;
        
        private Stopwatch _RunningTime;

        private sonesRPCServer _RPCServer;

        #endregion

        #region  C'tors

        public RemoteAPIService(){}

        public RemoteAPIService(IGraphDS myGraphDS)
        {
            _RunningTime = new Stopwatch();
        }

        #endregion
        


        public string PluginName
        {
            get { return "sones.RemoteAPIService"; }
        }

        public void Start(IDictionary<string, object> myStartParameter = null)
        {
            
            
            try
            {
                if (_RPCServer != null && _RPCServer.IsRunning)
                    _RPCServer.StopServiceHost();
                
                Boolean IsSecure = false;
                if (myStartParameter != null && myStartParameter.ContainsKey("IsSecure"))
                    IsSecure = (Boolean)Convert.ChangeType(myStartParameter["IsSecure"], typeof(Boolean));

                String UriPattern = "rpc";
                if (myStartParameter != null && myStartParameter.ContainsKey("URI"))
                    UriPattern = (String)Convert.ChangeType(myStartParameter["URI"], typeof(String));
                                
                String Namespace = "http://www.sones.com";
                if (myStartParameter != null && myStartParameter.ContainsKey("Namespace"))
                    Namespace = (String)Convert.ChangeType(myStartParameter["Namespace"], typeof(String));

                IPAddress Address = IPAddress.Any;
                if (myStartParameter != null && myStartParameter.ContainsKey("IPAddress"))
                    Address = (IPAddress)Convert.ChangeType(myStartParameter["IPAddress"], typeof(IPAddress));

                ushort Port = 9970;
                if (myStartParameter != null && myStartParameter.ContainsKey("Port"))
                    Port = (ushort)Convert.ChangeType(myStartParameter["Port"], typeof(ushort));

                _RunningTime.Start();
                _RPCServer = new sonesRPCServer(_GraphDS, Address, Port, UriPattern, IsSecure, Namespace);
                _RPCServer.StartServiceHost();

            }
            catch (Exception Ex)
            {
                throw Ex;
            }
        }

        public void Stop()
        {
            _RunningTime.Reset();
            _RPCServer.StopServiceHost();
        }

        public AServiceStatus GetCurrentStatus()
        {
            return new RemoteAPIServiceStatus(_RPCServer.URI, _RPCServer.ListeningIPAdress, _RPCServer.ListeningPort, _RPCServer.IsSecure, _RPCServer.IsRunning,
                _RunningTime.Elapsed);
        }

        #region IPluginable

        public PluginParameters<Type> SetableParameters
        {
            get
            {
                PluginParameters<Type> Parameters = new PluginParameters<Type>();
                Parameters.Add("GraphDS", typeof(IGraphDS));
                Parameters.Add("IsSecure", typeof(Boolean));
                Parameters.Add("IPAddress", typeof(IPAddress));
                Parameters.Add("Port", typeof(ushort));
                Parameters.Add("URI", typeof(String));
                return Parameters;
            }
        }

        public IPluginable InitializePlugin(string UniqueString, Dictionary<string, object> myParameters = null)
        {
            IGraphDS GraphDS = null;
            if (myParameters != null && myParameters.ContainsKey("GraphDS"))
                GraphDS = (IGraphDS)myParameters["GraphDS"];

            return new RemoteAPIService(GraphDS);
        }

        public void Dispose()
        {
        
        }

        #endregion
        
    }
}
