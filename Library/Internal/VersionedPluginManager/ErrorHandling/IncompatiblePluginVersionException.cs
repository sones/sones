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
using System.Reflection;

namespace sones.Library.VersionedPluginManager.ErrorHandling
{
    /// <summary>
    /// This exception occurs if a version of a plugin does not match the expected version.
    /// </summary>
    public sealed class IncompatiblePluginVersionException : APluginManagerException
    {
        #region data

        /// <summary>
        /// The current plugin version.
        /// </summary>
        public readonly Version CurrentVersion;

        /// <summary>
        /// The maximum accepted version.
        /// </summary>
        public readonly Version MaxVersion;

        /// <summary>
        /// The minimum accepted version.
        /// </summary>
        public readonly Version MinVersion;

        /// <summary>
        /// The plugin assembly which implements the incompatible version.
        /// </summary>
        public readonly Assembly PluginAssembly;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new IncompatiblePluginVersionException exception
        /// </summary>
        /// <param name="myPluginAssembly">The current plugin assembly</param>
        /// <param name="myCurrentVersion">The current version of the plugin interface</param>
        /// <param name="myMinVersion">The minimum expected verion of the plugin interface</param>
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
        public IncompatiblePluginVersionException(Assembly myPluginAssembly, Version myCurrentVersion,
                                                  Version myMinVersion, Exception innerException = null) : base(innerException)
        {
            PluginAssembly = myPluginAssembly;
            CurrentVersion = myCurrentVersion;
            MinVersion = myMinVersion;
        }

        /// <summary>
        /// Creates a new IncompatiblePluginVersionException exception
        /// </summary>
        /// <param name="myPluginAssembly">The current plugin assembly</param>
        /// <param name="myCurrentVersion">The current version of the plugin interface</param>
        /// <param name="myMinVersion">The minimum expected verion of the plugin interface</param>
        /// <param name="myMaxVersion">The maximum expected verion of the plugin interface</param>
        public IncompatiblePluginVersionException(Assembly myPluginAssembly, Version myCurrentVersion,
                                                  Version myMinVersion, Version myMaxVersion)
        {
            PluginAssembly = myPluginAssembly;
            CurrentVersion = myCurrentVersion;
            MinVersion = myMinVersion;
            MaxVersion = myMaxVersion;
        }        

        #endregion
    }
}