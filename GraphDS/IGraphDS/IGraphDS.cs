using System;
using sones.GraphFS;
using sones.Library.Internal.Token;
using sones.GraphDB;
using sones.GraphQL;
using sones.Library.Internal.Security;
using sones.GraphQL.Result;
using sones.GraphDB.Transaction;

namespace sones.GraphDS
{
    /// <summary>
    /// The interface for all graphDS
    /// </summary>
    public interface IGraphDS : IGraphDB
    {
        /// <summary>
        /// Shutdown of the current database
        /// </summary>
        /// <param name="mySessionToken"></param>
        void Shutdown(SessionToken mySessionToken);

        /// <summary>
        /// Returns a query result by passing a query string
        /// </summary>
        /// <param name="mySessionToken">The current session token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myQueryString">The query string that should be executed</param>
        /// <param name="myQueryLanguageName">The identifier of the language that should be used for parsing the query</param>
        /// <returns>A query result</returns>
        QueryResult Query(SessionToken mySessionToken, TransactionToken myTransactionToken,
            String myQueryString,
            String myQueryLanguageName);
    }
}
