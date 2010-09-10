/*
* sones GraphDB - OpenSource Graph Database - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB OpenSource Edition.
*
* sones GraphDB OSE is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
*
* sones GraphDB OSE is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB OSE. If not, see <http://www.gnu.org/licenses/>.
*/

#region Usings

using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.ServiceModel;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using sones.GraphDB.Structures;
using sones.GraphDS.API.CSharp;
using sones.GraphDS.API.CSharp.Notifications;
using sones.GraphDS.API.CSharp.Reflection;
using sones.GraphDS.Connectors.CLI;
using sones.GraphDS.Connectors.REST;
using sones.GraphDS.Connectors.WebDAV;
using sones.GraphFS.DataStructures;
using sones.Lib.DataStructures.UUID;
using sones.Networking.HTTP;
using sones.Notifications;
using sones.Notifications.Messages;
using System.Net;
using System.Threading;
using sones.GraphDB.NewAPI;
using sones.GraphDBInterface.Result;

#endregion

namespace sonesExample
{

    #region PassValidator

    public class PassValidator : UserNamePasswordValidator
    {
        public override void Validate(String myUserName, String myPassword)
        {

            Debug.WriteLine(String.Format("Authenticate {0} and {1}", myUserName, myPassword));

            if (!(myUserName == "test" && myPassword == "test") && !(myUserName == "test2" && myPassword == "test2"))
            {
                throw new SecurityTokenException("Unknown Username or Password");
            }

        }
    }

    #endregion

    #region Custom GraphDB types

    #region Document

    public class Document : DBVertex
    {

        [Indexed(DBIndexTypes.HashTable)]
        public String Name { get; set; }

        // Edges to type Website
        public Set<Tag> Tags { get; set; }

        // Edges to type Author
        public Set<Author> Authors { get; set; }

        public Document() { }

    }

    #endregion

    #region Author

    public class Author : DBVertex
    {

        [Indexed(DBIndexTypes.HashTable)]
        public String Name { get; set; }

        public String EMail { get; set; }

        // Backwardedges to attribute tags of type Website
        [BackwardEdge("Authors")]
        public Set<Document> Writtings { get; set; }

        public Author() { }

    }

    #endregion

    #region Tag

    public class Tag : DBVertex
    {

        [Indexed(DBIndexTypes.HashTable)]
        public String Name { get; set; }

        // Backwardedges to attribute tags of type Website
        [BackwardEdge("Tags")]
        public Set<Document> Documents { get; set; }

        public Tag() { }

    }

    #endregion

    #endregion

    #region sonesExampleClass

    public class sonesExampleClass
    {
        private bool quiet = false;

        #region CheckResult(myQueryResult)

        private void CheckResult(QueryResult myQueryResult)
        {
            if (!quiet)
            {
                if (myQueryResult.ResultType != ResultType.Successful)
                    Console.WriteLine("{0} => {1}", myQueryResult.Query, myQueryResult.Errors.First().Message);

                else
                    Console.WriteLine("{0}", myQueryResult.Query);
            }
        }

        #endregion

        public sonesExampleClass(String[] myArgs)
        {


            if (myArgs.Count() > 0)
            {
                foreach (String parameter in myArgs)
                {
                    if (parameter.ToUpper() == "--Q")
                        quiet = true;
                }
            }

            #region Init NotificationDispatcher
            var _NotificationSettings = new NotificationSettings()
            {
                StartDispatcher = false,
                StartBrigde = false
            };

            var _NotificationDispatcher = new NotificationDispatcher(UUID.NewUUID, _NotificationSettings);

            #endregion

            #region Create or open GraphDS

            var _GraphDSSharp = new GraphDSSharp()
            {
                DatabaseName = "sonesExample",
                Username = "Dr.Falken",
                Password = "Joshua",
                NotificationSettings = _NotificationSettings,
                NotificationDispatcher = _NotificationDispatcher,
            };

            // Create a InMemory data storage
            _GraphDSSharp.CreateDatabase(true);


            #endregion

            #region Create GraphDB types

            _GraphDSSharp.CreateTypes(CheckResult, typeof(Document), typeof(Author), typeof(Tag));

            #endregion

            #region Create a document upload directory

            _GraphDSSharp.CreateDirectoryObject(new ObjectLocation("Uploads"));

            #endregion

            #region Insert some data

            _GraphDSSharp.Query("INSERT INTO Tag VALUES (Name = 'good')", CheckResult);
            _GraphDSSharp.Query("INSERT INTO Tag VALUES (Name = 'funny')", CheckResult);
            _GraphDSSharp.Query("INSERT INTO Tag VALUES (Name = 'science fiction')", CheckResult);

            _GraphDSSharp.Query("INSERT INTO Author VALUES (Name = 'Holger', EMail = 'holger.liebau@sones.de')", CheckResult);
            _GraphDSSharp.Query("INSERT INTO Author VALUES (Name = 'Achim',  EMail = 'achim@sones.de')", CheckResult);

            #endregion

            #region Start REST, WebDAV and WebAdmin services, send GraphDS notification

            var _HttpSecurity = new HTTPSecurity()
            {
                CredentialType = HttpClientCredentialType.Basic,
                UserNamePasswordValidator = new PassValidator()
            };

            // Start a REST service on localhost port 9975
            var _RESTService      = _GraphDSSharp.StartREST(IPAddress.Any, 9975, _HttpSecurity);


            // Start a WebDAV service on localhost port 9978
            var _WebDAVService    = _GraphDSSharp.StartWebDAV(IPAddress.Any, 9978, _HttpSecurity);


            // Send GraphDS notification
            _NotificationDispatcher.SendNotification(typeof(NGraphDSReady),
                    new NGraphDSReady.Arguments() { Message = "sonesExample up'n'ready!" },
                    NotificationPriority.Normal,
                    true);

            #endregion

            #region Some helping lines...
            if (!quiet)
            {
                Console.WriteLine();
                Console.WriteLine("This small example demonstrates how to start a sones GraphDB");
                Console.WriteLine("Instance and it's several services:");
                Console.WriteLine("   * If you want to suppress console output add --Q as a");
                Console.WriteLine("     parameter.");
                Console.WriteLine("   * REST Service is started at http://localhost:9975");
                Console.WriteLine("      * access it directly like in this example: ");
                Console.WriteLine("           http://localhost:9975/gql?DESCRIBE%20TYPES");
                Console.WriteLine("      * if you want JSON Output add ACCEPT: application/json ");
                Console.WriteLine("        to the client request header (or application/xml or");
                Console.WriteLine("        application/text)");
                Console.WriteLine("   * we recommend to use the AJAX WebShell. ");
                Console.WriteLine("        Browse to http://localhost:9975/WebShell and use");
                Console.WriteLine("        the username \"test\" and password \"test\"");
                Console.WriteLine("   * Additionally a WebDAV service is started on port 9978.");
                Console.WriteLine();
            }
            #endregion


            #region Start the GraphDS command line interface

            if (!quiet)
                _GraphDSSharp.OpenCLI();
            else
                while (true)
                {
                    Thread.Sleep(1000);
                }

            #endregion

            #region Shutdown

            _GraphDSSharp.Shutdown();

            #endregion

        }

    }

    #endregion

    #region sonesExample.Main(myArgs)

    public class sonesExample
    {

        static void Main(String[] myArgs)
        {
            var _WebDAVHosting = new sonesExampleClass(myArgs);
        }

    }

    #endregion

}
