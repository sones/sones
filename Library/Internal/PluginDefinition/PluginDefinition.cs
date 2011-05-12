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
    /// This class is used to specify which plugin should be used
    /// </summary>
    public sealed class PluginDefinition
    {
        #region data

        /// <summary>
        /// The name of the plugin
        /// </summary>
        public readonly String NameOfPlugin;

        /// <summary>
        /// The parameters for plugins
        /// </summary>
        public readonly Dictionary<String, Object> PluginParameter;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new plugin definition
        /// </summary>
        /// <param name="myPluginName">The name of the plugin</param>
        /// <param name="myParameter">The parameter for this plugin</param>
        public PluginDefinition(String myPluginName, Dictionary<String, Object> myParameter = null)
        {
            NameOfPlugin = myPluginName;

            if (myParameter != null)
            {
                PluginParameter = myParameter;
            }
            else
            {
                PluginParameter = new Dictionary<string, object>();
            }
        }

        #endregion

        #region fluid methods

        /// <summary>
        /// Fluid method to add a parameter
        /// </summary>
        /// <param name="myParameterName">The name of the parameter</param>
        /// <param name="myParameterValue">The value of the parameter</param>
        /// <returns>The plugindefinition itself</returns>
        public PluginDefinition AddParameter(String myParameterName, Object myParameterValue)
        {
            PluginParameter[myParameterName] = myParameterValue;

            return this;
        }

        #endregion

    }
}
