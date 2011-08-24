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
using sones.Library.PropertyHyperGraph;
using sones.Library.Commons.VertexStore.Definitions;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;

namespace sones.Library.Commons.VertexStore
{
    /// <summary>
    /// The interface for all actions that are related to vertices
    /// </summary>
    public interface IVertexStore
    {
        /// <summary>
        /// Returns the count of vertices corresponding to a vertex type
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionID">The current transaction id</param>
        /// <param name="myVertexTypeID">The interesting vertex type id</param>
        /// <returns>The count of vertices corresponding to a vertex type</returns>
        UInt64 GetVertexCount(
            SecurityToken mySecurityToken,
            Int64 myTransactionID, 
            Int64 myVertexTypeID);

        /// <summary>
        /// Returns the highest vertex id corresponding to a vertex type
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionID">The current transaction id</param>
        /// <param name="myVertexTypeID">The interesting vertex type id<</param>
        /// <returns>The highest vertex id</returns>
        Int64 GetHighestVertexID(
            SecurityToken mySecurityToken,
            Int64 myTransactionID,
            Int64 myVertexTypeID);

        /// <summary>
        /// Checks if a vertex exists
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionID">The current transaction id</param>
        /// <param name="myVertexID">The id of the vertex</param>
        /// <param name="myVertexTypeID">The id of the vertex type</param>
        /// <param name="myEdition">The edition of the vertex  (if left out, the default edition is assumed)</param>
        /// <param name="myVertexRevisionID">The revision id if the vertex (if left out, the latest revision is assumed)</param>
        /// <returns>True if the vertex exists, otherwise false</returns>
        Boolean VertexExists(
            SecurityToken mySecurityToken,
            Int64 myTransactionID,
            Int64 myVertexID,
            Int64 myVertexTypeID,
            String myEdition = null,
            Int64 myRevisionID = 0L);

        /// <summary>
        /// Gets a vertex 
        /// If there is no edition or revision given, the default edition and the latest revision is returned
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionID">The current transaction id</param>
        /// <param name="myVertexID">The id of the vertex</param>
        /// <param name="myVertexTypeID">The id of the vertex type</param>
        /// <param name="myEdition">The edition of the vertex (if left out, the default edition is returned)</param>
        /// <param name="myVertexRevisionID">The revision id if the vertex (if left out, the latest revision is returned)</param>
        /// <returns>A vertex object or null if there is no such vertex</returns>
        IVertex GetVertex(
            SecurityToken mySecurityToken,
            Int64 myTransactionID,
            Int64 myVertexID,
            Int64 myVertexTypeID,
            String myEdition = null,
            Int64 myVertexRevisionID = 0L);

        /// <summary>
        /// Gets a vertex 
        /// If there is no edition or revision given, the default edition and the latest revision is returned
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionID">The current transaction id</param>
        /// <param name="myVertexID">The id of the vertex</param>
        /// <param name="myVertexTypeID">The id of the vertex type</param>
        /// <param name="myEditionsFilterFunc">func to filter editions</param>
        /// <param name="myInterestingRevisionIDFilterFunc">func to filter revisions</param>
        /// <returns>A vertex object or null if there is no such vertex</returns>
        IVertex GetVertex(
            SecurityToken mySecurityToken,
            Int64 myTransactionID,
            Int64 myVertexID,
            Int64 myVertexTypeID,
            VertexStoreFilter.EditionFilter myEditionsFilterFunc = null,
            VertexStoreFilter.RevisionFilter myInterestingRevisionIDFilterFunc = null);

        /// <summary>
        /// Returns all vertex by a given typeID. It's possible to filter interesting vertices.
        /// Edition and Revision filtering works by using a filter func.
        /// 
        /// Beware: defining funcs means that the function has to be called on any vertex.
        /// If you know the exact definitions use the overloaded version of this method using
        /// IEnumerable instead.
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionID">The current transaction id</param>
        /// <param name="myTypeID">the considered vertex type</param>
        /// <param name="myInterestingVertexIDs">a set of vertexID which shall be loaded</param>
        /// <param name="myEditionsFilterFunc">func to filter editions</param>
        /// <param name="myInterestingRevisionIDFilterFunc">func to filter revisions</param>
        /// <returns>vertices</returns>
        IEnumerable<IVertex> GetVerticesByTypeID(
            SecurityToken mySecurityToken,
            Int64 myTransactionID,
            Int64 myTypeID,
            IEnumerable<Int64> myInterestingVertexIDs,
            VertexStoreFilter.EditionFilter myEditionsFilterFunc,
            VertexStoreFilter.RevisionFilter myInterestingRevisionIDFilterFunc);

