using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.VersionedPluginManager;
using sones.Plugins.GraphDS;
using sones.Plugins.GraphDS.IO;

namespace sones.GraphDS.PluginManager.RESTServicePluginManager
{
    public sealed class RESTServicePluginManager : AComponentPluginManager
    {
        #region Constructor

        /// <summary>
        /// A plugin manager for the GraphDS
        /// </summary>
        public RESTServicePluginManager()
        {
            #region Register & Discover

            // Change the version if there are ANY changes which will prevent loading the plugin.
            // As long as there are still some plugins which does not have their own assembly you need to change the compatibility of ALL plugins of the GraphDS and GraphFSInterface assembly.
            // So, if any plugin in the GraphDS changes you need to change the AssemblyVersion of the GraphDS AND modify the compatibility version of the other plugins.
            _pluginManager
                .Register<IOInterface>(IOInterfaceCompatibility.MinVersion, IOInterfaceCompatibility.MaxVersion);
              
            _pluginManager.Discover();

            #endregion

            #region Get all plugins and fill the lookup dictionaries

            var componentName = this.GetType().Assembly.GetName().Name;

            FillLookup<IOInterface>(componentName, _ => _.PluginName);
            
            #endregion

        }

        #endregion
    }
}
