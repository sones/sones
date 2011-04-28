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
        public readonly List<PluginDefinition> ISonesRESTServicePlugins;
        public readonly List<PluginDefinition> IGraphQLPlugins;
        public readonly List<PluginDefinition> IDrainPipePlugins;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new sones GraphDB plugins definition wrapper
        /// </summary>
        public GraphDSPlugins(
            List<PluginDefinition> myISonesRESTServicePlugins = null,
            List<PluginDefinition> myIGraphQLPlugins = null,
            List<PluginDefinition> myIDrainPipePlugins = null)
        {
            ISonesRESTServicePlugins = myISonesRESTServicePlugins;
            IGraphQLPlugins = myIGraphQLPlugins;
            IDrainPipePlugins = myIDrainPipePlugins;
        }

        #endregion
    }
}
