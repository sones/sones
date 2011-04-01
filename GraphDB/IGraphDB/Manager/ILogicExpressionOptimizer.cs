using sones.GraphDB.Request.Helper.Expression;

namespace sones.GraphDB.Manager
{
    /// <summary>
    /// The interface for all logic expression optimizer
    /// </summary>
    public interface ILogicExpressionOptimizer
    {
        /// <summary>
        /// Optimizes a expression
        /// </summary>
        /// <param name="myExpression">The expression that is going to be optimized</param>
        /// <returns>The optimized expression</returns>
        IExpression OptimizeExpression(IExpression myExpression);
    }
}