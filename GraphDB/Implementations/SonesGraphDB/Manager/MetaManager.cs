using System;
using sones.GraphDB.Manager.Index;
using sones.GraphDB.Manager.Plugin;
using sones.GraphDB.Manager.QueryPlan;
using sones.GraphDB.Manager.TypeManagement;
using sones.GraphDB.Manager.Vertex;
using sones.Library.Commons.VertexStore;
using sones.Library.Settings;
using sones.Library.Commons.Transaction;
using sones.Library.Commons.Security;

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
        private SecurityToken _security;

        /// <summary>
        /// The system transaction token.
        /// </summary>
        private TransactionToken _transaction;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new meta manager 
        /// </summary>
        /// <param name="myVertexStore">The vertex store on which all other manager rely on</param>
        /// <param name="myPlugins">The plugin definitions</param>
        /// <param name="myPluginManager">Used to load pluginable manager</param>
        /// <param name="myApplicationSettings">The current application settings</param>
        private MetaManager(IVertexStore myVertexStore, IDManager myIDManager, GraphDBPluginManager myPluginManager, GraphDBPlugins myPlugins)
        {
            _vertexStore = myVertexStore;
            _vertexTypeManager = new VertexTypeManager(myIDManager);
            _vertexManager = new VertexManager(myIDManager);
            _edgeTypeManager = new EdgeTypeManager(myIDManager);
            _queryPlanManager = new QueryPlanManager();
            _indexManager = new IndexManager(myIDManager, myPluginManager, myPlugins.IndexPlugins);
        }

        #endregion

        #region IMetaManager Members

        public static IMetaManager CreateMetaManager(IVertexStore myVertexStore, IDManager myIDManager, GraphDBPlugins myPlugins, GraphDBPluginManager myPluginManager, GraphApplicationSettings myApplicationSettings, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            var result = new MetaManager(myVertexStore, myIDManager, myPluginManager, myPlugins);

            DBCreationManager creationManager = new DBCreationManager(result.SystemSecurityToken, result.SystemTransactionToken);
            if (!creationManager.CheckBaseGraph(myVertexStore))
            {
                creationManager.CreateBaseGraph(myVertexStore);
            }


            result.Initialize();
            result.Load();

            return result;
        }

        private void Initialize()
        {
            _vertexTypeManager.Initialize(this);
            _vertexManager.Initialize(this);
            _queryPlanManager.Initialize(this);
            _edgeTypeManager.Initialize(this);
            _indexManager.Initialize(this);
        }

        private void Load()
        {
            _vertexTypeManager.Load(SystemTransactionToken, SystemSecurityToken);
            _vertexManager.Load(SystemTransactionToken, SystemSecurityToken);
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

        public SecurityToken SystemSecurityToken
        {
            get { return _security; }
        }

        public TransactionToken SystemTransactionToken
        {
            get { return _transaction; }
        }

        #endregion
    }
}