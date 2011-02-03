using System;
using System.Collections.Generic;
using System.IO;
using sones.GraphFS;
using sones.Library.Internal.Definitions;
using sones.PropertyHyperGraph;

namespace sones.InMemoryNonRevisioned
{
    /// <summary>
    /// The in-memory-store is a non persisitent vertex store without handling any revisions
    /// </summary>
    public sealed class InMemoryNonRevisionedFS : IGraphFS
    {
        #region IGraphFS Members

        public bool IsPersistent
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsMounted
        {
            get { throw new NotImplementedException(); }
        }

        public string GetFileSystemDescription()
        {
            throw new NotImplementedException();
        }

        public ulong GetNumberOfBytes()
        {
            throw new NotImplementedException();
        }

        public ulong GetNumberOfFreeBytes()
        {
            throw new NotImplementedException();
        }

        public FileSystemAccessMode GetAccessMode()
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

        public ulong GrowFileSystem(ulong myNumberOfBytesToAdd)
        {
            throw new NotImplementedException();
        }

        public ulong ShrinkFileSystem(ulong myNumberOfBytesToRemove)
        {
            throw new NotImplementedException();
        }

        public void WipeFileSystem()
        {
            throw new NotImplementedException();
        }

        public Stream ReplicateFileSystem()
        {
            throw new NotImplementedException();
        }

        public void MountFileSystem(FileSystemAccessMode myAccessMode)
        {
            throw new NotImplementedException();
        }

        public void RemountFileSystem(FileSystemAccessMode myFSAccessMode)
        {
            throw new NotImplementedException();
        }

        public void UnmountFileSystem()
        {
            throw new NotImplementedException();
        }

        public bool VertexExists(VertexID myVertexID, string myEdition = null, VertexRevisionID myVertexRevisionID = null)
        {
            throw new NotImplementedException();
        }

        public IVertex GetVertex(VertexID myVertexID, string myEdition = null, VertexRevisionID myVertexRevisionID = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IVertex> GetAllVertices(IEnumerable<ulong> myInterestingVertexTypeIDs = null, IEnumerable<VertexID> myInterestingVertexIDs = null, IEnumerable<string> myInterestingEditionNames = null, IEnumerable<VertexRevisionID> myInterestingRevisionIDs = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetVertexEditions(VertexID myVertexID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<VertexRevisionID> GetVertexRevisionIDs(VertexID myVertexID, IEnumerable<string> myInterestingEditions = null)
        {
            throw new NotImplementedException();
        }

        public void AddVertex(IVertex myIVertex, string myEdition = null, VertexRevisionID myVertexRevisionID = null)
        {
            throw new NotImplementedException();
        }

        public bool RemoveVertexRevision(VertexID myVertexID, IEnumerable<string> myInterestingEditions = null, IEnumerable<VertexRevisionID> myToBeRemovedRevisionIDs = null)
        {
            throw new NotImplementedException();
        }

        public bool RemoveVertexEdition(VertexID myVertexID, IEnumerable<string> myToBeRemovedEditions = null)
        {
            throw new NotImplementedException();
        }

        public bool RemoveVertex(VertexID myVertexID)
        {
            throw new NotImplementedException();
        }

        public void UpdateVertex(VertexID myToBeUpdatedVertexID, VertexUpdate myVertexUpdate, IEnumerable<string> myToBeUpdatedEditions = null, IEnumerable<VertexRevisionID> myToBeUpdatedRevisionIDs = null, bool myCreateNewRevision = false)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
