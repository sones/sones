using System;
using System.Collections.Generic;
using sones.Library.PropertyHyperGraph;
using sones.Library.VertexStore.Definitions;

namespace sones.Library.VertexStore
{
    /// <summary>
    /// The interface for all actions that are related to vertices
    /// </summary>
    public interface IVertexStore
    {
        /// <summary>
        /// Checks if a vertex exists
        /// </summary>
        /// <param name="myVertexID">The id of the vertex</param>
        /// <param name="myVertexTypeID">The id of the vertex type</param>
        /// <param name="myEdition">The edition of the vertex  (if left out, the default edition is assumed)</param>
        /// <param name="myVertexRevisionID">The revision id if the vertex (if left out, the latest revision is assumed)</param>
        /// <returns>True if the vertex exists, otherwise false</returns>
        Boolean VertexExists(
            Int64 myVertexID,
            Int64 myVertexTypeID,
            String myEdition = null,
            VertexRevisionID myVertexRevisionID = null);

        /// <summary>
        /// Gets a vertex 
        /// If there is no edition or revision given, the default edition and the latest revision is returned
        /// </summary>
        /// <param name="myVertexID">The id of the vertex</param>
        /// <param name="myVertexTypeID">The id of the vertex type</param>
        /// <param name="myEdition">The edition of the vertex (if left out, the default edition is returned)</param>
        /// <param name="myVertexRevisionID">The revision id if the vertex (if left out, the latest revision is returned)</param>
        /// <returns>A vertex object or null if there is no such vertex</returns>
        IVertex GetVertex(
            Int64 myVertexID,
            Int64 myVertexTypeID,
            String myEdition = null,
            VertexRevisionID myVertexRevisionID = null);

        /// <summary>
        /// Returns all vertex by a given typeID. It's possible to filter interesting vertices.
        /// Edition and Revision filtering works by using a filter func.
        /// 
        /// Beware: defining funcs means that the function has to be called on any vertex.
        /// If you know the exact definitions use the overloaded version of this method using
        /// IEnumerable instead.
        /// </summary>
        /// <param name="myTypeID">the considered vertex type</param>
        /// <param name="myInterestingVertexIDs">a set of vertexID which shall be loaded</param>
        /// <param name="myEditionsFilterFunc">func to filter editions</param>
        /// <param name="myInterestingRevisionIDFilterFunc">func to filter revisions</param>
        /// <returns>vertices</returns>
        IEnumerable<IVertex> GetVerticesByTypeID(
            Int64 myTypeID,
            IEnumerable<Int64> myInterestingVertexIDs = null,
            Func<String, bool> myEditionsFilterFunc = null,
            Func<VertexRevisionID, bool> myInterestingRevisionIDFilterFunc = null);

        /// <summary>
        /// Returns all vertex by a given typeID. It's possible to filter interesting vertices.
        /// It's also possible to filter specified vertices by id, their editions and revisions.
        /// </summary>
        /// <param name="myTypeID">the considered vertex type</param>
        /// <param name="myInterestingVertexIDs">a set of vertexID which shall be loaded</param>
        /// <param name="myInterestingEditionNames">a set of interesting editions of a vertex</param>
        /// <param name="myInterestingRevisionIDs">a set of interesting revisions of a vertex</param>
        /// <returns>vertices</returns>
        IEnumerable<IVertex> GetVerticesByTypeID(
            Int64 myTypeID,
            IEnumerable<Int64> myInterestingVertexIDs = null,
            IEnumerable<String> myInterestingEditionNames = null,
            IEnumerable<VertexRevisionID> myInterestingRevisionIDs = null);

        /// <summary>
        /// Returns all vertices considering a given vertex type.
        /// 
        /// The default edition and latest revision of an existing vertex will be returned.
        /// </summary>
        /// <param name="myTypeID">the considered vertex type</param>
        /// <param name="myInterestingVertexIDs">a set of vertexID which shall be loaded</param>
        /// <returns>all interesting vertices of given type with default edition and latest revision</returns>
        IEnumerable<IVertex> GetVerticesByTypeID(
            Int64 myTypeID,
            IEnumerable<Int64> myInterestingVertexIDs);

        /// <summary>
        /// Returns all vertices considering a given vertex type.
        /// 
        /// The default edition and latest revision of an existing vertex will be returned.
        /// </summary>
        /// <param name="myTypeID">the considered vertex type</param>
        /// <returns>all interesting vertices of given type with default edition and latest revision</returns>
        IEnumerable<IVertex> GetVerticesByTypeID(
            Int64 myTypeID);

