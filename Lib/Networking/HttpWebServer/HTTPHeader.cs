/*
* sones GraphDB - Open Source Edition - http://www.sones.com
* Copyright (C) 2007-2010 sones GmbH
*
* This file is part of sones GraphDB Open Source Edition (OSE).
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
* 
*/

/* <id name="Networking – HttpHeader" />
 * <copyright file="HttpHandler.cs"
 *            company="sones GmbH">
 * Copyright (c) sones GmbH. All rights reserved.
 * </copyright>
 * <developer>Stefan Licht</developer>
 * <summary>A http header</summary>
 */

#region Usings

using System;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using sones.Lib;

#endregion

namespace sones.Networking.HTTP
{

    public class HTTPHeader
    {

        #region Data

        private readonly Char[] _SlashSeperator = new char[] { '/' };
        public List<AcceptType> AcceptTypes     { get; set; }
        public ClientTypes      ClientType      { get; set; }
        public Encoding         ContentEncoding { get; set; }
        public String           AcceptEncoding  { get; set; }

        private UInt64 _ContentLength;
        public UInt64 ContentLength
        {
            get { return _ContentLength; }
            set { _ContentLength = value; }
        }


        public ContentType ContentType { get; set; }


        /// <summary>
        /// All undefined header entries
        /// </summary>
        public NameValueCollection Headers { get; set; }
        public String HostName { get; set; }

        /// <summary>
        /// The requested HTTPMethod
        /// </summary>
        public HTTPMethods HttpMethod { get; set; }

        /// <summary>
        /// The requested HTTPMethod String
        /// </summary>
        public String HttpMethodString { get; set; }

        public Stream InputStream { get; set; }
        public String Protocol { get; set; }
        public String ProtocolName { get; set; }
        public Version ProtocolVersion { get; set; }
        public NameValueCollection QueryString { get; set; }
        public String RawUrl { get; set; }
        public Boolean KeepAlive { get; set; }

        /// <summary>
        /// The destination folder/myPath of the request
        /// </summary>
        public String Destination { get; set; }

        public HTTPStatusCodes HttpStatusCode { get; set; }
        public String PlainHeader { get; set; }

        public String SVNParameters { get; set; }

        public String CacheControl { get; set; }
        public String ServerName { get; set; }

        public Boolean IsSVNClient
        {
            get
            {
                return ClientType == ClientTypes.SVN;
            }
        }

        /// <summary>
        /// Authorize the request based on the passed <paramref name="myHTTPCredentials"/>
        /// </summary>
        public HTTPCredentials Authorization { get; set; }

        #endregion

        #region Ctor

        public HTTPHeader()
        {
            Headers = new NameValueCollection();
            ContentType = new System.Net.Mime.ContentType(MediaTypeNames.Text.Html);
            CacheControl = "no-cache";
            _ContentLength = 0;
        }

