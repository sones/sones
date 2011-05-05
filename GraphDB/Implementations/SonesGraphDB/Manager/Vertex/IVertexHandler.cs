using System;
using System.Collections.Generic;
using sones.GraphDB.Expression;
using sones.Library.PropertyHyperGraph;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.GraphDB.Expression.Tree;
using sones.Library.Commons.VertexStore.Definitions;
using sones.Library.Commons.VertexStore;
using sones.GraphDB.Request;

namespace sones.GraphDB.Manager.Vertex
{
    /// <summary>
    /// This interface represents a parentVertex manager.
    /// </summary>
    /// The responibilities of the parentVertex manager is an optimized access to the underlying parentVertex store (FS).
    public interface IVertexHandler: IManager
    {
        #region Get Vertices

        IEnumerable<IVertex> GetVertices(RequestGetVertices _request, TransactionToken TransactionToken, SecurityToken SecurityToken);

        /// <summary>
        /// Gets all vertices for one vertex type.
        /// </summary>
        /// <param name="myVertexType">The interesting vertex type.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <returns>
        /// All vertices of the interesting vertex type.
        /// </returns>
        IEnumerable<IVertex> GetVertices(String myVertexType, TransactionToken myTransaction, SecurityToken mySecurity);

        /// <summary>
        /// Gets all vertices for one vertex type ID.
        /// </summary>
        /// <param name="myVertexType">The interesting vertex type ID.</param>
        /// <param name="myTransactionToken">A transaction token for this operation.</param>
        /// <param name="mySecurityToken">A security token for this operation.</param>
        /// <returns>
        /// All vertices of the interesting vertex type.
        /// </returns>
        IEnumerable<IVertex> GetVertices(long myTypeID, TransactionToken myTransaction, SecurityToken mySecurity);

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

        #endregion

        #region GetVertex

        /// <summary>
        /// Execution of the request
        /// </summary>
        /// <param name="myVertexTypeID">The vertex type id of the requested vertex</param>
        /// <param name="myVertexID">The id of the requested vertex</param>
        /// <param name="myEdition">The edition that should be processed</param>
        /// <param name="myTimespan">The timespan that should be processed</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation</param>
        /// <returns>The requested vertex</returns>
        IVertex GetVertex(long myVertexTypeID, long myVertexID, string myEdition, TimeSpanDefinition myTimespan, TransactionToken myTransaction, SecurityToken mySecurity);
  
        /// <summary>
        /// Execution of the request
        /// </summary>
        /// <param name="myVertexTypeName">The vertex type name of the requested vertex</param>
        /// <param name="myVertexID">The id of the requested vertex</param>
        /// <param name="myEdition">The edition that should be processed</param>
        /// <param name="myTimespan">The timespan that should be processed</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation</param>
        /// <returns>The requested vertex</returns>
        IVertex GetVertex(string myVertexTypeName, long myVertexID, string myEdition, TimeSpanDefinition myTimespan, TransactionToken myTransaction, SecurityToken mySecurity);
        
        IVertex GetSingleVertex(IExpression myExpression, TransactionToken myTransaction, SecurityToken mySecurity);

        #endregion

        /// <summary>
        /// Adds a vertex to the FS.
        /// </summary>
        /// <param name="myInsertDefinition">The insert request.</param>
        /// <param name="TransactionToken">A transaction token for this operation.</param>
        /// <param name="SecurityToken">A security token for this operation.</param>
        /// <returns>The added vertex.</returns>
        IVertex AddVertex(RequestInsertVertex myInsertDefinition, TransactionToken myTransaction, SecurityToken mySecurity);


        /// <summary>
        /// Updates a set of vertices and returns them.
        /// </summary>
        /// <param name="myUpdate">The request that represents the update.</param>
        /// <param name="TransactionToken">A transaction token for this operation.</param>
        /// <param name="SecurityToken">A security token for this operation.</param>
        /// <returns>The updated vertivess.</returns>
        IEnumerable<IVertex> UpdateVertices(RequestUpdate myUpdate, TransactionToken myTransaction, SecurityToken mySecurity);



        /// <summary>
        /// Gets the vertex store this vertex manager is acting on.
        /// </summary>
        IVertexStore VertexStore { get;  }


    }
}
