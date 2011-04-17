using System.Collections.Generic;
using sones.Library.PropertyHyperGraph;
using System;

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
        private readonly QueryPlanProperty _property;

        /// <summary>
        /// The constant value
        /// </summary>
        private readonly QueryPlanConstant _constant;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new queryplan that processes an equals operation without any index
        /// </summary>
        /// <param name="myProperty">The interesting property</param>
        /// <param name="myConstant">The constant value</param>
        public QueryPlanEqualsWithoutIndex(QueryPlanProperty myProperty, QueryPlanConstant myConstant)
        {
            _property = myProperty;
            _constant = myConstant;
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