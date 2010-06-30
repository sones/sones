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


/* TCPSocketListener
 * (c) Stefan Licht, 2009
 * 
 * This class is the main TCP listener class.
 * StartListener will start the socket listener and wait for any requests. For each new
 * request (without KeepAlive) a new instance of the generic parameter TServerLogic will
 * be created and the ConnectionEstablished() is invoked
 * 
 * Lead programmer:
 *      Stefan Licht
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace sones.Networking.TCPSocket
{

    class SocketConnection
    {
        public ATCPSocketConnection Instance { get; set; }
    }

    /// <summary>
    /// Realize a TCPSocketListener. If a new Client is connected to the server, the method ConnectionEstablished() will be invoked from the generic type parameter.
    /// </summary>
    /// <typeparam name="TServerLogic">Is a class which needs to implement the abstract class ATcpSocketConnection</typeparam>
    public class TCPSocketListener<TServerLogic> : IDisposable
        where TServerLogic : ATCPSocketConnection, new()
    {

        public event ExceptionOccuredHandler OnExceptionOccured;

        #region Data

        /// <summary>
        /// If true, the dispose will be invoked in the _Instance.
        /// This should NOT be done if a instance was passed with the ctor
        /// </summary>
        private Boolean _AllowDispose = false;

        private TcpListener _TcpListener = null;
        //private Socket _TcpListener = null;
        private HashSet<SocketConnection> _TcpSocketConnections = null;

        #region IsRunning

        /// <summary>
        /// Is true as long as the Server is listening for new clients
        /// </summary>
        public Boolean IsRunning
        {
            get { return _IsRunning; }
        }
        private Boolean _IsRunning = false;

        #endregion

        #region StopRequested

        /// <summary>
        /// The Server is requested to stop and will not accept any clients
        /// </summary>
        public Boolean StopRequested
        {
            get { return _Stopped; }
        }
        private Boolean _Stopped = false;

        #endregion

        #region ConnectedClients

        /// <summary>
        /// The current number of connected clients
        /// </summary>
        public Int64 ConnectedClients
        {
            get
            {
                return _TcpSocketConnections.Count;
            }
        }

        #endregion

        #region MaxClientConnections

        /// <summary>
        /// The maximum of clients which can connect to the server
        /// </summary>
        public Int64 MaxClientConnections
        {
            get { return _MaxClientConnections; }
            set { _MaxClientConnections = value; }
        }
        private Int64 _MaxClientConnections = 50;

        #endregion

        #region ClientTimeout

        /// <summary>
        /// The Client should timeout after X Milliseconds (default: 60 seconds) - must be impemented in ConnectionEstablished logic
        /// </summary>
        public Int32 ClientTimeout
        {
            get { return _ClientTimeout; }
            set { _ClientTimeout = value; }
        }
        private Int32 _ClientTimeout = 10 * 1000;

        #endregion

        #region Instance

        TServerLogic _Instance = null;

        #endregion
        
        #endregion

        #region Constructors

        private TCPSocketListener(TServerLogic myInstance = null)
        {

            if (typeof(TServerLogic).BaseType != typeof(ATCPSocketConnection))
                throw new Exception("Invalid generic type param class - needs to be derived from ATcpSocketConnection");
            
            _TcpSocketConnections = new HashSet<SocketConnection>();
            _Instance = myInstance;

        }

        /// <summary>
        /// Initialize the Listener with the first available IPAddress on this machine
        /// </summary>
        /// <param name="myPort">The Port where the Listener is attached to</param>
        public TCPSocketListener(UInt16 myPort, TServerLogic instance = null)
            : this(IPAddress.Any, myPort, instance)
        { }

        /// <summary>
        /// Initialize the Listener
        /// </summary>
        /// <param name="myIPAddress">The IPAddress where the Listener is attached to</param>
        /// <param name="myPort">The Port where the Listener is attached to</param>
        public TCPSocketListener(IPAddress myIPAddress, UInt16 myPort, TServerLogic myInstance = null)
            : this(myInstance)
        {
            _TcpListener = new TcpListener(myIPAddress, myPort);
        }

        #endregion

        #region Start / Stop the TCPSocketListener

        /// <summary>
        /// Start the TCPSocketListener
        /// </summary>
        public void StartListener()
        {
            
            // Start the TCP Listener
            _TcpListener.Start(10000);
            
            // Start the Server listening for a new client
            Thread thread           = new Thread(new ThreadStart(Run));
            thread.IsBackground     = true;
            thread.Name             = "TCPSocketListener<" + typeof(TServerLogic).ToString() + ">.Run()";
            thread.Start();

            while (!_IsRunning) // Wait until socket has opened
            {
                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// Stop the TCPSocketListener
        /// </summary>
        public void Stop()
        {
            _Stopped = true;

            if (_TcpListener != null)
            {
                _TcpListener.Stop();
            }
        }

        /// <summary>
        /// Stop the TCPSocketListener and wait until all connections are closed
        /// </summary>
        public void StopAndWait()
        {
            _Stopped = true;
            while (_TcpSocketConnections.Count > 0)
            {
                Thread.Sleep(1);
            }

            if (_TcpListener != null)
            {
                _TcpListener.Stop();
            }
        }

        #endregion

        #region Threads

        /// <summary>
        /// The thread in which the server will wait for client connections
        /// </summary>
        private void Run()
        {

            try
            {

                // while the listener is not shutted down, he will accept new clients
                while (!_Stopped)
                {

                    _IsRunning = true;
                                        
                    // no more Clients will be accepted
                    while (_TcpSocketConnections.Count >= MaxClientConnections)
                        Thread.Sleep(1);

                    // wait for a new client request or for stop - avoid blocking _TcpListener.AcceptTcpClient()
                    while (!_Stopped && !_TcpListener.Pending())
                    {
                        Thread.Sleep(1);
                    }

                    // break on requested server stop
                    if (_Stopped) break;

                    // waiting for a new client connecting to the server
                    _TcpListener.BeginAcceptTcpClient(new AsyncCallback(acceptClient), _TcpListener);
                    
                }
                lock (_TcpSocketConnections)
                {
                    foreach (var Conn in _TcpSocketConnections)
                    {
                        Conn.Instance.StopRequested = true;
                    }
                }

                // after shutting down the server - wait for all client finishing their jobs
                while (_TcpSocketConnections.Count > 0)
                {
                    Thread.Sleep(1);
                }

            }
            catch (Exception ex)
            {
                if (OnExceptionOccured != null)
                {
                    OnExceptionOccured(this, ex);
                }
            }


            _IsRunning = false;

        }
        
        private void acceptClient(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;
            TcpClient tcpClient = listener.EndAcceptTcpClient(ar);

            // get the Generic class and create a new instance
            TServerLogic serverInstance = null;
            if (_Instance == null)
            {
                serverInstance = new TServerLogic();
                _AllowDispose = true;
            }
            else
            {
                serverInstance = _Instance;
                _AllowDispose = false;
            }

            serverInstance.IsConnected = false;
            serverInstance.TcpClientConnection = tcpClient;
            serverInstance.Timeout = ClientTimeout;
            serverInstance.StopRequested = false;

            var socketConnection = new SocketConnection() { Instance = serverInstance };

            // To have tomething to stop each connection activily we will store them in _TcpSocketConnections
            lock (_TcpSocketConnections)
            {
                _TcpSocketConnections.Add(socketConnection);
            }

            StartTcpSocketConnection(socketConnection);
        }

        /// <summary>
        /// The thread in which the server will call the implemented ConnectionEstablished on the Generic class type parameter TServerLogic
        /// </summary>
        /// <param name="socketConnection">A class (given by the generic type parameter TServerLogic) which implements the Abstract class ATcpSocketConnection</param>
        private void StartTcpSocketConnection(SocketConnection socketConnection)
        {

            ATCPSocketConnection tcpSocketConnection = socketConnection.Instance;

            try
            {

                tcpSocketConnection.IsConnected = true;
                tcpSocketConnection.InitializeSocket(null);//_DataObject);

                while (tcpSocketConnection.ConnectionEstablished() && tcpSocketConnection.KeepAlive && tcpSocketConnection.TcpClientConnection.Connected && !_Stopped)
                {
                    //Console.WriteLine("(" + _TcpSocketConnections.IndexOf(TcpSocketConnection) + ") TcpSocketConnection.KeepAlive: " + TcpSocketConnection.KeepAlive);
                }

            }
            catch (Exception ex)
            {
                tcpSocketConnection.ExceptionThrowed(this, ex);
                tcpSocketConnection.StopRequested = true;
                if (OnExceptionOccured != null)
                {
                    OnExceptionOccured(this, ex);
                }
            }

            lock (_TcpSocketConnections)
            {
                if (_TcpSocketConnections.Contains(socketConnection))
                {
                    _TcpSocketConnections.Remove(socketConnection);
                }
            }

            tcpSocketConnection.IsConnected = false;
            if (_AllowDispose) // Only if the socketListener created a new Instance of the connection
            {
                tcpSocketConnection.Dispose();
            }

        }

        #endregion

        public override string ToString()
        {
            return String.Concat("ConnectedClients: " + ConnectedClients.ToString().PadLeft(3), " IsRunning:", IsRunning, " StopRequested: ", StopRequested);
        }


        #region IDisposable Members

        public void Dispose()
        {
            StopAndWait();

            if (_TcpListener != null)
            {
                _TcpListener.Stop();
            }

        }

        #endregion
    }
}