        /// <summary>
        /// Returns all vertices considering a given vertex type.
        /// 
        /// The default edition of the vertex will be returned (if the defined revision exists)
        /// </summary>
        /// <param name="myTypeID">the considered vertex type</param>
        /// <param name="myInterestingRevisions">a set of (vertex-)revisions which are of interest</param>
        /// <returns>all vertices of given type which are available at the given revisions</returns>
        IEnumerable<IVertex> GetVerticesByTypeID(
            Int64 myTypeID,
            IEnumerable<VertexRevisionID> myInterestingRevisions);

        /// <summary>
        /// Returns all editions corresponding to a certain vertex
        /// </summary>
        /// <param name="myVertexID">The id of the vertex</param>
        /// <param name="myVertexTypeID">The id of the vertex type</param>
        /// <returns>An IEnumerable of editions</returns>
        IEnumerable<String> GetVertexEditions(
            Int64 myVertexID,
            Int64 myVertexTypeID);

        /// <summary>
        /// Returns all revision ids to a certain vertex and edition
        /// </summary>
        /// <param name="myVertexID">The id of the vertex</param>
        /// <param name="myVertexTypeID">The id of the vertex type</param>
        /// <param name="myInterestingEditions">The interesting vertex editions</param>
        /// <returns>An IEnumerable of VertexRevisionIDs</returns>
        IEnumerable<VertexRevisionID> GetVertexRevisionIDs(
            Int64 myVertexID,
            Int64 myVertexTypeID,
            IEnumerable<String> myInterestingEditions = null);

        /// <summary>
        /// Removes a certain revision of a vertex
        /// </summary>
        /// <param name="myVertexID">The id of the vertex</param>
        /// <param name="myVertexTypeID">The id of the vertex type</param>
        /// <param name="myInterestingEdition">The interesting edition of the vertex</param>
        /// <param name="myToBeRemovedRevisionID">The revision that should be removed</param>
        /// <returns>True if the revision have been removed, false otherwise</returns>
        bool RemoveVertexRevision(
            Int64 myVertexID,
            Int64 myVertexTypeID,
            String myInterestingEdition,
            VertexRevisionID myToBeRemovedRevisionID);

        /// <summary>
        /// Removes a certain edition of a vertex
        /// </summary>
        /// <param name="myVertexID">The id of the vertex</param>
        /// <param name="myVertexTypeID">The id of the vertex type</param>
        /// <param name="myToBeRemovedEdition">The edition that should be removed</param>
        /// <returns>True if the revision have been removed, false otherwise</returns>
        bool RemoveVertexEdition(
            Int64 myVertexID,
            Int64 myVertexTypeID,
            String myToBeRemovedEdition);

        /// <summary>
        /// Removes a vertex entirely
        /// </summary>
        /// <param name="myVertexID">The id of the vertex</param>
        /// <param name="myVertexTypeID">The id of the vertex type</param>
        /// <returns>True if a vertex has been erased, false otherwise</returns>
        bool RemoveVertex(
            Int64 myVertexID,
            Int64 myVertexTypeID);

        /// <summary>
        /// Adds a new vertex to the graph fs
        /// </summary>
        /// <param name="myVertexDefinition">The vertex definition that represents the new vertex</param>
        /// <param name="myVertexRevisionID">The revision id of the vertex</param>
        /// <param name="myCreateIncomingEdges">Create the incoming edges</param>
        /// <returns>The added vertex</returns>
        IVertex AddVertex(
            VertexAddDefinition myVertexDefinition,
            VertexRevisionID myVertexRevisionID = null,
            Boolean myCreateIncomingEdges = true);

        /// <summary>
        /// Updates a vertex
        /// </summary>
        /// <param name="myToBeUpdatedVertexID">The vertex id that is going to be updated</param>
        /// <param name="myCorrespondingVertexTypeID">The vertex type id that is going to be updated</param>
        /// <param name="myVertexUpdate">The update definition for the vertex</param>
        /// <param name="myToBeUpdatedEditions">The editions that should be updated</param>
        /// <param name="myToBeUpdatedRevisionIDs">The revisions that should be updated</param>
        /// <param name="myCreateNewRevision">Determines if it is necessary to create a new revision of the vertex</param>
        /// <returns>The updated vertex</returns>
        IVertex UpdateVertex(
            Int64 myToBeUpdatedVertexID,
            Int64 myCorrespondingVertexTypeID,
            VertexUpdateDefinition myVertexUpdate,
            String myToBeUpdatedEditions = null,
            VertexRevisionID myToBeUpdatedRevisionIDs = null,
            Boolean myCreateNewRevision = false);

    }
}
