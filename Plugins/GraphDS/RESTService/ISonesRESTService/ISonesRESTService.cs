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
using sones.GraphDS;

namespace sones.Plugins.GraphDS.RESTService
{
    #region ISonesRESTServiceCompatibility

    /// <summary>
    /// A static implementation of the compatible ISonesRESTService plugin versions. 
    /// Defines the min and max version for all ISonesRESTService implementations which will be activated used this ISonesRESTService.
    /// </summary>
    public static class ISonesRESTServiceCompatibility
    {
        public static Version MinVersion
        {
            get { return new Version("2.0.0.0"); }
        }

        public static Version MaxVersion
        {
            get { return new Version("2.0.0.0"); }
        }
    }

    #endregion

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
        /// <param name="myGraphDS">The GraphDS instance that should be requested</param>
        /// <param name="myPort">The used port</param>
        /// <param name="myIPAddress">The used ip-address</param>
        void Initialize(IGraphDS myGraphDS, UInt16 myPort, IPAddress myIPAddress);
    }
}