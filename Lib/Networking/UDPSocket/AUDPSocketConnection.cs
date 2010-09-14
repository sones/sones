using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace sones.Networking.UDPSocket
{
    public abstract class AUDPSocketConnection
    {

        /// <summary>
        /// Will be invoked when a client established a connection to the server. Put you logic in this method.
        /// </summary>
        public abstract void DataReceived(Object state);

        /// <summary>
        /// This Object is the caller instance which Started the listener
        /// </summary>
        public Object CallerObject { get; set; }

    }
}
