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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using sones.GraphDS;
using sones.Library.VersionedPluginManager;

namespace sones.Plugins.GraphDS.Services
{
    #region IServiceCompatibility

    /// <summary>
    /// A static implementation of the compatible ISonesRESTService plugin versions. 
    /// Defines the min and max version for all ISonesRESTService implementations which will be activated used this ISonesRESTService.
    /// </summary>
    public static class IServiceCompatibility
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
    
    public interface IService 
    {
        /// <summary>
        /// The name of the service
        /// </summary>
        String PluginName { get; }

        /// <summary>
        /// Starts the service
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the service
        /// </summary>
        void Stop();

        /// <summary>
        /// Returns the status of the service
        /// </summary>
        /// <returns>The status object</returns>
        AServiceStatus GetCurrentStatus();
    }
}
