using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace sones.GraphDSServer
{
    /// <summary>
    /// The interface for starting / stopping rest services
    /// </summary>
    public interface IGraphDSREST
    {
        /// <summary>
        /// Starts a new REST service
        /// </summary>
        /// <param name="myServiceID">The unique identifier of the service</param>
        /// <param name="myPort">The used port</param>
        /// <param name="myIPAddress">The used ip-address</param>
        void StartRESTService(String myServiceID, UInt16 myPort, IPAddress myIPAddress);

        /// <summary>
        /// Stops a REST service
        /// </summary>
        /// <param name="myServiceID">The unique identifier of the REST service that is going to be stopped</param>
        /// <returns>True for successful stop, otherwise false</returns>
        bool StopRESTService(String myServiceID);
    }
}
