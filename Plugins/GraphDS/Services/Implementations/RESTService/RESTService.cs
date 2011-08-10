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
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using sones.Library.Commons.Security;
using sones.GraphDS.Services.RESTService.Networking;
using System.Net;
using sones.GraphDS.Services.RESTService.ServiceStatus;
using System.Diagnostics;
using sones.GraphDS.Services.RESTService.ErrorHandling;

namespace sones.GraphDS.Services.RESTService
{
    public class RESTService : IService, IPluginable
    {
        #region Data

        private IGraphDS _GraphDS;

        private GraphDSREST_Service _RESTService;

        private BasicServerSecurity _Security;

        private HttpServer _HttpServer;

        private Stopwatch _RunningTime;

        #endregion

        #region C'tors

        public RESTService()
        {
        }

        public RESTService(IGraphDS myGraphDS)
        {
            _GraphDS = myGraphDS;
            _RunningTime = new Stopwatch();
        }


        #endregion
            
        public void Start(IDictionary<String, Object> myParameters = null)
        {
            try
            {
                if(_HttpServer != null && _HttpServer.IsRunning)
                    _HttpServer.Stop();
                                 
                String Username = "test";
                if (myParameters != null && myParameters.ContainsKey("Username"))
                    Username = (String)Convert.ChangeType(myParameters["Username"], typeof(String));

                String Password = "test";
                if (myParameters != null && myParameters.ContainsKey("Password"))
                    Password = (String)Convert.ChangeType(myParameters["Password"], typeof(String));

                IPAddress Address = IPAddress.Any;
                if (myParameters != null && myParameters.ContainsKey("IPAddress"))
                    Address = (IPAddress)Convert.ChangeType(myParameters["IPAddress"], typeof(IPAddress));

                ushort Port = 9975;
                if (myParameters != null && myParameters.ContainsKey("Port"))
                    Port = (ushort)Convert.ChangeType(myParameters["Port"], typeof(ushort));

                _Security = new BasicServerSecurity(new PasswordValidator(null, Username, Password));
                _RESTService = new GraphDSREST_Service();
                _RESTService.Initialize(_GraphDS, Port, Address);
                _HttpServer = new HttpServer(
                        Address,
                        Port,
                        _RESTService,
                        mySecurity: _Security,
                        myAutoStart: false);

                _RunningTime.Start();
                _HttpServer.Start();
            }
            catch (Exception Ex)
            {
                throw new RESTServiceCouldNotStartException("The REST Service could not be started. See inner exception for details.", Ex);
            }
        }

        public void Stop()
        {
            _HttpServer.Stop();
            _RunningTime.Reset();
        }

        public AServiceStatus GetCurrentStatus()
        {
            return new RESTServiceStatus(_HttpServer.ListeningAddress,_HttpServer.ListeningPort,_HttpServer.IsRunning, _RunningTime.Elapsed);
        }

        public string PluginName
        {
            get { return "sones.RESTService"; }
        }

        #region IPluginable

        public PluginParameters<Type> SetableParameters
        {
            get
            {
                PluginParameters<Type> Parameters = new PluginParameters<Type>();
                Parameters.Add("GraphDS", typeof(IGraphDS));
                Parameters.Add("Username", typeof(String));
                Parameters.Add("Password", typeof(String));
                Parameters.Add("IPAddress", typeof(IPAddress));
                Parameters.Add("Port", typeof(ushort));
                return Parameters;
            }
        }

        public IPluginable InitializePlugin(string UniqueString, Dictionary<string, object> myParameters = null)
        {
            IGraphDS GraphDS = null;
            if (myParameters != null && myParameters.ContainsKey("GraphDS"))
                GraphDS = (IGraphDS)myParameters["GraphDS"];

            return new RESTService(GraphDS);
        }

        public void Dispose()
        {
            Stop();
        }

        #endregion
        
    }


    internal class PasswordValidator : UserNamePasswordValidator
    {
        private readonly IUserAuthentication _dbauth;
        private String _Username;
        private String _Password;

        public PasswordValidator(IUserAuthentication dbAuthentcator, String Username, String Password)
        {
            _dbauth = dbAuthentcator;
            _Username = Username;
            _Password = Password;
        }

        public override void Validate(string userName, string password)
        {
            if (!(userName == _Username && password == _Password))
            {
                throw new SecurityTokenException("Username or password incorrect.");
            }
        }
    }
}
