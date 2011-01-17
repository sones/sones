using sones.GraphFS;
using sones.Library.Internal.Token;
using sones.Library.Internal.Security;
using sones.GraphFS.Element;

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
            get { throw new System.NotImplementedException(); }
        }

        public bool IsMounted
        {
            get { throw new System.NotImplementedException(); }
        }

        public string GetFileSystemDescription(SessionToken mySessionToken)
        {
            throw new System.NotImplementedException();
        }

        public ulong GetNumberOfBytes(SessionToken mySessionToken)
        {
            throw new System.NotImplementedException();
        }

        public ulong GetNumberOfFreeBytes(SessionToken mySessionToken)
        {
            throw new System.NotImplementedException();
        }

        public AccessModeTypes GetAccessMode(SessionToken mySessionToken)
        {
            throw new System.NotImplementedException();
        }

        public void MakeFileSystem(SessionToken mySessionToken, string myDescription, ulong myNumberOfBytes, bool myOverwriteExistingFileSystem)
        {
            throw new System.NotImplementedException();
        }

        public ulong GrowFileSystem(SessionToken mySessionToken, ulong myNumberOfBytesToAdd)
        {
            throw new System.NotImplementedException();
        }

        public ulong ShrinkFileSystem(SessionToken mySessionToken, ulong myNumberOfBytesToRemove)
        {
            throw new System.NotImplementedException();
        }

        public void WipeFileSystem(SessionToken mySessionToken)
        {
            throw new System.NotImplementedException();
        }

        public void MountFileSystem(SessionToken mySessionToken, AccessModeTypes myAccessMode)
        {
            throw new System.NotImplementedException();
        }

        public void RemountFileSystem(SessionToken mySessionToken, AccessModeTypes myFSAccessMode)
        {
            throw new System.NotImplementedException();
        }

        public void UnmountFileSystem(SessionToken mySessionToken)
        {
            throw new System.NotImplementedException();
        }

        public void UnmountAllFileSystems(SessionToken mySessionToken)
        {
            throw new System.NotImplementedException();
        }

        public bool VertexExists(SessionToken mySessionToken, TransactionToken myTransactionToken, VertexID myVertexID, string myEdition = null, VertexRevisionID myVertexRevisionID = null)
        {
            throw new System.NotImplementedException();
        }

        public IVertex GetVertex(SessionToken mySessionToken, TransactionToken myTransactionToken, VertexID myVertexID, string myEdition = null, VertexRevisionID myVertexRevisionID = null)
        {
            throw new System.NotImplementedException();
        }

        public System.Collections.Generic.IEnumerable<string> GetVertexEditions(SessionToken mySessionToken, TransactionToken myTransactionToken, VertexID myVertexID)
        {
            throw new System.NotImplementedException();
        }

        public System.Collections.Generic.IEnumerable<VertexRevisionID> GetVertexRevisionIDs(SessionToken mySessionToken, TransactionToken myTransactionToken, VertexID myVertexID, string myEdition)
        {
            throw new System.NotImplementedException();
        }

        public void AddVertex(SessionToken mySessionToken, TransactionToken myTransactionToken, IVertex myIVertex, string myEdition = null, VertexRevisionID myVertexRevisionID = null)
        {
            throw new System.NotImplementedException();
        }

        public bool RemoveVertex(SessionToken mySessionToken, TransactionToken myTransactionToken, VertexID myVertexID, string myEdition = null, VertexRevisionID myVertexRevisionID = null)
        {
            throw new System.NotImplementedException();
        }

        public bool EraseVertex(SessionToken mySessionToken, TransactionToken myTransactionToken, VertexID myVertexID)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
