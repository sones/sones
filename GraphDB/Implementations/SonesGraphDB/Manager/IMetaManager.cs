using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Manager.Index;
using sones.GraphDB.Manager.TypeManagement;
using sones.GraphDB.Manager.Vertex;
using sones.Library.Commons.VertexStore;

namespace sones.GraphDB.Manager
{
    public interface IMetaManager
    {
        /// <summary>
        /// The interface to the indices
        /// </summary>
        IIndexManager IndexManager { get; }

        /// <summary>
        /// The interface to the vertex types
        /// </summary>
        IVertexTypeManager VertexTypeManager { get; }

        /// <summary>
        /// The managed interface to the vertices
        /// </summary>
        IVertexManager VertexManager { get; }

        /// <summary>
        /// The raw interface to the interfaces
        /// </summary>
        IVertexStore VertexStore { get; }

        /// <summary>
        /// The interface to the edge types
        /// </summary>
        IEdgeTypeManager EdgeTypeManager { get; }
    }
}
