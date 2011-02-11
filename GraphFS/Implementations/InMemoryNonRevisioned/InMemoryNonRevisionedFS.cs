using System;
using System.Collections.Generic;
using System.IO;
using sones.GraphFS;
using sones.PropertyHyperGraph;

namespace sones.InMemoryNonRevisioned
{
    /// <summary>
    /// The in-memory-store is a non persisitent vertex store without handling any revisions
    /// </summary>
    public sealed class InMemoryNonRevisionedFS : IGraphFS
    {

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

        public IEnumerable<IVertex> CloneFileSystem(ulong myTimeStamp = 0UL)
        {
            throw new NotImplementedException();
        }

        public void ReplicateFileSystem(IEnumerable<IVertex> myReplicationStream)
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

        public IEnumerable<IVertex> GetVerticesByTypeID(ulong myTypeID, IEnumerable<VertexID> myInterestingVertexIDs = null, Func<string, bool> myEditionsFilterFunc = null, Func<VertexRevisionID, bool> myInterestingRevisionIDFilterFunc = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(ulong myTypeID, IEnumerable<VertexID> myInterestingVertexIDs = null, IEnumerable<string> myInterestingEditionNames = null, IEnumerable<VertexRevisionID> myInterestingRevisionIDs = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(ulong myTypeID, IEnumerable<VertexID> myInterestingVertexIDs)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(ulong myTypeID, IEnumerable<VertexRevisionID> myInterestingRevisions)
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

        public bool AddVertex(IVertex myVertex, string myEdition = null, VertexRevisionID myVertexRevisionID = null)
        {
            throw new NotImplementedException();
        }

        public IVertex UpdateVertex(VertexID myToBeUpdatedVertexID, IVertex myVertexUpdateDiff, string myToBeUpdatedEditions = null, VertexRevisionID myToBeUpdatedRevisionIDs = null, bool myCreateNewRevision = false)
        {
            throw new NotImplementedException();
        }
    }
}
