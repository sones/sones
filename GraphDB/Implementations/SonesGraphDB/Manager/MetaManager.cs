using sones.GraphDB.Manager.Index;
using sones.GraphDB.Manager.TypeManagement;
using sones.GraphDB.Manager.Vertex;
using sones.Library.VertexStore;

namespace sones.GraphDB.Manager
{
    /// <summary>
    /// A manager that contains all the other managers
    /// to support smaller method signatures
    /// </summary>
    public sealed class MetaManager
    {
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