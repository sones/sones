using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.VersionedPluginManager;
using sones.GraphFS;
using sones.Library.Transaction;
using sones.Library.Security;
using sones.GraphDB.ErrorHandling;
using sones.Library.Settings;

namespace sones.GraphDB.Manager.Plugin
{
    /// <summary>
    /// A definition of plugins that are available for the sones GraphDB
    /// </summary>
    public sealed class GraphDBPlugins
    {
        #region data

        public PluginDefinition IGraphFSDefinition { get; private set; }
        public PluginDefinition TransactionManagerPlugin { get; private set; }
        public PluginDefinition SecurityManagerPlugin { get; private set; }
        public PluginDefinition RequestSchedulerPlugin { get; private set; }
        public PluginDefinition LogicExpressionOptimizerPlugin { get; private set; }
        public PluginDefinition RequestManagerPlugin { get; private set; }

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new sones GraphDB plugins definition wrapper
        /// </summary>
        /// <param name="myIGraphFSDefinition">The definition of the IGraphFS plugin</param>
        /// <param name="myTransactionManagerPlugin">The definition of the transaction manager plugin</param>
        /// <param name="mySecurityManagerPlugin">The definition of the transaction manager plugin</param>
        /// <param name="myRequestSchedulerPlugin">The definition of the request scheduler plugin</param>
        /// <param name="myLogicExpressionOptimizerPlugin">The definition of the logic expression optimizer plugin</param>
        /// <param name="myRequestManagerPlugin">The definition of the request manager plugin</param>
        public GraphDBPlugins(
            PluginDefinition myIGraphFSDefinition = null,
            PluginDefinition myTransactionManagerPlugin = null,
            PluginDefinition mySecurityManagerPlugin = null,
            PluginDefinition myRequestSchedulerPlugin = null,
            PluginDefinition myLogicExpressionOptimizerPlugin = null,
            PluginDefinition myRequestManagerPlugin = null)
        {
            IGraphFSDefinition = myIGraphFSDefinition;
            TransactionManagerPlugin = myTransactionManagerPlugin;
            SecurityManagerPlugin = mySecurityManagerPlugin;
            RequestSchedulerPlugin = myRequestSchedulerPlugin;
            LogicExpressionOptimizerPlugin = myLogicExpressionOptimizerPlugin;
            RequestManagerPlugin = myRequestManagerPlugin;
        }

        #endregion
    }
}
