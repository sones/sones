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

        #endregion

        public RemoteAPIService(){}

        public RemoteAPIService(IGraphDS myGraphDS)
        {
        
        }


        public string PluginName
        {
            get { return "sones.RemoteAPIService"; }
        }

        public void Start(IDictionary<string, object> myStartParameter = null)
        {
            
        }

        public void Stop()
        {
            
        }

        public AServiceStatus GetCurrentStatus()
        {
            return null;
        }


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
            throw new NotImplementedException();
        }
    }
}
