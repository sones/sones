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

namespace sones.Library.VersionedPluginManager.ErrorHandling
{
    /// <summary>
    /// There's a unknown plugin
    /// </summary>
    public sealed class UnknownPluginException : APluginManagerException
    {
        #region data

        /// <summary>
        /// The name of the plugin
        /// </summary>
        public String PluginName { get; private set; }

        /// <summary>
        /// The type of the plugin
        /// </summary>
        public Type PluginType { get; private set; }

        #endregion

        #region constructor
        
        /// <summary>
        /// Creates a new unknown plugin exception
        /// </summary>
        /// <param name="myUnknownPluginName">The name of the unknown plugin</param>
        /// <param name="myPluginType">The type of the unknown plugin</param>
		/// <param name="innerException">The exception that is the cause of the current exception, this parameter can be NULL.</param>
		public UnknownPluginException(String myUnknownPluginName, Type myPluginType, Exception innerException = null) : base(innerException)
        {
            PluginName = myUnknownPluginName;
            PluginType = myPluginType;
            
            _msg = String.Format("{0} : The plugin name '{1}' is unknown! Maybe you are missing a reference?", PluginName, PluginType.Name);
        }

        #endregion
                
                
    }
}
