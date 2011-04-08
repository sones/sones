using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.Expression.QueryPlan
{
    /// <summary>
    /// An equals operation without any index
    /// </summary>
    public sealed class QueryPlanEqualsWithoutIndex : IQueryPlan
    {
        #region data

        /// <summary>
        /// The interesting property
        /// </summary>
        public readonly QueryPlanProperty Property;

        /// <summary>
        /// The constant value
        /// </summary>
        public readonly QueryPlanConstant Constant;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new queryplan that processes an equals operation without any index
        /// </summary>
        /// <param name="myProperty">The interesting property</param>
        /// <param name="myConstant">The constant value</param>
        public QueryPlanEqualsWithoutIndex(QueryPlanProperty myProperty, QueryPlanConstant myConstant)
        {
            Property = myProperty;
            Constant = myConstant;
        }

        #endregion
    }
}