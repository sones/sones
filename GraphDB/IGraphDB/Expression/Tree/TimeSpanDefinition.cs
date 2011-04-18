using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.Library.PropertyHyperGraph;

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
        /// The converted from value
        /// </summary>
        private readonly long _fromConverted;

        /// <summary>
        /// The point in time where the timespan ends
        /// </summary>
        public readonly DateTime To;

        /// <summary>
        /// The converted to value
        /// </summary>
        private readonly long _toConverted;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new timespan definition
        /// </summary>
        /// <param name="myFrom">The point in time where the timespan begins</param>
        /// <param name="myTo">The point in time where the timespan ends</param>
        public TimeSpanDefinition(DateTime myFrom, DateTime myTo)
        {
            From = myFrom;
            _fromConverted = From.ToBinary();

            To = myTo;
            _toConverted = To.ToBinary();
        }

        #endregion

        #region public methods
        
        /// <summary>
        /// Checks wheter a given vertex revision id is valid for this timespan
        /// </summary>
        /// <param name="myToBeCheckedID">The to be checked id</param>
        /// <returns>True or false</returns>
        public bool IsWithinTimeStamp(Int64 myToBeCheckedID)
        {
            return myToBeCheckedID >= _fromConverted && myToBeCheckedID <= _toConverted;
        }

        #endregion
    }
}
