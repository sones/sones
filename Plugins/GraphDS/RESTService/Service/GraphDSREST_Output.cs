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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Reflection;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using sones.GraphDS;
using sones.GraphQL.Result;
using sones.Library.DiscordianDate;
using sones.Library.Network.HttpServer;
using sones.Plugins.GraphDS.IO;

#endregion

namespace sones.Plugins.GraphDS.RESTService
{
    public class GraphDSREST_Output
    {
        #region Data

        private GraphDSREST_Errors              _ErrorMsg;
        private IGraphDS                        _GraphDS;
        private String                          _ServerID;
        private Dictionary<String, IOInterface> _Plugins;
        #endregion

        #region Constructors

        public GraphDSREST_Output(IGraphDS myGraphDS, String myServerID, Dictionary<String, IOInterface> myPlugins)
        {
            _Plugins = myPlugins;
            _ServerID = myServerID;
            _ErrorMsg = new GraphDSREST_Errors(_ServerID);
            _GraphDS = myGraphDS;
        }

        #endregion

        #region private Helpers

        private void ExportContent(String myServerID, byte[] myContent, ContentType myContentType)
        {
            var _Header = HttpServer.HttpContext.Response;

            _Header.SetHttpStatusCode(HttpStatusCode.OK);
            _Header.SetCacheControl("no-cache");
            _Header.SetServerName(myServerID);
            _Header.SetContentType(myContentType);

            _Header.OutputStream.Write(myContent, 0, myContent.Length);
        }

        #endregion

        #region GenerateResultOutput(myResult, myQuery)

        public void GenerateResultOutput(QueryResult myResult)
        {

            var _ContentType = HttpServer.GetBestMatchingAcceptHeader(GraphDSREST_Constants._HTML, GraphDSREST_Constants._JSON, GraphDSREST_Constants._XML, GraphDSREST_Constants._GEXF, GraphDSREST_Constants._TEXT);
            
           
            IOInterface plugin = null;


            if (_Plugins.TryGetValue(_ContentType.MediaType, out plugin))
            {
                var content = plugin.GenerateOutputResult(myResult);
                ExportContent(_ServerID, System.Text.Encoding.UTF8.GetBytes(content), plugin.ContentType);
            }
            else
            {
                _ErrorMsg.Error406_NotAcceptable(String.Format("The server does not support the requested content type {0} ", _ContentType.ToString()));

            }

        }

        #endregion

        #region GetGQL()

        public String GetGQL()
        {
            if (HttpServer.HttpContext.Request.QueryString.Count == 0)
            {
                _ErrorMsg.Error400_BadRequest("[Syntax Error] Please use '...gql?query'!");
                return String.Empty;
            }

// Mono 2.10.2 has a bug regarding Request.QueryString.
// Leave this ifdef until this bug is fixed.
#if __MonoCS__

            //we know we have a '?', so we can access position 1
            var raw = HttpServer.HttpContext.Request.RawUrl;
            var index = raw.IndexOf('?') + 1;
            var _QueryString = raw.Substring(index, raw.Length - index);

#else
            var _QueryString = HttpServer.HttpContext.Request.QueryString[null];

#endif
            if (_QueryString == null)
            {
                _ErrorMsg.Error400_BadRequest("[Syntax Error] Please use '...gql?query'!");
                return String.Empty;
            }

            var _GQLQuery = HttpUtility.UrlDecode(_QueryString);

            return _GQLQuery;
        }

        #endregion

        #region RedirectContent(myContent, myContentType)

        public void RedirectContent(String myContent, ContentType myContentType)
        {
            ExportContent(_ServerID, Encoding.Default.GetBytes(myContent), myContentType);
        }

        #endregion

        #region ExecuteGQLQuery(myGQLStatement)

        public QueryResult ExecuteGQL(String myQuery)
        {
            QueryResult _QueryResult = null;
            try
            {
                _QueryResult = _GraphDS.Query(null, null, myQuery, "sones.gql");

                GenerateResultOutput(_QueryResult);

                return _QueryResult;

            }
            catch (Exception ex)
            {
                _ErrorMsg.Error400_BadRequest(ex.Message + ex.StackTrace);
            }
           

            return _QueryResult;
        }

        #endregion

        #region GetResources(myResource)

        /// <summary>
        /// Returns resources embedded within the assembly to the user!
        /// </summary>
        /// <param name="myResource">The path and name </param>
        /// <returns>an embedded resource or an error page</returns>
        public void GetResources(String myResource)
        {

            #region Data

            var _Assembly = Assembly.GetExecutingAssembly();
            var _Resources = _Assembly.GetManifestResourceNames();
            Stream _Content = null;

            if (myResource.Contains("/"))
                myResource = myResource.Replace('/', '.');

            #endregion

            #region Return assembly resource...

            var _ContentType = HttpServer.GetBestMatchingAcceptHeader(GraphDSREST_Constants._GEXF);
            var response = HttpServer.HttpContext.Response;

            if (_Resources.Contains("GraphDSREST.resources." + myResource))
            {


                _Content = _Assembly.GetManifestResourceStream("GraphDSREST.resources." + myResource);

                #region Set the content type - sones REST

                if (HttpServer.HttpContext!= null)
                {

                    var _FileNameSuffix = myResource.Remove(0, myResource.LastIndexOf(".") + 1);

                    switch (_FileNameSuffix)
                    {
                        case "htm":
                            response.SetContentType(GraphDSREST_Constants._HTML_UTF8); 
                            break;
                        case "html": 
                            response.SetContentType(GraphDSREST_Constants._HTML_UTF8); 
                            break;
                        case "css": 
                            response.SetContentType(GraphDSREST_Constants._CSS); 
                            break;
                        case "gif": 
                            response.SetContentType(GraphDSREST_Constants._GIF); 
                            break;
                        case "ico": 
                            response.SetContentType(GraphDSREST_Constants._ICO); 
                            break;
                        case "swf": 
                            response.SetContentType(new ContentType("application/x-shockwave-flash")); 
                            break;
                        case "js": 
                            response.SetContentType(new ContentType("text/javascript")); 
                            break;
                        default: 
                            response.SetContentType(GraphDSREST_Constants._TEXT_UTF8); 
                            break;
                    }

                }

                #endregion

                response.SetHttpStatusCode(HttpStatusCode.OK);
                response.SetCacheControl("no-cache");
                response.SetServerName(_ServerID);

                _Content.CopyTo(response.OutputStream);

                return;

            }

            #endregion

            #region ...or send custom Error 404 page!

            else
            {

                _Content = _Assembly.GetManifestResourceStream("GraphDSREST.resources.errorpages.Error404.html");

                if (_Content == null)
                    _Content = new MemoryStream(UTF8Encoding.UTF8.GetBytes("Error 404 - File not found!"));

                response.SetHttpStatusCode(HttpStatusCode.NotFound);
                response.SetCacheControl("no-cache");
                response.SetServerName(_ServerID);
                response.SetContentType(GraphDSREST_Constants._HTML);

                _Content.CopyTo(response.OutputStream);

            }

            #endregion

        }

        #endregion

        #region GetDDate()

        public Stream GetDDate()
        {
            if (WebOperationContext.Current != null)
            {
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain";
            }
            else if (HttpServer.HttpContext != null)
            {
                HttpServer.HttpContext.Response.SetContentType(new ContentType("text/plain"));
            }

            return new MemoryStream(Encoding.UTF8.GetBytes(new DiscordianDate().ToString()));
        }

        #endregion
                
    }

}

