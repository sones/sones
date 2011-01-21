using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.Result;

namespace sones.GraphQL
{
    /// <summary>
    /// The interface for all graph query languages
    /// </summary>
    public interface IGraphQL : IDumpable
    {
        /// <summary>
        /// Returns a query result by passing a query string
        /// </summary>
        /// <param name="myQueryString">The query string that should be executed</param>
        /// <returns>A query result</returns>
        QueryResult Query(String myQueryString);
    }
}
