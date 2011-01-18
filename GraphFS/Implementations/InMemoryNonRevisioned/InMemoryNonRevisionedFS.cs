using System;
using sones.GraphFS;
using sones.Library.Internal.Token;
using sones.Library.Internal.Security;
using sones.GraphFS.Element;
using System.Collections.Generic;

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

        public void MakeFileSystem(SessionToken mySessionToken, string myDescription, ulong myNumberOfBytes, bool myOverwriteExistingFileSystem)
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

        public IEnumerable<string> GetVertexEditions(SessionToken mySessionToken, TransactionToken myTransactionToken, VertexID myVertexID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<VertexRevisionID> GetVertexRevisionIDs(SessionToken mySessionToken, TransactionToken myTransactionToken, VertexID myVertexID, string myEdition)
        {
            throw new NotImplementedException();
        }

        public void AddVertex(SessionToken mySessionToken, TransactionToken myTransactionToken, IVertex myIVertex, string myEdition = null, VertexRevisionID myVertexRevisionID = null)
        {
            throw new NotImplementedException();
        }

        public bool RemoveVertex(SessionToken mySessionToken, TransactionToken myTransactionToken, VertexID myVertexID, string myEdition = null, VertexRevisionID myVertexRevisionID = null)
        {
            throw new NotImplementedException();
        }

        public bool EraseVertex(SessionToken mySessionToken, TransactionToken myTransactionToken, VertexID myVertexID)
        {
            throw new NotImplementedException();
        }
    }
}
