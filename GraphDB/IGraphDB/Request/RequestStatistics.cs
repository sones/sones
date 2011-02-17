using System;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// this class contains some statistic information concerning the execution of a request
    /// </summary>
    public sealed class RequestStatistics : IRequestStatistics
    {
        #region properties

        /// <summary>
        /// The time to execute a request
        /// </summary>
        private readonly TimeSpan _executionTime;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates some new request statistcs
        /// </summary>
        /// <param name="myExecutionTime"></param>
        public RequestStatistics(TimeSpan myExecutionTime)
        {
            _executionTime = myExecutionTime;
        }

        #endregion

        #region IRequestStatistics Members

        TimeSpan IRequestStatistics.ExecutionTime
        {
            get { return _executionTime; }
        }

        #endregion
    }
}