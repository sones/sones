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

#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel;
using sones.GraphDS;
using sones.GraphDS.PluginManager.RESTServicePluginManager;
using sones.Plugins.GraphDS.IO;


#endregion

namespace sones.Plugins.GraphDS.RESTService
{

    // NOTE: If you change the class name "Service1" here, you must also update the reference to "Service1" in App.config.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class GraphDSREST_Service : IGraphDSREST_Service
    {

        #region Data

        private const String _ServerName = "sones GraphDSREST";
        private const String _ServerVersion = "0.1a";
        private const String _ServerID = _ServerName + " " + _ServerVersion;

        private        IGraphDS                     _IGraphDS;
        private        GraphDSREST_Errors           _ErrorMsg;
        private        GraphDSREST_Output           _RESTOutput;
        private        RESTServicePluginManager     _RestPluginManager;
        private Dictionary<String, IOInterface> _IOPlugins; 
        #endregion

        #region Constructor

        #region GraphDSREST_Service()

        public GraphDSREST_Service()
        {
            _RestPluginManager = new RESTServicePluginManager();
            _IOPlugins = new Dictionary<String, IOInterface>();

            foreach (var item in _RestPluginManager.GetPluginsForType<IOInterface>())
            {
                var plugin = _RestPluginManager.GetAndInitializePlugin<IOInterface>(item);
                _IOPlugins.Add(plugin.ContentType.MediaType,plugin);
            }
        }

        #endregion

        #endregion

        #region Applications

        #region GetWebShell()

        /// <summary>
        /// Send the WebShell HTML to the user!
        /// </summary>
        public void GetWebShell()
        {
            _RESTOutput.GetResources("WebShell/WebShell.html");
        }

        #endregion

    
        #endregion

        #region Queries

        #region ExecuteGQLQuery()

        /// <summary>
        /// Invoke a gql query to the underlying database.
        /// </summary>
        /// <example>/gql?FROM+Website+w+SELECT+*+WHERE+w.Name+%3d+%27xkcd%27"</example>
        /// <returns>The result of the GQL query</returns>
        public void ExecuteGQLQuery()
        {
            var gqlQuery = _RESTOutput.GetGQL();
            
            if (gqlQuery == String.Empty)
            {
                return;    
            }

            _RESTOutput.ExecuteGQL(gqlQuery);
        }

        #endregion

        #region ExecuteCLIQuery()

        public void ExecuteCLIQuery()
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region Vertex manipulation

        #region StoreObject

        #region StoreObject(myObjectType, myObjectName, myObjectStream)

        public void StoreObject(String myObjectType, String myObjectName, String myObjectStream)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region StoreObject(myObjectType, myObjectName, myObjectStream, myObjectEdition)

        public void StoreObject(String myObjectType, String myObjectName, String myObjectStream, String myObjectEdition)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region StoreObject(myObjectType, myObjectName, myObjectStream, myObjectEdition, myObjectRevision)

        public void StoreObject(String myObjectType, String myObjectName, String myObjectStream, String myObjectEdition, String myObjectRevision)
        {

            throw new NotImplementedException();

        }

        #endregion

        #endregion

        #region List...

        #region ListObjects(myObjectType)

        public void ListObjects(String myObjectType)
        {

            throw new NotImplementedException();
        }

        #endregion

        #region ListObjectStreams(myObjectType, myObjectName)

        public void ListObjectStreams(String myObjectType, String myObjectName)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ListObjectEditions(myObjectType, myObjectName, myObjectStream)

        public void ListObjectEditions(String myObjectType, String myObjectName, String myObjectStream)
        {

            throw new NotImplementedException();

        }

        #endregion

        #region ListObjectRevisions(myObjectType, myObjectName, myObjectStream, myObjectEdition)

        public void ListObjectRevisions(String myObjectType, String myObjectName, String myObjectStream, String myObjectEdition)
        {

            throw new NotImplementedException();

        }

        #endregion

        #endregion

        #region GetObject

        #region GetObject(myObjectType, myObjectName, myObjectStream, myObjectEdition, myObjectRevision)

        public void GetObject(String myObjectType, String myObjectName, String myObjectStream, String myObjectEdition, String myObjectRevision)
        {
            throw new NotImplementedException();

        }

        #endregion

        #region GetObject(myObjectType, myObjectName, myObjectStream, myObjectEdition, myObjectRevision, myObjectCopy)

        public void GetObject(String myObjectType, String myObjectName, String myObjectStream, String myObjectEdition, String myObjectRevision, UInt64 myObjectCopy)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region DeleteObject

        #region DeleteObject(myObjectType, myObjectName, myObjectStream, myObjectEdition, myObjectRevision)

        public void DeleteObject(String myObjectType, String myObjectName, String myObjectStream, String myObjectEdition, String myObjectRevision)
        {

            throw new NotImplementedException();

        }

        #endregion

        #endregion

        #endregion

        #region Utilities

        #region GetResources(myResource)

        /// <summary>
        /// Returns resources embedded within the assembly to the user!
        /// </summary>
        /// <param name="myResource">The path and name </param>
        /// <returns>an embedded resource or an error page</returns>
        public void GetResources(String myResource)
        {
            _RESTOutput.GetResources(myResource);
        }

        #endregion

        #region GetHTMLLandingPage()

        public void GetHTMLLandingPage()
        {
            //as long as there is no landing page, redirect to the webshell
            _RESTOutput.GetResources("WebShell/WebShell.html");
        }

        #endregion

        #region GetWADL()

        /// <summary>
        /// Get the WADL description of this service
        /// </summary>
        public void GetWADL()
        {
            GetResources("GraphDSREST.wadl");
        }

        #endregion

        #region GetFavicon()

        public void GetFavicon()
        {
            GetResources("favicon.ico");
        }

        #endregion

        #region GetLogfile()

        public Stream GetLogfile()
        {

            /*var _StringBuilder = new StringBuilder();

            foreach (var _Logline in ConsoleOutputLogger.GetLoggedLines())
                if (_Logline != null)
                    _StringBuilder.AppendLine(_Logline);

            if (WebOperationContext.Current != null)
            {
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain";
            }
            else if (HTTPServer.HTTPContext != null)
            {
                HTTPServer.HTTPContext.ResponseHeader.ContentType = new System.Net.Mime.ContentType("text/plain");
            }
            return new MemoryStream(Encoding.UTF8.GetBytes(_StringBuilder.ToString())); ;*/

            throw new NotImplementedException();

        }

        #endregion

        #region GetDDate()

        public Stream GetDDate()
        {
            return _RESTOutput.GetDDate();
        }

        #endregion

        #region GetClientAccessPolicy

        public void GetClientAccessPolicy()
        {
            GetResources("ClientAccessPolicy.xml");
        }

        #endregion

        #region GetCrossDomain

        public void GetCrossDomain()
        {
            GetResources("CrossDomain.xml");
        }

        #endregion

        #endregion


        public string ID
        {
            get { throw new NotImplementedException(); }
        }

        public ushort Port
        {
            get { throw new NotImplementedException(); }
        }

        public IPAddress IPAddress
        {
            get { throw new NotImplementedException(); }
        }

        public void Initialize(IGraphDS myGraphDS, ushort myPort, IPAddress myIPAddress)
        {
            _IGraphDS = myGraphDS;
            _ErrorMsg = new GraphDSREST_Errors(_ServerID);
            _RESTOutput = new GraphDSREST_Output(myGraphDS, _ServerID, _IOPlugins);        
        }
    }

}