        /// <summary>
        /// Returns all vertex by a given typeID. It's possible to filter interesting vertices.
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionID">The current transaction id</param>
        /// <param name="myTypeID">The interesting vertex type id</param>
        /// <param name="myEdition">The edition of the vertex (if left out, the default edition is returned)</param>
        /// <param name="myInterestingRevisionIDFilterFunc">A delegate to filter revisions</param>
        /// <returns>Vertices</returns>
        IEnumerable<IVertex> GetVerticesByTypeID(
            SecurityToken mySecurityToken,
            Int64 myTransactionID,
            Int64 myTypeID,
            String myEdition,
            VertexStoreFilter.RevisionFilter myInterestingRevisionIDFilterFunc);

        /// <summary>
        /// Returns all vertex by a given typeID. It's possible to filter interesting vertices.
        /// It's also possible to filter specified vertices by id, their editions and revisions.
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionID">The current transaction id</param>
        /// <param name="myTypeID">the considered vertex type</param>
        /// <param name="myInterestingVertexIDs">a set of vertexID which shall be loaded</param>
        /// <param name="myInterestingEditionNames">a set of interesting editions of a vertex</param>
        /// <param name="myInterestingRevisionIDs">a set of interesting revisions of a vertex</param>
        /// <returns>vertices</returns>
        IEnumerable<IVertex> GetVerticesByTypeID(
            SecurityToken mySecurityToken,
            Int64 myTransactionID,
            Int64 myTypeID,
            IEnumerable<Int64> myInterestingVertexIDs,
            IEnumerable<String> myInterestingEditionNames,
            IEnumerable<Int64> myInterestingRevisionIDs);

        /// <summary>
        /// Returns all vertices considering a given vertex type.
        /// 
        /// The default edition and latest revision of an existing vertex will be returned.
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionID">The current transaction id</param>
        /// <param name="myTypeID">the considered vertex type</param>
        /// <param name="myInterestingVertexIDs">a set of vertexID which shall be loaded</param>
        /// <returns>all interesting vertices of given type with default edition and latest revision</returns>
        IEnumerable<IVertex> GetVerticesByTypeID(
            SecurityToken mySecurityToken,
            Int64 myTransactionID,
            Int64 myTypeID,
            IEnumerable<Int64> myInterestingVertexIDs);

        /// <summary>
        /// Returns all vertices considering a given vertex type.
        /// 
        /// The default edition and latest revision of an existing vertex will be returned.
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionID">The current transaction id</param>
        /// <param name="myTypeID">the considered vertex type</param>
        /// <returns>all interesting vertices of given type with default edition and latest revision</returns>
        IEnumerable<IVertex> GetVerticesByTypeID(
            SecurityToken mySecurityToken,
            Int64 myTransactionID,
            Int64 myTypeID);

        /// <summary>
        /// Returns all vertices considering a given vertex type.
        /// 
        /// The default edition of the vertex will be returned (if the defined revision exists)
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionID">The current transaction id</param>
        /// <param name="myTypeID">the considered vertex type</param>
        /// <param name="myInterestingRevisions">a set of (vertex-)revisions which are of interest</param>
        /// <returns>all vertices of given type which are available at the given revisions</returns>
        IEnumerable<IVertex> GetVerticesByTypeIDAndRevisions(
            SecurityToken mySecurityToken,
            Int64 myTransactionID,
            Int64 myTypeID,
            IEnumerable<Int64> myInterestingRevisions);

        /// <summary>
        /// Returns all editions corresponding to a certain vertex
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionID">The current transaction id</param>
        /// <param name="myVertexID">The id of the vertex</param>
        /// <param name="myVertexTypeID">The id of the vertex type</param>
        /// <returns>An IEnumerable of editions</returns>
        IEnumerable<String> GetVertexEditions(
            SecurityToken mySecurityToken,
            Int64 myTransactionID,
            Int64 myVertexID,
            Int64 myVertexTypeID);

