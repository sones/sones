using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.Expression.QueryPlan
{
    /// <summary>
    /// A compley equals operation without any index
    /// </summary>
    public sealed class QueryPlanEqualsWithoutIndexComplex : IQueryPlan
    {
        #region data

        /// <summary>
        /// The interesting property
        /// </summary>
        public readonly QueryPlanProperty Left;

        /// <summary>
        /// The other interesting property
        /// </summary>
        public readonly QueryPlanProperty Right;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new queryplan that processes a complex equals operation without any index
        /// </summary>
        /// <param name="myLeft">The left interesting property</param>
        /// <param name="myRight">The right interesting property</param>
        public QueryPlanEqualsWithoutIndexComplex(QueryPlanProperty myLeft, QueryPlanProperty myRight)
        {
            Left = myLeft;
            Right = myRight;
        }

        #endregion
    }
}