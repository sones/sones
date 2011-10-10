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

/*using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using sones.GraphDB;
using sones.Library.DiscordianDate;
using sones.Library.VersionedPluginManager;
using sones.GraphDS.PluginManager;
using sones.GraphDSServer;
using sones.GraphDSServer.ErrorHandling;
using sones.GraphDB.Manager.Plugin;*/

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
using sones.GraphDS.GraphDSRemoteClient;
using sones.GraphDS.GraphDSRESTClient;
using sones.GraphDB.TypeSystem;
using sones.GraphDB.Request;
using sones.Library.PropertyHyperGraph;

namespace TagExampleWithGraphMappingFramework
{
    
    public class TagExampleWithGraphMappingFramework
    {
        #region sones GraphDB Startup
        private bool quiet = false;
        private bool shutdown = false;
        private IGraphDSServer _dsServer;
        private bool _ctrlCPressed;
        private IGraphDSClient GraphDSClient;
        private sones.Library.Commons.Security.SecurityToken SecToken;
        private long TransToken;

        public TagExampleWithGraphMappingFramework(String[] myArgs)
        {
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("en-us");

            if (myArgs.Count() > 0)
            {
                foreach (String parameter in myArgs)
                {
                    if (parameter.ToUpper() == "--Q")
                        quiet = true;
                }
            }
            #region Start RemoteAPI, WebDAV and WebAdmin services, send GraphDS notification

            IGraphDB GraphDB;

            GraphDB = new SonesGraphDB(null, true, new CultureInfo("en-us"));

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

            List<PluginDefinition> UsageDataCollector = new List<PluginDefinition>();

            #endregion

            GraphDSPlugins PluginsAndParameters = new GraphDSPlugins(QueryLanguages);
            _dsServer = new GraphDS_Server(GraphDB, PluginsAndParameters);

            #region Start GraphDS Services

            #region pre-configure REST Service
            Dictionary<string, object> RestParameter = new Dictionary<string, object>();
            RestParameter.Add("IPAddress", IPAddress.Any);
            RestParameter.Add("Port", 9975);
            RestParameter.Add("Username", "test");
            RestParameter.Add("Password", "test");
            _dsServer.StartService("sones.RESTService", RestParameter);
            #endregion

            #region Remote API Service
            Dictionary<string, object> RemoteAPIParameter = new Dictionary<string, object>();
            RemoteAPIParameter.Add("IPAddress", IPAddress.Parse("127.0.0.1"));
            RemoteAPIParameter.Add("Port", (ushort)9970);
            _dsServer.StartService("sones.RemoteAPIService", RemoteAPIParameter);
            #endregion

            #endregion

            #endregion

            #endregion

            #region Some helping lines...
            if (!quiet)
            {
                Console.WriteLine("This GraphDB Instance offers the following options:");
                Console.WriteLine("   * If you want to suppress console output add --Q as a");
                Console.WriteLine("     parameter.");
                Console.WriteLine();
                Console.WriteLine("   * the following GraphDS Service Plugins are initialized and started: ");

                foreach (var Service in _dsServer.AvailableServices)
                {
                    Console.WriteLine("      * " + Service.PluginName);
                }
                Console.WriteLine();

                foreach (var Service in _dsServer.AvailableServices)
                {
                    Console.WriteLine(Service.ServiceDescription);
                    Console.WriteLine();
                }

                Console.WriteLine("Enter 'shutdown' to initiate the shutdown of this instance.");
            }

            Run();

            Console.CancelKeyPress += OnCancelKeyPress;

            while (!shutdown)
            {
                String command = Console.ReadLine();

                if (!_ctrlCPressed)
                {
                    if (command != null)
                    {
                        if (command.ToUpper() == "SHUTDOWN")
                            shutdown = true;
                    }
                }
            }

            Console.WriteLine("Shutting down GraphDS Server");
            _dsServer.Shutdown(null);
            Console.WriteLine("Shutdown complete");
            #endregion
        }

        #region Cancel KeyPress
        /// <summary>
        ///  Cancel KeyPress
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true; //do not abort Console here.
            _ctrlCPressed = true;
            Console.Write("Shutdown GraphDB (y/n)?");
            string input;
            do
            {
                input = Console.ReadLine();
            } while (input == null);

            switch (input.ToUpper())
            {
                case "Y":
                    shutdown = true;
                    return;
                default:
                    shutdown = false;
                    return;
            }
        }//method
        #endregion

        #region the actual example

        public void Run()
        {
            GraphDSClient = new GraphDS_RemoteClient(new Uri("http://localhost:9970/rpc"));
            SecToken = GraphDSClient.LogOn(new RemoteUserPasswordCredentials("test", "test"));
            TransToken = GraphDSClient.BeginTransaction(SecToken);

            GraphDBRequests();

            Console.WriteLine("Run :-)");
        }

