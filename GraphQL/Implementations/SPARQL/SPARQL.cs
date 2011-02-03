using System;
using System.Collections.Generic;
using sones.GraphQL.Result;
using sones.Security;
using sones.Transaction;

namespace sones.GraphQL
{
    /// <summary>
    /// A SPARQL implementation
    /// </summary>
    public sealed class SPARQL : IGraphQL
    {
        #region IGraphQL

        public QueryResult Query(SecurityToken mySecurityToken, TransactionToken myTransactionToken, string myQueryString)
        {
            throw new NotImplementedException();
        }

        public string Name
        {
            get { return "SPARQL"; }
        }

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
