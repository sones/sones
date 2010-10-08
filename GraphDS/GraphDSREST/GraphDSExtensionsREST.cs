/*
 * GraphDSExtensionsREST
 * (c) Achim Friedland, 2010
 */

#region Usings

using System;
using System.Net;

using sones.GraphDS.API.CSharp;

using sones.Networking.HTTP;

#endregion

namespace sones.GraphDS.Connectors.REST
{

    /// <summary>
    /// Extension methods to start the GraphDS REST interface.
    /// </summary>

    public static class GraphDSExtensionsREST
    {

        #region StartREST(this myGraphDSSharp, myURI, myHttpWebSecurity)

        /// <summary>
        /// Start the REST interface using the given URI and HTTPSecurity parameters.
        /// </summary>
        /// <param name="myURI">The URI for binding the interface at</param>
        /// <param name="myHttpWebSecurity">A HTTPSecurity class for checking security parameters like user credentials</param>
        public static HTTPServer<GraphDSREST_Service> StartREST(this AGraphDSSharp myAGraphDSSharp, Uri myURI, HTTPSecurity myHttpWebSecurity = null)
        {

            // Initialize REST service
            var _HttpWebServer = new HTTPServer<GraphDSREST_Service>(
                myURI,
                new GraphDSREST_Service(myAGraphDSSharp),
                myAutoStart : true)
            {
                HTTPSecurity = myHttpWebSecurity,
            };


            // Register the REST service within the list of services
            // to stop before shutting down the GraphDSSharp instance
            myAGraphDSSharp.ShutdownEvent += new GraphDSSharp.ShutdownEventHandler((o, e) =>
            {
                _HttpWebServer.StopAndWait();
            });

            return _HttpWebServer;

        }

        #endregion

        #region StartREST(this AGraphDSSharp, myURI, myPort, myHttpWebSecurity)

        /// <summary>
        /// Start the REST interface using the given port and HTTPSecurity parameters.
        /// </summary>
        /// <param name="myPort">The port for binding the interface at</param>
        /// <param name="myHttpWebSecurity">A HTTPSecurity class for checking security parameters like user credentials</param>
        public static HTTPServer<GraphDSREST_Service> StartREST(this AGraphDSSharp myAGraphDSSharp, UInt16 myPort, HTTPSecurity myHttpWebSecurity = null)
        {
            return myAGraphDSSharp.StartREST(IPAddress.Any, myPort, myHttpWebSecurity);
        }

        #endregion

        #region StartREST(this myAGraphDSSharp, myURI, myIPAdress, myPort, myHttpWebSecurity)

        /// <summary>
        /// Start the REST interface using the given ip address, port and HTTPSecurity parameters.
        /// </summary>
        /// <param name="myIPAddress">The IPAddress for binding the interface at</param>
        /// <param name="myPort">The port for binding the interface at</param>
        /// <param name="myHttpWebSecurity">A HTTPSecurity class for checking security parameters like user credentials</param>
        public static HTTPServer<GraphDSREST_Service> StartREST(this AGraphDSSharp myAGraphDSSharp, IPAddress myIPAddress, UInt16 myPort, HTTPSecurity myHttpWebSecurity = null)
        {

            // Initialize REST service
            var _HttpWebServer = new HTTPServer<GraphDSREST_Service>(
                myIPAddress,
                myPort,
                new GraphDSREST_Service(myAGraphDSSharp),
                myAutoStart: true)
            {
                HTTPSecurity = myHttpWebSecurity,
            };


            // Register the REST service within the list of services
            // to stop before shutting down the GraphDSSharp instance
            myAGraphDSSharp.ShutdownEvent += new GraphDSSharp.ShutdownEventHandler((o, e) =>
            {
                _HttpWebServer.StopAndWait();
            });

            return _HttpWebServer;

        }

        #endregion

    }

}
