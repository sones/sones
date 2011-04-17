using System;
using System.Collections.Generic;
using sones.GraphDB.Expression;
using sones.Library.PropertyHyperGraph;
using sones.Library.Security;
using sones.Library.Transaction;
using sones.Library.VertexStore.Definitions;
using sones.GraphDB.Expression.Tree;

namespace sones.GraphDB.Manager.Vertex
{
    /// <summary>
    /// This interface represents a parentVertex manager.
    /// </summary>
    /// The responibilities of the parentVertex manager is an optimized access to the underlying parentVertex store (FS).
    public interface IVertexManager
    {
        #region Get Vertices

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

        /// <summary>
        /// Checks. whether it is possible to get the specified vertices 
        /// </summary>
        /// <param name="myExpression">An logical expression tree. Migth be unoptimized.</param>
        /// <param name="myIsLongrunning">Determines whether it is anticipated that the request could take longer.</param>
        /// <param name="myTransactionToken">A transaction token for this operation.</param>
        /// <param name="mySecurityToken">A security token for this operation.</param>
        void CanGetVertices(IExpression myExpression, bool myIsLongrunning, TransactionToken myTransactionToken, SecurityToken mySecurityToken);

        #endregion

        IVertex GetSingleVertex(IExpression myExpression, TransactionToken myTransactionToken, SecurityToken mySecurityToken);

        IVertex AddVertex(VertexAddDefinition myVertexDefinition, TransactionToken myTransactionToken, SecurityToken mySecurityToken);

        #region GetVertex

        /// <summary>
        /// Checks whether the get vertex request is valid
        /// </summary>
        /// <param name="myVertexTypeName">The vertex type name of the requested vertex</param>
        /// <param name="myVertexID">The id of the requested vertex</param>
        /// <param name="myEdition">The edition that should be processed</param>
        /// <param name="myTimespan">The timespan that should be processed</param>
        /// <param name="TransactionToken">A transaction token for this operation.</param>
        /// <param name="SecurityToken">A security token for this operation</param>
        void CanGetVertex(string myVertexTypeName, long myVertexID, string myEdition, TimeSpanDefinition myTimespan, TransactionToken TransactionToken, SecurityToken SecurityToken);

        /// <summary>
        /// Execution of the request
        /// </summary>
        /// <param name="myVertexTypeName">The vertex type name of the requested vertex</param>
        /// <param name="myVertexID">The id of the requested vertex</param>
        /// <param name="myEdition">The edition that should be processed</param>
        /// <param name="myTimespan">The timespan that should be processed</param>
        /// <param name="TransactionToken">A transaction token for this operation.</param>
        /// <param name="SecurityToken">A security token for this operation</param>
        /// <returns>The requested vertex</returns>
        IVertex GetVertex(string myVertexTypeName, long myVertexID, string myEdition, TimeSpanDefinition myTimespan, TransactionToken TransactionToken, SecurityToken SecurityToken);

        #endregion
    }
}
