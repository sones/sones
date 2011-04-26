using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using sones.Library.Threading;

namespace sones.Networking.UDPSocket
{

    public delegate void UDPSocketExceptionOccuredHandler(Object mySender, Exception myException);

    /// <summary>
    /// Realize a TCPSocketListener. If a new Client is connected to the server, the method ConnectionEstablished() will be invoked from the generic type parameter.
    /// </summary>
    /// <typeparam name="TServerLogic">Is a class which needs to implement the abstract class ATcpSocketConnection</typeparam>
    public class UDPSocketMulticast<TServerLogic> where TServerLogic : new()
    {
        private readonly Int32 MAX_UDPBUFFER_SIZE = 1024 * 1024 * 10;

        public event UDPSocketExceptionOccuredHandler OnExceptionOccured;

        #region Properties

        /// <summary>
        /// This Object is the caller instance which Started the listener
        /// </summary>
        public Object CallerObject
        {
            get
            {
                return _CallerObject;
            }
        }
        private Object _CallerObject;

        public Int32 MulticastTTL
        {
            get { return _MulticastTTL; }
            set { _MulticastTTL = value; }
        }
        private Int32 _MulticastTTL = 2;

        #endregion

        #region Fields

        private IPAddress _IPAddress;
        private Int32 _Port;

        private Socket _UDPSocket;

        private Boolean _IsRunning;
        public Boolean IsRunning
        {
            get { return _IsRunning; }
        }

        private Boolean _StopRequested;

        #endregion

        #region Setters

        public void SetStateObject(Object myStateObject)
        {

            _CallerObject = myStateObject;

        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize the Listener
        /// </summary>
        /// <param name="IPAddress">The IPAddress where the Listener is attached to</param>
        /// <param name="Port">The Port where the Listener is attached to</param>
        public UDPSocketMulticast(IPAddress IPAddress, Int32 Port)
        {

            _IPAddress = IPAddress;
            _Port = Port;

            _IsRunning = false;
            _StopRequested = false;

        }

        #endregion

        #region Start / Stop the TCPSocketListener

        /// <summary>
        /// Start the TCPSocketListener
        /// </summary>
        public void StartListener()
        {

            if (typeof(TServerLogic).BaseType != typeof(AUDPSocketConnection))
                throw new Exception("Invalid generic type param class - needs to be derived from AUDPSocketConnection");

            #region Check UDP IP Address

            //224.0.0.0 to 239.255.255.255
            String[] parts = _IPAddress.ToString().Split(new char[] { '.' });
            if (Int32.Parse(parts[0]) < 224 || Int32.Parse(parts[0]) > 239)
                throw new Exception("Invalid multicast IP Address");

            if (_Port < 0 || _Port > 65535)
                throw new Exception(_Port + " is not a valid port (must between 0 and )");

            #endregion

            // Start the Server listening for a new client
            Thread Thread       = new Thread(new ThreadStart(Run));
            Thread.IsBackground = true;
            Thread.Start();

        }

        public void StopListener()
        {
            _StopRequested = true;
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

                _UDPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint LocalHostIPEnd = new IPEndPoint(IPAddress.Any, _Port);

                _UDPSocket.SetSocketOption(SocketOptionLevel.Udp, SocketOptionName.NoDelay, 1);
                //allow for loopback testing
                _UDPSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);

                _UDPSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, MAX_UDPBUFFER_SIZE);

                //extremly important to bind the Socket before joining multicast groups
                _UDPSocket.Bind(LocalHostIPEnd);

                //set multicast flags, sending flags - TimeToLive (TTL)
                // 0 - LAN
                // 1 - Single Router Hop
                // 2 - Two Router Hops... 
                _UDPSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, _MulticastTTL);

