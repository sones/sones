#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Networking.HTTP;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Net.Mime;
using System.Reflection;
using System.ServiceModel.Web;
using sones.GraphDS;
using sones.GraphQL.Result;
using sones.Library.LanguageExtensions;
using sones.Plugins.GraphDS.IOInterface.XML_IO;
using sones.Library.DiscordianDate;
#region DEBRIS
using sones.Plugins.GraphDS.IOInterface.XML_IO.Result;
#endregion



#endregion

namespace sones.Plugins.GraphDS.RESTService
{
    public class GraphDSREST_Output
    {
        #region Data

        private GraphDSREST_Errors  _ErrorMsg;
        private IGraphDS            _GraphDS;
        private String              _ServerID;

        #endregion

        #region Constructors

        public GraphDSREST_Output(IGraphDS myGraphDS, String myServerID)
        {
            _ServerID = myServerID;
            _ErrorMsg = new GraphDSREST_Errors(_ServerID);
            _GraphDS = myGraphDS;
        }

        #endregion

        #region private Helpers

        private void ExportContent(String myServerID, byte[] myContent, ContentType myContentType)
        {
            var _Header = HTTPServer.HTTPContext.ResponseHeader;

            _Header.HttpStatusCode  = HTTPStatusCodes.OK;
            _Header.CacheControl    = "no-cache";
            _Header.ServerName      = myServerID;
            _Header.ContentLength   = myContent.ULongLength();
            _Header.ContentType     = myContentType;

            var _HeaderBytes        = _Header.ToBytes();

            HTTPServer.HTTPContext.WriteToResponseStream(_HeaderBytes, 0, _HeaderBytes.Length);
            HTTPServer.HTTPContext.WriteToResponseStream(myContent);
        }        

        #endregion

        #region GenerateResultOutput(myResult, myQuery, myStopWatch)
        
        public void GenerateResultOutput(QueryResult myResult, Stopwatch myStopWatch)
        {   

            var _ContentType = HTTPServer.HTTPContext.RequestHeader.GetBestMatchingAcceptHeader(GraphDSREST_Constants._HTML, GraphDSREST_Constants._JSON, GraphDSREST_Constants._XML, GraphDSREST_Constants._GEXF, GraphDSREST_Constants._TEXT);
            
            #region application/xml

            if (_ContentType.MediaType == GraphDSREST_Constants._XML.MediaType)
            {

                var _XMLExport = new XML_IO();

                var content = _XMLExport.GenerateOutputResult(myResult);

                ExportContent(_ServerID, System.Text.Encoding.UTF8.GetBytes(content), _XMLExport.ContentType);

                return;

            }

            #endregion

            #region DEBRIS text/html

            if (_ContentType.MediaType == GraphDSREST_Constants._HTML.MediaType)
            {

                var _XMLExport = new XML_IO();

                var content = _XMLExport.GenerateOutputResult(myResult);

                ExportContent(_ServerID, System.Text.Encoding.UTF8.GetBytes(content), _XMLExport.ContentType);

                return;

            }

            #endregion
        }

        #endregion


        #region GetGQL()

        public String GetGQL()
        {
            if (HTTPServer.HTTPContext.RequestHeader.QueryString == null)
            {
                _ErrorMsg.Error400_BadRequest("[Syntax Error] Please use '...gql?query'!");
                return String.Empty;
            }

            var _QueryString = HTTPServer.HTTPContext.RequestHeader.QueryString[null];
            if (_QueryString == null)
            {
                _ErrorMsg.Error400_BadRequest("[Syntax Error] Please use '...gql?query'!");
                return String.Empty;
            }

            var _GQLQuery = HttpUtility.UrlDecode(_QueryString);

            return _GQLQuery;
        }        

        #endregion

        #region ExecuteGQLQuery(myGQLQuery)

        /// <summary>
        /// Invoke a gql query to the underlying database.
        /// </summary>
        /// <example>/gql?FROM+Website+w+SELECT+*+WHERE+w.Name+%3d+%27xkcd%27"</example>
        /// <returns>The result of the GQL query</returns>
        /*public QueryResult ExecuteGQL(String myGQLQuery)
        {
            QueryResult _QueryResult = null;
            
            try
            {
                #region Check settings
                
                if (1 > 1)
                { }

                #endregion

                else
                {
                    var _StopWatch = new Stopwatch();

                    /// this is a workaround to implement EXECDBSCRIPT into the webshell
                    /// if anything entered as a GQL command starts with "EXECDBSCRIPT" we'll take it from here.
                    if (myGQLQuery.ToUpper().StartsWith("EXECDBSCRIPT "))
                    {

                        #region EXECDBSCRIP HACK

                        // get the script from the path...
                        if (!File.Exists(".\\scripts\\" + myGQLQuery.Remove(0, 13)))
                        {
                            _StopWatch.Start();
                            // output error 

                            _QueryResult = new QueryResult(new Error_FileNotFound(".\\scripts\\" + myGQLQuery.Remove(0, 13)));
                            _QueryResult.Query = myGQLQuery;
                            _StopWatch.Stop();
                        }

                        else
                        {

                            _StopWatch.Start();

                            var QueryResults = new List<QueryResult>();

                            using (var streamReader = new StreamReader(".\\scripts\\" + myGQLQuery.Remove(0, 13)))
                            {
                                String line;
                                while ((line = streamReader.ReadLine()) != null)
                                {
                                    ConsoleOutputLogger.WriteLine(line);
                                    QueryResult newResult = _GraphDS.Query(line);
                                    QueryResults.Add(newResult);
                                    String Logging2 = _ErrorMsg.PrintErrorToString(newResult.ResultType, newResult.Errors);
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

                        _QueryResult.PushIWarning(new Warning_ObsoleteGQL("EXECDBSCRIPT", "IMPORT FROM '<file or http ressource>' FORMAT GQL"));

                    }
                    else
                    {   
                    _QueryResult = _GraphDS.Query(null, null, myGQLQuery, "");
                    }

                    GenerateResultOutput(_QueryResult, new Stopwatch());

                    return _QueryResult;
                }
            }
            catch (Exception ex)
            {
                _ErrorMsg.Error400_BadRequest(ex.Message + ex.StackTrace);
            }

            return _QueryResult;
        }*/

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

