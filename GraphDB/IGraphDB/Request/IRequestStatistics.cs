using System;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// The interface for all kinds of request statistics (execution time, used indices, cash hit ratio)
    /// </summary>
    public interface IRequestStatistics
    {
        /// <summary>
        /// The time to execute a request
        /// </summary>
        TimeSpan ExecutionTime { get; }
    }
}