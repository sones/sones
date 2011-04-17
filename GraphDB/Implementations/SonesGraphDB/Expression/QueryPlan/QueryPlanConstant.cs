using sones.Library.PropertyHyperGraph;
using System.Collections.Generic;
using System;
using sones.GraphDB.ErrorHandling.QueryPlan;

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

        #region IQueryPlan Members

        public IEnumerable<IVertex> Execute()
        {
            throw new InvalidQueryPlanExecutionException("It is not possible to execute a query plan constant.");
        }

        #endregion
    }
}