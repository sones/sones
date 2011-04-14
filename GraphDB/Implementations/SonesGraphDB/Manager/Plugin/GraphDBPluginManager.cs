using sones.GraphFS;
using sones.Library.Security;
using sones.Library.Transaction;
using sones.Library.VersionedPluginManager;
using sones.Plugins.Index;
using System;
using sones.Plugins.Index.Interfaces;
using System.Collections.Generic;

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
                .Register<ILogicExpressionOptimizer>(ILogicExpressionOptimizerVersionCompatibility.MinVersion, ILogicExpressionOptimizerVersionCompatibility.MaxVersion)
                .Register<ISingleValueIndex<IComparable, Int64>>(ISonesIndexVersionCompatibility.MinVersion, ISonesIndexVersionCompatibility.MaxVersion)
                .Register<IVersionedIndex<IComparable, Int64, Int64>>(ISonesIndexVersionCompatibility.MinVersion, ISonesIndexVersionCompatibility.MaxVersion)
                .Register<IMultipleValueIndex<IComparable, Int64>>(ISonesIndexVersionCompatibility.MinVersion, ISonesIndexVersionCompatibility.MaxVersion);

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
            FillLookup<ISingleValueIndex<IComparable, Int64>>(componentName);
            FillLookup<IVersionedIndex<IComparable, Int64, Int64>>(componentName);
            FillLookup<IMultipleValueIndex<IComparable, Int64>>(componentName);

            #endregion   
        }

        #endregion
    }
}
