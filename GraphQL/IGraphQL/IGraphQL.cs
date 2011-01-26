using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.Result;
using sones.GraphDB.Transaction;
using sones.Library.Internal.Token;

namespace sones.GraphQL
{
    /// <summary>
    /// The interface for all graph query languages
    /// </summary>
    public interface IGraphQL : IQueryableLanguage
    {
        /// <summary>
        /// The name of the graph query language (i.e. GQL, SPARQL, ...)
        /// </summary>
        String Name { get; }
    }
}
