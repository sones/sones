using System;
using System.Collections.Generic;
using sones.Library.Internal.Security;

using sones.Library.Internal.Definitions;
using sones.GraphInfrastructure.Element;
using System.IO;

namespace sones.GraphFS
{
    /// <summary>
    /// The interface for all kinds of GraphFS
    /// </summary>
    public interface IGraphFS
    {
        #region Information Methods

        #region IsPersistent

        Boolean IsPersistent { get; }

        #endregion

        #region isMounted

        /// <summary>
        /// Returns true if the file system was mounted correctly
        /// </summary>
        /// <returns>true if the file system was mounted correctly</returns>
        Boolean IsMounted { get; }

        #endregion

        #region GetFileSystemDescription(...)

        /// <summary>
        /// Returns the name or a description of this file system.
        /// </summary>
        /// <returns>The name or a description of this file system</returns>
        String GetFileSystemDescription(SecurityToken mySecurityToken);

        #endregion

        #region GetNumberOfBytes(...)

        /// <summary>
        /// Returns the size (number of bytes) of this file system
        /// </summary>
        /// <returns>The size (number of bytes) of this file system</returns>
        UInt64 GetNumberOfBytes(SecurityToken mySecurityToken);

        #endregion

        #region GetNumberOfFreeBytes(...)

        /// <summary>
        /// Returns the number of free bytes of this file system
        /// </summary>
        /// <returns>The number of free bytes of this file system</returns>
        UInt64 GetNumberOfFreeBytes(SecurityToken mySecurityToken);

        #endregion

        #region GetAccessMode(...)

        /// <summary>
        /// Returns the access mode of this file system
        /// </summary>
        /// <returns>The access mode of this file system</returns>
        FileSystemAccessMode GetAccessMode(SecurityToken mySecurityToken);

        #endregion

        #endregion

        #region Make-/Grow-/Shrink-/Replicate-/WipeFileSystem

        /// <summary>
        /// This initialises a GraphFS in a given device or file using the given sizes
        /// </summary>
        /// <param name="myDescription">a distinguishable Name or description for the file system (can be changed later)</param>
        /// <param name="myNumberOfBytes">the size of the file system in byte</param>
        /// <returns>the UUID of the new file system</returns>
        void MakeFileSystem(String myDescription, UInt64 myNumberOfBytes);

        /// <summary>
        /// Initializes a GraphFS using the stream of a replicated one
        /// </summary>
        /// <param name="myReplicationStream">The stream of a replicated IGraphFS</param>
        void MakeFileSystem(Stream myReplicationStream);

        /// <summary>
        /// This enlarges the size of a GraphFS
        /// </summary>
        /// <param name="myNumberOfBytesToAdd">the number of bytes to add to the size of the current file system</param>
        /// <returns>New total number of bytes</returns>
        UInt64 GrowFileSystem(SecurityToken mySecurityToken, UInt64 myNumberOfBytesToAdd);

        /// <summary>
        /// This reduces the size of a GraphFS
        /// </summary>
        /// <param name="myNumberOfBytesToRemove">the number of bytes to remove from the size of the current file system</param>
        /// <returns>New total number of bytes</returns>
        UInt64 ShrinkFileSystem(SecurityToken mySecurityToken, UInt64 myNumberOfBytesToRemove);

        /// <summary>
        /// Wipe the file system
        /// </summary>
        void WipeFileSystem(SecurityToken mySecurityToken);

        /// <summary>
        /// Replicates the IGraphFS instance into a stream
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <returns>A stream that contains a IGraphFSReplication</returns>
        Stream ReplicateFileSystem(SecurityToken mySecurityToken);

        #endregion

        #region Mount-/Remount-/UnmountFileSystem

        /// <summary>
        /// Mounts this file system.
        /// </summary>
        /// <param name="mySecurityToken">The SecurityToken.</param>
        /// <param name="myAccessMode">The file system access mode, e.g. "read-write" or "read-only".</param>
        void MountFileSystem(SecurityToken mySecurityToken, FileSystemAccessMode myAccessMode);

        /// <summary>
        /// Remounts a file system in order to change its access mode.
        /// </summary>
        /// <param name="mySecurityToken">The SecurityToken.</param>
        /// <param name="myAccessMode">The file system access mode, e.g. "read-write" or "read-only".</param>
        void RemountFileSystem(SecurityToken mySecurityToken, FileSystemAccessMode myFSAccessMode);

        /// <summary>
        /// Flush all caches and unmount this file system.
        /// </summary>
        /// <param name="mySecurityToken">The SecurityToken.</param>
        void UnmountFileSystem(SecurityToken mySecurityToken);

        #endregion

        #region Vertex

        /// <summary>
        /// Checks if a vertex exists
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myVertexID">The id of the vertex</param>
        /// <param name="myEdition">The edition of the vertex  (if left out, the default edition is assumed)</param>
        /// <param name="myVertexRevisionID">The revision id if the vertex (if left out, the latest revision is assumed)</param>
        /// <returns>True if the vertex exists, otherwise false</returns>
        Boolean VertexExists(SecurityToken mySecurityToken, 
            VertexID            myVertexID, 
            String              myEdition = null, 
            VertexRevisionID    myVertexRevisionID = null);

