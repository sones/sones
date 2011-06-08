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

using System;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Diagnostics;
using System.IO;

namespace sones.Library.Network.HttpServer
{
    /// <summary>
    /// An instance of this class maps urls to methods.
    /// </summary>
    public class HttpServer:
        IDisposable
    {
        /// <summary>
        /// A thread static variable, that stores the current http context.
        /// </summary>
        [ThreadStatic]
        public static HttpListenerContext HttpContext;

        #region Data

        private readonly Object _lock = new object();

        /// <summary>
        /// Stores the current server logic object.
        /// </summary>
        private readonly object _logic;
        
        /// <summary>
        /// Stores the http listener.
        /// </summary>
        private HttpListener _listener;

        /// <summary>
        /// Stores a thread that polls the http listener.
        /// </summary>
        private Thread _serverThread;

        /// <summary>
        /// Stores an instance of an url parser.
        /// </summary>
        private readonly UrlParser _parser;

        /// <summary>
        /// Stores the security validation algorithm.
        /// </summary>
        private readonly IServerSecurity _security;

        #endregion

        /// <summary>
        /// This is the time span this server waits for new request. After this time it checks if it was stopped. If not it waits again.
        /// </summary>
        /// <remarks>If Stop is called it it takes at most AsyncWaitTime plus working time of a request that was received during the AsyncWaitTime.</remarks>
        public const short AsyncWaitTime = 1000;

        #region c'tor

        /// <summary>
        /// Creates a new instance of HttpServer.
        /// </summary>
        /// <param name="myIPAddress">The IP address this server will listen for new connections.</param>
        /// <param name="myPort">The port this server will listen for new connections.</param>
        /// <param name="myServerLogic">An instance of the server logic. The class of this instance must implement at least one interface that is decorated with a ServiceContractAttribute.</param>
        /// <param name="mySecurity"></param>
        /// <param name="mySecureConnection">If true, this server will listen for request with HTTPS protocol, otherwise HTTP.</param>
        /// <param name="myAutoStart">If true, the method Start is called implicitly, otherwise not.</param>
        /// <exception cref="ArgumentNullException">If myIPAddress is <c>NULL</c>.</exception>
        /// <exception cref="ArgumentNullException">If myServerLogic is <c>NULL</c>.</exception>
        public HttpServer(IPAddress myIPAddress, ushort myPort, object myServerLogic, IServerSecurity mySecurity = null, bool mySecureConnection = false, bool myAutoStart = false)
        {
            #region argument checks

            if (myIPAddress == null)
                throw new ArgumentNullException("myIPAddress");

            if (myServerLogic == null)
                throw new ArgumentNullException("myServerLogic");

            #endregion

            #region set data

            ListeningAddress = myIPAddress;
            ListeningPort = myPort;
            IsSecureConnection = mySecureConnection;
            IsRunning = false;

            _logic = myServerLogic;
            _security = mySecurity;

            _parser = ParseInterface();

            #endregion

            CreateListener(); //create listener as late as possible

            if (myAutoStart)
                Start();
        }

        #endregion

        #region public properties

        /// <summary>
        /// The IP address this server is listening for new connections.
        /// </summary>
        public IPAddress ListeningAddress { get; private set; }

        /// <summary>
        /// The port this server is listening for new connections.
        /// </summary>
        public ushort ListeningPort { get; private set; }

        /// <summary>
        /// Gets whether this server is listening for requests on HTTPS protocol or on HTTP protocol.
        /// </summary>
        public bool IsSecureConnection { get; private set; }

        /// <summary>
        /// Gets whether this server is listening for requests.
        /// </summary>
        public bool IsRunning { get; private set; }


        #endregion

        #region Start/Stop

        /// <summary>
        /// Starts this server.
        /// </summary>
        /// <remarks>After the call of this method, the server will listening for new connections.</remarks>
        public void Start()
        {
            lock (_lock)
            {
                if (IsRunning)
                    return;

                _listener.Start();
                _serverThread = new Thread(DoListen);

                IsRunning = true;
                _serverThread.Start();
            }
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        /// <remarks>
        /// The call of this method stops the listening process. 
        /// Connections that were accepted before this call are processed normally. 
        /// The call of this method waits until all connections are handled.
        /// </remarks>
        public void Stop()
        {
            lock (_lock)
            {
                IsRunning = false;
                if (_serverThread != null)
                {
                    _serverThread.Join();
                    _serverThread = null;
                }
                _listener.Stop();
            }
        }

        /// <summary>
        /// Closes the server.
        /// </summary>
        /// <remarks>
        /// A call of this method releases all resources. After this, the instance can not be started anymore.
        /// </remarks>
        public void Close()
        {
            Stop();
            if (_listener != null)
            {
                _listener.Close();
            }
        }

        #endregion

        #region private members

        /// <summary>
        /// Creates the instance of the http listener reagarding the ListeningAddress, ListeningPort.
        /// </summary>
        /// <exception cref="ArgumentException">If ListeningAddress is a 'none' ip address.</exception>
        /// <exception cref="ArgumentException">If ListeningAddress is a 'broadcast' ip address.</exception>
        private void CreateListener()
        {
            if (IPAddress.None.Equals(ListeningAddress) || IPAddress.IPv6None.Equals(ListeningAddress))
                throw new ArgumentException("It is not allowed to bind this server to an ip address 'none'.");

            if (IPAddress.Broadcast.Equals(ListeningAddress))
                throw new ArgumentException("It is not allowed to bind this server to an broadcast address.");

            var protocol = (IsSecureConnection) ? "https://" : "http://";

            var prefix = (IPAddress.IPv6Any.Equals(ListeningAddress) || IPAddress.Any.Equals(ListeningAddress))
                              ? protocol + "*:" + ListeningPort + "/"
                              : protocol + ListeningAddress + ":" + ListeningPort + "/";
            
            _listener = new HttpListener();
            _listener.Prefixes.Add(prefix);
            _listener.AuthenticationSchemeSelectorDelegate = new AuthenticationSchemeSelector(SchemaSelector);
        }

        /// <summary>
        /// Parses the class of _logic for interesting attributes.
        /// </summary>
        /// <remarks>
        /// Searches for interesting attributes in the inheritance hierarchy of the class of the _logic. 
        /// After that it parses the attributes and builds a connection from the url pattern to the implementation method.
        /// </remarks>
        private UrlParser ParseInterface()
        {
            var result = new UrlParser(new[] { '/' });

            #region Find the correct interfaces

            var allInterfaces =  from interfaceInst
                                 in _logic.GetType().GetInterfaces()
                                 where interfaceInst.GetCustomAttributes(typeof (ServiceContractAttribute), false).Count() > 0
                                 select interfaceInst;

            if (allInterfaces.Count() == 0)
                throw new ArgumentException("Could not find any valid interface having the ServiceContract attribute!");

            #endregion

            foreach (var currentInterfaces in allInterfaces)
            {
                #region Check global Force-/NoAuthenticationAttribute

                var globalNeedsExplicitAuthentication = false;

                if (currentInterfaces.GetCustomAttributes(typeof (ForceAuthenticationAttribute), false).Count() > 0)
                    globalNeedsExplicitAuthentication = true;

                #endregion


                foreach (var method in currentInterfaces.GetMethods())
                {

                    var needsExplicitAuthentication = globalNeedsExplicitAuthentication;

                    var attributes = method.GetCustomAttributes(false);

                    #region Authentication

                    if (attributes.Any(_ => _.GetType() == typeof(NoAuthenticationAttribute)))
                        needsExplicitAuthentication = false;

                    if (attributes.Any(_ => _.GetType() == typeof(ForceAuthenticationAttribute)))
                        needsExplicitAuthentication = true;

                    #endregion

                    foreach (var attribute in attributes)
                    {
                        String aURIPattern = null;
                        String webMethod = null;

                        #region WebGet

                        var webAttribute = attribute as WebGetAttribute;
                        if (webAttribute != null)
                        {
                            aURIPattern = webAttribute.UriTemplate.ToLower();
                            webMethod = "GET";
                        }

                        #endregion

                        #region WebInvoke

                        var webInvokeAttribute = attribute as WebInvokeAttribute;
                        if (webInvokeAttribute != null)
                        {
                            aURIPattern = webInvokeAttribute.UriTemplate.ToLower();
                            webMethod = webInvokeAttribute.Method;
                        }

                        #endregion

                        if (aURIPattern != null)
                            result.AddUrl(aURIPattern, method, needsExplicitAuthentication, webMethod);

                    }

                }
            }

            return result;
        }

        /// <summary>
        /// The method the sever thread executes.
        /// </summary>
        private void DoListen()
        {
            while (IsRunning)
            {
                var asyncCall =_listener.BeginGetContext(AsyncGetContext, _listener);
                asyncCall.AsyncWaitHandle.WaitOne(AsyncWaitTime); 
            }
        }



        private AuthenticationSchemes SchemaSelector(HttpListenerRequest myRequest)
        {
            var callback = _parser.GetCallback(myRequest.RawUrl, myRequest.HttpMethod);
            
            return (callback != null && callback.Item1 != null && callback.Item1.NeedsExplicitAuthentication)
                ? _security.SchemaSelector(myRequest) 
                : AuthenticationSchemes.Anonymous;
        }


        /// <summary>
        /// The entire method that is called if a connection was established.
        /// </summary>
        /// <param name="myResult">The IAsyncResult object that was created be HttpListener.BeginGetContext.</param>
        private void AsyncGetContext(IAsyncResult myResult)
        {
            if (!_listener.IsListening)
                return;

            var context = _listener.EndGetContext(myResult);

            HttpContext = context;

            //gets the method that will be invoked
            var callback = _parser.GetCallback(context.Request.RawUrl, context.Request.HttpMethod);

            try
            {
                if (callback == null || callback.Item1 == null)
                {
                    NoPatternFound404(context);

                }
                else
                {
                    #region Check authentification

                    try
                    {
                        if (callback.Item1.NeedsExplicitAuthentication)
                        {
                            _security.Authentificate(context.User.Identity);
                        }
                    }
                    catch (Exception)
                    {
                        context.Response.StatusCode = (int) HttpStatusCode.Forbidden;
                        return;
                    }

                    #endregion

                    try
                    {
                        var method = callback.Item2 == null ? null : callback.Item2.ToArray();

                        var targetInvocationResult = callback.Item1.Callback.Invoke(_logic, method);

                        if (context.Response.ContentLength64 == 0)
                        {
                            // The user did not write into the stream itself - we will add header and the invocation result

                            #region Get invocation result and create header and body

                            if (targetInvocationResult is Stream)
                            {
                                var result = targetInvocationResult as Stream;
                                result.Seek(0, SeekOrigin.Begin);
                                result.CopyTo(context.Response.OutputStream);

                            }
                            else if (targetInvocationResult is String)
                            {
                                var toWrite = Encoding.UTF8.GetBytes((string) (targetInvocationResult));
                                context.Response.OutputStream.Write(toWrite, 0, toWrite.Length);
                            }

                            #endregion
                        }
                    }
                    catch (Exception ex)
                    {
                        context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

                        var msg = Encoding.UTF8.GetBytes(ex.ToString());
                        context.Response.OutputStream.Write(msg, 0, msg.Length);

                        return;

                    }
                }
            }
            finally
            {
                context.Response.Close();
            }

        }

        /// <summary>
        /// A method that creates a 404 response.
        /// </summary>
        /// <param name="context">The context of the request, that caused the 404.</param>
        private static void NoPatternFound404(HttpListenerContext context)
        {
            Debug.WriteLine("Could not find a valid handler for URI: " + context.Request.RawUrl);
            var responseBodyBytes = Encoding.UTF8.GetBytes("Could not find a valid handler for URI: " + context.Request.RawUrl);

            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            context.Response.ContentType = "text/plain";
            context.Response.OutputStream.Write(responseBodyBytes, 0, responseBodyBytes.Length);
        }

        #endregion



        #region IDisposable Members

        void IDisposable.Dispose()
        {
            Close();
        }

        #endregion
    }
}
