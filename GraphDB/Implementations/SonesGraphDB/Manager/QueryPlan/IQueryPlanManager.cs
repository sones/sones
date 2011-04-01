using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Expression;
using sones.GraphDB.Expression.QueryPlan;

namespace sones.GraphDB.Manager.QueryPlan
{
    /// <summary>
    /// The interface for all query plan manager
    /// It's main task is to convert an expression into a queryplan
    /// </summary>
    public interface IQueryPlanManager
    {
        /// <summary>
        /// Creates a queryplan from an expression
        /// </summary>
        /// <param name="myExpression">The expression that is going to be transfered into a queryplan</param>
        /// <returns>A queryplan</returns>
        IQueryPlan CreateQueryPlan(IExpression myExpression);
    }
}
