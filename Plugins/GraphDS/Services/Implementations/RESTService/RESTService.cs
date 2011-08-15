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
