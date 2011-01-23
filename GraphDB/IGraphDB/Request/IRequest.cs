using System;
using System.Collections.Generic;
using sones.Library.Internal.Security;

namespace sones.GraphDB.Request
{
    /// <summary>
    /// A generic interface for requests
    /// </summary>
    /// <typeparam name="TResult">The result type of the request</typeparam>
    public interface IRequest<TResult>
    {
        GraphDBAccessModeEnum AccessMode { get; }

        /// <summary>
        /// Generates the desired result
        /// </summary>
        /// <returns>A generic result</returns>
        TResult GenerateResult();
    }
}
