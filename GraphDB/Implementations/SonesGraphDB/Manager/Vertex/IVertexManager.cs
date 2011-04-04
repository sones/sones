using System.Collections.Generic;
using sones.Library.PropertyHyperGraph;
using sones.Library.Security;
using sones.Library.Transaction;
using sones.GraphDB.Expression;

namespace sones.GraphDB.Manager.Vertex
{
    /// <summary>
    /// This interface represents a vertex manager.
    /// </summary>
    /// The responibilities of the vertex manager is an optimized access to the underlying vertex store (FS).
    public interface IVertexManager
    {
        /// <summary>
        /// Returns the list of vertices that matches the expression.
        /// </summary>
        /// <param name="myExpression">An logical expression tree. Migth be unoptimized.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The current meta manager.</param>
        /// <returns>
        /// A possible emtpy list of vertices that matches the expression. The result is never <c>NULL</c>.
        /// Any implementation should try to optimize the way the underlying vertex store and indices are used to get the result.
        /// </returns>
        IEnumerable<IVertex> GetVertex(IExpression myExpression, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);
    }
}
