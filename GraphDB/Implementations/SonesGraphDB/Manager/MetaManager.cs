/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Linq;
using sones.GraphDB.Manager.Index;
using sones.GraphDB.Manager.Plugin;
using sones.GraphDB.Manager.QueryPlan;
using sones.GraphDB.Manager.TypeManagement;
using sones.GraphDB.Manager.Vertex;
using sones.Library.Commons.VertexStore;
using sones.Library.Settings;
using sones.Library.Commons.Transaction;
using sones.Library.Commons.Security;
using sones.GraphDB.Manager.BaseGraph;
using sones.Library.VersionedPluginManager;

namespace sones.GraphDB.Manager
{
    /// <summary>
    /// A manager that contains all the other managers
    /// to support smaller method signatures
    /// </summary>
    public sealed class MetaManager : IMetaManager
    {
        #region Data

        /// <summary>
        /// The query plan manager
        /// </summary>
        private readonly IQueryPlanManager _queryPlanManager;

        /// <summary>
        /// Gets or sets the myOutgoingEdgeVertex instance of the index manager.
        /// </summary>
        private readonly IIndexManager _indexManager;

        /// <summary>
        /// Gets or sets the myOutgoingEdgeVertex instance of the type manager.
        /// </summary>
        private readonly IManagerOf<IVertexTypeHandler> _vertexTypeManager;

        /// <summary>
        /// The edge type manager
        /// </summary>
        private readonly IManagerOf<IEdgeTypeHandler> _edgeTypeManager;

        /// <summary>
        /// Gets or sets the myOutgoingEdgeVertex instance of the parentVertex manager.
        /// </summary>
        private readonly IManagerOf<IVertexHandler> _vertexManager;

        /// <summary>
        /// Gets or sets the myOutgoingEdgeVertex instance of parentVertex store.
        /// </summary>
        private readonly IVertexStore _vertexStore;

        /// <summary>
        /// The system security token.
        /// </summary>
        private SecurityToken _security = null;

        /// <summary>
        /// The system transaction token.
        /// </summary>
        private TransactionToken _transaction = null;

        /// <summary>
        /// The base type manager.
        /// </summary>
        private BaseTypeManager _baseTypeManager;

        /// <summary>
        /// The base graph storage manager.
        /// </summary>
        private BaseGraphStorageManager _baseGraphStorageManager;

        /// <summary>
        /// The plugin manager.
        /// </summary>
        private AComponentPluginManager _pluginManager;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new meta manager 
        /// </summary>
        /// <param name="myVertexStore">The vertex store on which all other manager rely on</param>
        /// <param name="myPlugins">The plugin definitions</param>
        /// <param name="myPluginManager">Used to load pluginable manager</param>
        /// <param name="myApplicationSettings">The current application settings</param>
        private MetaManager(IVertexStore myVertexStore, IDManager myIDManager, GraphDBPluginManager myPluginManager, GraphDBPlugins myPlugins, GraphApplicationSettings myApplicationSettings)
        {
            _vertexStore = myVertexStore;
            _vertexTypeManager = new VertexTypeManager(myIDManager);
            _vertexManager = new VertexManager(myIDManager);
            _edgeTypeManager = new EdgeTypeManager(myIDManager);
            _queryPlanManager = new QueryPlanManager();
            _indexManager = new IndexManager(myIDManager, myPluginManager, myApplicationSettings, myPlugins.IndexPlugins);
            _baseTypeManager = new BaseTypeManager();
            _baseGraphStorageManager = new BaseGraphStorageManager();
            _pluginManager = myPluginManager;
        }

        #endregion

        #region IMetaManager Members

        public static IMetaManager CreateMetaManager(IVertexStore myVertexStore, IDManager myIDManager, GraphDBPlugins myPlugins, GraphDBPluginManager myPluginManager, GraphApplicationSettings myApplicationSettings, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            var result = new MetaManager(myVertexStore, myIDManager, myPluginManager, myPlugins, myApplicationSettings);
            
            DBCreationManager creationManager = new DBCreationManager(result.SystemSecurityToken, result.SystemTransactionToken, result._baseGraphStorageManager);
            
            if (!creationManager.CheckBaseGraph(myVertexStore))
            {
                creationManager.CreateBaseGraph(myVertexStore);
            }

            creationManager.AddUserDefinedDataTypes(myVertexStore, myPluginManager);
            
            result.Initialize();
            result.Load();

            SetMaxID(myVertexStore, myIDManager, myTransaction, mySecurity, result);            

            return result;
        }

        private static void SetMaxID(IVertexStore myVertexStore, IDManager myIDManager, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager result)
        {
            foreach (var aUserDefinedVertexType in result._vertexTypeManager.ExecuteManager.GetAllVertexTypes(myTransaction, mySecurity))
            {
                myIDManager[aUserDefinedVertexType.ID].SetToMaxID(myVertexStore.GetHighestVertexID(mySecurity, myTransaction, aUserDefinedVertexType.ID) + 1);
            }
        }

        private void Initialize()
        {
            _baseGraphStorageManager.Initialize(this);
            _vertexTypeManager.Initialize(this);
            _vertexManager.Initialize(this);
            _baseTypeManager.Initialize(this);
            _queryPlanManager.Initialize(this);
            _edgeTypeManager.Initialize(this);
            _indexManager.Initialize(this);            
        }

        private void Load()
        {
            _baseGraphStorageManager.Load(SystemTransactionToken, SystemSecurityToken);
            _vertexTypeManager.Load(SystemTransactionToken, SystemSecurityToken);
            _vertexManager.Load(SystemTransactionToken, SystemSecurityToken);
            _baseTypeManager.Load(SystemTransactionToken, SystemSecurityToken);
            _queryPlanManager.Load(SystemTransactionToken, SystemSecurityToken);
            _edgeTypeManager.Load(SystemTransactionToken, SystemSecurityToken);
            _indexManager.Load(SystemTransactionToken, SystemSecurityToken);            
        }

        public IIndexManager IndexManager
        {
            get { return _indexManager; }
        }

        public IManagerOf<IVertexTypeHandler> VertexTypeManager
        {
            get { return _vertexTypeManager; }
        }

        public IManagerOf<IEdgeTypeHandler> EdgeTypeManager
        {
            get { return _edgeTypeManager; }
        }

        public IManagerOf<IVertexHandler> VertexManager
        {
            get { return _vertexManager; }
        }

        public IVertexStore VertexStore
        {
            get { return _vertexStore; }
        }

        public IQueryPlanManager QueryPlanManager
        {
            get { return _queryPlanManager; }
        }

        public BaseTypeManager BaseTypeManager
        {
            get { return _baseTypeManager; }
        }

        public SecurityToken SystemSecurityToken
        {
            get { return _security; }
        }

        public TransactionToken SystemTransactionToken
        {
            get { return _transaction; }
        }

        public BaseGraphStorageManager BaseGraphStorageManager
        {
            get { return _baseGraphStorageManager; }
        }

        public AComponentPluginManager PluginManager
        {
            get { return _pluginManager; }
        }

        #endregion
    }
}