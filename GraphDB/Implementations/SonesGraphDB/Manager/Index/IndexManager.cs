using sones.GraphDB.TypeSystem;
using sones.GraphDB.Manager.Plugin;
using System.Collections.Generic;
using sones.Library.VersionedPluginManager;
using sones.Library.Settings;
using System.Linq;
using System;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.Commons.VertexStore;
using sones.Plugins.Index.Interfaces;
using sones.GraphDB.Request.CreateVertexTypes;

namespace sones.GraphDB.Manager.Index
{
    /// <summary>
    /// This class represents an index manager.
    /// </summary>
    /// The responsibilities of the index manager are creating, removing und retrieving of indices.
    /// Each database has one index manager.
    public sealed class IndexManager : IIndexManager
    {
        #region data

        /// <summary>
        /// The plugin manager that is used to generate new instances of indices
        /// </summary>
        private readonly GraphDBPluginManager _pluginManager;

        /// <summary>
        /// The potential parameters for plugin-indices
        /// </summary>
        private readonly Dictionary<string, PluginDefinition> _indexPluginParameter;

        private readonly IVertexStore _vertexStore;

        #endregion

        #region constructor

        /// <summary>
        /// Create a new index manager
        /// </summary>
        /// <param name="myVertexStore">The vertex store of the graphDB</param>
        /// <param name="myPluginManager">The sones graphDB plugin manager</param>
        /// <param name="myPluginDefinitions">The parameters for plugin-indices</param>
        public IndexManager(IVertexStore myVertexStore, GraphDBPluginManager myPluginManager, HashSet<PluginDefinition> myPluginDefinitions = null)
        {
            _vertexStore = myVertexStore;

            _pluginManager = myPluginManager;

            _indexPluginParameter = myPluginDefinitions != null 
                ? myPluginDefinitions.ToDictionary(key => key.NameOfPlugin, value => value) 
                : new Dictionary<string, PluginDefinition>();
        }

        #endregion

        #region IIndexManager Members

        public IIndexDefinition CreateIndex(IndexPredefinition myIndexDefinition, SecurityToken mySecurityToken, TransactionToken myTransactionToken, bool myIsUserDefined = true)
        {
            throw new NotImplementedException();
        }

        public bool HasIndex(IVertexType myVertexType, IPropertyDefinition myPropertyDefinition, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IIndex<IComparable, long>> GetIndices(IVertexType myVertexType, IPropertyDefinition myPropertyDefinition, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            throw new NotImplementedException();
        }

        public IIndex<IComparable, long> GetIndex(IVertexType myVertexType, IList<IPropertyDefinition> myPropertyDefinition, SecurityToken mySecurityToken, TransactionToken myTransactionToken)
        {
            throw new NotImplementedException();
        }

        public string GetBestMatchingIndexName(bool myIsSingleValue, bool myIsRange, bool myIsVersioned)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
