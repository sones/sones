using System.Collections.Generic;
using sones.Library.PropertyHyperGraph;
using System;

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
        private readonly QueryPlanProperty _left;

        /// <summary>
        /// The other interesting property
        /// </summary>
        private readonly QueryPlanProperty _right;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new queryplan that processes a complex equals operation without any index
        /// </summary>
        /// <param name="myLeft">The left interesting property</param>
        /// <param name="myRight">The right interesting property</param>
        public QueryPlanEqualsWithoutIndexComplex(QueryPlanProperty myLeft, QueryPlanProperty myRight)
        {
            _left = myLeft;
            _right = myRight;
        }

        #endregion

        #region IQueryPlan Members

        public IEnumerable<IVertex> Execute()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}