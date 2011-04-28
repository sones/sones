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
        private readonly IVertexTypeManager _vertexTypeManager;

        /// <summary>
        /// Gets or sets the myOutgoingEdgeVertex instance of the parentVertex manager.
        /// </summary>
        private readonly IVertexManager _vertexManager;

        /// <summary>
        /// Gets or sets the myOutgoingEdgeVertex instance of parentVertex store.
        /// </summary>
        private readonly IVertexStore _vertexStore;

        /// <summary>
        /// The edge type manager
        /// </summary>
        private readonly IEdgeTypeManager _edgeTypeManager;

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
        public MetaManager(IVertexStore myVertexStore, GraphDBPlugins myPlugins, GraphDBPluginManager myPluginManager, GraphApplicationSettings myApplicationSettings, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            _transaction = myTransaction;
            _security = mySecurity;

            _vertexStore = myVertexStore;

            #region IndexManager

            _indexManager = new IndexManager(_vertexStore, myPluginManager, myPlugins.IndexPlugins);

            #endregion

            #region vertex(Type)Manager

            var vertexTypeManager = new VertexTypeManager();
            var vertexManager = new VertexManager();


            vertexManager.SetVertexStore(myVertexStore);
            vertexManager.SetIndexManager(_indexManager);
            vertexManager.SetVertexTypeManager(vertexTypeManager);
            vertexManager.SetQueryPlanManager(new QueryPlanManager(vertexTypeManager, _vertexStore, _indexManager));

            _vertexTypeManager = vertexTypeManager;
            _vertexManager = vertexManager;

            vertexTypeManager.Initialize(_indexManager, vertexManager, _transaction, _security);
            #endregion

            #region queryPlanManager

            _queryPlanManager = new QueryPlanManager(_vertexTypeManager, _vertexStore, _indexManager);

            #endregion
        }

        #endregion

        #region IMetaManager Members

        public IIndexManager IndexManager
        {
            get { return _indexManager; }
        }

        public IVertexTypeManager VertexTypeManager
        {
            get { return _vertexTypeManager; }
        }

        public IVertexManager VertexManager
        {
            get { return _vertexManager; }
        }

        public IVertexStore VertexStore
        {
            get { return _vertexStore; }
        }

        public IEdgeTypeManager EdgeTypeManager
        {
            get { return _edgeTypeManager; }
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