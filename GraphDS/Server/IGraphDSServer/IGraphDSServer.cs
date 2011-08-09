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

using sones.GraphDS;
using System;
using sones.Plugins.GraphDS.Services;



namespace sones.GraphDSServer
{
    /// <summary>
    /// The interface for all GraphDS server
    /// </summary>
    public interface IGraphDSServer : IGraphDSREST, IGraphDS
    {
        /// <summary>
        /// Starts a service by the name of the service
        /// </summary>
        /// <param name="myServiceName">The name of the service</param>
        void StartService(String myServiceName);

        /// <summary>
        /// Stops a service by the name of the service
        /// </summary>
        /// <param name="myServiceName">The name of the service</param>
        void StopService(String myServiceName);

        /// <summary>
        /// Returns the status of a service 
        /// </summary>
        /// <param name="myServiceName">The name of the service</param>
        /// <returns>The status object of the service</returns>
        AServiceStatus GetServiceStatus(String myServiceName);

    }
}