        private void GraphDBRequests()
        {
            Console.WriteLine("performing DB requests...");
            #region define type "Tag"

            //create a VertexTypePredefinition
            var Tag_VertexTypePredefinition = new VertexTypePredefinition("Tag");

            //create property
            var PropertyName = new PropertyPredefinition("Name", "String")
                                           .SetComment("This is a property on type 'Tag' named 'Name' and is of type 'String'");

            //add property
            Tag_VertexTypePredefinition.AddProperty(PropertyName);

            //create outgoing edge to "Website"
            var OutgoingEdgesTaggedWebsites = new OutgoingEdgePredefinition("TaggedWebsites", "Website")
                                                          .SetMultiplicityAsMultiEdge()
                                                          .SetComment(@"This is an outgoing edge on type 'Tag' wich points to the type 'Website' (the AttributeType) 
                                                                            and is defined as 'MultiEdge', which means that this edge can contain multiple single edges");

            //add outgoing edge
            Tag_VertexTypePredefinition.AddOutgoingEdge(OutgoingEdgesTaggedWebsites);

            #endregion

            #region define type "Website"

            //create a VertexTypePredefinition
            var Website_VertexTypePredefinition = new VertexTypePredefinition("Website");

            //create properties
            PropertyName = new PropertyPredefinition("Name", "String")
                                       .SetComment("This is a property on type 'Website' named 'Name' and is of type 'String'");

            var PropertyUrl = new PropertyPredefinition("URL", "String")
                                         .SetAsMandatory();

            //add properties
            Website_VertexTypePredefinition.AddProperty(PropertyName);
            Website_VertexTypePredefinition.AddProperty(PropertyUrl);

            #region create an index on type "Website" on property "Name"
            //there are three ways to set an index on property "Name" 
            //Beware: Use just one of them!

            //1. create an index definition and specifie the property- and type name
            var MyIndex = new IndexPredefinition("MyIndex").SetIndexType("SonesIndex").AddProperty("Name").SetVertexType("Website");
            //add index
            Website_VertexTypePredefinition.AddIndex((IndexPredefinition)MyIndex);

            //2. on creating the property definition of property "Name" call the SetAsIndexed() method, the GraphDB will create the index
            //PropertyName = new PropertyPredefinition("Name")
            //                           .SetAttributeType("String")
            //                           .SetComment("This is a property on type 'Website' with name 'Name' and is of type 'String'")
            //                           .SetAsIndexed();

            //3. make a create index request, like creating a type
            //BEWARE: This statement must be execute AFTER the type "Website" is created.
            //var MyIndex = GraphDSServer.CreateIndex<IIndexDefinition>(SecToken,
            //                                                          TransToken,
            //                                                          new RequestCreateIndex(
            //                                                          new IndexPredefinition("MyIndex")
            //                                                                   .SetIndexType("SonesIndex")
            //                                                                   .AddProperty("Name")
            //                                                                   .SetVertexType("Website")), (Statistics, Index) => Index);

            #endregion

            //add IncomingEdge "Tags", the related OutgoingEdge is "TaggedWebsites" on type "Tag"
            Website_VertexTypePredefinition.AddIncomingEdge(new IncomingEdgePredefinition("Tags",
                                                                                            "Tag",
                                                                                            "TaggedWebsites"));

            #endregion


            #region create types by sending requests

            //create the types "Tag" and "Website"
            var DBTypes = GraphDSClient.CreateVertexTypes<IEnumerable<IVertexType>>(SecToken,
                                                                                    TransToken,
                                                                                    new RequestCreateVertexTypes(
                                                                                        new List<VertexTypePredefinition> { Tag_VertexTypePredefinition, 
                                                                                                                            Website_VertexTypePredefinition }),
                                                                                    (Statistics, VertexTypes) => VertexTypes);

            /* 
             * BEWARE: The following two operations won't work because the two types "Tag" and "Website" depending on each other,
             *          because one type has an incoming edge to the other and the other one has an incoming edge, 
             *          so they cannot be created separate (by using create type),
             *          they have to be created at the same time (by using create types)
             *          
             * //create the type "Website"
             * var Website = GraphDSServer.CreateVertexType<IVertexType>(SecToken, 
             *                                                           TransToken, 
             *                                                           new RequestCreateVertexType(Website_VertexTypePredefinition), 
             *                                                           (Statistics, VertexType) => VertexType);
             * 
             * //create the type "Tag"
             * var Tag = GraphDSServer.CreateVertexType<IVertexType>(SecToken, 
             *                                                       TransToken, 
             *                                                       new RequestCreateVertexType(Tag_VertexTypePredefinition), 
             *                                                       (Statistics, VertexType) => VertexType);
             */

            var Tag = DBTypes.Where(type => type.Name == "Tag").FirstOrDefault();
            if (Tag != null)
                Console.WriteLine("Vertex Type 'Tag' created");
            var Website = DBTypes.Where(type => type.Name == "Website").FirstOrDefault();
            if(Website != null)
                Console.WriteLine("Vertex Type 'Website' created");

            #endregion


            #region insert some Websites by sending requests

            var cnn = GraphDSClient.Insert<IVertex>(SecToken, TransToken, new RequestInsertVertex("Website")
                                                                                    .AddStructuredProperty("Name", "CNN")
                                                                                    .AddStructuredProperty("URL", "http://cnn.com/"),
                                                                                    (Statistics, Result) => Result);

            var xkcd = GraphDSClient.Insert<IVertex>(SecToken, TransToken, new RequestInsertVertex("Website")
                                                                                    .AddStructuredProperty("Name", "xkcd")
                                                                                    .AddStructuredProperty("URL", "http://xkcd.com/"),
                                                                                    (Statistics, Result) => Result);

            var onion = GraphDSClient.Insert<IVertex>(SecToken, TransToken, new RequestInsertVertex("Website")
                                                                                    .AddStructuredProperty("Name", "onion")
                                                                                    .AddStructuredProperty("URL", "http://theonion.com/"),
                                                                                    (Statistics, Result) => Result);

            //adding an unknown property means the property isn't defined before
            var test = GraphDSClient.Insert<IVertex>(SecToken, TransToken, new RequestInsertVertex("Website")
                                                                                    .AddStructuredProperty("Name", "Test")
                                                                                    .AddStructuredProperty("URL", "")
                                                                                    .AddUnknownProperty("Unknown", "unknown property"),
                                                                                    (Statistics, Result) => Result);

            if (cnn != null)
                Console.WriteLine("Website 'cnn' successfully inserted");

            if (xkcd != null)
                Console.WriteLine("Website 'xkcd' successfully inserted");

            if (onion != null)
                Console.WriteLine("Website 'onion' successfully inserted");

            if (test != null)
                Console.WriteLine("Website 'test' successfully inserted");

            #endregion

            #region insert some Tags by sending requests

            //insert a "Tag" with an OutgoingEdge to a "Website" include that the GraphDB creates an IncomingEdge on the given Website instances
            //(because we created an IncomingEdge on type "Website") --> as a consequence we never have to set any IncomingEdge
            var good = GraphDSClient.Insert<IVertex>(SecToken, TransToken, new RequestInsertVertex("Tag")
                                                                                    .AddStructuredProperty("Name", "good")
                                                                                    .AddEdge(new EdgePredefinition("TaggedWebsites")
                                                                                        .AddVertexID(Website.ID, cnn.VertexID)
                                                                                        .AddVertexID(Website.ID, xkcd.VertexID)),
                                                                                    (Statistics, Result) => Result);

            var funny = GraphDSClient.Insert<IVertex>(SecToken, TransToken, new RequestInsertVertex("Tag")
                                                                                    .AddStructuredProperty("Name", "funny")
                                                                                    .AddEdge(new EdgePredefinition("TaggedWebsites")
                                                                                        .AddVertexID(Website.ID, xkcd.VertexID)
                                                                                        .AddVertexID(Website.ID, onion.VertexID)),
                                                                                    (Statistics, Result) => Result);

            if (good != null)
                Console.WriteLine("Tag 'good' successfully inserted");

            if (funny != null)
                Console.WriteLine("Tag 'funny' successfully inserted");

            #endregion

            #region how to get a type from the DB, properties of the type, instances of a specific type and read out property values

            //how to get a type from the DB
            var TagDBType = GraphDSClient.GetVertexType<IVertexType>(SecToken, TransToken, new RequestGetVertexType(Tag.Name), (Statistics, Type) => Type);

            //read informations from type
            var typeName = TagDBType.Name;
            //are there other types wich extend the type "Tag"
            var hasChildTypes = TagDBType.HasChildTypes;
            //get the definition of the property "Name"
            var propName = TagDBType.GetPropertyDefinition("Name");

            //how to get all instances of a type from the DB
            var TagInstances = GraphDSClient.GetVertices(SecToken, TransToken, new RequestGetVertices(TagDBType.Name), (Statistics, Vertices) => Vertices);

            foreach (var item in TagInstances)
            {
                //to get the value of a property of an instance, you need the property ID 
                //(that's why we fetched the type from DB an read out the property definition of property "Name")
                var name = item.GetPropertyAsString(propName.ID);
            }

            #endregion
        }
        #endregion
    }
    

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
                var sonesGraphDBStartup = new TagExampleWithGraphMappingFramework(args);
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
