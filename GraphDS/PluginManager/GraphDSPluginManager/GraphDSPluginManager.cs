using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.VersionedPluginManager;
using sones.GraphQL;
using sones.Plugins.GraphDS.RESTService;
using sones.Plugins.GraphDS;

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
                .Register<ISonesRESTService>(ISonesRESTServiceCompatibility.MinVersion, ISonesRESTServiceCompatibility.MaxVersion)
                .Register<IDrainPipe>(IDrainPipeCompatibility.MinVersion, IDrainPipeCompatibility.MaxVersion);

            _pluginManager.Discover();

            #endregion

            #region Get all plugins and fill the lookup dictionaries

            var componentName = this.GetType().Assembly.GetName().Name;

            FillLookup<IGraphQL>(componentName);
            FillLookup<ISonesRESTService>(componentName);
            FillLookup<IDrainPipe>(componentName);

            #endregion   
        
        }

        #endregion
    }
}
