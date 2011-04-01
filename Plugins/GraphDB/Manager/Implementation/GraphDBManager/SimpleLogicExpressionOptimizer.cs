using sones.GraphDB.Expression;

namespace sones.GraphDB.Manager
{
    /// <summary>
    /// A simple optimizer which doesn't optimize anything
    /// </summary>
    public sealed class SimpleLogicExpressionOptimizer : ILogicExpressionOptimizer
    {
        #region ILogicExpressionOptimizer Members

        public IExpression OptimizeExpression(IExpression myExpression)
        {
            //return the expression itself and do not optimize anything
            return myExpression;
        }

        #endregion
    }
}