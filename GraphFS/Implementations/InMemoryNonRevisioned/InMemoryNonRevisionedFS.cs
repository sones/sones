using System;
using sones.GraphFS;
using sones.Library.Internal.Token;
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

        public string GetFileSystemDescription(SessionToken mySessionToken)
        {
            throw new NotImplementedException();
        }

        public ulong GetNumberOfBytes(SessionToken mySessionToken)
        {
            throw new NotImplementedException();
        }

        public ulong GetNumberOfFreeBytes(SessionToken mySessionToken)
        {
            throw new NotImplementedException();
        }

        public AccessModeTypes GetAccessMode(SessionToken mySessionToken)
        {
            throw new NotImplementedException();
        }

        public void MakeFileSystem(SessionToken mySessionToken, string myDescription, ulong myNumberOfBytes)
        {
            throw new NotImplementedException();
        }

        public void MakeFileSystem(Stream myReplicationStream)
        {
            throw new NotImplementedException();
        }

        public ulong GrowFileSystem(SessionToken mySessionToken, ulong myNumberOfBytesToAdd)
        {
            throw new NotImplementedException();
        }

        public ulong ShrinkFileSystem(SessionToken mySessionToken, ulong myNumberOfBytesToRemove)
        {
            throw new NotImplementedException();
        }

        public void WipeFileSystem(SessionToken mySessionToken)
        {
            throw new NotImplementedException();
        }

        public Stream ReplicateFileSystem(SessionToken mySessionToken)
        {
            throw new NotImplementedException();
        }

        public void MountFileSystem(SessionToken mySessionToken, AccessModeTypes myAccessMode)
        {
            throw new NotImplementedException();
        }

        public void RemountFileSystem(SessionToken mySessionToken, AccessModeTypes myFSAccessMode)
        {
            throw new NotImplementedException();
        }

        public void UnmountFileSystem(SessionToken mySessionToken)
        {
            throw new NotImplementedException();
        }

        public bool VertexExists(SessionToken mySessionToken, TransactionToken myTransactionToken, VertexID myVertexID, string myEdition = null, VertexRevisionID myVertexRevisionID = null)
        {
            throw new NotImplementedException();
        }

        public IVertex GetVertex(SessionToken mySessionToken, TransactionToken myTransactionToken, VertexID myVertexID, string myEdition = null, VertexRevisionID myVertexRevisionID = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IVertex> GetAllVertices(SessionToken mySessionToken, TransactionToken myTransactionToken, Func<string, bool> myVertexTypeFilterFunc = null, Func<IVertex, bool> myVertexFilterFunc = null, Func<string, bool> myEditionFilterFunc = null, Func<VertexRevisionID, bool> myRevisionFilterFunc = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetVertexEditions(SessionToken mySessionToken, TransactionToken myTransactionToken, VertexID myVertexID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<VertexRevisionID> GetVertexRevisionIDs(SessionToken mySessionToken, TransactionToken myTransactionToken, VertexID myVertexID, Func<string, bool> myEditionFilterFunc = null)
        {
            throw new NotImplementedException();
        }

        public void AddVertex(SessionToken mySessionToken, TransactionToken myTransactionToken, IVertex myIVertex, string myEdition = null, VertexRevisionID myVertexRevisionID = null)
        {
            throw new NotImplementedException();
        }

        public bool RemoveVertexRevision(SessionToken mySessionToken, TransactionToken myTransactionToken, VertexID myVertexID, Func<string, bool> myEditionFilterFunc = null, Func<VertexRevisionID, bool> myRevisionFilterFunc = null)
        {
            throw new NotImplementedException();
        }

        public bool RemoveVertex(SessionToken mySessionToken, TransactionToken myTransactionToken, VertexID myVertexID)
        {
            throw new NotImplementedException();
        }

        public void UpdateVertex(SessionToken mySessionToken, TransactionToken myTransactionToken, VertexID myToBeUpdatedVertexID, VertexUpdate myVertexUpdate, Func<string, bool> myEditionFilterFunc = null, Func<VertexRevisionID, bool> myRevisionFilterFunc = null, bool myCreateNewRevision = false)
        {
            throw new NotImplementedException();
        }

        public void MakeFileSystem(string myDescription, ulong myNumberOfBytes)
        {
            throw new NotImplementedException();
        }

        public bool RemoveVertexEdition(SessionToken mySessionToken, TransactionToken myTransactionToken, VertexID myVertexID, Func<string, bool> myEditionFilterFunc = null)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