        public HTTPHeader(String myHeader) : this()
        {

            PlainHeader     = myHeader;
            HttpStatusCode  = HTTPStatusCodes.OK;
            KeepAlive       = false;
            String[] ProtocolArray;

            // Split the Request into lines
            String[] RequestParts = myHeader.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            if (RequestParts.Length == 0)
            {
                HttpStatusCode = HTTPStatusCodes.BadRequest;
                return;
            }
                
            #region Header first line

            // The first line containing the information about the HTTPMethod, the current folder/myPath (destination) and the protocol
            // e.g: PROPFIND /file/file Name HTTP/1.1
            String[] HTTPMethodHeader = RequestParts[0].Split(new char[] { ' ' });

            if (HTTPMethodHeader.Length < 3)
            {
                HttpStatusCode = HTTPStatusCodes.BadRequest;
                return;
            }

            // This is a response
            if (HTTPMethodHeader[0].StartsWith("HTTP"))
            {                
                Protocol        = HTTPMethodHeader[0];
                ProtocolArray   = Protocol.Split(_SlashSeperator);
                ProtocolName    = ProtocolArray[0].ToLower();
                ProtocolVersion = new Version(ProtocolArray[1]);
                HttpStatusCode  = (HTTPStatusCodes) Enum.Parse(typeof(HTTPStatusCodes), HTTPMethodHeader[1]);
            }

            // This is a request
            else
            {

                if (!Enum.IsDefined(typeof(HTTPMethods), HTTPMethodHeader[0]))
                {
                    //HttpStatusCode = HTTPStatusCodes.NotImplemented;
                    HttpMethod       = HTTPMethods.UNKNOWN;
                    HttpMethodString = HTTPMethodHeader[0];
                    //return;
                    //throw new Exception("Invalid HTTP Method " + HTTPMethodHeader[0]);
                }
                else
                {
                    HttpMethod       = (HTTPMethods) Enum.Parse(typeof(HTTPMethods), HTTPMethodHeader[0]);
                    HttpMethodString = HTTPMethodHeader[0];
                }

                // Decode UTF-8 Hex encoding "%C3%B6" -> "ö" etc...
                Destination = RawUrl = HTTPMethodHeader[1];// HttpUtility.UrlDecode(HTTPMethodHeader[1]);
                //if (Destination.Length > 1 && Destination.EndsWith("/") && !Destination.Contains("/?"))
                //{
                //    RawUrl = HttpUtility.UrlDecode(HTTPMethodHeader[1]);//.TrimEnd(((String)"/").ToCharArray());
                //    if (RawUrl[RawUrl.Length-1] == '/')
                //    {
                //        RawUrl = RawUrl.Substring(0, RawUrl.Length - 1);
                //    }
                //    Destination = RawUrl;
                //}

                // Parse QueryString after '?' and maybe fix the Destination
                var _Questionmark = RawUrl.IndexOf('?');
                if (_Questionmark > -1)
                {
                    Destination = RawUrl.Substring(0, _Questionmark);
                    QueryString = HttpUtility.ParseQueryString(RawUrl.Substring(_Questionmark + 1));
                }

                //!svn/vcc/default
                var _Exclamationmark = RawUrl.IndexOf('!');
                if (_Exclamationmark > -1)
                {
                    SVNParameters = RawUrl.Substring(_Exclamationmark + 1);
                    Destination   = RawUrl.Substring(0, _Exclamationmark - 1);
                    if (Destination == "") Destination = "/";
                }

                Protocol        = HTTPMethodHeader[2];
                ProtocolArray   = Protocol.Split(_SlashSeperator);
                ProtocolName    = ProtocolArray[0].ToLower();
                ProtocolVersion = new Version(ProtocolArray[1]);

                RawUrl = HTTPMethodHeader[1];

            }


            #endregion

            if (ProtocolVersion >= new Version(1, 1))
                KeepAlive = true;

            #region Parse all other Header informations

            Headers     = new NameValueCollection();
            AcceptTypes = new List<AcceptType>();

            for (Int16 i = 1; i < RequestParts.Length; i++)
            {

                // The Header is finished
                if (RequestParts[i] == String.Empty)
                    break;

                // Should never happen, however
                if (!RequestParts[i].Contains(':'))
                    continue;

                String Key = RequestParts[i].Substring(0, RequestParts[i].IndexOf(':'));
                String Value = RequestParts[i].Substring(Key.Length + 1).Trim();

                if (Key.ToLower() == "accept")
                {
                    if (Value.Contains(","))
                    {
                        UInt32 place = 0;
                        foreach (var acc in Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            AcceptTypes.Add(new AcceptType(acc.Trim(), place++));
                        }
                    }
                    else
                    {
                        AcceptTypes.Add(new AcceptType(Value.Trim()));
                    }
                }
                else if (Key.ToLower() == "content-length")
                    UInt64.TryParse(Value, out _ContentLength);
                else if (Key.ToLower() == "content-type")
                    ContentType = new System.Net.Mime.ContentType(Value);
                else if (Key.ToLower() == "host")
                    HostName = Value;
                else if (Key.ToLower() == "accept-encoding")
                    AcceptEncoding = Value;
                /*
                    try
                    {
                        ContentEncoding = Encoding.GetEncoding(Value);
                    }
                    catch
                    {
                        ContentEncoding = Encoding.UTF8;
                    }
                 * */
                else if (Key.ToLower() == "user-agent")
                {
                    if (Value.Contains("Microsoft-WebDAV-MiniRedir"))
                        ClientType = ClientTypes.MicrosoftWebDAVMiniRedir;
                    else if (Value.Contains("MSIE 7"))
                        ClientType = ClientTypes.MSInternetExplorer7;
                    else if (Value.Contains("MSIE 8"))
                        ClientType = ClientTypes.MSInternetExplorer8;
                    else if (Value.Contains("Firefox"))
                        ClientType = ClientTypes.Firefox;
                    else if (Value.ToLower().Contains("svn"))
                        ClientType = ClientTypes.SVN;
                    else
                        ClientType = ClientTypes.Other;
                }
                else if (Key.ToLower() == "connection" && Value.ToLower().Contains("keep-alive"))
                {
                    KeepAlive = true;
                }
                else if (Key.ToLower() == "authorization")
                {
                    try
                    {
                        Authorization = new HTTPCredentials(Value);
                    }
                    catch (Exception)
                    {
                        //NLOG: temporarily commented
                        //_Logger.ErrorException("could not parse authorization of HTTPHeader", ex);
                    }
                }
                else
                    Headers.Add(Key, Value);
                
            }

            AcceptTypes.Sort();
            //AcceptTypes.Sort(new Comparison<AcceptType>((at1, at2) =>
            //{
            //    if (at1.Quality > at2.Quality) return 1;
            //    else if (at1.Quality == at2.Quality) return 0;
            //    else return -1;
            //    //if (at2.Quality > at1.Quality) return -1;
            //    //else return 1;
            //    //return at1.Quality.CompareTo(at2.Quality);
            //}
            //    ));

            #endregion

        }

        #endregion

        #region ParseHttpStatus

