using sones.GraphFS;
using sones.Library.Security;
using sones.Library.Transaction;
using sones.Library.VersionedPluginManager;

namespace sones.GraphDB.Manager.Plugin
{
    /// <summary>
    /// A plugin manager for the sones GraphDB component
    /// </summary>
    public sealed class GraphDBPluginManager : AComponentPluginManager
    {
        #region constructor

        /// <summary>
        /// Create a new sones graphDB plugin manager
        /// </summary>
        public GraphDBPluginManager()
        {
            #region Register & Discover

            // Change the version if there are ANY changes which will prevent loading the plugin.
            // As long as there are still some plugins which does not have their own assembly you need to change the compatibility of ALL plugins of the GraphDB and GraphFSInterface assembly.
            // So, if any plugin in the GraphDB changes you need to change the AssemblyVersion of the GraphDB AND modify the compatibility version of the other plugins.
            _pluginManager
                .Register<IGraphFS>(IGraphFSVersionCompatibility.MinVersion, IGraphFSVersionCompatibility.MaxVersion)
                .Register<ITransactionManager>(ITransactionManagerVersionCompatibility.MinVersion, ITransactionManagerVersionCompatibility.MaxVersion)
                .Register<ISecurityManager>(ISecurityManagerVersionCompatibility.MinVersion, ISecurityManagerVersionCompatibility.MaxVersion)
                .Register<IRequestScheduler>(IRequestSchedulerVersionCompatibility.MinVersion, IRequestSchedulerVersionCompatibility.MaxVersion)
                .Register<IRequestManager>(IRequestManagerVersionCompatibility.MinVersion, IRequestManagerVersionCompatibility.MaxVersion)
                .Register<ILogicExpressionOptimizer>(ILogicExpressionOptimizerVersionCompatibility.MinVersion, ILogicExpressionOptimizerVersionCompatibility.MaxVersion);

            _pluginManager.Discover();

            #endregion

            #region Get all plugins and fill the lookup dictionaries

            var componentName = this.GetType().Assembly.GetName().Name;

            FillLookup<IGraphFS>(componentName);
            FillLookup<ITransactionManager>(componentName);
            FillLookup<ISecurityManager>(componentName);
            FillLookup<IRequestScheduler>(componentName);
            FillLookup<IRequestManager>(componentName);
            FillLookup<ILogicExpressionOptimizer>(componentName);

            #endregion   
        }

        #endregion
    }
}
