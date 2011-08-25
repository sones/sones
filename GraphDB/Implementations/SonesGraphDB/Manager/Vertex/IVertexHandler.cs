/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

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
using sones.GraphDB.TypeSystem;

namespace sones.GraphDB.Manager.Vertex
{
    /// <summary>
    /// This interface represents a parentVertex manager.
    /// </summary>
    /// The responibilities of the parentVertex manager is an optimized access to the underlying parentVertex store (FS).
    public interface IVertexHandler: IManager
    {
        #region Get Vertices

        /// <summary>
        /// Gets all vertices correspondig to a vertex type
        /// </summary>
        /// <param name="myVertexType">The interesting vertex type</param>
        /// <param name="myTransaction">The current transaction token</param>
        /// <param name="mySecurity">The current security token</param>
        /// <returns>An enumerable of all vertices</returns>
        IEnumerable<IVertex> GetVertices(IVertexType myVertexType, Int64 myTransaction, SecurityToken mySecurity, Boolean includeSubtypes = true);

        /// <summary>
        /// Gets all vertices depending to a request
        /// </summary>
        /// <param name="myRequest">The request</param>
        /// <param name="Int64">The current transaction token</param>
        /// <param name="SecurityToken">The current security token</param>
        /// <returns>An enumerable of all vertices</returns>
        IEnumerable<IVertex> GetVertices(RequestGetVertices myRequest, Int64 Int64, SecurityToken SecurityToken);

        /// <summary>
        /// Gets all vertices for one vertex type.
        /// </summary>
        /// <param name="myVertexType">The interesting vertex type.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <returns>
        /// All vertices of the interesting vertex type.
        /// </returns>
        IEnumerable<IVertex> GetVertices(String myVertexType, Int64 myTransaction, SecurityToken mySecurity, Boolean includeSubtypes = true);

        /// <summary>
        /// Gets all vertices for one vertex type ID.
        /// </summary>
        /// <param name="myVertexType">The interesting vertex type ID.</param>
        /// <param name="myTransactionToken">A transaction token for this operation.</param>
        /// <param name="mySecurityToken">A security token for this operation.</param>
        /// <returns>
        /// All vertices of the interesting vertex type.
        /// </returns>
        IEnumerable<IVertex> GetVertices(long myTypeID, Int64 myTransaction, SecurityToken mySecurity, Boolean includeSubtypes = true);

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
        IEnumerable<IVertex> GetVertices(IExpression myExpression, Boolean myIsLongrunning, Int64 myTransactionToken, SecurityToken mySecurityToken);

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
        IVertex GetVertex(long myVertexTypeID, long myVertexID, string myEdition, TimeSpanDefinition myTimespan, Int64 myTransaction, SecurityToken mySecurity);
  
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
        IVertex GetVertex(string myVertexTypeName, long myVertexID, string myEdition, TimeSpanDefinition myTimespan, Int64 myTransaction, SecurityToken mySecurity);
        
        IVertex GetSingleVertex(IExpression myExpression, Int64 myTransaction, SecurityToken mySecurity);

        #endregion

        /// <summary>
        /// Adds a vertex to the FS.
        /// </summary>
        /// <param name="myInsertDefinition">The insert request.</param>
        /// <param name="Int64">A transaction token for this operation.</param>
        /// <param name="SecurityToken">A security token for this operation.</param>
        /// <returns>The added vertex.</returns>
        IVertex AddVertex(RequestInsertVertex myInsertDefinition, Int64 myTransaction, SecurityToken mySecurity);


        /// <summary>
        /// Updates a set of vertices and returns them.
        /// </summary>
        /// <param name="myUpdate">The request that represents the update.</param>
        /// <param name="Int64">A transaction token for this operation.</param>
        /// <param name="SecurityToken">A security token for this operation.</param>
        /// <returns>The updated vertivess.</returns>
        IEnumerable<IVertex> UpdateVertices(RequestUpdate myUpdate, Int64 myTransaction, SecurityToken mySecurity);

        /// <summary>
        /// Gets the vertex store this vertex manager is acting on.
        /// </summary>
        IVertexStore VertexStore { get;  }

        /// <summary>
        /// Deletes a set of vertices
        /// </summary>
        /// <param name="myDeleteRequest">The request that represents the delete operation</param>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionToken">The current transaction token</param>
        void Delete(RequestDelete myDeleteRequest, SecurityToken mySecurityToken, Int64 myTransactionToken);
    }
}
