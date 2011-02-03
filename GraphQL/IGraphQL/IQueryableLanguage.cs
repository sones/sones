using System;
using sones.GraphDB.Security;
using sones.GraphDB.Transaction;
using sones.GraphQL.Result;

namespace sones.GraphQL
{
    public interface IQueryableLanguage : IDumpable
    {
        /// <summary>
        /// Returns a query result by passing a query string
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myQueryString">The query string that should be executed</param>
        /// <returns>A query result</returns>
        QueryResult Query(SecurityToken mySecurityToken, TransactionToken myTransactionToken
            , String myQueryString);
    }
}
