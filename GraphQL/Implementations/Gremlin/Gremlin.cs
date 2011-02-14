using System;
using System.Collections.Generic;
using sones.GraphQL;
using sones.GraphQL.Result;
using sones.Security;
using sones.Transaction;

namespace Gremlin
{
    /// <summary>
    /// A Gremlin implementation
    /// </summary>
    public sealed class Gremlin : IGraphQL
    {
        #region IGraphQL Members

        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IQueryableLanguage Members

        public QueryResult Query(SecurityToken mySecurityToken, TransactionToken myTransactionToken, string myQueryString)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDumpable Members

        public IEnumerable<string> ExportGraphDDL(SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> ExportGraphDML(SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
