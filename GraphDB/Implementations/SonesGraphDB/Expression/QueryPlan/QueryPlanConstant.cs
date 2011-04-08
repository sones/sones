using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.Expression.QueryPlan
{
    /// <summary>
    /// A constant value
    /// </summary>
    public sealed class QueryPlanConstant : IQueryPlan
    {
        #region data

        /// <summary>
        /// A constant valie
        /// </summary>
        public readonly object Constant;

        #endregion

        #region constructor

        /// <summary>
        /// Create a new constant value
        /// </summary>
        /// <param name="myValue">The constant value</param>
        public QueryPlanConstant(object myValue)
        {
            Constant = myValue;
        }

        #endregion
    }
}