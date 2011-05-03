using sones.Library.VersionedPluginManager;
using System.Collections.Generic;

namespace sones.GraphDB.Manager.Plugin
{
    /// <summary>
    /// A definition of plugins that are available for the sones GraphDB
    /// </summary>
    public sealed class GraphDBPlugins
    {
        #region data

        public readonly PluginDefinition IGraphFSDefinition;
        public readonly PluginDefinition TransactionManagerPlugin;
        public readonly PluginDefinition SecurityManagerPlugin;
        public readonly PluginDefinition LogicExpressionOptimizerPlugin;
        public readonly List<PluginDefinition> IndexPlugins;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new sones GraphDB plugins definition wrapper
        /// </summary>
        /// <param name="myIGraphFSDefinition">The definition of the IGraphFS plugin</param>
        /// <param name="myTransactionManagerPlugin">The definition of the transaction manager plugin</param>
        /// <param name="mySecurityManagerPlugin">The definition of the transaction manager plugin</param>
        /// <param name="myLogicExpressionOptimizerPlugin">The definition of the logic expression optimizer plugin</param>
        public GraphDBPlugins(
            PluginDefinition myIGraphFSDefinition = null,
            PluginDefinition myTransactionManagerPlugin = null,
            PluginDefinition mySecurityManagerPlugin = null,
            PluginDefinition myLogicExpressionOptimizerPlugin = null,
            List<PluginDefinition> myIndexPlugins = null)
        {
            IGraphFSDefinition = myIGraphFSDefinition;
            TransactionManagerPlugin = myTransactionManagerPlugin;
            SecurityManagerPlugin = mySecurityManagerPlugin;
            LogicExpressionOptimizerPlugin = myLogicExpressionOptimizerPlugin;
            IndexPlugins = myIndexPlugins;
        }

        #endregion
    }
}
