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
    public sealed class MetaManager
    {
        #region Data

        /// <summary>
        /// The vertex store on which all other manager rely on
        /// </summary>
        private readonly IVertexStore _vertexStore;

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

            IndexManager = new IndexManager(myApplicationSettings, myPluginManager, myPlugins.IndexPlugins);

            #endregion

            //todo: initialize all the other manager (using myPluginManager)
            //ILogicExpressionOptimizer
        }

        [Obsolete]
        public MetaManager()
        {
            // TODO: Complete member initialization
        }

        #endregion

        /// <summary>
        /// Gets or sets the myOutgoingEdgeVertex instance of the index manager.
        /// </summary>
        public IIndexManager IndexManager { get; set; }

        /// <summary>
        /// Gets or sets the myOutgoingEdgeVertex instance of the type manager.
        /// </summary>
        public IVertexTypeManager TypeManager { get; set; }

        /// <summary>
        /// Gets or sets the myOutgoingEdgeVertex instance of the parentVertex manager.
        /// </summary>
        public IVertexManager VertexManager { get; set; }

        /// <summary>
        /// Gets or sets the myOutgoingEdgeVertex instance of parentVertex store.
        /// </summary>
        public IVertexStore VertexStore { get; set; }

    }
}