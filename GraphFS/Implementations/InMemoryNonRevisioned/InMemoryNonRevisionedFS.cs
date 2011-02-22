using System;
using System.Collections.Generic;
using sones.GraphFS;
using sones.PropertyHyperGraph;
using System.Collections.Concurrent;

namespace sones.GraphFS
{
    /// <summary>
    /// The in-memory-store is a non persisitent vertex store without handling any revisions
    /// </summary>
    public sealed class InMemoryNonRevisionedFS : IGraphFS
    {
        /// <summary>
        /// The concurrent datastructure where all the vertices are stored
        /// 
        /// TypeID ( VertexID, IVertex)
        /// </summary>
        private ConcurrentDictionary<UInt64, ConcurrentDictionary<UInt64,IVertex>> _vertexStore;

        #region Constructor

        /// <summary>
        /// Creats a new in memory filesystem
        /// </summary>
        public InMemoryNonRevisionedFS()
        {
            _vertexStore = new ConcurrentDictionary<ulong, ConcurrentDictionary<ulong, IVertex>>();
        }

        #endregion

        #region IGraphFS Members

        public bool IsPersistent
        {
            get { return false; }
        }

        public bool IsMounted
        {
            //this filesystem is always mounted

            get { return true; }
        }

        public bool HasRevisions
        {
            get { return false; }
        }

        public bool HasEditions
        {
            get { return false; }
        }

        public string GetFileSystemDescription()
        {
            return String.Format("A simple in memory fi^lesystem without any revision or edition handling.");
        }

        public ulong GetNumberOfBytes()
        {
            return 0;
        }

        public ulong GetNumberOfFreeBytes()
        {
            return 0;
        }

        public ulong GrowFileSystem(ulong myNumberOfBytesToAdd)
        {
            return 0;
        }

        public ulong ShrinkFileSystem(ulong myNumberOfBytesToRemove)
        {
            return 0;
        }

        public void WipeFileSystem()
        {
            _vertexStore = new ConcurrentDictionary<ulong, ConcurrentDictionary<ulong, IVertex>>();
        }

        public IEnumerable<IVertex> CloneFileSystem(ulong myTimeStamp = 0UL)
        {
            throw new NotImplementedException();
        }

        public void ReplicateFileSystem(IEnumerable<IVertex> myReplicationStream)
        {
            throw new NotImplementedException();
        }

        public void MountFileSystem()
        {
            throw new NotImplementedException();
        }

        public void RemountFileSystem()
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