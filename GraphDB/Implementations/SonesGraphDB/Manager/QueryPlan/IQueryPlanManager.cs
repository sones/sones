using sones.GraphDB.Expression;
using sones.GraphDB.Expression.QueryPlan;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;

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
        /// <param name="myIsLongRunning">Determines whether it is anticipated that the request could take longer</param>
        /// <param name="myTransaction">The current transaction token</param>
        /// <param name="mySecurity">The current transaction token</param>
        /// <returns>A queryplan</returns>
        IQueryPlan CreateQueryPlan(IExpression myExpression, bool myIsLongRunning, TransactionToken myTransaction, SecurityToken mySecurity);
    }
}
