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
 * GraphDSREST_Service
 * Achim Friedland, 2010
 */

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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using sones.GraphDB;
using sones.GraphDB.Errors;
using sones.GraphDB.Structures.Result;
using sones.GraphDB.Structures;
using sones.GraphFS.DataStructures;
using sones.GraphFS.Objects;
using sones.GraphFS.Session;
using sones.Lib;
using sones.Lib.CLI;
using sones.Lib.DDate;
using sones.Lib.ErrorHandling;
using sones.Lib.SimpleLogger;
using sones.Networking;
using sones.Networking.HTTP;
using sones.GraphDS.API.CSharp;
using sones.GraphDB.GraphQL;

#endregion

namespace sones.GraphDS.Connectors.REST
{

    // NOTE: If you change the class name "Service1" here, you must also update the reference to "Service1" in App.config.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class GraphDSREST_Service : IGraphDSREST_Service
    {

        #region Data

        private        IGraphDBSession              _IGraphDBSession;
        private        IGraphFSSession              _IGraphFSSession;
        private        Dictionary<String, sonesCLI> _SessionSonesCLIs;
        private        MemoryStream                 _MemoryStream;
        private        GraphQLQuery                     _GraphQLQuery;

        private static ContentType _JSON            = new ContentType("application/json");
        private static ContentType _XML             = new ContentType("application/xml");
        private static ContentType _TEXT            = new ContentType(MediaTypeNames.Text.Plain);
        private static ContentType _HTML            = new ContentType("text/html");
        private static ContentType _GEXF            = new ContentType("application/gexf");

        private static ContentType _JSON_UTF8       = new ContentType("application/json")        { CharSet = "UTF-8" };
        private static ContentType _XML_UTF8        = new ContentType("application/xml")         { CharSet = "UTF-8" };
        private static ContentType _TEXT_UTF8       = new ContentType(MediaTypeNames.Text.Plain) { CharSet = "UTF-8" };
        private static ContentType _HTML_UTF8       = new ContentType("text/html")               { CharSet = "UTF-8" };
        private static ContentType _GEXF_UTF8       = new ContentType("application/gexf")        { CharSet = "UTF-8" };

        private static ContentType _CSS             = new ContentType("text/css");
        private static ContentType _GIF             = new ContentType("image/gif");
        private static ContentType _ICO             = new ContentType("image/ico");
        private static ContentType _PNG             = new ContentType("image/png");
        private static ContentType _JPG             = new ContentType("image/jpg");

        private static ContentType _OCTET           = new ContentType("application/octet-stream");

        private const  String      _ServerName      = "sones GraphDSREST";
        private const  String      _ServerVersion   = "0.1a";
        private const  String      _ServerID        = _ServerName + " " + _ServerVersion;

        #endregion

        #region Constructor

        #region GraphDSREST_Service()

        public GraphDSREST_Service()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region GraphDSREST_Service(myIGraphDBSession, myIGraphFSSession)

        public GraphDSREST_Service(IGraphDBSession myIGraphDBSession, IGraphFSSession myIGraphFSSession)
        {
            _IGraphDBSession  = myIGraphDBSession;
            _IGraphFSSession  = myIGraphFSSession;
            _GraphQLQuery = new GraphQLQuery(_IGraphDBSession.DBPluginManager);
            _SessionSonesCLIs = new Dictionary<String, sonesCLI>();
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
            GetResources("WebShell/WebShell.html");
        }

        #endregion

        #region GetVisualGraph()

        /// <summary>
        /// Send the VisualGraph HTML to the user!
        /// </summary>
        public void GetVisualGraph()
        {
            GetResources("VisualGraph/VisualGraph.html");
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

            try
            {

                #region Check settings

                //var _GraphDBREST_Settings = parseSettings(myEncodedSettings);

                //if (_GraphDBREST_Settings == null)
                //    return "invalid settings";

                //if (!checkAuthentication(_GraphDBREST_Settings))
                //    return "TODO implement: error handling checkAuth...";
                if (1 > 1)
                { }

                #endregion

                else
                {

                    if (HTTPServer.HTTPContext.RequestHeader.QueryString == null)
                    {
                        Error400_BadRequest("[Syntax Error] Please use '...gql?query'!");
                        return;
                    }

                    var _QueryString = HTTPServer.HTTPContext.RequestHeader.QueryString[null];
                    if (_QueryString == null)
                    {
                        Error400_BadRequest("[Syntax Error] Please use '...gql?query'!");
                        return;
                    }

                    var _GQLQuery = HttpUtility.UrlDecode(_QueryString);
                    ConsoleOutputLogger.WriteLine(_GQLQuery);

                    var _StopWatch   = new Stopwatch();
                    var _QueryResult = new QueryResult(new Exceptional<ResultType>(ResultType.Failed));

                    /// this is a workaround to implement EXECDBSCRIPT into the webshell
                    /// if anything entered as a GQL command starts with "EXECDBSCRIPT" we'll take it from here.
                    if (_GQLQuery.ToUpper().StartsWith("EXECDBSCRIPT "))
                    {

                        #region EXECDBSCRIP HACK

                        // get the script from the path...
                        if (!File.Exists(".\\scripts\\" + _GQLQuery.Remove(0, 13)))
                        {
                            _StopWatch.Start();
                            // output error 

                            _QueryResult = new QueryResult(new Error_FileNotFound(".\\scripts\\" + _GQLQuery.Remove(0, 13)));
                            _QueryResult.Query = _GQLQuery;
                            _StopWatch.Stop();
                        }

                        else
                        {

                            _StopWatch.Start();

                            var QueryResults = new List<QueryResult>();

                            using (var streamReader = new StreamReader(".\\scripts\\" + _GQLQuery.Remove(0, 13)))
                            {
                                String line;
                                while ((line = streamReader.ReadLine()) != null)
                                {
                                    ConsoleOutputLogger.WriteLine(line);
                                    QueryResult newResult = _GraphQLQuery.Query(line, _IGraphDBSession);

                                    QueryResults.Add(newResult);
                                    String Logging2 = PrintErrorToString(newResult.ResultType, newResult.Errors);
                                    ConsoleOutputLogger.WriteLine_NotIntoLogfile(Logging2);
                                }

                            }

                            // cummulate results
                            var Errors = new List<IError>();
                            var Warnings = new List<IWarning>();
                            ulong ResultLine = 0;

                            foreach (var qr in QueryResults)
                            {
                                ResultLine++;

                                if (qr.ResultType != ResultType.Successful)
                                {

                                    if (!qr.Errors.IsNullOrEmpty())
                                        Errors.AddRange(qr.Errors);
                                    if (!qr.Warnings.IsNullOrEmpty())
                                        Warnings.AddRange(qr.Warnings);
                                }

                            }
                            _QueryResult = new QueryResult(Errors, Warnings);

                            _StopWatch.Stop();
                        }

                        #endregion

                    }
                    else
                    {
                        _StopWatch.Start();
                        _QueryResult = _GraphQLQuery.Query(_GQLQuery, _IGraphDBSession);
                        _StopWatch.Stop();
                    }

                    var Logging = PrintErrorToString(_QueryResult.ResultType, _QueryResult.Errors);
                    ConsoleOutputLogger.WriteLine_NotIntoLogfile(Logging);

                    var _ContentType = HTTPServer.HTTPContext.RequestHeader.GetBestMatchingAcceptHeader(_HTML, _JSON, _XML, _GEXF, _TEXT);

                    _QueryResult.Duration = (UInt64) _StopWatch.ElapsedMilliseconds;

                    #region text/html

                    if (_ContentType.MediaType == _HTML.MediaType)
                    {

                        var _HTMLExport = new HTML_IO();

                        var _String = HTML_IO_Extensions.HTMLBuilder(_IGraphDBSession.DatabaseRootPath, _StringBuilder =>
                        {

                            _StringBuilder.Append("<p><a href=\"/\">back...</a></p>");

                            _HTMLExport.Export(_QueryResult, _StringBuilder);

                            //if (_QueryResult.ResultType == ResultType.Successful)
                            //{
                            //    if (_QueryResult[0] != null)
                            //        foreach (var _DBObjectReadout in _QueryResult[0])
                            //            new HTML_IO().Export(_DBObjectReadout, _StringBuilder);
                            //}

                            //else
                            //{

                            //    _StringBuilder.AppendLine("<b>FAILED!</b><br />");

                            //    foreach (var _IError in _QueryResult.Errors)
                            //        _StringBuilder.Append(_IError.ToString()).AppendLine("<br /><br />");

                            //}

                         //   _StringBuilder.Append("<b>Duration:</b> ").Append(_StopWatch.ElapsedMilliseconds).Append(" ms<br />");

                        });

                        var content = Encoding.UTF8.GetBytes(_String);
                        var _Header = HTTPServer.HTTPContext.ResponseHeader;

                        _Header.HttpStatusCode  = HTTPStatusCodes.OK;
                        _Header.CacheControl    = "no-cache";
                        _Header.ServerName      = _ServerID;
                        _Header.ContentLength   = content.ULongLength();
                        _Header.ContentType     = _HTMLExport.ExportContentType;

                        HTTPServer.HTTPContext.WriteToResponseStream(_Header.ToBytes());
                        HTTPServer.HTTPContext.WriteToResponseStream(content);

                        return;

                    }

                    #endregion

                    #region application/xml

                    else if (_ContentType.MediaType == _XML.MediaType)
                    {

                        var _XMLExport = new XML_IO();

                        var content = Encoding.UTF8.GetBytes(_XMLExport.ExportString(_QueryResult));
                        var _Header = HTTPServer.HTTPContext.ResponseHeader;

                        _Header.HttpStatusCode  = HTTPStatusCodes.OK;
                        _Header.CacheControl    = "no-cache";
                        _Header.ServerName      = _ServerID;
                        _Header.ContentLength   = content.ULongLength();
                        _Header.ContentType     = _XMLExport.ExportContentType;

                        var HeaderBytes = _Header.ToBytes();
                        HTTPServer.HTTPContext.WriteToResponseStream(HeaderBytes, 0, HeaderBytes.Length);
                        HTTPServer.HTTPContext.WriteToResponseStream(content, 0, content.Length);

                        return;

                    }

                    #endregion

                    #region application/gexf

                    else if (_ContentType.MediaType == _GEXF.MediaType)
                    {

                        var _GEXFExport = new GEXF_IO();

                        var content = Encoding.UTF8.GetBytes(_GEXFExport.ExportString(_QueryResult));
                        var _Header = HTTPServer.HTTPContext.ResponseHeader;

                        _Header.HttpStatusCode  = HTTPStatusCodes.OK;
                        _Header.CacheControl    = "no-cache";
                        _Header.ServerName      = _ServerID;
                        _Header.ContentLength   = content.ULongLength();
                        _Header.ContentType     = _GEXFExport.ExportContentType;

                        var HeaderBytes = _Header.ToBytes();
                        HTTPServer.HTTPContext.WriteToResponseStream(HeaderBytes, 0, HeaderBytes.Length);
                        HTTPServer.HTTPContext.WriteToResponseStream(content, 0, content.Length);

                        return;

                    }

                    #endregion

                    #region text/plain

                    else if (_ContentType.MediaType == _TEXT.MediaType)
                    {

                        var _StringBuilder = new StringBuilder(_QueryResult.ToTEXT());
                        _StringBuilder.Append("\nDuration: " + _StopWatch.ElapsedMilliseconds + " ms");

                        var content = Encoding.UTF8.GetBytes(_StringBuilder.ToString());
                        var _Header = HTTPServer.HTTPContext.ResponseHeader;

                        _Header.HttpStatusCode  = HTTPStatusCodes.OK;
                        _Header.CacheControl    = "no-cache";
                        _Header.ServerName      = _ServerID;
                        _Header.ContentLength   = content.ULongLength();
                        _Header.ContentType     = _TEXT_UTF8;

                        var HeaderBytes = _Header.ToBytes();
                        HTTPServer.HTTPContext.WriteToResponseStream(HeaderBytes, 0, HeaderBytes.Length);
                        HTTPServer.HTTPContext.WriteToResponseStream(content, 0, content.Length);

                        return;

                    }

                    #endregion

                    #region application/json (DEFAULT)

                    else 
                    {

                        var _JSONExport = new JSON_IO();

                        var content = Encoding.UTF8.GetBytes(_JSONExport.ExportString(_QueryResult));
                        var _Header = HTTPServer.HTTPContext.ResponseHeader;

                        _Header.HttpStatusCode  = HTTPStatusCodes.OK;
                        _Header.CacheControl    = "no-cache";
                        _Header.ServerName      = _ServerID;
                        _Header.ContentLength   = content.ULongLength();
                        _Header.ContentType     = _JSONExport.ExportContentType;

                        var HeaderBytes = _Header.ToBytes();
                        HTTPServer.HTTPContext.WriteToResponseStream(HeaderBytes, 0, HeaderBytes.Length);
                        HTTPServer.HTTPContext.WriteToResponseStream(content, 0, content.Length);

                        return;

                    }

                    #endregion

                }

            }

            catch (Exception ex)
            {
                Error400_BadRequest(ex.Message + ex.StackTrace);
            }

        }

        #endregion

        #region ExecuteCLIQuery()

        public void ExecuteCLIQuery()
        {

            try
            {

                #region Check settings

                var _GraphDBREST_Settings = new GraphDSREST_Settings();

                if (!checkAuthentication(_GraphDBREST_Settings))
                    throw new NotImplementedException("TODO implement: error handling checkAuth...");

                #endregion

                else
                {


                    if (HTTPServer.HTTPContext.RequestHeader.QueryString == null)
                    {
                        Error400_BadRequest("[Syntax Error] Please use '...cli?query'!");
                        return;
                    }

                    var _QueryString = HTTPServer.HTTPContext.RequestHeader.QueryString[null];
                    if (_QueryString == null)
                    {
                        Error400_BadRequest("[Syntax Error] Please use '...cli?query'!");
                        return;
                    }

                    var _CLIQuery = HttpUtility.UrlDecode(_QueryString);



                    if (HTTPServer.HTTPContext != null)
                        HTTPServer.HTTPContext.ResponseHeader.ContentType = new ContentType("text/plain");

                    #region Start a PandoraCLI // Please refactor me!

                    sonesCLI _PandoraCLI;

                    if (_GraphDBREST_Settings.Username == null)
                        _GraphDBREST_Settings.Username = "";

                    if (!_SessionSonesCLIs.ContainsKey(_GraphDBREST_Settings.Username))
                    {
                        _MemoryStream = new MemoryStream();
                        _PandoraCLI = new sonesCLI(_IGraphDBSession, _IGraphFSSession, _IGraphDBSession.DatabaseRootPath, _MemoryStream, CLI_Output.Standard, typeof(AllCLICommands));
                        _SessionSonesCLIs.Add(_GraphDBREST_Settings.Username, _PandoraCLI);
                    }

                    else
                    {
                        _PandoraCLI = _SessionSonesCLIs[_GraphDBREST_Settings.Username];
                        _MemoryStream = new MemoryStream();
                        _PandoraCLI.StreamWriter = new StreamWriter(_MemoryStream);
                    }

                    var sw = new Stopwatch();

                    _PandoraCLI.ReadAndExecuteCommand(_CLIQuery);
                    _MemoryStream.Seek(0, SeekOrigin.Begin);

                    var _Header = HTTPServer.HTTPContext.ResponseHeader;

                    _Header.HttpStatusCode = HTTPStatusCodes.OK;
                    _Header.CacheControl = "no-cache";
                    _Header.ServerName = _ServerID;
                    _Header.ContentLength = _MemoryStream.ULength();
                    _Header.ContentType = _XML_UTF8;

                    HTTPServer.HTTPContext.WriteToResponseStream(_Header.ToBytes());
                    HTTPServer.HTTPContext.WriteToResponseStream(_MemoryStream);

                    return;


                    #endregion

                }

            }

            catch (Exception ex)
            {
                Error400_BadRequest(ex.Message + ex.StackTrace);
            }

        }

        #endregion

        #endregion

        #region Vertex manipulation

        #region StoreObject

        #region StoreObject(myObjectType, myObjectName, myObjectStream)

        public void StoreObject(String myObjectType, String myObjectName, String myObjectStream)
        {
            StoreObject(myObjectType, myObjectName, myObjectStream, FSConstants.DefaultEdition, null);
        }

        #endregion

        #region StoreObject(myObjectType, myObjectName, myObjectStream, myObjectEdition)

        public void StoreObject(String myObjectType, String myObjectName, String myObjectStream, String myObjectEdition)
        {
            StoreObject(myObjectType, myObjectName, myObjectStream, myObjectEdition, null);
        }

        #endregion

        #region StoreObject(myObjectType, myObjectName, myObjectStream, myObjectEdition, myObjectRevision)

        public void StoreObject(String myObjectType, String myObjectName, String myObjectStream, String myObjectEdition, String myObjectRevision)
        {

            #region Parse RevisionID

            ObjectRevisionID _RevisionID = null;

            try
            {

                if (myObjectRevision.IsNullOrEmpty() || myObjectRevision == "null")
                    _RevisionID = new ObjectRevisionID(_IGraphFSSession.GetFileSystemUUID());
                else
                    _RevisionID = new ObjectRevisionID(myObjectRevision);

            }
            catch (Exception)
            {
                
                var _Header2 = HTTPServer.HTTPContext.RequestHeader;

                _Header2.HttpStatusCode  = HTTPStatusCodes.PreconditionFailed;
                _Header2.CacheControl    = "no-cache";
                _Header2.ServerName      = "Sones GraphDSREST Connector";
                _Header2.ContentLength   = 0;
                _Header2.ContentType     = new ContentType(MediaTypeNames.Application.Octet);

                var HeaderBytes = _Header2.ToBytes();
                HTTPServer.HTTPContext.WriteToResponseStream(HeaderBytes, 0, HeaderBytes.Length);

                return;

            }

            #endregion

            var _Body   = HTTPServer.HTTPContext.RequestBody;
            var _Header = HTTPServer.HTTPContext.RequestHeader;

            _IGraphFSSession.StoreFSObject(new FileObject() {
                                               ObjectLocation   = new ObjectLocation(_IGraphDBSession.DatabaseRootPath, myObjectType, "Objects", myObjectName),
                                               ObjectStream     = myObjectStream,
                                               ObjectEdition    = myObjectEdition,
                                               ObjectRevisionID   = _RevisionID,
                                               ObjectData       = _Body,
                                               ContentType      = _Header.ContentType.ToString()
                                           }, true).

                #region Failed!

                FailedAction(e =>
                    {
                        _Header.HttpStatusCode   = HTTPStatusCodes.PreconditionFailed;
                        _Header.CacheControl     = "no-cache";
                        _Header.ServerName       = _ServerID;
                        _Header.ContentLength    = 0;
                        _Header.ContentType      = new ContentType(MediaTypeNames.Application.Octet);

                        var HeaderBytes = _Header.ToBytes();
                        HTTPServer.HTTPContext.WriteToResponseStream(HeaderBytes, 0, HeaderBytes.Length);
                    }).

                #endregion

                #region Success

                SuccessAction(e =>
                    {
                        _Header.HttpStatusCode   = HTTPStatusCodes.NoContent;
                        _Header.CacheControl     = "no-cache";
                        _Header.ServerName       = _ServerID;
                        _Header.ContentLength    = 0;
                        _Header.ContentType      = new ContentType("text/plain");

                        var HeaderBytes = _Header.ToBytes();
                        HTTPServer.HTTPContext.WriteToResponseStream(HeaderBytes, 0, HeaderBytes.Length);
                    });

                #endregion

        }

        #endregion

        #endregion

        #region List...

        #region ListObjects(myObjectType)

        public void ListObjects(String myObjectType)
        {

            var _StopWatch = new Stopwatch();

            QueryResult _QueryResult;

            _QueryResult = _GraphQLQuery.Query("SETTING DB SET ('TYPE'     = 'true')", _IGraphDBSession);
            _QueryResult = _GraphQLQuery.Query("SETTING DB SET ('UUID'     = 'true')", _IGraphDBSession);
            _QueryResult = _GraphQLQuery.Query("SETTING DB SET ('EDITION'  = 'true')", _IGraphDBSession);
            _QueryResult = _GraphQLQuery.Query("SETTING DB SET ('REVISION' = 'true')", _IGraphDBSession);
            
            _StopWatch.Start();
            _QueryResult = _GraphQLQuery.Query("FROM " + myObjectType + " SELECT TYPE,UUID,EDITION,REVISION,* DEPTH 1", _IGraphDBSession);
            _StopWatch.Stop();

            var _ContentType = HTTPServer.HTTPContext.RequestHeader.GetBestMatchingAcceptHeader(_HTML, _JSON, _XML, _TEXT);

            #region text/html

            if (_ContentType.MediaType == _HTML.MediaType)
            {

                var _String = HTML_IO_Extensions.HTMLBuilder(_IGraphDBSession.DatabaseRootPath, _StringBuilder =>
                {

                    _StringBuilder.Append("<p><a href=\"/\">back...</a></p>");

                    foreach (var _DBObjectReadout in _QueryResult[0])
                        new HTML_IO().Export(_DBObjectReadout, _StringBuilder);

                    _StringBuilder.Append("<b>Duration:</b> ").Append(_StopWatch.ElapsedMilliseconds).Append(" ms<br />");

                });


                var content = Encoding.UTF8.GetBytes(_String);
                var _Header = HTTPServer.HTTPContext.ResponseHeader;

                _Header.HttpStatusCode  = HTTPStatusCodes.OK;
                _Header.CacheControl    = "no-cache";
                _Header.ServerName      = _ServerID;
                _Header.ContentLength   = content.ULongLength();
                _Header.ContentType     = _HTML_UTF8;

                var HeaderBytes = _Header.ToBytes();
                HTTPServer.HTTPContext.WriteToResponseStream(HeaderBytes, 0, HeaderBytes.Length);
                HTTPServer.HTTPContext.WriteToResponseStream(content, 0, content.Length);

                return;

            }

            #endregion

            #region application/xml

            else if (_ContentType.MediaType == _XML.MediaType)
            {

                var _xml = _QueryResult.ToXML();
                ((XElement)_xml.FirstNode).Add(new XElement("Duration", new XAttribute("resolution", "ms"), _StopWatch.ElapsedMilliseconds));

                var content = Encoding.UTF8.GetBytes(_xml.ToString());
                var _Header = HTTPServer.HTTPContext.ResponseHeader;

                _Header.HttpStatusCode  = HTTPStatusCodes.OK;
                _Header.CacheControl    = "no-cache";
                _Header.ServerName      = _ServerID;
                _Header.ContentLength   = content.ULongLength();
                _Header.ContentType     = _XML_UTF8;

                var HeaderBytes = _Header.ToBytes();
                HTTPServer.HTTPContext.WriteToResponseStream(HeaderBytes, 0, HeaderBytes.Length);
                HTTPServer.HTTPContext.WriteToResponseStream(content, 0, content.Length);

                return;

            }

            #endregion

            #region text/plain

            else if (_ContentType.MediaType == _TEXT.MediaType)
            {

                var _StringBuilder = new StringBuilder(_QueryResult.ToTEXT());

                var content = Encoding.UTF8.GetBytes(_StringBuilder.ToString());
                var _Header = HTTPServer.HTTPContext.ResponseHeader;

                _Header.HttpStatusCode  = HTTPStatusCodes.OK;
                _Header.CacheControl    = "no-cache";
                _Header.ServerName      = _ServerID;
                _Header.ContentLength   = content.ULongLength();
                _Header.ContentType     = _TEXT_UTF8;

                var HeaderBytes = _Header.ToBytes();
                HTTPServer.HTTPContext.WriteToResponseStream(HeaderBytes, 0, HeaderBytes.Length);
                HTTPServer.HTTPContext.WriteToResponseStream(content, 0, content.Length);

                return;

            }

            #endregion

            #region application/json (DEFAULT)

            else 
            {

                var _json = _QueryResult.ToJSON();
                _json.Add(new JProperty("Duration", _StopWatch.ElapsedMilliseconds.ToString()));

                var content = Encoding.UTF8.GetBytes(_json.ToString(Formatting.Indented));
                var _Header = HTTPServer.HTTPContext.ResponseHeader;

                _Header.HttpStatusCode  = HTTPStatusCodes.OK;
                _Header.CacheControl    = "no-cache";
                _Header.ServerName      = _ServerID;
                _Header.ContentLength   = content.ULongLength();
                _Header.ContentType     = _JSON_UTF8;

                var HeaderBytes = _Header.ToBytes();
                HTTPServer.HTTPContext.WriteToResponseStream(HeaderBytes, 0, HeaderBytes.Length);
                HTTPServer.HTTPContext.WriteToResponseStream(content, 0, content.Length);

                return;

            }

            #endregion


        }

        #endregion

        #region ListObjectStreams(myObjectType, myObjectName)

        public void ListObjectStreams(String myObjectType, String myObjectName)
        {

            var _Body        = HTTPServer.HTTPContext.RequestBody;
            var _Header      = HTTPServer.HTTPContext.RequestHeader;
            var _ContentType = HTTPServer.HTTPContext.RequestHeader.GetBestMatchingAcceptHeader(_HTML, _JSON, _XML, _TEXT);


            _IGraphFSSession.GetObjectStreams(new ObjectLocation(_IGraphDBSession.DatabaseRootPath, myObjectType, "Objects", myObjectName)).

                #region Failed!

                FailedAction<IEnumerable<String>>(e =>
                    {

                        _Header.HttpStatusCode = HTTPStatusCodes.NotFound;
                        _Header.CacheControl   = "no-cache";
                        _Header.ServerName     = _ServerID;
                        _Header.ContentLength  = 0;
                        _Header.ContentType    = new ContentType("text/plain");

                        var HeaderBytes = _Header.ToBytes();
                        HTTPServer.HTTPContext.WriteToResponseStream(HeaderBytes, 0, HeaderBytes.Length);
                        HTTPServer.HTTPContext.WriteToResponseStream(new Byte[0], 0, 0);

                    }).

                #endregion

                #region Success

                SuccessAction<IEnumerable<String>>(e =>
                    {

                        Byte[] content = null;

                        #region text/html

                        if (_ContentType.MediaType == _HTML.MediaType)
                        {

                            var _String = HTML_IO_Extensions.HTMLBuilder(_IGraphDBSession.DatabaseRootPath, _StringBuilder =>
                            {

                                _StringBuilder.Append("<p><a href=\"/\">back...</a></p>");

                                foreach (var _ObjectStream in e.Value)
                                    _StringBuilder.Append("<a href=\"/objects/").Append(myObjectType).Append("/").Append(myObjectName).Append("/").Append(_ObjectStream).Append("/\">").Append(_ObjectStream).AppendLine("</a><br />");

                                //_StringBuilder.Append("<b>Duration:</b> ").Append(_StopWatch.ElapsedMilliseconds).Append(" ms<br />");

                            });


                            content = Encoding.UTF8.GetBytes(_String);
                            _Header.ContentType = _HTML_UTF8;

                        }

                        #endregion

                        #region XML

                        else if (_ContentType.MediaType == _XML.MediaType)
                        {

                            var Streams = new StringBuilder();

                            Streams.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>").
                                    AppendLine("<ObjectStreams>");

                            foreach (var _ObjectStreams in e.Value)
                                Streams.AppendLine("<ObjectStream>" + _ObjectStreams + "</ObjectStream>");

                            Streams.AppendLine("</ObjectStreams>");

                            _Header.ContentType = _XML_UTF8;
                            content = Encoding.UTF8.GetBytes(Streams.ToString());

                        }

                        #endregion

                        #region text/plain

                        else if (_ContentType.MediaType == _TEXT.MediaType)
                        {

                            var Streams = new StringBuilder();

                            foreach (var _ObjectStreams in e.Value)
                                Streams.AppendLine(_ObjectStreams);

                            _Header.ContentType = _TEXT_UTF8;
                            content = Encoding.UTF8.GetBytes(Streams.ToString());

                        }

                        #endregion

                        #region JSON (default!)

                        else
                        {

                            var Streams = new StringBuilder();

                            Streams.AppendLine("{").AppendLine("\"  ObjectStreams\": [");

                            foreach (var _ObjectStreams in e.Value)
                                Streams.AppendLine("    \"" + _ObjectStreams + "\", ");

                            Streams.AppendLine("]").AppendLine("}");

                            _Header.ContentType = _JSON_UTF8;
                            content = Encoding.UTF8.GetBytes(Streams.ToString());

                        }

                        #endregion

                        _Header.HttpStatusCode = HTTPStatusCodes.OK;
                        _Header.CacheControl   = "no-cache";
                        _Header.ServerName     = _ServerID;
                        _Header.ContentLength  = content.ULongLength();

                        var HeaderBytes = _Header.ToBytes();
                        HTTPServer.HTTPContext.WriteToResponseStream(HeaderBytes, 0, HeaderBytes.Length);
                        HTTPServer.HTTPContext.WriteToResponseStream(content, 0, content.Length);

                    });

                #endregion

        }

        #endregion

        #region ListObjectEditions(myObjectType, myObjectName, myObjectStream)

        public void ListObjectEditions(String myObjectType, String myObjectName, String myObjectStream)
        {

            var _Body        = HTTPServer.HTTPContext.RequestBody;
            var _Header      = HTTPServer.HTTPContext.RequestHeader;
            var _ContentType = HTTPServer.HTTPContext.RequestHeader.GetBestMatchingAcceptHeader(_JSON, _XML, _TEXT);


            _IGraphFSSession.GetObjectEditions(new ObjectLocation(_IGraphDBSession.DatabaseRootPath, myObjectType, "Objects", myObjectName), myObjectStream).

                #region Failed!

                FailedAction<IEnumerable<String>>(e =>
                    {

                        _Header.HttpStatusCode = HTTPStatusCodes.NotFound;
                        _Header.CacheControl   = "no-cache";
                        _Header.ServerName     = _ServerID;
                        _Header.ContentLength  = 0;
                        _Header.ContentType    = new ContentType("text/plain");

                        var HeaderBytes = _Header.ToBytes();
                        HTTPServer.HTTPContext.WriteToResponseStream(HeaderBytes, 0, HeaderBytes.Length);
                        HTTPServer.HTTPContext.WriteToResponseStream(new Byte[0], 0, 0);

                    }).

                #endregion

                #region Success

                SuccessAction<IEnumerable<String>>(e =>
                    {

                        var    Editions = new StringBuilder();
                        Byte[] content  = null;

                        #region XML

                        if (_ContentType.MediaType == _XML.MediaType)
                        {

                            Editions.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>").
                                    AppendLine("<ObjectEditions>");

                            foreach (var _ObjectEditions in e.Value)
                                Editions.AppendLine("<ObjectEdition>" + _ObjectEditions + "</ObjectEdition>");

                            Editions.AppendLine("</ObjectEditions>");

                            _Header.ContentType = _XML_UTF8;
                            content = Encoding.UTF8.GetBytes(Editions.ToString());

                        }

                        #endregion

                        #region text/plain

                        else if (_ContentType.MediaType == _TEXT.MediaType)
                        {

                            foreach (var _ObjectEditions in e.Value)
                                Editions.AppendLine(_ObjectEditions);

                            _Header.ContentType = _TEXT_UTF8;
                            content = Encoding.UTF8.GetBytes(Editions.ToString());

                        }

                        #endregion

                        #region JSON (default!)

                        else
                        {

                            Editions.AppendLine("{").AppendLine("\"  ObjectStreams\": [");

                            foreach (var _ObjectEditions in e.Value)
                                Editions.AppendLine("    \"" + _ObjectEditions + "\", ");

                            Editions.AppendLine("]").AppendLine("}");

                            _Header.ContentType = _JSON_UTF8;
                            content = Encoding.UTF8.GetBytes(Editions.ToString());

                        }

                        #endregion

                        _Header.HttpStatusCode = HTTPStatusCodes.OK;
                        _Header.CacheControl   = "no-cache";
                        _Header.ServerName     = _ServerID;
                        _Header.ContentLength  = content.ULongLength();

                        var HeaderBytes = _Header.ToBytes();
                        HTTPServer.HTTPContext.WriteToResponseStream(HeaderBytes, 0, HeaderBytes.Length);
                        HTTPServer.HTTPContext.WriteToResponseStream(content, 0, content.Length);

                    });

                #endregion

        }

        #endregion

        #region ListObjectRevisions(myObjectType, myObjectName, myObjectStream, myObjectEdition)

        public void ListObjectRevisions(String myObjectType, String myObjectName, String myObjectStream, String myObjectEdition)
        {

            var _Body        = HTTPServer.HTTPContext.RequestBody;
            var _Header      = HTTPServer.HTTPContext.RequestHeader;
            var _ContentType = HTTPServer.HTTPContext.RequestHeader.GetBestMatchingAcceptHeader(_JSON, _XML, _TEXT);


            _IGraphFSSession.GetObjectRevisionIDs(new ObjectLocation(_IGraphDBSession.DatabaseRootPath, myObjectType, "Objects", myObjectName), myObjectStream, myObjectEdition).

                #region Failed!

                FailedAction<IEnumerable<ObjectRevisionID>>(e =>
                    {

                        _Header.HttpStatusCode = HTTPStatusCodes.NotFound;
                        _Header.CacheControl   = "no-cache";
                        _Header.ServerName     = _ServerID;
                        _Header.ContentLength  = 0;
                        _Header.ContentType    = new ContentType("text/plain");

                        var HeaderBytes = _Header.ToBytes();
                        HTTPServer.HTTPContext.WriteToResponseStream(HeaderBytes, 0, HeaderBytes.Length);
                        HTTPServer.HTTPContext.WriteToResponseStream(new Byte[0], 0, 0);

                    }).

                #endregion

                #region Success

                SuccessAction<IEnumerable<ObjectRevisionID>>(e =>
                    {

                        var    RevisionIDs = new StringBuilder();
                        Byte[] content     = null;

                        var _OL = _IGraphFSSession.GetObjectLocator(new ObjectLocation(_IGraphDBSession.DatabaseRootPath, myObjectType, "Objects", myObjectName)).Value;
                        var _OE = _OL[myObjectStream][myObjectEdition];

                        #region XML

                        if (_ContentType.MediaType == _XML.MediaType)
                        {

                            RevisionIDs.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>").
                                    AppendLine("<ObjectRevisions MinNumberOfRevisions=\"" + _OE.MinNumberOfRevisions + "\" MaxNumberOfRevisions=\"" + _OE.MaxNumberOfRevisions + "\">");

                            foreach (var _ObjectRevisionIDs in e.Value)
                                RevisionIDs.AppendLine("<ObjectRevisionID>" + _ObjectRevisionIDs + "</ObjectRevisionID>");

                            RevisionIDs.AppendLine("</ObjectRevisions>");

                            _Header.ContentType = _XML_UTF8;
                            content = Encoding.UTF8.GetBytes(RevisionIDs.ToString());

                        }

                        #endregion

                        #region text/plain

                        else if (_ContentType.MediaType == _TEXT.MediaType)
                        {

                            foreach (var _ObjectRevisionIDs in e.Value)
                                RevisionIDs.AppendLine(_ObjectRevisionIDs.ToString());

                            _Header.ContentType = _TEXT_UTF8;
                            content = Encoding.UTF8.GetBytes(RevisionIDs.ToString());

                        }

                        #endregion

                        #region JSON (default!)

                        else
                        {

                            RevisionIDs.AppendLine("{");

                            RevisionIDs.Append("  \"MinNumberOfRevisions\": \"").Append(_OE.MinNumberOfRevisions).AppendLine("\",");
                            RevisionIDs.Append("  \"MaxNumberOfRevisions\": \"").Append(_OE.MaxNumberOfRevisions).AppendLine("\",");

                            RevisionIDs.Append("  \"ObjectRevisionIDs\": [");

                            foreach (var _ObjectRevisionIDs in e.Value)
                                RevisionIDs.Append("\"").Append(_ObjectRevisionIDs).Append("\", ");

                            RevisionIDs.Length = RevisionIDs.Length - 2;

                            RevisionIDs.AppendLine("]").AppendLine("}");

                            _Header.ContentType = _JSON_UTF8;
                            content = Encoding.UTF8.GetBytes(RevisionIDs.ToString());

                        }

                        #endregion

                        _Header.HttpStatusCode = HTTPStatusCodes.OK;
                        _Header.CacheControl   = "no-cache";
                        _Header.ServerName     = _ServerID;
                        _Header.ContentLength  = content.ULongLength();

                        var HeaderBytes = _Header.ToBytes();
                        HTTPServer.HTTPContext.WriteToResponseStream(HeaderBytes, 0, HeaderBytes.Length);
                        HTTPServer.HTTPContext.WriteToResponseStream(content, 0, content.Length);

                    });

                #endregion

        }

        #endregion

        #endregion

        #region GetObject

        #region GetObject(myObjectType, myObjectName, myObjectStream, myObjectEdition, myObjectRevision)

        public void GetObject(String myObjectType, String myObjectName, String myObjectStream, String myObjectEdition, String myObjectRevision)
        {

            
            ObjectRevisionID _RevisionID = null;
            if (myObjectRevision != null)
                _RevisionID = new ObjectRevisionID(myObjectRevision);

            var _Body   = HTTPServer.HTTPContext.RequestBody;
            var _Header = HTTPServer.HTTPContext.RequestHeader;


            _IGraphFSSession.GetFSObject<FileObject>(
                                 new ObjectLocation(_IGraphDBSession.DatabaseRootPath, myObjectType, "Objects", myObjectName),
                                 myObjectStream, myObjectEdition, _RevisionID, 0, false).

                #region Failed!

                FailedAction<FileObject>(e =>
                    {
                        _Header.HttpStatusCode = HTTPStatusCodes.NotFound;
                        _Header.CacheControl   = "no-cache";
                        _Header.ServerName     = _ServerID;
                        _Header.ContentLength  = 0;
                        _Header.ContentType    = new ContentType("text/plain");

                        var HeaderBytes = _Header.ToBytes();
                        HTTPServer.HTTPContext.WriteToResponseStream(HeaderBytes, 0, HeaderBytes.Length);
                        HTTPServer.HTTPContext.WriteToResponseStream(new Byte[0], 0, 0);
                    }).

                #endregion

                #region Success

                SuccessAction<FileObject>(e =>
                    {
                        var content = e.Value.ObjectData;

                        _Header.HttpStatusCode = HTTPStatusCodes.OK;
                        _Header.CacheControl   = "no-cache";
                        _Header.ServerName     = _ServerID;
                        _Header.ContentLength  = content.ULongLength();
                        _Header.ContentType    = new ContentType(e.Value.ContentType);

                        var HeaderBytes = _Header.ToBytes();
                        HTTPServer.HTTPContext.WriteToResponseStream(HeaderBytes, 0, HeaderBytes.Length);
                        HTTPServer.HTTPContext.WriteToResponseStream(content, 0, content.Length);
                    });

                #endregion

        }

        #endregion

        #region GetObject(myObjectType, myObjectName, myObjectStream, myObjectEdition, myObjectRevision, myObjectCopy)

        public void GetObject(String myObjectType, String myObjectName, String myObjectStream, String myObjectEdition, String myObjectRevision, UInt64 myObjectCopy)
        {
        }

        #endregion

        #endregion

        #region DeleteObject

        #region DeleteObject(myObjectType, myObjectName, myObjectStream, myObjectEdition, myObjectRevision)

        public void DeleteObject(String myObjectType, String myObjectName, String myObjectStream, String myObjectEdition, String myObjectRevision)
        {

            #region Parse RevisionID

            ObjectRevisionID _RevisionID = null;

            try
            {
                _RevisionID = new ObjectRevisionID(myObjectRevision);
            }
            catch (Exception)
            {

                var _Header2 = HTTPServer.HTTPContext.RequestHeader;

                _Header2.HttpStatusCode  = HTTPStatusCodes.PreconditionFailed;
                _Header2.CacheControl    = "no-cache";
                _Header2.ServerName      = "Sones GraphDSREST Connector";
                _Header2.ContentLength   = 0;
                _Header2.ContentType     = new ContentType(MediaTypeNames.Application.Octet);

                var HeaderBytes = _Header2.ToBytes();
                HTTPServer.HTTPContext.WriteToResponseStream(HeaderBytes, 0, HeaderBytes.Length);

                return;

            }

            #endregion

            var _Body   = HTTPServer.HTTPContext.RequestBody;
            var _Header = HTTPServer.HTTPContext.RequestHeader;

            _IGraphFSSession.RemoveFSObject(
                                new ObjectLocation(_IGraphDBSession.DatabaseRootPath, myObjectType, "Objects", myObjectName),
                                myObjectStream, myObjectEdition, _RevisionID).

                #region Failed!

                FailedAction(e =>
                {
                    _Header.HttpStatusCode  = HTTPStatusCodes.Unknown;
                    _Header.CacheControl    = "no-cache";
                    _Header.ServerName      = _ServerID;
                    _Header.ContentLength   = 0;
                    _Header.ContentType     = new ContentType(MediaTypeNames.Application.Octet);

                    var HeaderBytes = _Header.ToBytes();
                    HTTPServer.HTTPContext.WriteToResponseStream(HeaderBytes, 0, HeaderBytes.Length);
                }).

                #endregion

                #region Success

                SuccessAction(e =>
                {
                    _Header.HttpStatusCode  = HTTPStatusCodes.NoContent;
                    _Header.CacheControl    = "no-cache";
                    _Header.ServerName      = _ServerID;
                    _Header.ContentLength   = 0;
                    _Header.ContentType     = new ContentType("text/plain");

                    var HeaderBytes = _Header.ToBytes();
                    HTTPServer.HTTPContext.WriteToResponseStream(HeaderBytes, 0, HeaderBytes.Length);
                });

                #endregion

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

            #region Data

            var _Assembly = Assembly.GetExecutingAssembly();
            var _Resources = _Assembly.GetManifestResourceNames();
            Stream _Content = null;

            if (myResource.Contains("/"))
                myResource = myResource.Replace('/', '.');

            #endregion

            #region Return assembly resource...

            var _ContentType = HTTPServer.HTTPContext.RequestHeader.GetBestMatchingAcceptHeader(_GEXF);
            var _Header = HTTPServer.HTTPContext.ResponseHeader;

            if (_Resources.Contains("GraphDSREST.resources." + myResource))
            {


                _Content = _Assembly.GetManifestResourceStream("GraphDSREST.resources." + myResource);

                #region Set the content type - sones REST

                if (HTTPServer.HTTPContext != null)
                {

                    var _FileNameSuffix = myResource.Remove(0, myResource.LastIndexOf(".") + 1);

                    switch (_FileNameSuffix)
                    {
                        case "htm": _Header.ContentType = _HTML_UTF8; break;
                        case "html": _Header.ContentType = _HTML_UTF8; break;
                        case "css": _Header.ContentType = _CSS; break;
                        case "gif": _Header.ContentType = _GIF; break;
                        case "ico": _Header.ContentType = _ICO; break;
                        case "swf": _Header.ContentType = new ContentType("application/x-shockwave-flash"); break;
                        case "js": _Header.ContentType = new ContentType("text/javascript"); break;
                        default: _Header.ContentType = _TEXT_UTF8; break;
                    }

                }

                #endregion

                _Header.HttpStatusCode = HTTPStatusCodes.OK;
                _Header.CacheControl = "no-cache";
                _Header.ServerName = _ServerID;
                _Header.ContentLength = (UInt64)_Content.Length;

                HTTPServer.HTTPContext.WriteToResponseStream(_Header.ToBytes());
                HTTPServer.HTTPContext.WriteToResponseStream(_Content);

                return;

            }

            #endregion

            #region ...or send custom Error 404 page!

            else
            {

                _Content = _Assembly.GetManifestResourceStream("GraphDSREST.resources.errorpages.Error404.html");

                if (_Content == null)
                    _Content = new MemoryStream(UTF8Encoding.UTF8.GetBytes("Error 404 - File not found!"));

                _Header.HttpStatusCode = HTTPStatusCodes.NotFound;
                _Header.CacheControl = "no-cache";
                _Header.ServerName = _ServerID;
                _Header.ContentType = _HTML;
                _Header.ContentLength = (UInt64)_Content.Length;

                HTTPServer.HTTPContext.WriteToResponseStream(_Header.ToBytes());
                HTTPServer.HTTPContext.WriteToResponseStream(_Content);

            }

            #endregion

        }

        #endregion

        #region GetHTMLLandingPage()

        public void GetHTMLLandingPage()
        {

            var _StopWatch = new Stopwatch();
            _StopWatch.Start();


                var _String = HTML_IO_Extensions.HTMLBuilder(_IGraphDBSession.DatabaseRootPath, _StringBuilder =>
                {

                    _StringBuilder.Append("<p><a href=\"/WebShell\">WebShell</a></p><br /><br />");

                    _IGraphFSSession.GetFilteredDirectoryListing(new ObjectLocation(_IGraphDBSession.DatabaseRootPath), null, null, null, new List<String>() {DBConstants.DBTYPESTREAM}, null, null, null, null ,null ,null).

                    #region FailedAction...

                        FailedAction<IEnumerable<String>>(e =>
                                {

                                    _StringBuilder.Append("ERROR");

                                }).

                    #endregion

                    #region SuccessAction...

                        SuccessAction<IEnumerable<String>>(e =>
                                {

                                    foreach (var _result in e.Value)
                                    {
                                        _StringBuilder.Append("<p><a href=\"/objects/").Append(_result).Append("/\">").Append(_result).Append("</a></p>");
                                    }
                                });

                    #endregion

                    _StringBuilder.Append("<b>Duration:</b>").Append(_StopWatch.ElapsedMilliseconds).Append(" ms<br />");

                });


                var content = Encoding.UTF8.GetBytes(_String);
                var _Header = HTTPServer.HTTPContext.ResponseHeader;

                _Header.HttpStatusCode  = HTTPStatusCodes.OK;
                _Header.CacheControl    = "no-cache";
                _Header.ServerName      = _ServerID;
                _Header.ContentLength   = content.ULongLength();
                _Header.ContentType     = _HTML_UTF8;

                var HeaderBytes = _Header.ToBytes();
                HTTPServer.HTTPContext.WriteToResponseStream(HeaderBytes, 0, HeaderBytes.Length);
                HTTPServer.HTTPContext.WriteToResponseStream(content, 0, content.Length);

                return;

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

            var _StringBuilder = new StringBuilder();

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
            return new MemoryStream(Encoding.UTF8.GetBytes(_StringBuilder.ToString())); ;

        }

        #endregion

        #region GetDDate()

        public Stream GetDDate()
        {
            if (WebOperationContext.Current != null)
            {
                WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain";
            }
            else if (HTTPServer.HTTPContext != null)
            {
                HTTPServer.HTTPContext.ResponseHeader.ContentType = new System.Net.Mime.ContentType("text/plain");
            }

            return new MemoryStream(Encoding.UTF8.GetBytes(new DiscordianDate().ToString()));
        }

        #endregion

        #endregion


        #region Private Helpers

        #region (private) BadRequest(myErrorMessage)

        private void Error400_BadRequest(String myErrorMessage)
        {

            var _Header  = HTTPServer.HTTPContext.ResponseHeader;
            var _Content = Encoding.UTF8.GetBytes("Error 400 - Bad Request : " + myErrorMessage);

            _Header.HttpStatusCode  = HTTPStatusCodes.BadRequest;
            _Header.CacheControl    = "no-cache";
            _Header.ServerName      = _ServerID;
            _Header.ContentLength   = _Content.ULongLength();
            _Header.ContentType     = _TEXT_UTF8;

            HTTPServer.HTTPContext.WriteToResponseStream(_Header.ToBytes());
            HTTPServer.HTTPContext.WriteToResponseStream(_Content);

        }

        #endregion

        #region (private) QueryFailed(myErrorMessage)

        private void Error400_QueryFailed(String myErrorMessage)
        {

            var _Header  = HTTPServer.HTTPContext.ResponseHeader;
            var _Content = Encoding.UTF8.GetBytes("Error 400 - Query Failed : " + myErrorMessage);

            _Header.HttpStatusCode  = HTTPStatusCodes.BadRequest;
            _Header.CacheControl    = "no-cache";
            _Header.ServerName      = _ServerID;
            _Header.ContentLength   = _Content.ULongLength();
            _Header.ContentType     = _TEXT_UTF8;

            HTTPServer.HTTPContext.WriteToResponseStream(_Header.ToBytes());
            HTTPServer.HTTPContext.WriteToResponseStream(_Content);

        }

        #endregion

        #region (private) Error404_NotFound(myStream)

        private void Error404_NotFound(Stream myCustom404Error)
        {

            var _Header = HTTPServer.HTTPContext.ResponseHeader;

            _Header.HttpStatusCode  = HTTPStatusCodes.NotFound;
            _Header.CacheControl    = "no-cache";
            _Header.ServerName      = _ServerID;

            #region Send custom Error404page...

            if (myCustom404Error != null && myCustom404Error.Length > 0)
            {
                _Header.ContentType    = _HTML;
                _Header.ContentLength  = (UInt64) myCustom404Error.Length;
            }

            #endregion

            #region ...or non-custom Error404page!

            else
            {
                _Header.ContentType     = _TEXT_UTF8;
                _Header.ContentLength   = 0;
            }

            #endregion

            HTTPServer.HTTPContext.WriteToResponseStream(_Header.ToBytes());
            HTTPServer.HTTPContext.WriteToResponseStream(myCustom404Error);

        }

        #endregion

        #region (private) checkAuthentication(mySettings)

        private Boolean checkAuthentication(GraphDSREST_Settings mySettings)
        {

            if (mySettings != null)
            {
                Debug.WriteLine("Currently not authentication mechanism was choosen!");
                return true;
            }

            else
                return false;

        }

        #endregion

        #region Print Error Messages internally

        private String PrintErrorToString(ResultType myResultType, IEnumerable<IError> myIErrors)
        {

            var Output = new StringBuilder();

            if (myResultType == ResultType.Successful)
            {
                Output.AppendLine("OK");
            }

            else
            {
                foreach (var aError in myIErrors)
                {
                    Output.AppendLine("ERROR!");
                    Output.AppendLine("Errorclass: " + aError.GetType().Name);
                    Output.AppendLine(aError.ToString());
                    Output.AppendLine(Environment.NewLine);
                }
                return Output.ToString();
            }

            return Output.ToString();
        }

        #endregion

        #endregion

    }

}
