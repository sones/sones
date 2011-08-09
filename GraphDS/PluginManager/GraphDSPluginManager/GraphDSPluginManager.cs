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
using sones.Library.VersionedPluginManager;
using sones.GraphQL;
using sones.Plugins.GraphDS.RESTService;
using sones.Plugins.GraphDS;
using sones.Plugins.GraphDS.Services;

namespace sones.GraphDS.PluginManager.GraphDSPluginManager
{
    public sealed class GraphDSPluginManager : AComponentPluginManager
    {
        #region Constructor

        /// <summary>
        /// A plugin manager for the GraphDS
        /// </summary>
        public GraphDSPluginManager()
        {
            #region Register & Discover

            // Change the version if there are ANY changes which will prevent loading the plugin.
            // As long as there are still some plugins which does not have their own assembly you need to change the compatibility of ALL plugins of the GraphDS assembly.
            // So, if any plugin in the GraphDS changes you need to change the AssemblyVersion of the GraphDS AND modify the compatibility version of the other plugins.
            _pluginManager
                .Register<IGraphQL>(IGraphQLVersionCompatibility.MinVersion, IGraphQLVersionCompatibility.MaxVersion)
                .Register<ISonesRESTService>(ISonesRESTServiceCompatibility.MinVersion, ISonesRESTServiceCompatibility.MaxVersion)  // not yet used
                .Register<IDrainPipe>(IDrainPipeCompatibility.MinVersion, IDrainPipeCompatibility.MaxVersion)
                .Register<IService>(IServiceCompatibility.MinVersion, IServiceCompatibility.MaxVersion);

            _pluginManager.Discover();

            #endregion

            #region Get all plugins and fill the lookup dictionaries

            var componentName = this.GetType().Assembly.GetName().Name;

            FillLookup<IGraphQL>(componentName, _ => _.PluginName);
            FillLookup<ISonesRESTService>(componentName, _ => _.ID);   // not yet used
            FillLookup<IDrainPipe>(componentName, _ => _.PluginName);
            FillLookup<IService>(componentName, _ => _.PluginName);

            #endregion   
        
        }

        #endregion
    }
}