                //join multicast group
                _UDPSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(_IPAddress));

                IPEndPoint LocalIPEndPoint                        = new IPEndPoint(IPAddress.Any, _Port);
                EndPoint LocalEndPoint                            = (EndPoint)LocalIPEndPoint;

                Object TServerLogic                               = new TServerLogic();
                ((AUDPSocketConnection)TServerLogic).CallerObject = _CallerObject;

                GraphThreadPool GraphThreadPool               = new GraphThreadPool("UDPSocketMulticast");
                GraphThreadPool.OnWorkerThreadException       += new WorkerThreadExceptionHandler(ThreadPool_OnWorkerThreadException);

                //SmartThreadPool SmartThreadPool                   = new SmartThreadPool(1000, 2, Environment.ProcessorCount);

                _IsRunning = true;

                while (!_StopRequested)
                {

                    while (_UDPSocket.Available == 0 && !_StopRequested)
                        Thread.Sleep(1);

                    if (_StopRequested) break;

                    if (_UDPSocket.Available >= MAX_UDPBUFFER_SIZE)
                        throw new UDPSocketBufferOverflowException("The UDPSocket buffer reached the maximum size of " + MAX_UDPBUFFER_SIZE);

                    Byte[] totalRecievedBytes;
                    Byte[] buffer = new Byte[_UDPSocket.Available];

                    Int32 bytesRecieved = _UDPSocket.ReceiveFrom(buffer, buffer.Length, SocketFlags.None, ref LocalEndPoint);
                    totalRecievedBytes = new Byte[bytesRecieved];
                    Array.Copy(buffer, totalRecievedBytes, bytesRecieved);

                    GraphThreadPool.QueueWorkItem(new GraphThreadPool.ThreadPoolEntry(new ParameterizedThreadStart(((AUDPSocketConnection)TServerLogic).DataReceived), totalRecievedBytes));
                    //SmartThreadPool.QueueWorkItem(new Action<Object>(((AUDPSocketConnection)TServerLogic).DataReceived), totalRecievedBytes);
                }

            }
            catch (Exception ex)
            {
                if (OnExceptionOccured != null)
                    OnExceptionOccured(this, ex);
            }
            
            _IsRunning = false;

        }

        #endregion

        #region Send a multicast package

        public void Send(IPAddress myIPAddress, Int32 myPort, Byte[] mySendData)
        {

            Socket _UDPSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint LocalHostIPEnd = new IPEndPoint(IPAddress.Any, _Port);

            _UDPSocket.SetSocketOption(SocketOptionLevel.Udp,    SocketOptionName.NoDelay, 1);
            //allow for loopback testing
            _UDPSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);

            //set multicast flags, sending flags - TimeToLive (TTL)
            // 0 - LAN
            // 1 - Single Router Hop
            // 2 - Two Router Hops... 
            _UDPSocket.SetSocketOption(SocketOptionLevel.IP,     SocketOptionName.MulticastTimeToLive, _MulticastTTL);

            //set the target IP
            IPEndPoint RemoteIPEndPoint = new IPEndPoint(myIPAddress, myPort);
            EndPoint RemoteEndPoint = (EndPoint)RemoteIPEndPoint;

            if (mySendData.Length > MAX_UDPBUFFER_SIZE)
                throw new Exception("Message (" + mySendData.Length + "b) is bigger than the maximum message size of " + MAX_UDPBUFFER_SIZE + "b");

            //Console.WriteLine(Environment.NewLine + "NotificationBridge Sent: " + message.NotificationType.Name + " " + bytesToSend.Length + "bytes from dispatcher " + _Dispatcher.Uuid + " - I'm dispatcher " + Dispatcher.Uuid);
            Int32 result = _UDPSocket.SendTo(mySendData, SocketFlags.None, RemoteEndPoint);

        }

        #endregion

        void ThreadPool_OnWorkerThreadException(object mySender, Exception myException)
        {
            //Console.WriteLine("[UDPSocketMulticast] ThreadPool_OnWorkerThreadException: " + myException.Message);
            OnExceptionOccured(mySender, myException);
        }
    }
}
