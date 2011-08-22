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

using sones.Library.VersionedPluginManager;
using System.Collections.Generic;

namespace sones.GraphDS.PluginManager
{
    /// <summary>
    /// A definition of plugins that are available for the sones GraphDB
    /// </summary>
    public sealed class GraphDSPlugins
    {
        #region data
        public readonly List<PluginDefinition> IGraphQLPlugins;
        public readonly List<PluginDefinition> IDrainPipePlugins;
        public readonly List<PluginDefinition> IUsageDataCollectorPlugIns;
        #endregion

        #region constructor

        /// <summary>
        /// Creates a new sones GraphDB plugins definition wrapper
        /// </summary>
        public GraphDSPlugins(
            List<PluginDefinition> myIGraphQLPlugins = null,
            List<PluginDefinition> myIDrainPipePlugins = null,
            List<PluginDefinition> myIUsageDataCollectorPlugIns = null)
        {
            IGraphQLPlugins = myIGraphQLPlugins;
            IDrainPipePlugins = myIDrainPipePlugins;
            IUsageDataCollectorPlugIns = myIUsageDataCollectorPlugIns;
        }

        #endregion
    }
}
