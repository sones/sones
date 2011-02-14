using System;
using sones.GraphDB;
using sones.GraphQL.Result;
using sones.Security;
using sones.Transaction;

namespace sones.GraphDS
{
    /// <summary>
    /// The interface for all graphDS
    /// </summary>
    public interface IGraphDS : IUserAuthentication
    {
        /// <summary>
        /// The interface to the graph database
        /// </summary>
        IGraphDB GraphDB { get; }

        /// <summary>
        /// Shutdown of the current database
        /// </summary>
        /// <param name="mySecurityToken"></param>
        void Shutdown(SecurityToken mySecurityToken);

        /// <summary>
        /// Returns a query result by passing a query string
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myQueryString">The query string that should be executed</param>
        /// <param name="myQueryLanguageName">The identifier of the language that should be used for parsing the query</param>
        /// <returns>A query result</returns>
        QueryResult Query(SecurityToken mySecurityToken, TransactionToken myTransactionToken,
            String myQueryString,
            String myQueryLanguageName);
    }
}
