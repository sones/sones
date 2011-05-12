/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Net;
using System.IdentityModel.Selectors;

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