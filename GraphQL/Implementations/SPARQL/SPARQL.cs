using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.Result;

namespace sones.GraphQL
{
    /// <summary>
    /// A SPARQL implementation
    /// </summary>
    public sealed class SPARQL : IGraphQL
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
