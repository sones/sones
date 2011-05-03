using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Manager.Index;
using sones.GraphDB.Manager.TypeManagement;
using sones.GraphDB.Manager.Vertex;
using sones.Library.Commons.VertexStore;
using sones.GraphDB.Manager.QueryPlan;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;

namespace sones.GraphDB.Manager
{
    /// <summary>
    /// A meta manager that aggregates all GraphDB manager
    /// </summary>
    public interface IMetaManager
    {
        /// <summary>
        /// The interface to the query plan manager
        /// </summary>
        IQueryPlanManager QueryPlanManager { get; }

        /// <summary>
        /// The interface to the indices
        /// </summary>
        IIndexManager IndexManager { get; }

        /// <summary>
        /// The interface to the vertex types
        /// </summary>
        IManagerOf<IVertexTypeHandler> VertexTypeManager { get; }

        /// <summary>
        /// The managed interface to the vertices
        /// </summary>
        IManagerOf<IVertexHandler> VertexManager { get; }

        /// <summary>
        /// The raw interface to the interfaces
        /// </summary>
        IVertexStore VertexStore { get; }

        /// <summary>
        /// The interface to the edge types
        /// </summary>
        IManagerOf<IEdgeTypeHandler> EdgeTypeManager { get; }

        /// <summary>
        /// The security token for graph db intern usage.
        /// </summary>
        SecurityToken SystemSecurityToken { get; }

        /// <summary>
        /// The transaction token for graph db intern usage.
        /// </summary>
        TransactionToken SystemTransactionToken { get; }
    }
}
