using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL;
using sones.GraphQL.Result;

namespace Gremlin
{
    /// <summary>
    /// A Gremlin implementation
    /// </summary>
    public sealed class Gremlin : IGraphQL
    {
        #region IGraphQL

        public QueryResult Query(string myQueryString)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> ExportGraphDDL()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> ExportGraphDML()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
