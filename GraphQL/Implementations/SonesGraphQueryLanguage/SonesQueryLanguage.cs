using System;
using System.Collections.Generic;
using sones.GraphDB;
using sones.GraphQL.Result;
using sones.Library.Commons.Security;
using sones.Library.Settings;
using sones.Library.Commons.Transaction;
using sones.Library.VersionedPluginManager;
using sones.GraphQL.GQL.Manager.Plugin;

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
        private IGraphDB _IGraphDBInstance;

        /// <summary>
        /// The settings of the application
        /// </summary>
        private readonly GraphApplicationSettings _settings;

        /// <summary>
        /// A manager to dynamically load versioned plugins
        /// </summary>
        private readonly GQLPluginManager _GQLPluginManager;

        #endregion

        #region Constructor

        /// <summary>
        /// The empty constructor, needed to load the query language as plugin.
        /// </summary>
        public SonesQueryLanguage()
        {
        }

        /// <summary>
        /// Creates a new sones GQL instance
        /// </summary>
        /// <param name="myIGraphDBInstace">The graph database instance on which the gql statements are executed</param>
        public SonesQueryLanguage(IGraphDB myIGraphDBInstace)
        {
            _IGraphDBInstance = myIGraphDBInstace;
            _settings = new GraphApplicationSettings(SonesGQLConstants.ApplicationSettingsLocation);

            #region plugin manager

            _GQLPluginManager = new GQLPluginManager();

            #endregion
        }

        #endregion

        #region IGraphQL Members

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

        #region IPluginable Members

        public string PluginName
        {
            get { return "GQL"; }
        }

        public Dictionary<string, Type> SetableParameters
        {
            get
            {
                return new Dictionary<string, Type> 
                { 
                    { "GraphDB", typeof(IGraphDB) }
                };
            }
        }

        public IPluginable InitializePlugin(Dictionary<string, object> myParameters = null)
        {
            IGraphDB dbInstance = null;

            if (myParameters != null)
            {
                if (myParameters.ContainsKey("GraphDB"))
                {
                    dbInstance = (IGraphDB)myParameters["GraphDB"];
                }
            }

            object result = typeof(SonesQueryLanguage).
                GetConstructor(new Type[] { typeof(IGraphDB) }).Invoke(new object[] { dbInstance });

            return (IPluginable)result;
        }

        #endregion
    }
}