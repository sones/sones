using sones.GraphDB.Request;

namespace sones.GraphDB.Manager
{
    /// <summary>
    /// The interface for all request scheduler
    /// </summary>
    public interface IRequestScheduler
    {
        /// <summary>
        /// Decides whether a request can be executed in parallel or not
        /// </summary>
        /// <param name="myRequest">The request which is analyzed</param>
        /// <returns>True for parallel execution, otherwise false</returns>
        bool ExecuteRequestInParallel(IRequest myRequest);
    }
}