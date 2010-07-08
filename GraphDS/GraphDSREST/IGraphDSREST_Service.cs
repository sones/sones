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


/*
 * IGraphDSREST_Service
 * Achim Friedland, 2010
 */

#region Usings

using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Web;
using sones.Networking.HTTP;

#endregion

namespace sones.GraphDS.Connectors.REST
{

    [ServiceContract, ForceAuthentication]
    public interface IGraphDSREST_Service
    {

        /// <summary>
        /// Get the WADL describtion of this service
        /// </summary>
        /// <returns>The WADL description</returns>
        [OperationContract, NoAuthentication]
        [WebGet(UriTemplate = "/application.wadl")]
        void GetWADL();


        /// <summary>
        /// Get Landingpage
        /// </summary>
        /// <returns>Some HTML and JavaScript</returns>
        [OperationContract, NoAuthentication]
        [WebGet(UriTemplate = "/")]
        void GetHTMLLandingPage();


        /// <summary>
        /// Get favicon.ico
        /// </summary>
        /// <returns>Some HTML and JavaScript</returns>
        [OperationContract, NoAuthentication]
        [WebGet(UriTemplate = "/favicon.ico")]
        void GetFavicon();


        /// <summary>
        /// Invoke a javascript shell
        /// </summary>
        /// <returns>Some HTML and JavaScript</returns>
        [OperationContract]
        [WebGet(UriTemplate = "/WebShell")]
        void GetWebShell();

        /// <summary>
        /// Will return internal resources
        /// </summary>
        /// <returns>internal resources</returns>
        [OperationContract, NoAuthentication]
        [WebGet(UriTemplate = "/WebShell/{myResource}")]
        void GetResources(String myResource);


        /// <summary>
        /// Invoke a gql query to the underlying database.
        /// </summary>
        /// <example>/gql?FROM+Website+w+SELECT+*+WHERE+w.Name+%3d+%27xkcd%27"</example>
        /// <returns>The result of the GQL query</returns>
        [OperationContract]
        [WebGet(UriTemplate = "/gql")]
        void ExecuteGQLQuery();


        // StoreObject
        [OperationContract]
        [WebInvoke(UriTemplate = "/objects/{myObjectType}/{myObjectName}/{myObjectStream}/", Method = "PUT")]
        void StoreObject(String myObjectType, String myObjectName, String myObjectStream);

        [OperationContract]
        [WebInvoke(UriTemplate = "/objects/{myObjectType}/{myObjectName}/{myObjectStream}/{myObjectEdition}/", Method = "PUT")]
        void StoreObject(String myObjectType, String myObjectName, String myObjectStream, String myObjectEdition);

        [OperationContract]
        [WebInvoke(UriTemplate = "/objects/{myObjectType}/{myObjectName}/{myObjectStream}/{myObjectEdition}/{myObjectRevision}/", Method = "PUT")]
        void StoreObject(String myObjectType, String myObjectName, String myObjectStream, String myObjectEdition, String myObjectRevision);


        // List...
        [OperationContract]
        [WebGet(UriTemplate = "/objects/{myObjectType}/")]
        void ListObjects(String myObjectType);

        [OperationContract]
        [WebGet(UriTemplate    = "/objects/{myObjectType}/{myObjectName}/")]
        void ListObjectStreams(String myObjectType, String myObjectName);

        [OperationContract]
        [WebGet(UriTemplate    = "/objects/{myObjectType}/{myObjectName}/{myObjectStream}/")]
        void ListObjectEditions(String myObjectType, String myObjectName, String myObjectStream);

        [OperationContract]
        [WebGet(UriTemplate    = "/objects/{myObjectType}/{myObjectName}/{myObjectStream}/{myObjectEdition}/")]
        void ListObjectRevisions(String myObjectType, String myObjectName, String myObjectStream, String myObjectEdition);


        // GetObject
        [OperationContract]
        [WebGet(UriTemplate    = "/objects/{myObjectType}/{myObjectName}/{myObjectStream}/{myObjectEdition}/{myObjectRevision}/")]
        void GetObject(String myObjectType, String myObjectName, String myObjectStream, String myObjectEdition, String myObjectRevision);

        [OperationContract]
        [WebGet(UriTemplate    = "/objects/{myObjectType}/{myObjectName}/{myObjectStream}/{myObjectEdition}/{myObjectRevision}/{myObjectCopy}/")]
        void GetObject(String myObjectType, String myObjectName, String myObjectStream, String myObjectEdition, String myObjectRevision, UInt64 myObjectCopy);


        // DeleteObject
        [OperationContract]
        [WebInvoke(UriTemplate = "/objects/{myObjectType}/{myObjectName}/{myObjectStream}/{myObjectEdition}/{myObjectRevision}/", Method = "DELETE")]
        void DeleteObject(String myObjectType, String myObjectName, String myObjectStream, String myObjectEdition, String myObjectRevision);




        /// <summary>
        /// Invoke a cli command to the underlying cli
        /// </summary>
        /// <example>/cli?ll"</example>
        /// <returns>The result of the CLI query</returns>
        [OperationContract]
        [WebGet(UriTemplate = "/cli")]
        void ExecuteCLIQuery();



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


    }

}
