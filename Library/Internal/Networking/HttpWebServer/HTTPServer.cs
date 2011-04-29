
#region Usings

using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Generic;

using sones.Networking.TCPSocket;

#endregion

namespace sones.Networking.HTTP
{

    #region A static HttpWebServer to return the HttpWebContext of the current thread

    public static class HTTPServer
    {
        [ThreadStatic]
        public static HTTPContext HTTPContext;
    }

    #endregion

    /// <summary>
    ///  This server will listen on a port and maps incoming urls to methods of T. 
    ///  T should implement an interface (with attribute [ServiceContract]) and some methods (with attribute [OperationContract]) and a URL definition [WebGet(UriTemplate = "/theurl/{someOptionalParams}")]
    /// </summary>
    /// <typeparam name="T">A class which implements a [ServiceContract] Interface</typeparam>
    public class HTTPServer<T> : IDisposable
        where T : class, new ()
    {

        #region Data

        private TCPSocketListener<HttpHandler<T>> _Socket;
        private HttpHandler<T> _HttpHandler;

        #endregion

        #region Properties

        public HTTPSecurity HTTPSecurity
        {

            get
            {
                return _HttpHandler.HTTPSecurity;
            }
            
            set
            {
                _HttpHandler.HTTPSecurity = value;
            }
        
        }

        public IPAddress ListenAddress { get; private set; }

        public UInt16 ListenPort { get; private set; }

        #endregion

        #region Constructors

        #region HttpWebServer(myPort, myInstance = null, myTimeout = 100000, myAutoStart = false)

        /// <summary>
        /// Create an instance of the server.
        /// </summary>
        /// <param name="myInstance">An optional instance. If null, each request will create a new instance of <typeparamref name="T"/></param>
        /// <param name="myTimeout">An optional timeout</param>
        public HTTPServer(UInt16 myPort, T myInstance = null, Int32 myTimeout = 100000, Boolean myAutoStart = false)
            : this (IPAddress.Any, myPort, myInstance, myTimeout, myAutoStart)
        {
        }

        #endregion

        #region HttpWebServer(myIPAddress, myPort, myInstance = null, myTimeout = 100000, myAutoStart = false)

        /// <summary>
        /// Create an instance of the server.
        /// </summary>
        /// <param name="myInstance">An optional instance. If null, each request will create a new instance of <typeparamref name="T"/></param>
        /// <param name="myTimeout">An optional timeout</param>
        public HTTPServer(IPAddress myIPAddress, UInt16 myPort, T myInstance = null, Int32 myTimeout = 5000, Boolean myAutoStart = false)
        {
            ListenAddress = myIPAddress;
            ListenPort = myPort;
            _HttpHandler = new HttpHandler<T>(myInstance);
            _Socket = new TCPSocketListener<HttpHandler<T>>(myIPAddress, myPort, myInstance: _HttpHandler);
            _Socket.ClientTimeout = myTimeout;

            if (myAutoStart)
                Start();

        }

        #endregion

        #region HttpWebServer(myURI, myInstance = null, myPort = 80, myTimeout = 100000, myAutoStart = false)

        /// <summary>
        /// Create an instance of the server.
        /// </summary>
        /// <param name="myInstance">An optional instance. If null, each request will create a new instance of <typeparamref name="T"/></param>
        /// <param name="myTimeout">An optional timeout</param>
        public HTTPServer(Uri myURI, T myInstance = null, Int32 myTimeout = 100000, Boolean myAutoStart = false)
        {

            var _IPHostEntry = Dns.GetHostEntry(myURI.Host);
            var _IPAddresses = _IPHostEntry.AddressList;

            if (!_IPAddresses.Any())
                throw new ArgumentException("Could not resolve '" + myURI.Host + "'!");

            ListenAddress = _IPAddresses[0];
            ListenPort = (UInt16)myURI.Port;

            _HttpHandler = new HttpHandler<T>(myInstance);
            _Socket                 = new TCPSocketListener<HttpHandler<T>>(ListenAddress, ListenPort, myInstance: _HttpHandler);
            _Socket.ClientTimeout   = myTimeout;


            if (myAutoStart)
                Start();

        }

        #endregion

        #endregion


        #region Event OnExceptionOccured

        /// <summary>
        /// This event will be raised on any exception of the server or handler
        /// </summary>
        public event ExceptionOccuredHandler OnExceptionOccured
        {
            add
            {
                _Socket.OnExceptionOccured      += value;
                _HttpHandler.OnExceptionOccured += value;
            }
            remove
            {
                _Socket.OnExceptionOccured      -= value;
                _HttpHandler.OnExceptionOccured -= value;
            }
        }

        #endregion

        #region Start()

        /// <summary>
        /// Starts the HTTP server
        /// </summary>
        public void Start()
        {
            _Socket.StartListener();
        }

        #endregion

        #region StopAndWait()

        /// <summary>
        /// Stops the WebServer and waits until the last request was handled
        /// </summary>
        public void StopAndWait()
        {
            if (_Socket != null)
            {
                _Socket.StopAndWait();
            }
        }

        #endregion


        #region IDisposable Members

        /// <summary>
        /// Closes the WebServer and waits until the last request was handled and free all resources
        /// </summary>
        public void Dispose()
        {
            if (_Socket != null)
            {
                _Socket.Dispose();
                _Socket = null;
            }
        }

        #endregion

    }

}
