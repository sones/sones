using System;
using sones.GraphDB.Expression;

namespace sones.GraphDB.ErrorHandling.QueryPlan
{
    /// <summary>
    /// An invalid query plan execution exception occured
    /// </summary>
    public sealed class InvalidQueryPlanExecutionException : AGraphDBException
    {
        #region data

        /// <summary>
        /// A description concerning the exception
        /// </summary>
        public readonly String Info;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new invalid query plan execution exception
        /// </summary>
        /// <param name="myInfo">A description concerning the exception</param>
        public InvalidQueryPlanExecutionException(String myInfo)
        {
            Info = myInfo;
            _msg = Info;
        }

        #endregion
    }
}
