using System;
using System.Collections.Generic;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// A generic interface for requests
    /// </summary>
    /// <typeparam name="TResult">The result type of the request</typeparam>
    public interface IRequest<TResult>
    {
        /// <summary>
        /// Generates the desired result
        /// </summary>
        /// <returns>A generic result</returns>
        TResult GenerateResult();
    }
}
