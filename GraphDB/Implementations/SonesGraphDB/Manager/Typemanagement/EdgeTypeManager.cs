using sones.GraphDB.TypeSystem;
using sones.GraphDB.Request;
using sones.Library.VertexStore;

namespace sones.GraphDB.Manager.TypeManagement
{
    public sealed class EdgeTypeManager : IEdgeTypeManager
    {
        #region IEdgeTypeManager Members

        public IEdgeType GetEdgeType(string myTypeName)
        {
            throw new System.NotImplementedException();
        }

        public bool CanAddEdge(EdgeTypeDefinition myEdgeTypeDefinition, Library.Transaction.TransactionToken myTransaction, Library.Security.SecurityToken mySecurity, MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        public IEdgeType AddEdge(EdgeTypeDefinition myEdgeTypeDefinition, Library.Transaction.TransactionToken myTransaction, Library.Security.SecurityToken mySecurity, MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        public bool CanAddEdge(System.Collections.Generic.IEnumerable<EdgeTypeDefinition> myEdgeTypeDefinitions, Library.Transaction.TransactionToken myTransaction, Library.Security.SecurityToken mySecurity, MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        public IEdgeType AddEdge(System.Collections.Generic.IEnumerable<EdgeTypeDefinition> myEdgeTypeDefinitions, Library.Transaction.TransactionToken myTransaction, Library.Security.SecurityToken mySecurity, MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        public bool CanRemoveEdge(IEdgeType myEdgeType, Library.Transaction.TransactionToken myTransaction, Library.Security.SecurityToken mySecurity, MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveEdge(IEdgeType myEdgeType, Library.Transaction.TransactionToken myTransaction, Library.Security.SecurityToken mySecurity, MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        public bool CanRemoveEdge(System.Collections.Generic.IEnumerable<IEdgeType> myEdgeTypes, Library.Transaction.TransactionToken myTransaction, Library.Security.SecurityToken mySecurity, MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveEdge(System.Collections.Generic.IEnumerable<IEdgeType> myEdgeTypes, Library.Transaction.TransactionToken myTransaction, Library.Security.SecurityToken mySecurity, MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        public bool CanUpdateEdge(EdgeTypeDefinition myEdgeTypeDefinition, Library.Transaction.TransactionToken myTransaction, Library.Security.SecurityToken mySecurity, MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateEdge(EdgeTypeDefinition myEdgeTypeDefinition, Library.Transaction.TransactionToken myTransaction, Library.Security.SecurityToken mySecurity, MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        public bool CanUpdateEdge(System.Collections.Generic.IEnumerable<EdgeTypeDefinition> myEdgeTypeDefinitions, Library.Transaction.TransactionToken myTransaction, Library.Security.SecurityToken mySecurity, MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateEdge(System.Collections.Generic.IEnumerable<EdgeTypeDefinition> myEdgeTypeDefinitions, Library.Transaction.TransactionToken myTransaction, Library.Security.SecurityToken mySecurity, MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region IStorageUsingManager Members

        public void Load(MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        public void Create(Index.IIndexManager myIndexMgr, IVertexStore myVertexStore)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