            #region DEBRIS to be deleted
            var _StopWatch = new Stopwatch();
            _StopWatch.Start();
            _QueryResult = GenerateQueryResult();
            GenerateResultOutput(_QueryResult, _StopWatch);
            #endregion

            #region to be used
            //
            
            //try
            //{
            //    var _StopWatch = new Stopwatch();

            //    _StopWatch.Start();
            //    _QueryResult = _GraphDS.Query(null, null, myQuery, "");
            //    _StopWatch.Stop();
                
            //    GenerateResultOutput(_QueryResult, _StopWatch);

            //    return _QueryResult;
            
            //}
            //catch (Exception ex)
            //{
            //    _ErrorMsg.Error400_BadRequest(ex.Message + ex.StackTrace);
            //}
            #endregion
            

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

            var _Assembly   = Assembly.GetExecutingAssembly();
            var _Resources  = _Assembly.GetManifestResourceNames();
            Stream _Content = null;

            if (myResource.Contains("/"))
                myResource = myResource.Replace('/', '.');

            #endregion

            #region Return assembly resource...

            var _ContentType = HTTPServer.HTTPContext.RequestHeader.GetBestMatchingAcceptHeader(GraphDSREST_Constants._GEXF);
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
                        case "htm": _Header.ContentType = GraphDSREST_Constants._HTML_UTF8; break;
                        case "html": _Header.ContentType = GraphDSREST_Constants._HTML_UTF8; break;
                        case "css": _Header.ContentType = GraphDSREST_Constants._CSS; break;
                        case "gif": _Header.ContentType = GraphDSREST_Constants._GIF; break;
                        case "ico": _Header.ContentType = GraphDSREST_Constants._ICO; break;
                        case "swf": _Header.ContentType = new ContentType("application/x-shockwave-flash"); break;
                        case "js": _Header.ContentType = new ContentType("text/javascript"); break;
                        default: _Header.ContentType = GraphDSREST_Constants._TEXT_UTF8; break;
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
                _Header.ContentType = GraphDSREST_Constants._HTML;
                _Header.ContentLength = (UInt64)_Content.Length;

                HTTPServer.HTTPContext.WriteToResponseStream(_Header.ToBytes());
                HTTPServer.HTTPContext.WriteToResponseStream(_Content);

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
            else if (HTTPServer.HTTPContext != null)
            {
                HTTPServer.HTTPContext.ResponseHeader.ContentType = new System.Net.Mime.ContentType("text/plain");
            }

            return new MemoryStream(Encoding.UTF8.GetBytes(new DiscordianDate().ToString()));
        }

        #endregion


        #region DEBRIS test method to be deleted
        private static QueryResult GenerateQueryResult()
        {
            var propertyLeelaList = new Dictionary<String, Object>();
            var someBytes = new byte[12];
            var random = new Random();
            random.NextBytes(someBytes);

            propertyLeelaList.Add("Name", "Leela");
            propertyLeelaList.Add("Age", 27);
            propertyLeelaList.Add("Picture", new MemoryStream(someBytes));
            propertyLeelaList.Add("Address", "@home");
            propertyLeelaList.Add("VertexID", 121121);

            var friendsList = new Dictionary<String, IEdgeView>();

            var vertexLeela = new VertexView(propertyLeelaList, friendsList);

            var propertyFreyList = new Dictionary<String, Object>();

            propertyFreyList.Add("Name", "Frey");
            propertyFreyList.Add("Age", 26);
            propertyFreyList.Add("Address", "blue planet");

            var propertyBenderList = new Dictionary<String, Object>();

            propertyBenderList.Add("Name", "Bender");
            propertyBenderList.Add("Age", 23);
            propertyBenderList.Add("Address", "red planet");

            var vertexFrey = new VertexView(propertyFreyList, new Dictionary<string, IEdgeView>());
            var vertexBender = new VertexView(propertyBenderList, new Dictionary<string, IEdgeView>());

            var edgeProp = new Dictionary<String, Object>();
            edgeProp.Add("Weight", 34);

            var edgeViewFriends = new EdgeView(edgeProp, new List<IVertexView>() { vertexFrey, vertexBender });

            var propertyListZoidBerg = new Dictionary<String, Object>();
            propertyListZoidBerg.Add("Name", "Zoidberg");

            var vertexZoidberg = new VertexView(propertyListZoidBerg, new Dictionary<string, IEdgeView>());
            //var vertexNothing  = new VertexView(propertyListZoidBerg, new Dictionary<string, IEdgeView>());

            var edgeViewEnemys = new EdgeView(new Dictionary<string, object>(), new List<IVertexView>() { vertexZoidberg });

            friendsList.Add("Friends", edgeViewFriends);
            friendsList.Add("Enemys", edgeViewEnemys);


            var retVal = new QueryResult("From User select *", "GraphQL", 20,
                                         new List<IVertexView>() { vertexBender, vertexFrey, vertexLeela });

            return retVal;
        }
        #endregion
        

    }

}

