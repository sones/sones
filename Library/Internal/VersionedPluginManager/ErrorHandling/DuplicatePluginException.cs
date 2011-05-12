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
    /// There's a duplicate plugin
    /// </summary>
    public sealed class DuplicatePluginException : APluginManagerException
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

        /// <summary>
        /// The name of the component that complains about a duplicate plugin
        /// </summary>
        public String ComplainingComponent { get; private set; }

        #endregion

        #region constructor
        
        /// <summary>
        /// Creates a new duplicate plugin exception
        /// </summary>
        /// <param name="myDuplicatePluginName">The name of the duplicate plugin</param>
        /// <param name="myPluginType">The type of the duplicate plugin</param>
        /// <param name="myComplainingComponent">The name of the component that complains about a duplicate plugin</param>
        public DuplicatePluginException(String myDuplicatePluginName, Type myPluginType, String myComplainingComponent)
        {
            PluginName = myDuplicatePluginName;
            ComplainingComponent = myComplainingComponent;
            PluginType = myPluginType;
            
            _msg = String.Format("{0} : The {1} plugin name is duplicate within the {2} component! The name has to be unique!", PluginName, PluginType.Name, ComplainingComponent);
        }

        #endregion     
    }
}
