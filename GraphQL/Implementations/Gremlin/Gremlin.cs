using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL;
using sones.GraphQL.Result;
using sones.GraphDB.Transaction;
using sones.Library.Internal.Token;

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

        public QueryResult Query(SessionToken mySessionToken, TransactionToken myTransactionToken, string myQueryString)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> ExportGraphDDL(SessionToken mySessionToken, TransactionToken myTransactionToken)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> ExportGraphDML(SessionToken mySessionToken, TransactionToken myTransactionToken)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
