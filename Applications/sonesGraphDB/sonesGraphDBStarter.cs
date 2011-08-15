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
using System.IdentityModel.Selectors;
using sones.GraphDSServer;
using System.IdentityModel.Tokens;
using System.Diagnostics;
using sones.GraphDB;
using sones.Library.VersionedPluginManager;
using sones.GraphDS.PluginManager;
using sones.Library.Commons.Security;
using System.Net;
using System.Threading;
using sones.GraphDB.Manager.Plugin;
using System.IO;
using System.Globalization;
using sones.Library.DiscordianDate;
using System.Security.AccessControl;
using sones.GraphDSServer.ErrorHandling;


namespace sones.sonesGraphDBStarter
{
    #region PassValidator
    public class PassValidator : UserNamePasswordValidator
    {
        public override void Validate(String myUserName, String myPassword)
        {

            Debug.WriteLine(String.Format("Authenticate {0} and {1}", myUserName, myPassword));

            if (!(myUserName == Properties.Settings.Default.Username && myPassword == Properties.Settings.Default.Password))
            {
                throw new SecurityTokenException("Unknown Username or Password");
            }

        }
    }
    #endregion

    #region sones GraphDB Startup
    public class sonesGraphDBStartup
    {
        private bool quiet = false;
        private GraphDS_Server _dsServer;

        public sonesGraphDBStartup(String[] myArgs)
        {
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo(Properties.Settings.Default.DatabaseCulture);

            if (myArgs.Count() > 0)
            {
                foreach (String parameter in myArgs)
                {
                    if (parameter.ToUpper() == "--Q")
                        quiet = true;
                }
            }
            #region Start REST, WebDAV and WebAdmin services, send GraphDS notification

            IGraphDB GraphDB;

            if (Properties.Settings.Default.UsePersistence)
            {
                if (!quiet)
                   Console.WriteLine("Initializing persistence layer...");
 
                string configuredLocation = Properties.Settings.Default.PersistenceLocation;
                string configuredPageSize = Properties.Settings.Default.PageSize;
                string configuredBufferSize = Properties.Settings.Default.BufferSizeInPages;

                /* Configure the location */

                Uri location = null;
                
                if (configuredLocation.Contains("file:"))
                {
                    location = new Uri(configuredLocation);
                }
                else
                {
                    string rootPath = Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly((typeof(sones.Library.Commons.VertexStore.IVertexStore))).Location);
                    string dataPath = rootPath + Path.DirectorySeparatorChar + configuredLocation;
                    location = new Uri(@dataPath);
                }

                /* Configuration for the page size */
                int pageSize = Int32.Parse(configuredPageSize);

                /* Configuration for the buffer size */
                int bufferSize = Int32.Parse(configuredBufferSize);

                /* Make a new instance by applying the configuration */
                try
                {
                    //Make a new GraphDB instance
                    GraphDB = new SonesGraphDB(new GraphDBPlugins(new PluginDefinition("sones.pagedfsnonrevisionedplugin", new Dictionary<string, object>() { { "location", location }, { "pageSize", pageSize }, { "bufferSizePages", bufferSize} })));
                
                    if (!quiet)
                        Console.WriteLine("Persistence layer initialized.");
                }
                catch (Exception a)
                {
                    if (!quiet)
                    { 
                        Console.WriteLine(a.Message);
                        Console.WriteLine(a.StackTrace);

                        Console.Error.WriteLine("Could not access the data directory " + location.AbsoluteUri + ". Please make sure you that you have the right file access permissions!");
                        Console.Error.WriteLine("Using in memory storage instead.");
                    }
                    GraphDB = new SonesGraphDB(null,true,new CultureInfo(Properties.Settings.Default.DatabaseCulture));
                }
            }
            else
            {
                GraphDB = new SonesGraphDB(null, true, new CultureInfo(Properties.Settings.Default.DatabaseCulture));
            }

            #region Configure PlugIns
            // Plugins are loaded by the GraphDS with their according PluginDefinition and only if they are listed
            // below - there is no auto-discovery for plugin types in GraphDS (!)

                #region Query Languages
                // the GQL Query Language Plugin needs the GraphDB instance as a parameter
                List<PluginDefinition> QueryLanguages = new List<PluginDefinition>();
                Dictionary<string, object> GQL_Parameters = new Dictionary<string, object>();
                GQL_Parameters.Add("GraphDB", GraphDB);

                QueryLanguages.Add(new PluginDefinition("sones.gql", GQL_Parameters));
                #endregion

                #region REST Service Plugins
                List<PluginDefinition> SonesRESTServices = new List<PluginDefinition>();
                // not yet used
                #endregion

                #region GraphDS Service Plugins
                List<PluginDefinition> GraphDSServices = new List<PluginDefinition>();
                #endregion

                #region Drain Pipes            
                
                //// QueryLog DrainPipe
                //Dictionary<string, object> QueryLog_Parameters = new Dictionary<string, object>();
                //QueryLog_Parameters.Add("AsynchronousMode", true);  // do the work in a separate thread to not slow down queries
                //QueryLog_Parameters.Add("MaximumAsyncBufferSize", (Int32)1024 * 1024 * 10); // 10 Mbytes of maximum async queue size
                //QueryLog_Parameters.Add("AppendLogPathAndName", "sones.drainpipelog");
                //QueryLog_Parameters.Add("CreateNew", false); // always create a new file on start-up
                //QueryLog_Parameters.Add("FlushOnWrite", true);  // always flush on each write
            
