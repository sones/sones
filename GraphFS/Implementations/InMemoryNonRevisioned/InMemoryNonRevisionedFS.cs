using System;
using System.Collections.Generic;
using System.IO;
using sones.GraphFS;
using sones.PropertyHyperGraph;
using System.Collections.Concurrent;

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

        public bool HasRevisions
        {
            get { throw new NotImplementedException(); }
        }

        public bool HasEditions
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

        public bool VertexExists(ulong myVertexID, ulong myVertexTypeID, string myEdition = null, VertexRevisionID myVertexRevisionID = null)
        {
            throw new NotImplementedException();
        }

        public IVertex GetVertex(ulong myVertexID, ulong myVertexTypeID, string myEdition = null, VertexRevisionID myVertexRevisionID = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IVertex> GetAllVertices(IEnumerable<ulong> myInterestingVertexTypeIDs = null, IEnumerable<ulong> myInterestingVertexIDs = null, IEnumerable<string> myInterestingEditionNames = null, IEnumerable<VertexRevisionID> myInterestingRevisionIDs = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(ulong myTypeID, IEnumerable<ulong> myInterestingVertexIDs = null, Func<string, bool> myEditionsFilterFunc = null, Func<VertexRevisionID, bool> myInterestingRevisionIDFilterFunc = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(ulong myTypeID, IEnumerable<ulong> myInterestingVertexIDs = null, IEnumerable<string> myInterestingEditionNames = null, IEnumerable<VertexRevisionID> myInterestingRevisionIDs = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(ulong myTypeID, IEnumerable<ulong> myInterestingVertexIDs)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IVertex> GetVerticesByTypeID(ulong myTypeID, IEnumerable<VertexRevisionID> myInterestingRevisions)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetVertexEditions(ulong myVertexID, ulong myVertexTypeID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<VertexRevisionID> GetVertexRevisionIDs(ulong myVertexID, ulong myVertexTypeID, IEnumerable<string> myInterestingEditions = null)
        {
            throw new NotImplementedException();
        }

        public bool RemoveVertexRevision(ulong myVertexID, ulong myVertexTypeID, IEnumerable<string> myInterestingEditions = null, IEnumerable<VertexRevisionID> myToBeRemovedRevisionIDs = null)
        {
            throw new NotImplementedException();
        }

        public bool RemoveVertexEdition(ulong myVertexID, ulong myVertexTypeID, IEnumerable<string> myToBeRemovedEditions = null)
        {
            throw new NotImplementedException();
        }

        public bool RemoveVertex(ulong myVertexID, ulong myVertexTypeID)
        {
            throw new NotImplementedException();
        }

        public bool AddVertex(IVertex myVertex, string myEdition = null, VertexRevisionID myVertexRevisionID = null)
        {
            throw new NotImplementedException();
        }

        public IVertex UpdateVertex(ulong myToBeUpdatedVertexID, ulong myCorrespondingVertexTypeID, IVertex myVertexUpdateDiff, string myToBeUpdatedEditions = null, VertexRevisionID myToBeUpdatedRevisionIDs = null, bool myCreateNewRevision = false)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}
