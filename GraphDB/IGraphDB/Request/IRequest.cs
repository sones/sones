
namespace sones.GraphDB.Request
{
    /// <summary>
    /// A generic interface for requests
    /// </summary>
    /// <typeparam name="TResult">The result type of the request</typeparam>
    public interface IRequest<TResult>
    {
        /// <summary>
        /// The access mode for this request
        /// </summary>
        GraphDBAccessModeEnum AccessMode { get; }

        /// <summary>
        /// Generates the desired result
        /// </summary>
        /// <returns>A generic result</returns>
        TResult GenerateResult();

        /// <summary>
        /// Sets the statistics
        /// </summary>
        /// <param name="myRequestStatistics">The request statistics</param>
        void SetStatistics(IRequestStatistics myRequestStatistics);
    }
}
