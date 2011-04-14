using System;
using System.Collections.Generic;
using sones.GraphDB.Expression;
using sones.Library.PropertyHyperGraph;
using sones.Library.Security;
using sones.Library.Transaction;
using sones.Library.VertexStore.Definitions;

namespace sones.GraphDB.Manager.Vertex
{
    /// <summary>
    /// This interface represents a parentVertex manager.
    /// </summary>
    /// The responibilities of the parentVertex manager is an optimized access to the underlying parentVertex store (FS).
    public interface IVertexManager
    {
        /// <summary>
        /// Returns the list of vertices that matches the expression.
        /// </summary>
        /// <param name="myExpression">An logical expression tree. Migth be unoptimized.</param>
        /// <param name="myIsLongrunning">Determines whether it is anticipated that the request could take longer.</param>
        /// <param name="myTransactionToken">A transaction token for this operation.</param>
        /// <param name="mySecurityToken">A security token for this operation.</param>
        /// <returns>
        /// A possible emtpy list of vertices that matches the expression. The myResult is never <c>NULL</c>.
        /// Any implementation should try to optimize the way the underlying parentVertex store and indices are used to get the myResult.
        /// </returns>
        IEnumerable<IVertex> GetVertices(IExpression myExpression, Boolean myIsLongrunning, TransactionToken myTransactionToken, SecurityToken mySecurityToken);

        IVertex GetSingleVertex(IExpression myExpression, TransactionToken myTransactionToken, SecurityToken mySecurityToken);

        void CanGetVertices(IExpression myExpression, bool myIsLongrunning, TransactionToken TransactionToken, SecurityToken SecurityToken);

        IVertex AddVertex(VertexAddDefinition myVertexDefinition, TransactionToken TransactionToken, SecurityToken SecurityToken);
    }
}
