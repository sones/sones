#region Usings

using System;
using System.IO;
using System.Web;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Collections.Generic;

using sones.GraphDB;

using sones.Networking;
using sones.Networking.HTTP;
using sones.GraphDS;
using System.Net;


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

        #endregion

        #region Constructor

        #region GraphDSREST_Service()

        public GraphDSREST_Service()
        {
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

        #region GetVisualGraph()

        /// <summary>
        /// Send the VisualGraph HTML to the user!
        /// </summary>
        public void GetVisualGraph()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            _RESTOutput = new GraphDSREST_Output(myGraphDS, _ServerID);        
        }
    }

}
