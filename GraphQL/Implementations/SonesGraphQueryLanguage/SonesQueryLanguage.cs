using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL;
using sones.GraphQL.Result;
using sones.GraphDB;

namespace sones.GraphQL
{
    /// <summary>
    /// The sones query language
    /// </summary>
    public sealed class SonesQueryLanguage : IGraphQL
    {

        #region Data

        /// <summary>
        /// The IGraphDB instance for accessing the graph database
        /// </summary>
        private readonly IGraphDB _IGraphDBInstance;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new sones GQL instance
        /// </summary>
        /// <param name="myIGraphDBInstace"></param>
        public SonesQueryLanguage(IGraphDB myIGraphDBInstace)
        {
            _IGraphDBInstance = myIGraphDBInstace;
        }

        #endregion


        #region IGraphQL

        public IEnumerable<string> ExportGraphDDL()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> ExportGraphDML()
        {
            throw new NotImplementedException();
        }

        public QueryResult Query(string myQueryString)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
