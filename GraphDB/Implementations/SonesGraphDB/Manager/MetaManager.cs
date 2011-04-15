using System;
using sones.GraphDB.Manager.Index;
using sones.GraphDB.Manager.Plugin;
using sones.GraphDB.Manager.TypeManagement;
using sones.GraphDB.Manager.Vertex;
using sones.Library.VertexStore;
using sones.Library.Settings;

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

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new meta manager 
        /// </summary>
        /// <param name="myVertexStore">The vertex store on which all other manager rely on</param>
        /// <param name="myPlugins">The plugin definitions</param>
        /// <param name="myPluginManager">Used to load pluginable manager</param>
        /// <param name="myApplicationSettings">The current application settings</param>
        public MetaManager(IVertexStore myVertexStore, GraphDBPlugins myPlugins, GraphDBPluginManager myPluginManager, GraphApplicationSettings myApplicationSettings)
        {
            _vertexStore = myVertexStore;

            #region IndexManager

            var indexManager = new IndexManager(myPluginManager, myPlugins.IndexPlugins);

            #endregion

            #region vertex(Type)Manager

            var vertexTypeManager = new VertexTypeManager();
            var vertexManager = new VertexManager();

            vertexTypeManager.SetIndexManager(indexManager);
            vertexTypeManager.SetVertexManager(vertexManager);

            vertexManager.SetVertexStore(myVertexStore);
            vertexManager.SetIndexManager(indexManager);
            vertexManager.SetVertexTypeManager(vertexTypeManager);

            _vertexTypeManager = vertexTypeManager;
            _vertexManager = vertexManager;

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

        #endregion
    }
}