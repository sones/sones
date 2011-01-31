using System;
using sones.GraphFS;
using sones.Library.Internal.Security;
using System.Collections.Generic;
using sones.GraphInfrastructure.Element;
using System.IO;
using sones.Library.Internal.Definitions;

namespace sones.InMemoryNonRevisioned
{
    /// <summary>
    /// The in-memory-store is a non persisitent vertex store without handling any revisions
    /// </summary>
    public sealed class InMemoryNonRevisionedFS : IGraphFS
    {

        #region IGraphFS

        public bool IsPersistent
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsMounted
        {
            get { throw new NotImplementedException(); }
        }

        public string GetFileSystemDescription(SecurityToken mySecurityToken)
        {
            throw new NotImplementedException();
        }

        public ulong GetNumberOfBytes(SecurityToken mySecurityToken)
        {
            throw new NotImplementedException();
        }

        public ulong GetNumberOfFreeBytes(SecurityToken mySecurityToken)
        {
            throw new NotImplementedException();
        }

        public FileSystemAccessMode GetAccessMode(SecurityToken mySecurityToken)
        {
            throw new NotImplementedException();
        }

        public void MakeFileSystem(string myDescription, ulong myNumberOfBytes)
        {
            throw new NotImplementedException();
        }

        public void MakeFileSystem(Stream myReplicationStream)
        {
            throw new NotImplementedException();
        }

        public ulong GrowFileSystem(SecurityToken mySecurityToken, ulong myNumberOfBytesToAdd)
        {
            throw new NotImplementedException();
        }

        public ulong ShrinkFileSystem(SecurityToken mySecurityToken, ulong myNumberOfBytesToRemove)
        {
            throw new NotImplementedException();
        }

        public void WipeFileSystem(SecurityToken mySecurityToken)
        {
            throw new NotImplementedException();
        }

        public Stream ReplicateFileSystem(SecurityToken mySecurityToken)
        {
            throw new NotImplementedException();
        }

        public void MountFileSystem(SecurityToken mySecurityToken, FileSystemAccessMode myAccessMode)
        {
            throw new NotImplementedException();
        }

        public void RemountFileSystem(SecurityToken mySecurityToken, FileSystemAccessMode myFSAccessMode)
        {
            throw new NotImplementedException();
        }

        public void UnmountFileSystem(SecurityToken mySecurityToken)
        {
            throw new NotImplementedException();
        }

        public bool VertexExists(SecurityToken mySecurityToken, VertexID myVertexID, string myEdition = null, VertexRevisionID myVertexRevisionID = null)
        {
            throw new NotImplementedException();
        }

        public IVertex GetVertex(SecurityToken mySecurityToken, VertexID myVertexID, string myEdition = null, VertexRevisionID myVertexRevisionID = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IVertex> GetAllVertices(SecurityToken mySecurityToken, Func<string, bool> myVertexTypeFilterFunc = null, Func<IVertex, bool> myVertexFilterFunc = null, Func<string, bool> myEditionFilterFunc = null, Func<VertexRevisionID, bool> myRevisionFilterFunc = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetVertexEditions(SecurityToken mySecurityToken, VertexID myVertexID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<VertexRevisionID> GetVertexRevisionIDs(SecurityToken mySecurityToken, VertexID myVertexID, Func<string, bool> myEditionFilterFunc = null)
        {
            throw new NotImplementedException();
        }

        public void AddVertex(SecurityToken mySecurityToken, IVertex myIVertex, string myEdition = null, VertexRevisionID myVertexRevisionID = null)
        {
            throw new NotImplementedException();
        }

        public bool RemoveVertexRevision(SecurityToken mySecurityToken, VertexID myVertexID, Func<string, bool> myEditionFilterFunc = null, Func<VertexRevisionID, bool> myRevisionFilterFunc = null)
        {
            throw new NotImplementedException();
        }

        public bool RemoveVertexEdition(SecurityToken mySecurityToken, VertexID myVertexID, Func<string, bool> myEditionFilterFunc = null)
        {
            throw new NotImplementedException();
        }

        public bool RemoveVertex(SecurityToken mySecurityToken, VertexID myVertexID)
        {
            throw new NotImplementedException();
        }

        public void UpdateVertex(SecurityToken mySecurityToken, VertexID myToBeUpdatedVertexID, VertexUpdate myVertexUpdate, Func<string, bool> myEditionFilterFunc = null, Func<VertexRevisionID, bool> myRevisionFilterFunc = null, bool myCreateNewRevision = false)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
