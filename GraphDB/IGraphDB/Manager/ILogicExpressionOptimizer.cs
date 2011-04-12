using System;
using sones.GraphDB.Expression;
using sones.Library.VersionedPluginManager;

namespace sones.GraphDB.Manager
{
    #region ILogicExpressionOptimizerVersionCompatibility

    /// <summary>
    /// A static implementation of the compatible ILogicExpressionOptimizer plugin versions. 
    /// Defines the min and max version for all ILogicExpressionOptimizer implementations which will be activated
    /// </summary>
    public static class ILogicExpressionOptimizerVersionCompatibility
    {
        public static Version MinVersion
        {
            get
            {
                return new Version("2.0.0.0");
            }
        }
        public static Version MaxVersion
        {
            get
            {
                return new Version("2.0.0.0");
            }
        }
    }

    #endregion

    /// <summary>
    /// The interface for all logic expression optimizer
    /// </summary>
    public interface ILogicExpressionOptimizer : IPluginable
    {
        /// <summary>
        /// Optimizes a expression
        /// </summary>
        /// <param name="myExpression">The expression that is going to be optimized</param>
        /// <returns>The optimized expression</returns>
        IExpression OptimizeExpression(IExpression myExpression);
    }
}