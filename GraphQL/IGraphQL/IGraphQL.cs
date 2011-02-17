using System;

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