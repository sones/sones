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

namespace sones.Library.VersionedPluginManager
{
    /// <summary>
    /// The interface for all pluginable components
    /// </summary>
    public interface IPluginable
    {
        /// <summary>
        /// The unique name of the plugin
        /// </summary>
        String PluginName { get; }

        /// <summary>
        /// The parameters that are settable for this plugin
        /// </summary>
        Dictionary<String, Type> SetableParameters { get; }

        /// <summary>
        /// A method to initialize a plugin
        /// </summary>
        /// <param name="myParameters">The parameters for the plugin</param>
        /// <param name="UniqueString">A string, that is unique for all plugins in the system.</param>
        /// <returns>A new instance of the plugin</returns>
        IPluginable InitializePlugin(String UniqueString, Dictionary<String, Object> myParameters = null);
    }
}
