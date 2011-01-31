using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL;
using sones.GraphQL.Result;
using sones.GraphDB.Transaction;
using sones.Library.Internal.Security;


namespace Gremlin
{
    /// <summary>
    /// A Gremlin implementation
    /// </summary>
    public sealed class Gremlin : IGraphQL
    {
        #region IGraphQL

        public string Name
        {
            get { return "Gremlin"; }
        }

        public QueryResult Query(SecurityToken mySecurityToken, TransactionToken myTransactionToken, string myQueryString)
        {
            throw new NotImplementedException();
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
