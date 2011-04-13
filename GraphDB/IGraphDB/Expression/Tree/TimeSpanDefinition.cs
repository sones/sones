using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Expression.Tree
{
    /// <summary>
    /// A datastructure which defines a timespan
    /// </summary>
    public sealed class TimeSpanDefinition
    {
        #region data

        /// <summary>
        /// The point in time where the timespan begins
        /// </summary>
        public readonly DateTime From;

        /// <summary>
        /// The point in time where the timespan ends
        /// </summary>
        public readonly DateTime To;

        #endregion

        /// <summary>
        /// Creates a new timespan definition
        /// </summary>
        /// <param name="myFrom">The point in time where the timespan begins</param>
        /// <param name="myTo">The point in time where the timespan ends</param>
        public TimeSpanDefinition(DateTime myFrom, DateTime myTo)
        {
            From = myFrom;
            To = myTo;
        }
    }
}
