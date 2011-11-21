using System;
using System.Collections.Generic;
using sones.Library.ErrorHandling;

namespace sones.GraphQL.Result
{
    /// <summary>
    /// This interface hold all the data that comes out of the database after a query is run
    /// </summary>
    public interface IQueryResult
    {
        /// <summary>
        /// The time that was spent on executing the query
        /// </summary>
        ulong Duration { get; }

        /// <summary>
        /// An error that occured during the query process
        /// </summary>
        ASonesException Error { get; }

        /// <summary>
        /// The name of the query language that has been executed
        /// </summary>
        string NameOfQuerylanguage { get; }

        /// <summary>
        /// The number of affected vertices
        /// </summary>
        ulong NumberOfAffectedVertices { get; }

        /// <summary>
        /// The query that has been executed
        /// </summary>
        string Query { get; }

        /// <summary>
        /// The ReasultType of the executed query
        /// </summary>
        ResultType TypeOfResult { get; }

        /// <summary>
        /// The vertices that are contained in thisIQueryResult
        /// </summary>
        IEnumerable<IVertexView> Vertices { get; }
    }
}
