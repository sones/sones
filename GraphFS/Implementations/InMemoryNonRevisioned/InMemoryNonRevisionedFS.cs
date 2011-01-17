using sones.GraphFS;
using sones.Library.Internal.Token;
using sones.Library.Internal.Security;

namespace sones.InMemoryNonRevisioned
{
    /// <summary>
    /// The in-memory-store is a non persisitent vertex store without handling any revisions
    /// </summary>
    public sealed class InMemoryNonRevisionedFS : IGraphFS
    {
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

        public AccessModeTypes GetAccessMode(sones.Library.Internal.Token.SessionToken mySessionToken)
        {
            throw new System.NotImplementedException();
        }

        public void MakeFileSystem(sones.Library.Internal.Token.SessionToken mySessionToken, string myDescription, ulong myNumberOfBytes, bool myOverwriteExistingFileSystem)
        {
            throw new System.NotImplementedException();
        }

        public ulong GrowFileSystem(sones.Library.Internal.Token.SessionToken mySessionToken, ulong myNumberOfBytesToAdd)
        {
            throw new System.NotImplementedException();
        }

        public ulong ShrinkFileSystem(sones.Library.Internal.Token.SessionToken mySessionToken, ulong myNumberOfBytesToRemove)
        {
            throw new System.NotImplementedException();
        }

        public void WipeFileSystem(sones.Library.Internal.Token.SessionToken mySessionToken)
        {
            throw new System.NotImplementedException();
        }

        public void MountFileSystem(sones.Library.Internal.Token.SessionToken mySessionToken, sones.Library.Internal.Security.AccessModeTypes myAccessMode)
        {
            throw new System.NotImplementedException();
        }

        public void RemountFileSystem(sones.Library.Internal.Token.SessionToken mySessionToken, sones.Library.Internal.Security.AccessModeTypes myFSAccessMode)
        {
            throw new System.NotImplementedException();
        }

        public void UnmountFileSystem(sones.Library.Internal.Token.SessionToken mySessionToken)
        {
            throw new System.NotImplementedException();
        }

        public void UnmountAllFileSystems(sones.Library.Internal.Token.SessionToken mySessionToken)
        {
            throw new System.NotImplementedException();
        }

        public bool VertexExists(sones.Library.Internal.Token.SessionToken mySessionToken, sones.Library.Internal.Token.TransactionToken myTransactionToken, global::GraphFS.Element.VertexID myVertexID, string myEdition = null, global::GraphFS.Element.VertexRevisionID myVertexRevisionID = null)
        {
            throw new System.NotImplementedException();
        }

        public global::GraphFS.Element.IVertex GetVertex(sones.Library.Internal.Token.SessionToken mySessionToken, sones.Library.Internal.Token.TransactionToken myTransactionToken, global::GraphFS.Element.VertexID myVertexID, string myEdition = null, global::GraphFS.Element.VertexRevisionID myVertexRevisionID = null)
        {
            throw new System.NotImplementedException();
        }

        public System.Collections.Generic.IEnumerable<string> GetVertexEditions(sones.Library.Internal.Token.SessionToken mySessionToken, sones.Library.Internal.Token.TransactionToken myTransactionToken, global::GraphFS.Element.VertexID myVertexID)
        {
            throw new System.NotImplementedException();
        }

        public System.Collections.Generic.IEnumerable<global::GraphFS.Element.VertexRevisionID> GetVertexRevisionIDs(sones.Library.Internal.Token.SessionToken mySessionToken, sones.Library.Internal.Token.TransactionToken myTransactionToken, global::GraphFS.Element.VertexID myVertexID, string myEdition)
        {
            throw new System.NotImplementedException();
        }

        public void AddVertex(sones.Library.Internal.Token.SessionToken mySessionToken, sones.Library.Internal.Token.TransactionToken myTransactionToken, global::GraphFS.Element.IVertex myIVertex, string myEdition = null, global::GraphFS.Element.VertexRevisionID myVertexRevisionID = null)
        {
            throw new System.NotImplementedException();
        }

        public bool RemoveVertex(sones.Library.Internal.Token.SessionToken mySessionToken, sones.Library.Internal.Token.TransactionToken myTransactionToken, global::GraphFS.Element.VertexID myVertexID, string myEdition = null, global::GraphFS.Element.VertexRevisionID myVertexRevisionID = null)
        {
            throw new System.NotImplementedException();
        }

        public bool EraseVertex(sones.Library.Internal.Token.SessionToken mySessionToken, sones.Library.Internal.Token.TransactionToken myTransactionToken, global::GraphFS.Element.VertexID myVertexID)
        {
            throw new System.NotImplementedException();
        }

        public bool UpdateVertex(sones.Library.Internal.Token.SessionToken mySessionToken, sones.Library.Internal.Token.TransactionToken myTransactionToken, global::GraphFS.Element.VertexID myVertexID, global::GraphFS.Element.IVertex myIVertex)
        {
            throw new System.NotImplementedException();
        }
    }
}
