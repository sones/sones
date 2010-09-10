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
using System.Linq;
using System.Text;
using System.Collections.Generic;

using sones.GraphDS.API.CSharp;
using sones.GraphDS.Connectors.WebDAV;
using sones.Networking.HTTP;
using System.Net;

#endregion

namespace sones.GraphDS.Connectors.WebDAV
{

    public static class GraphDSExtensionsWebDAV
    {

        #region StartWebDAV(this myGraphDSSharp, myURI, myHttpWebSecurity)

        public static HTTPServer<GraphDSWebDAV_Service> StartWebDAV(this GraphDSSharp myGraphDSSharp, Uri myURI, HTTPSecurity myHttpWebSecurity = null)
        {

            // Initialize WebDAV service
            var _HttpWebServer = new HTTPServer<GraphDSWebDAV_Service>(
                myURI,
                new GraphDSWebDAV_Service(myGraphDSSharp.IGraphFSSession),
                myAutoStart: true)
            {
                HTTPSecurity = myHttpWebSecurity,
            };


            // Register the WebDAV service within the list of services
            // to stop before shutting down the GraphDSSharp instance
            myGraphDSSharp.ShutdownEvent += new GraphDSSharp.ShutdownEventHandler((o, e) =>
            {
                _HttpWebServer.StopAndWait();
            });

            return _HttpWebServer;

        }

        #endregion

        #region StartWebDAV(this myGraphDSSharp, myPort, myHttpWebSecurity)

        public static HTTPServer<GraphDSWebDAV_Service> StartWebDAV(this GraphDSSharp myGraphDSSharp, UInt16 myPort, HTTPSecurity myHttpWebSecurity = null)
        {
            return myGraphDSSharp.StartWebDAV(IPAddress.Any, myPort, myHttpWebSecurity);
        }

        #endregion

        #region StartWebDAV(this myGraphDSSharp, myIPAddress, myPort, myHttpWebSecurity)

        public static HTTPServer<GraphDSWebDAV_Service> StartWebDAV(this GraphDSSharp myGraphDSSharp, IPAddress myIPAddress, UInt16 myPort, HTTPSecurity myHttpWebSecurity = null)
        {

            // Initialize WebDAV service
            var _HttpWebServer = new HTTPServer<GraphDSWebDAV_Service>(
                myIPAddress,
                myPort,
                new GraphDSWebDAV_Service(myGraphDSSharp.IGraphFSSession),
                myAutoStart: true)
            {
                HTTPSecurity = myHttpWebSecurity,
            };


            // Register the WebDAV service within the list of services
            // to stop before shutting down the GraphDSSharp instance
            myGraphDSSharp.ShutdownEvent += new GraphDSSharp.ShutdownEventHandler((o, e) =>
            {
                _HttpWebServer.StopAndWait();
            });

            return _HttpWebServer;

        }

        #endregion


    }

}
