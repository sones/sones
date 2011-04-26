#region Usings

using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using sones.Networking.HTTP;

#endregion

namespace sones.Plugins.GraphDS.RESTService
{

    [ServiceContract, ForceAuthentication]
    public interface IGraphDSREST_Service : ISonesRESTService
    {

        #region Applications

        /// <summary>
        /// Invoke a javascript shell
        /// </summary>
        /// <returns>Some HTML and JavaScript</returns>
        [OperationContract]
        [WebGet(UriTemplate = "/WebShell")]
        void GetWebShell();
               
        #endregion

        #region Queries

        /// <summary>
        /// Invoke a gql query to the underlying database.
        /// </summary>
        /// <example>/gql?FROM+Website+w+SELECT+*+WHERE+w.Name+%3d+%27xkcd%27"</example>
        /// <returns>The result of the GQL query</returns>
        [OperationContract]
        [WebGet(UriTemplate = "/gql")]
        void ExecuteGQLQuery();

        /// <summary>
        /// Invoke a cli command to the underlying cli
        /// </summary>
        /// <example>/cli?ll"</example>
        /// <returns>The result of the CLI query</returns>
        [OperationContract]
        [WebGet(UriTemplate = "/cli")]
        void ExecuteCLIQuery();

        #endregion

        #region Vertex manipulation

        // StoreObject
        [OperationContract]
        [WebInvoke(UriTemplate = "/vertices/{myObjectType}/{myObjectName}/{myObjectStream}/", Method = "PUT")]
        void StoreObject(String myObjectType, String myObjectName, String myObjectStream);

        [OperationContract]
        [WebInvoke(UriTemplate = "/vertices/{myObjectType}/{myObjectName}/{myObjectStream}/{myObjectEdition}/", Method = "PUT")]
        void StoreObject(String myObjectType, String myObjectName, String myObjectStream, String myObjectEdition);

        [OperationContract]
        [WebInvoke(UriTemplate = "/vertices/{myObjectType}/{myObjectName}/{myObjectStream}/{myObjectEdition}/{myObjectRevision}/", Method = "PUT")]
        void StoreObject(String myObjectType, String myObjectName, String myObjectStream, String myObjectEdition, String myObjectRevision);


        // List...
        [OperationContract]
        [WebGet(UriTemplate    = "/vertices/{myObjectType}/")]
        void ListObjects(String myObjectType);

        [OperationContract]
        [WebGet(UriTemplate    = "/vertices/{myObjectType}/{myObjectName}/")]
        void ListObjectStreams(String myObjectType, String myObjectName);

        [OperationContract]
        [WebGet(UriTemplate    = "/vertices/{myObjectType}/{myObjectName}/{myObjectStream}/")]
        void ListObjectEditions(String myObjectType, String myObjectName, String myObjectStream);

        [OperationContract]
        [WebGet(UriTemplate    = "/vertices/{myObjectType}/{myObjectName}/{myObjectStream}/{myObjectEdition}/")]
        void ListObjectRevisions(String myObjectType, String myObjectName, String myObjectStream, String myObjectEdition);


        // GetObject
        [OperationContract]
        [WebGet(UriTemplate    = "/vertices/{myObjectType}/{myObjectName}/{myObjectStream}/{myObjectEdition}/{myObjectRevision}/")]
        void GetObject(String myObjectType, String myObjectName, String myObjectStream, String myObjectEdition, String myObjectRevision);

        [OperationContract]
        [WebGet(UriTemplate    = "/vertices/{myObjectType}/{myObjectName}/{myObjectStream}/{myObjectEdition}/{myObjectRevision}/{myObjectCopy}/")]
        void GetObject(String myObjectType, String myObjectName, String myObjectStream, String myObjectEdition, String myObjectRevision, UInt64 myObjectCopy);


        // DeleteObject
        [OperationContract]
        [WebInvoke(UriTemplate = "/vertices/{myObjectType}/{myObjectName}/{myObjectStream}/{myObjectEdition}/{myObjectRevision}/", Method = "DELETE")]
        void DeleteObject(String myObjectType, String myObjectName, String myObjectStream, String myObjectEdition, String myObjectRevision);

        #endregion

        #region Utilities

        /// <summary>
        /// Will return internal resources
        /// </summary>
        /// <returns>internal resources</returns>
        [OperationContract, NoAuthentication]
        [WebGet(UriTemplate = "/resources/{myResource}")]
        void GetResources(String myResource);

        /// <summary>
        /// Will return internal resources
        /// </summary>
        /// <returns>internal resources</returns>
        [OperationContract, NoAuthentication]
        [WebGet(UriTemplate = "/ClientAccessPolicy.xml")]
        void GetClientAccessPolicy();
        
        /// <summary>
        /// Will return internal resources
        /// </summary>
        /// <returns>internal resources</returns>
        [OperationContract, NoAuthentication]
        [WebGet(UriTemplate = "/CrossDomain.xml")]
        void GetCrossDomain();
        
        /// <summary>
        /// Get Landingpage
        /// </summary>
        /// <returns>Some HTML and JavaScript</returns>
        [OperationContract, NoAuthentication]
        [WebGet(UriTemplate = "/")]
        void GetHTMLLandingPage();

        /// <summary>
        /// Get the WADL describtion of this service
        /// </summary>
        /// <returns>The WADL description</returns>
        [OperationContract, NoAuthentication]
        [WebGet(UriTemplate = "/application.wadl")]
        void GetWADL();

        /// <summary>
        /// Get favicon.ico
        /// </summary>
        /// <returns>Some HTML and JavaScript</returns>
        [OperationContract, NoAuthentication]
        [WebGet(UriTemplate = "/favicon.ico")]
        void GetFavicon();


        /// <summary>
        /// Returns the last loglines
        /// </summary>
        /// <returns>Returns the last loglines</returns>
        [OperationContract]
        [WebGet(UriTemplate = "/logfile")]
        Stream GetLogfile();

        /// <summary>
        /// Returns the actual ddate
        /// </summary>
        /// <returns>Returns the actual ddate</returns>
        [OperationContract, NoAuthentication]
        [WebGet(UriTemplate = "/ddate")]
        Stream GetDDate();

        #endregion

    }

}
