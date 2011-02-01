using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using sones.GraphDS;

namespace ISonesRESTService
{
    /// <summary>
    /// The interface for all sones RESTful webservices
    /// </summary>
    public interface ISonesRESTService
    {
        /// <summary>
        /// The name of the rest service
        /// </summary>
        String ID { get; }

        /// <summary>
        /// The port of the web service
        /// </summary>
        UInt16 Port { get; }

        /// <summary>
        /// The ip-adress of the webservice
        /// </summary>
        IPAddress IPAddress { get; }

        /// <summary>
        /// Initialize the REST service
        /// </summary>
        /// <param name="myID">The unique identifier of the rest service</param>
        /// <param name="myGraphDS">The GraphDS instance that should be requested</param>
        /// <param name="myPort">The used port</param>
        /// <param name="myIPAdress">The used ip-address</param>
        void Initiaslize(IGraphDS myGraphDS, UInt16 myPort, IPAddress myIPAddress);
    }
}
