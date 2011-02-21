using System;
namespace sones.GraphDB.Request
{
    /// <summary>
    /// A generic interface for requests
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        /// The access mode for this request
        /// </summary>
        GraphDBAccessMode AccessMode { get; }
    }
}