        /// <summary>
        /// Returns all revision ids to a certain vertex and edition
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionID">The current transaction id</param>
        /// <param name="myVertexID">The id of the vertex</param>
        /// <param name="myVertexTypeID">The id of the vertex type</param>
        /// <param name="myInterestingEditions">The interesting vertex editions</param>
        /// <returns>An IEnumerable of VertexRevisionIDs</returns>
        IEnumerable<Int64> GetVertexRevisionIDs(
            SecurityToken mySecurityToken,
            Int64 myTransactionID,
            Int64 myVertexID,
            Int64 myVertexTypeID,
            IEnumerable<String> myInterestingEditions = null);

        /// <summary>
        /// Removes a certain revision of a vertex
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionID">The current transaction id</param>
        /// <param name="myVertexID">The id of the vertex</param>
        /// <param name="myVertexTypeID">The id of the vertex type</param>
        /// <param name="myInterestingEdition">The interesting edition of the vertex</param>
        /// <param name="myToBeRemovedRevisionID">The revision that should be removed</param>
        /// <returns>True if the revision have been removed, false otherwise</returns>
        bool RemoveVertexRevision(
            SecurityToken mySecurityToken,
            Int64 myTransactionID,
            Int64 myVertexID,
            Int64 myVertexTypeID,
            String myInterestingEdition,
            Int64 myToBeRemovedRevisionID);

        /// <summary>
        /// Removes a certain edition of a vertex
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionID">The current transaction id</param>
        /// <param name="myVertexID">The id of the vertex</param>
        /// <param name="myVertexTypeID">The id of the vertex type</param>
        /// <param name="myToBeRemovedEdition">The edition that should be removed</param>
        /// <returns>True if the revision have been removed, false otherwise</returns>
        bool RemoveVertexEdition(
            SecurityToken mySecurityToken,
            Int64 myTransactionID,
            Int64 myVertexID,
            Int64 myVertexTypeID,
            String myToBeRemovedEdition);

        /// <summary>
        /// Removes a vertex entirely
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionID">The current transaction id</param>
        /// <param name="myVertexID">The id of the vertex</param>
        /// <param name="myVertexTypeID">The id of the vertex type</param>
        /// <returns>True if a vertex has been erased, false otherwise</returns>
        bool RemoveVertex(
            SecurityToken mySecurityToken,
            Int64 myTransactionID,
            Int64 myVertexID,
            Int64 myVertexTypeID);

        /// <summary>
        /// Remove vertices from a vertex type
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionID">The current transaction id</param>
        /// <param name="myVertexTypeID">The vertex type id</param>
        /// <param name="myToBeDeltedVertices">The vertex ids that should be deleted</param>
        void RemoveVertices(SecurityToken mySecurityToken, Int64 myTransactionID, long myVertexTypeID, IEnumerable<long> myToBeDeltedVertices = null);

        /// <summary>
        /// Adds a new vertex to the graph fs
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionID">The current transaction id</param>
        /// <param name="myVertexDefinition">The vertex definition that represents the new vertex</param>
        /// <param name="myVertexRevisionID">The revision id of the vertex</param>
        /// <param name="myCreateAutoIncomingEdges">Create the incoming edges</param>
        /// <returns>The added vertex</returns>
        IVertex AddVertex(
            SecurityToken mySecurityToken,
            Int64 myTransactionID,
            VertexAddDefinition myVertexDefinition,
            Int64 myVertexRevisionID = 0L,
            Boolean myCreateAutoIncomingEdges = true);

        /// <summary>
        /// Updates a vertex
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myTransactionID">The current transaction id</param>
        /// <param name="myToBeUpdatedVertexID">The vertex id that is going to be updated</param>
        /// <param name="myCorrespondingVertexTypeID">The vertex type id that is going to be updated</param>
        /// <param name="myVertexUpdate">The update definition for the vertex</param>
        /// <param name="myCreateIncomingEdges">Create the incoming edges</param>
        /// <param name="myToBeUpdatedEditions">The editions that should be updated</param>
        /// <param name="myToBeUpdatedRevisionIDs">The revisions that should be updated</param>
        /// <param name="myCreateNewRevision">Determines if it is necessary to create a new revision of the vertex</param>
        /// <returns>The updated vertex</returns>
        IVertex UpdateVertex(
            SecurityToken mySecurityToken,
            Int64 myTransactionID,
            Int64 myToBeUpdatedVertexID,
            Int64 myCorrespondingVertexTypeID,
            VertexUpdateDefinition myVertexUpdate,
            Boolean myCreateAutoIncomingEdges = true,
            String myToBeUpdatedEditions = null,
            Int64 myToBeUpdatedRevisionIDs = 0L,
            Boolean myCreateNewRevision = false);
    }
}
