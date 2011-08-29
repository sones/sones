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

        private Stopwatch _LifeTime;

        private String _description;

        #endregion

        #region C'tors

        public RESTService()
        {
        }

        public RESTService(IGraphDS myGraphDS)
        {
            _GraphDS = myGraphDS;
            _LifeTime = new Stopwatch();
        }


        #endregion
            
        public void Start(IDictionary<String, Object> my_Parameters = null)
        {
            try
            {
                if(_HttpServer != null && _HttpServer.IsRunning)
                    _HttpServer.Stop();
                
                // iterate through input dictionary and upper case it
                Dictionary<string, object> myParameters = new Dictionary<string, object>();
                foreach (String Key in my_Parameters.Keys)
                {
                    myParameters.Add(Key.ToUpper(), my_Parameters[Key]);
                }
                

                String Username = "test";
                if (myParameters != null && myParameters.ContainsKey("USERNAME"))
                    Username = (String)Convert.ChangeType(myParameters["USERNAME"], typeof(String));

                String Password = "test";
                if (myParameters != null && myParameters.ContainsKey("PASSWORD"))
                    Password = (String)Convert.ChangeType(myParameters["PASSWORD"], typeof(String));

                IPAddress Address = IPAddress.Any;
                if (myParameters != null && myParameters.ContainsKey("IPADDRESS"))
                    Address = (IPAddress)Convert.ChangeType(myParameters["IPADDRESS"], typeof(IPAddress));

                ushort Port = 9975;
                if (myParameters != null && myParameters.ContainsKey("PORT"))
                    Port = (ushort)Convert.ChangeType(myParameters["PORT"], typeof(ushort));

                _Security = new BasicServerSecurity(new PasswordValidator(null, Username, Password));
                _RESTService = new GraphDSREST_Service();
                _RESTService.Initialize(_GraphDS, Port, Address);
                _HttpServer = new HttpServer(
                        Address,
                        Port,
                        _RESTService,
                        mySecurity: _Security,
                        myAutoStart: false);

                _LifeTime.Start();
                _HttpServer.Start();


                String MyIPAdressString = Address.ToString();

                if (MyIPAdressString == "0.0.0.0")
                    MyIPAdressString = "localhost";

               _description =        "   * REST Service is started at http://" + MyIPAdressString + ":" + Port + Environment.NewLine +
                                     "      * access it directly by passing the GraphQL query using the" + Environment.NewLine +
                                     "        REST interface or a client library. (see documentation)" + Environment.NewLine +
                                     "      * if you want JSON Output add ACCEPT: application/json " + Environment.NewLine +
                                     "        to the client request header (or application/xml or" + Environment.NewLine +
                                     "        application/text)" + Environment.NewLine +
                                     "   * for first steps we recommend to use the AJAX WebShell. " + Environment.NewLine +
                                     "     Browse to http://" + MyIPAdressString + ":" + Port + "/WebShell" + Environment.NewLine +
                                     "     (default username and passwort: test / test)";
            }
            catch (Exception Ex)
            {
                throw new RESTServiceCouldNotStartException("The REST Service could not be started. See inner exception for details.", Ex);
            }
        }

        public void Stop()
        {
            _HttpServer.Stop();
            _LifeTime.Reset();
        }

        public ServiceStatus GetCurrentStatus()
        {
            return new ServiceStatus(_HttpServer.ListeningAddress, _HttpServer.ListeningPort, _HttpServer.IsRunning, _LifeTime.Elapsed, true);
        }

        public string Description
        {
            get { return _description; }
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
                Parameters.Add("GRAPHDS", typeof(IGraphDS));
                Parameters.Add("USERNAME", typeof(String));
                Parameters.Add("PASSWORD", typeof(String));
                Parameters.Add("IPADDRESS", typeof(IPAddress));
                Parameters.Add("PORT", typeof(ushort));
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
        public string PluginShortName
        {
            get { return "RESTSVC"; }
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