        /// <summary>
        /// Parses a status from a status line <paramref name="Input"/>
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public HTTPStatusCodes ParseHttpStatus(String Input)
        {
            HTTPStatusCodes Result = HTTPStatusCodes.Unknown;

            String[] HTTPMethodHeader = Input.Split(new char[] { ' ' });

            if (HTTPMethodHeader.Length < 3)
            {
                return Result;
            }

            if (Input.StartsWith("HTTP"))
            {
                Result = (HTTPStatusCodes)Enum.Parse(typeof(HTTPStatusCodes), HTTPMethodHeader[1]);
            }
            return Result;
        }

        #endregion

        #region Create a response header

        public Byte[] ToBytes()
        {
            return Encoding.UTF8.GetBytes(ToString());
        }

        public override String ToString()
        {
            var result = new StringBuilder();
            result.AppendLine(HttpStatusCodeToString(HttpStatusCode));

            AddToResponseString(result, "Cache-Control", CacheControl);
            AddToResponseString(result, "Content-length", ContentLength.ToString());
            AddToResponseString(result, "content-type", ContentType.ToString());
            AddToResponseString(result, "Date", DateTime.Now.ToString());
            AddToResponseString(result, "Server", ServerName);
            if (KeepAlive)
            {
                AddToResponseString(result, "Connection", "Keep-Alive");
            }

            foreach (var key in Headers.AllKeys)
            {
                AddToResponseString(result, key, Headers[key]);
            }

            result.AppendLine();

            return result.ToString();
        }

        private void AddToResponseString(StringBuilder stringBuilder, String key, String value)
        {
            if (!String.IsNullOrWhiteSpace(key))
            {
                stringBuilder.AppendLine(key + ": " + value);
            }
        }

        #endregion

        #region HttpStatusCodeToString

        /// <summary>
        /// Create a status code line from a HTTPStatusCode
        /// </summary>
        public static String HttpStatusCodeToString(HTTPStatusCodes myHTTPStatusCode)
        {

            switch (myHTTPStatusCode)
            {

                case HTTPStatusCodes.Unauthorized:
                    return "HTTP/1.1 401 " + HttpStatusCodeToSimpleString(myHTTPStatusCode) + Environment.NewLine + "WWW-Authenticate: Basic realm=\"Intern\"";

                default:
                    return "HTTP/1.1 " + ((Int32) myHTTPStatusCode).ToString() + " " + HttpStatusCodeToSimpleString(myHTTPStatusCode);

            }

        }
            
        #endregion

        #region HttpStatusCodeToSimpleString

        /// <summary>
        /// Create a simple status code line from a HTTPStatusCode
        /// </summary>
        public static String HttpStatusCodeToSimpleString(HTTPStatusCodes myHTTPStatusCode)
        {

            switch (myHTTPStatusCode)
            {

                case HTTPStatusCodes.OK:
                    return "OK";
                case HTTPStatusCodes.Created:
                    return "Created";
                case HTTPStatusCodes.MultiStatus:
                    return "Multi-Status";
                case HTTPStatusCodes.BadRequest:
                    return "Bad Request";
                case HTTPStatusCodes.Unauthorized:
                    return "Authorization Required";
                case HTTPStatusCodes.NotFound:
                    return "Not Found";
                case HTTPStatusCodes.PreconditionFailed:
                    return "Precondition Failed";
                case HTTPStatusCodes.RequestUriTooLong:
                    return "Request URI Too Long";
                case HTTPStatusCodes.UnprocessableEntity:
                    return "Unprocessable Entity";
                case HTTPStatusCodes.Locked:
                    return "Locked";
                case HTTPStatusCodes.FailedDependency:
                    return "Failed Dependency";
                case HTTPStatusCodes.NotImplemented:
                    return "Not Implemented";
                case HTTPStatusCodes.InsufficientStorage:
                    return "Insufficient Storage";

                default:
                    return ((Int32) myHTTPStatusCode).ToString();

            }

        }

        #endregion

        /// <summary>
        /// Will return the best matching content type OR the first given!
        /// </summary>
        /// <param name="myContentTypes"></param>
        /// <returns></returns>
        public ContentType GetBestMatchingAcceptHeader(params ContentType[] myContentTypes)
        {

            var _ListOfFoundAcceptHeaders = new List<AcceptType>();
            UInt32 pos = 0;

            foreach (var _ContentType in myContentTypes)
            {
                
                var _AcceptType = new AcceptType(_ContentType.ToString(), pos++);

                var _Match = AcceptTypes.Find(_AType => _AType.Equals(_AcceptType));
                
                if (_Match != null)
                {

                    if (_Match.ContentType.GetMediaSubType() == "*") // this was a * and we will set the quality to lowest
                        _AcceptType.Quality = 0;
                    
                    _ListOfFoundAcceptHeaders.Add(_AcceptType); 

                }

            }

            _ListOfFoundAcceptHeaders.Sort();

            if (!_ListOfFoundAcceptHeaders.IsNullOrEmpty())
                return _ListOfFoundAcceptHeaders.First().ContentType;
            else if (!myContentTypes.IsNullOrEmpty())
                return myContentTypes.First();
            else
                return null;

        }

    }
}
