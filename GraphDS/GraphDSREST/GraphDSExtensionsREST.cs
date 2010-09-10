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
 * GraphDSExtensionsREST
 * (c) Achim Friedland, 2010
 */

#region Usings

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using sones.GraphDS.API.CSharp;
using sones.GraphDS.Connectors.REST;
using sones.Networking.HTTP;
using System.Net;
using sones.GraphDB.QueryLanguage.Result;
using System.Xml.Linq;

#endregion

namespace sones.GraphDS.Connectors.REST
{

    public static class GraphDSExtensionsREST
    {

        #region StartREST(this myGraphDSSharpmyURI, myHttpWebSecurity)

        public static HTTPServer<GraphDSREST_Service> StartREST(this GraphDSSharp myGraphDSSharp, Uri myURI, HTTPSecurity myHttpWebSecurity = null)
        {

            // Initialize REST service
            var _HttpWebServer = new HTTPServer<GraphDSREST_Service>(
                myURI,
                new GraphDSREST_Service(myGraphDSSharp.IGraphDBSession, myGraphDSSharp.IGraphFSSession),
                myAutoStart : true)
            {
                HTTPSecurity = myHttpWebSecurity,
            };


            // Register the REST service within the list of services
            // to stop before shutting down the GraphDSSharp instance
            myGraphDSSharp.ShutdownEvent += new GraphDSSharp.ShutdownEventHandler((o, e) =>
            {
                _HttpWebServer.StopAndWait();
            });

            return _HttpWebServer;

        }

        #endregion

        #region StartREST(this myGraphDSSharpmyURI, myPort, myHttpWebSecurity)

        public static HTTPServer<GraphDSREST_Service> StartREST(this GraphDSSharp myGraphDSSharp, UInt16 myPort, HTTPSecurity myHttpWebSecurity = null)
        {
            return myGraphDSSharp.StartREST(IPAddress.Any, myPort, myHttpWebSecurity);
        }

        #endregion

        #region StartREST(this myGraphDSSharpmyURI, myIPAdress, myPort, myHttpWebSecurity)

        public static HTTPServer<GraphDSREST_Service> StartREST(this GraphDSSharp myGraphDSSharp, IPAddress myIPAddress, UInt16 myPort, HTTPSecurity myHttpWebSecurity = null)
        {

            // Initialize REST service
            var _HttpWebServer = new HTTPServer<GraphDSREST_Service>(
                myIPAddress,
                myPort,
                new GraphDSREST_Service(myGraphDSSharp.IGraphDBSession, myGraphDSSharp.IGraphFSSession),
                myAutoStart: true)
            {
                HTTPSecurity = myHttpWebSecurity,
            };


            // Register the REST service within the list of services
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
