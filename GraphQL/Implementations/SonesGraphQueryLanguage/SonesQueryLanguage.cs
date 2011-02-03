using System;
using System.Collections.Generic;
using sones.GraphDB;
using sones.GraphDB.Security;
using sones.GraphQL.Result;

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

        public string Name
        {
            get { return "GQL"; }
        }

        public QueryResult Query(SecurityToken mySecurityToken, GraphDB.Transaction.TransactionToken myTransactionToken, string myQueryString)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> ExportGraphDDL(SecurityToken mySecurityToken, GraphDB.Transaction.TransactionToken myTransactionToken)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> ExportGraphDML(SecurityToken mySecurityToken, GraphDB.Transaction.TransactionToken myTransactionToken)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