        /// <summary>
        /// Gets a vertex 
        /// If there is no edition or revision given, the default edition and the latest revision is returned
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myVertexID">The id of the vertex</param>
        /// <param name="myEdition">The edition of the vertex (if left out, the default edition is returned)</param>
        /// <param name="myVertexRevisionID">The revision id if the vertex (if left out, the latest revision is returned)</param>
        /// <returns>A vertex object or null if there is no such vertex</returns>
        IVertex GetVertex(SecurityToken mySecurityToken, 
            VertexID            myVertexID, 
            String              myEdition = null, 
            VertexRevisionID    myVertexRevisionID = null);

        /// <summary>
        /// Returns all vertices.
        /// It is possible to filter the vertex type and the vertices itself
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myInterestingVertexTypeIDs">Interesting vertex type ids</param>
        /// <param name="myInterestingVertexIDs">Interesting vertex ids</param>
        /// <param name="myInterestingEditionNames">Interesting editions of the vertex</param>
        /// <param name="myInterestingRevisionIDs">Interesting revisions of the vertex</param>
        /// <returns>An IEnumerable of vertices</returns>
        IEnumerable<IVertex> GetAllVertices(SecurityToken mySecurityToken, 
            IEnumerable<UInt64>             myInterestingVertexTypeIDs  = null,
            IEnumerable<VertexID>           myInterestingVertexIDs      = null,
            IEnumerable<String>             myInterestingEditionNames   = null,
            IEnumerable<VertexRevisionID>   myInterestingRevisionIDs    = null);

        /// <summary>
        /// Returns all editions corresponding to a certain vertex
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myVertexID">The id of the vertex</param>
        /// <returns>An IEnumerable of editions</returns>
        IEnumerable<String> GetVertexEditions(SecurityToken mySecurityToken, 
            VertexID myVertexID);

        /// <summary>
        /// Returns all revision ids to a certain vertex and edition
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myVertexID">The id of the vertex</param>
        /// <param name="myInterestingEditions">The interesting vertex editions</param>
        /// <returns>An IEnumerable of VertexRevisionIDs</returns>
        IEnumerable<VertexRevisionID> GetVertexRevisionIDs(SecurityToken mySecurityToken, 
            VertexID                myVertexID,
            IEnumerable<String>     myInterestingEditions = null);

        /// <summary>
        /// Adds a new vertex to the graph fs
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myIVertex">The vertex that is going to be added</param>
        /// <param name="myEdition">The name of the edition of the new vertex</param>
        /// <param name="myVertexRevisionID">The revision id of the vertex</param>
        void AddVertex(SecurityToken mySecurityToken, 
            IVertex             myIVertex, 
            String              myEdition = null, 
            VertexRevisionID    myVertexRevisionID = null);

        /// <summary>
        /// Removes a certain revision of a vertex
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myVertexID">The id of the vertex</param>
        /// <param name="myInterestingEditions">The interesting editions of the vertex</param>
        /// <param name="myToBeRemovedRevisionIDs">The revisions that should be removed</param>
        /// <returns>True if some revisions have been removed, false otherwise</returns>
        bool RemoveVertexRevision(SecurityToken mySecurityToken, 
            VertexID                        myVertexID,
            IEnumerable<String>             myInterestingEditions = null,
            IEnumerable<VertexRevisionID>   myToBeRemovedRevisionIDs = null);

        /// <summary>
        /// Removes a certain edition of a vertex
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myVertexID">The id of the vertex</param>
        /// <param name="myToBeRemovedEditions">The editions that should be removed</param>
        /// <returns>True if some revisions have been removed, false otherwise</returns>
        bool RemoveVertexEdition(SecurityToken mySecurityToken,
            VertexID myVertexID,
            IEnumerable<String> myToBeRemovedEditions = null);

        /// <summary>
        /// Removes a vertex entirely
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myVertexID">The id of the vertex</param>
        /// <returns>True if a vertex has been erased, false otherwise</returns>
        bool RemoveVertex(SecurityToken mySecurityToken, 
            VertexID myVertexID);

        /// <summary>
        /// Updates a vertex corresponding to a vertex id
        /// </summary>
        /// <param name="mySecurityToken">The current security token</param>
        /// <param name="myToBeUpdatedVertexID">The vertex id that is going to be updated</param>
        /// <param name="myVertexUpdate">The update for the vertex</param>
        /// <param name="myToBeUpdatedEditions">The editions that should be updated</param>
        /// <param name="myToBeUpdatedRevisionIDs">The revisions that should be updated</param>
        /// <param name="myCreateNewRevision">Determines if it is necessary to create a new revision of the vertex</param>
        void UpdateVertex(SecurityToken mySecurityToken,
            VertexID                        myToBeUpdatedVertexID,
            VertexUpdate                    myVertexUpdate,
            IEnumerable<String>             myToBeUpdatedEditions = null,
            IEnumerable<VertexRevisionID>   myToBeUpdatedRevisionIDs = null,
            Boolean                         myCreateNewRevision = false);

        #endregion
    }
}
