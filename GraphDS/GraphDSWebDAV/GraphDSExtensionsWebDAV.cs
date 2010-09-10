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

/*
 * GraphDSExtensionsWebDAV
 * (c) Achim Friedland, 2010
 */

#region Usings

using System;
using System.Net;

using sones.GraphDS.API.CSharp;

using sones.Networking.HTTP;

#endregion

namespace sones.GraphDS.Connectors.WebDAV
{

    /// <summary>
    /// Extension methods to start the GraphDS WebDAV interface.
    /// </summary>

    public static class GraphDSExtensionsWebDAV
    {

        #region StartWebDAV(this myAGraphDSSharp, myURI, myHttpWebSecurity)

        /// <summary>
        /// Start the WebDAV interface using the given URI and HTTPSecurity parameters.
        /// </summary>
        /// <param name="myURI">The URI for binding the interface at</param>
        /// <param name="myHttpWebSecurity">A HTTPSecurity class for checking security parameters like user credentials</param>
        public static HTTPServer<GraphDSWebDAV_Service> StartWebDAV(this AGraphDSSharp myAGraphDSSharp, Uri myURI, HTTPSecurity myHttpWebSecurity = null)
        {

            // Initialize WebDAV service
            var _HttpWebServer = new HTTPServer<GraphDSWebDAV_Service>(
                myURI,
                new GraphDSWebDAV_Service(myAGraphDSSharp),
                myAutoStart: true)
            {
                HTTPSecurity = myHttpWebSecurity,
            };


            // Register the WebDAV service within the list of services
            // to stop before shutting down the GraphDSSharp instance
            myAGraphDSSharp.ShutdownEvent += new GraphDSSharp.ShutdownEventHandler((o, e) =>
            {
                _HttpWebServer.StopAndWait();
            });

            return _HttpWebServer;

        }

        #endregion

        #region StartWebDAV(this myGraphDSSharp, myPort, myHttpWebSecurity)

        /// <summary>
        /// Start the WebDAV interface using the given port and HTTPSecurity parameters.
        /// </summary>
        /// <param name="myPort">The port for binding the interface at</param>
        /// <param name="myHttpWebSecurity">A HTTPSecurity class for checking security parameters like user credentials</param>
        public static HTTPServer<GraphDSWebDAV_Service> StartWebDAV(this AGraphDSSharp myAGraphDSSharp, UInt16 myPort, HTTPSecurity myHttpWebSecurity = null)
        {
            return myAGraphDSSharp.StartWebDAV(IPAddress.Any, myPort, myHttpWebSecurity);
        }

        #endregion

        #region StartWebDAV(this myGraphDSSharp, myIPAddress, myPort, myHttpWebSecurity)

        /// <summary>
        /// Start the WebDAV interface using the given ip address, port and HTTPSecurity parameters.
        /// </summary>
        /// <param name="myIPAddress">The IPAddress for binding the interface at</param>
        /// <param name="myPort">The port for binding the interface at</param>
        /// <param name="myHttpWebSecurity">A HTTPSecurity class for checking security parameters like user credentials</param>
        public static HTTPServer<GraphDSWebDAV_Service> StartWebDAV(this AGraphDSSharp myAGraphDSSharp, IPAddress myIPAddress, UInt16 myPort, HTTPSecurity myHttpWebSecurity = null)
        {

            // Initialize WebDAV service
            var _HttpWebServer = new HTTPServer<GraphDSWebDAV_Service>(
                myIPAddress,
                myPort,
                new GraphDSWebDAV_Service(myAGraphDSSharp),
                myAutoStart: true)
            {
                HTTPSecurity = myHttpWebSecurity,
            };


            // Register the WebDAV service within the list of services
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
