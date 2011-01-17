using System;
using System.Collections.Generic;
using sones.Library.Internal.Security;
using sones.Library.Internal.Token;
using sones.Library.Internal.Definitions;
using sones.GraphFS.Element;

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
        String GetFileSystemDescription(SessionToken mySessionToken);

        #endregion

        #region GetNumberOfBytes(...)

        /// <summary>
        /// Returns the size (number of bytes) of this file system
        /// </summary>
        /// <returns>The size (number of bytes) of this file system</returns>
        UInt64 GetNumberOfBytes(SessionToken mySessionToken);

        #endregion

        #region GetNumberOfFreeBytes(...)

        /// <summary>
        /// Returns the number of free bytes of this file system
        /// </summary>
        /// <returns>The number of free bytes of this file system</returns>
        UInt64 GetNumberOfFreeBytes(SessionToken mySessionToken);

        #endregion

        #region GetAccessMode(...)

        /// <summary>
        /// Returns the access mode of this file system
        /// </summary>
        /// <returns>The access mode of this file system</returns>
        AccessModeTypes GetAccessMode(SessionToken mySessionToken);

        #endregion

        #endregion


        #region Make-/Grow-/Shrink-/WipeFileSystem

        /// <summary>
        /// This initialises a GraphFS in a given device or file using the given sizes
        /// </summary>
        /// <param name="myDescription">a distinguishable Name or description for the file system (can be changed later)</param>
        /// <param name="myNumberOfBytes">the size of the file system in byte</param>
        /// <param name="myOverwriteExistingFileSystem">overwrite an existing file system [yes|no]</param>
        /// <returns>the UUID of the new file system</returns>
        void MakeFileSystem(SessionToken mySessionToken, String myDescription, UInt64 myNumberOfBytes, Boolean myOverwriteExistingFileSystem);

        /// <summary>
        /// This enlarges the size of a GraphFS
        /// </summary>
        /// <param name="myNumberOfBytesToAdd">the number of bytes to add to the size of the current file system</param>
        /// <returns>New total number of bytes</returns>
        UInt64 GrowFileSystem(SessionToken mySessionToken, UInt64 myNumberOfBytesToAdd);

        /// <summary>
        /// This reduces the size of a GraphFS
        /// </summary>
        /// <param name="myNumberOfBytesToRemove">the number of bytes to remove from the size of the current file system</param>
        /// <returns>New total number of bytes</returns>
        UInt64 ShrinkFileSystem(SessionToken mySessionToken, UInt64 myNumberOfBytesToRemove);

        /// <summary>
        /// Wipe the file system
        /// </summary>
        void WipeFileSystem(SessionToken mySessionToken);

        #endregion

        #region Mount-/Remount-/UnmountFileSystem

        /// <summary>
        /// Mounts this file system.
        /// </summary>
        /// <param name="mySessionToken">The SessionToken.</param>
        /// <param name="myAccessMode">The file system access mode, e.g. "read-write" or "read-only".</param>
        void MountFileSystem(SessionToken mySessionToken, AccessModeTypes myAccessMode);

        /// <summary>
        /// Remounts a file system in order to change its access mode.
        /// </summary>
        /// <param name="mySessionToken">The SessionToken.</param>
        /// <param name="myAccessMode">The file system access mode, e.g. "read-write" or "read-only".</param>
        void RemountFileSystem(SessionToken mySessionToken, AccessModeTypes myFSAccessMode);

        /// <summary>
        /// Flush all caches and unmount this file system.
        /// </summary>
        /// <param name="mySessionToken">The SessionToken.</param>
        void UnmountFileSystem(SessionToken mySessionToken);

        /// <summary>
        /// Unmounts all file systems mounted via this file system.
        /// </summary>
        /// <param name="mySessionToken">The SessionToken.</param>
        void UnmountAllFileSystems(SessionToken mySessionToken);

        #endregion

        #region Vertex

        /// <summary>
        /// Checks if a vertex exists
        /// </summary>
        /// <param name="mySessionToken">The current session token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myVertexID">The id of the vertex</param>
        /// <param name="myEdition">The edition of the vertex</param>
        /// <param name="myVertexRevisionID">The revision id if the vertex</param>
        /// <returns>True if the vertex exists, otherwise false</returns>
        Boolean VertexExists(SessionToken mySessionToken, TransactionToken myTransactionToken, VertexID myVertexID, String myEdition = null, VertexRevisionID myVertexRevisionID = null);

        /// <summary>
        /// Gets a vertex
        /// </summary>
        /// <param name="mySessionToken">The current session token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myVertexID">The id of the vertex</param>
        /// <param name="myEdition">The edition of the vertex</param>
        /// <param name="myVertexRevisionID">The revision id if the vertex</param>
        /// <returns>A vertex object or null if there is no such vertex</returns>
        IVertex GetVertex(SessionToken mySessionToken, TransactionToken myTransactionToken, VertexID myVertexID, String myEdition = null, VertexRevisionID myVertexRevisionID = null);

        /// <summary>
        /// Returns all editions corresponding to a certain vertex
        /// </summary>
        /// <param name="mySessionToken">The current session token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myVertexID">The id of the vertex</param>
        /// <returns>An IEnumerable of editions</returns>
        IEnumerable<String> GetVertexEditions(SessionToken mySessionToken, TransactionToken myTransactionToken, VertexID myVertexID);

        /// <summary>
        /// Returns all revision ids to a certain vertex and edition
        /// </summary>
        /// <param name="mySessionToken">The current session token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myVertexID">The id of the vertex</param>
        /// <param name="myEdition">The edition of the vertex</param>
        /// <returns>An IEnumerable of VertexRevisionIDs</returns>
        IEnumerable<VertexRevisionID> GetVertexRevisionIDs(SessionToken mySessionToken, TransactionToken myTransactionToken, VertexID myVertexID, String myEdition);

        /// <summary>
        /// Adds a new vertex to the graph fs
        /// </summary>
        /// <param name="mySessionToken">The current session token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myIVertex">The vertex that is going to be added</param>
        /// <param name="myEdition">The name of the edition of the new vertex</param>
        /// <param name="myVertexRevisionID">The revision id of the vertex</param>
        void AddVertex(SessionToken mySessionToken, TransactionToken myTransactionToken, IVertex myIVertex, String myEdition = null, VertexRevisionID myVertexRevisionID = null);

        /// <summary>
        /// Removes a certain edition/revision of a vertex
        /// </summary>
        /// <param name="mySessionToken">The current session token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myVertexID">The id of the vertex</param>
        /// <param name="myEdition">The name of the edition that should be removed</param>
        /// <param name="myVertexRevisionID">The revision id of the vertex that should be removed</param>
        /// <returns>True if a vertex has been removed, false otherwise</returns>
        bool RemoveVertex(SessionToken mySessionToken, TransactionToken myTransactionToken, VertexID myVertexID, String myEdition = null, VertexRevisionID myVertexRevisionID = null);

        /// <summary>
        /// Erases a vertex entirely
        /// </summary>
        /// <param name="mySessionToken">The current session token</param>
        /// <param name="myTransactionToken">The current transaction token (null, if there is no transaction)</param>
        /// <param name="myVertexID">The id of the vertex</param>
        /// <returns>True if a vertex has been erased, false otherwise</returns>
        bool EraseVertex(SessionToken mySessionToken, TransactionToken myTransactionToken, VertexID myVertexID);

        
        //Todo: Update

        #endregion

        //Todo: Sync
    }
}
