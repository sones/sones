#region Usings

using System;
using sones.GraphDB.Result;

#endregion

namespace sones.GraphDB.Interfaces
{
    /// <summary>
    /// marks a grammar as query able language
    /// </summary>
    public interface IQueryableLanguage
    {
        /// <summary>
        /// Execute a query and return the result
        /// </summary>
        /// <param name="myQueryScript">The query string</param>
        /// <param name="myGraphDBSession">The db session</param>
        /// <returns>The result of the query as QueryResult</returns>
        QueryResult Query(String myQueryScript, IGraphDBSession myGraphDBSession);
    }
}
