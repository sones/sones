
namespace sones.GraphDB.Request
{
    /// <summary>
    /// A generic interface for requests
    /// </summary>
    /// <typeparam name="TResult">The result type of the request</typeparam>
    public interface IRequest
    {
        /// <summary>
        /// The access mode for this request
        /// </summary>
        GraphDBAccessMode AccessMode { get; }

        /// <summary>
        /// Sets the statistics
        /// </summary>
        /// <param name="myRequestStatistics">The request statistics</param>
        void SetStatistics(IRequestStatistics myRequestStatistics);
    }
}