                //// the DrainPipe Log expects several parameters
                //Dictionary<string, object> DrainPipeLog_Parameters = new Dictionary<string, object>();
                //DrainPipeLog_Parameters.Add("AsynchronousMode", true);  // do the work in a separate thread to not slow down queries
                //DrainPipeLog_Parameters.Add("MaximumAsyncBufferSize", (Int32)1024 * 1024 * 10); // 10 Mbytes of maximum async queue size
                //DrainPipeLog_Parameters.Add("AppendLogPathAndName", "sones.drainpipelog");
                //DrainPipeLog_Parameters.Add("CreateNew", false); // always create a new file on start-up
                //DrainPipeLog_Parameters.Add("FlushOnWrite", true);  // always flush on each write

                //Dictionary<string, object> DrainPipeLog2_Parameters = new Dictionary<string, object>();
                //DrainPipeLog2_Parameters.Add("AsynchronousMode", true);  // do the work in a separate thread to not slow down queries
                //DrainPipeLog2_Parameters.Add("MaximumAsyncBufferSize", (Int32)1024 * 1024 * 10); // 10 Mbytes of maximum async queue size
                //DrainPipeLog2_Parameters.Add("AppendLogPathAndName", "sones.drainpipelog2");
                //DrainPipeLog2_Parameters.Add("CreateNew", false); // always create a new file on start-up
                //DrainPipeLog2_Parameters.Add("FlushOnWrite", true);  // always flush on each write


                List<PluginDefinition> DrainPipes = new List<PluginDefinition>();
                //DrainPipes.Add(new PluginDefinition("sones.querylog", QueryLog_Parameters));
                //DrainPipes.Add(new PluginDefinition("sones.drainpipelog", DrainPipeLog_Parameters));
                //DrainPipes.Add(new PluginDefinition("sones.drainpipelog", DrainPipeLog2_Parameters));
                #endregion

            #endregion

            GraphDSPlugins PluginsAndParameters = new GraphDSPlugins(QueryLanguages, DrainPipes);
            _dsServer = new GraphDS_Server(GraphDB, PluginsAndParameters);

            #region Start GraphDS Services

            #region pre-configure REST Service
            Dictionary<string, object> RestParameter = new Dictionary<string, object>();
            RestParameter.Add("IPAddress", IPAddress.Any);
            RestParameter.Add("Port", Properties.Settings.Default.ListeningPort);
            RestParameter.Add("Username", Properties.Settings.Default.Username);
            RestParameter.Add("Password", Properties.Settings.Default.Password);
            _dsServer.StartService("sones.RESTService", RestParameter);
            #endregion

            #endregion
            
            _dsServer.LogOn(new UserPasswordCredentials(Properties.Settings.Default.Username, Properties.Settings.Default.Password));

            #endregion

            #region Some helping lines...
            if (!quiet)
            {
                Console.WriteLine("This GraphDB Instance offers the following options:");
                Console.WriteLine("   * If you want to suppress console output add --Q as a");
                Console.WriteLine("     parameter.");
                Console.WriteLine();
                Console.WriteLine("   * the following GraphDS Service Plugins are initialized and started: ");

                foreach (var Service in _dsServer.GraphDSServices)
                {
                    Console.WriteLine("      * "+Service.Key);
                    Console.WriteLine(_dsServer.GetServiceStatus(Service.Key).OtherStatistically["Description"].ToString());
                    
                }
              

                
                Console.WriteLine("Enter 'shutdown' to initiate the shutdown of this instance.");
            }

            bool shutdown = false;
            while (!shutdown)
            {
                String command = Console.ReadLine();

                if (command.ToUpper() == "SHUTDOWN")
                    shutdown = true;
            }

            Console.WriteLine("Shutting down");
            foreach (var Service in _dsServer.GraphDSServices)
            {
                Console.WriteLine("    "+Service.Key);
                _dsServer.StopService(Service.Key);
            }
            Console.WriteLine("    GraphDS Server");
            _dsServer.Shutdown(null);
            Console.WriteLine("    GraphDB Server");
            GraphDB.Shutdown(null);
            Console.WriteLine("Shutdown complete");
            #endregion
        }
    }
    #endregion

    public class sonesGraphDBStarter
    {
        static void Main(string[] args)
        {
            bool quiet = false;

            if (args.Count() > 0)
            {
                foreach (String parameter in args)
                {
                    if (parameter.ToUpper() == "--Q")
                        quiet = true;
                }
            }

            if (!quiet)
            {
                DiscordianDate ddate = new DiscordianDate();

                Console.WriteLine("sones GraphDB version 2.0 - " + ddate.ToString());
                Console.WriteLine("(C) sones GmbH 2007-2011 - http://www.sones.com");
                Console.WriteLine("-----------------------------------------------");
                Console.WriteLine();
                Console.WriteLine("Starting up GraphDB...");
            }

            try
            {
                var sonesGraphDBStartup = new sonesGraphDBStartup(args);                
            }
            catch (ServiceException e)
            {
                if (!quiet)
                { 
                    Console.WriteLine(e.Message);
                    Console.WriteLine("InnerException: " + e.InnerException.ToString());
                    Console.WriteLine();
                    Console.WriteLine("Press <return> to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}
