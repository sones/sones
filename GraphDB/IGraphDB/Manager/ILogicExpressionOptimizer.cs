using sones.GraphDB.Request.Helper.Expression;

namespace sones.GraphDB.Manager
{
    /// <summary>
    /// The interface for all logic expression optimizer
    /// </summary>
    public interface ILogicExpressionOptimizer
    {
        IExpression ExecuteRequestInParallel(IExpression myExpression);
    }
}