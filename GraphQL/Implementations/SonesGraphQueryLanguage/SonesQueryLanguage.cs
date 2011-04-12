using System;
using System.Collections.Generic;
using sones.GraphDB;
using sones.GraphQL.Result;
using sones.Library.Security;
using sones.Library.Transaction;
using sones.Library.Settings;

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

        /// <summary>
        /// The settings of the application
        /// </summary>
        private readonly GraphApplicationSettings _settings;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new sones GQL instance
        /// </summary>
        /// <param name="myApplicationSettings">The settings of the application</param>
        /// <param name="myIGraphDBInstace">The graph database instance on which the gql statements are executed</param>
        public SonesQueryLanguage(GraphApplicationSettings myApplicationSettings, IGraphDB myIGraphDBInstace)
        {
            _IGraphDBInstance = myIGraphDBInstace;
            _settings = myApplicationSettings;
        }

        #endregion

        #region IGraphQL Members

        public string Name
        {
            get { return "GQL"; }
        }

        public QueryResult Query(SecurityToken mySecurityToken, TransactionToken myTransactionToken,
                                 string myQueryString)
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