using sones.GraphDB.TypeSystem;
using sones.GraphDB.Request;
using sones.Library.VertexStore;
using sones.Library.Transaction;
using sones.Library.Security;

namespace sones.GraphDB.Manager.TypeManagement
{
    public sealed class EdgeTypeManager : IEdgeTypeManager
    {
        #region IEdgeTypeManager Members

        public IEdgeType GetEdgeType(string myTypeName, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        public bool CanAddEdgeType(EdgeTypeDefinition myEdgeTypeDefinition, Library.Transaction.TransactionToken myTransaction, Library.Security.SecurityToken mySecurity, MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        public IEdgeType AddEdgeType(EdgeTypeDefinition myEdgeTypeDefinition, Library.Transaction.TransactionToken myTransaction, Library.Security.SecurityToken mySecurity, MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        public bool CanAddEdgeType(System.Collections.Generic.IEnumerable<EdgeTypeDefinition> myEdgeTypeDefinitions, Library.Transaction.TransactionToken myTransaction, Library.Security.SecurityToken mySecurity, MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        public IEdgeType AddEdgeType(System.Collections.Generic.IEnumerable<EdgeTypeDefinition> myEdgeTypeDefinitions, Library.Transaction.TransactionToken myTransaction, Library.Security.SecurityToken mySecurity, MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        public bool CanRemoveEdgeType(IEdgeType myEdgeType, Library.Transaction.TransactionToken myTransaction, Library.Security.SecurityToken mySecurity, MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveEdgeType(IEdgeType myEdgeType, Library.Transaction.TransactionToken myTransaction, Library.Security.SecurityToken mySecurity, MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        public bool CanRemoveEdgeType(System.Collections.Generic.IEnumerable<IEdgeType> myEdgeTypes, Library.Transaction.TransactionToken myTransaction, Library.Security.SecurityToken mySecurity, MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveEdgeType(System.Collections.Generic.IEnumerable<IEdgeType> myEdgeTypes, Library.Transaction.TransactionToken myTransaction, Library.Security.SecurityToken mySecurity, MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        public bool CanUpdateEdgeType(EdgeTypeDefinition myEdgeTypeDefinition, Library.Transaction.TransactionToken myTransaction, Library.Security.SecurityToken mySecurity, MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateEdgeType(EdgeTypeDefinition myEdgeTypeDefinition, Library.Transaction.TransactionToken myTransaction, Library.Security.SecurityToken mySecurity, MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        public bool CanUpdateEdgeType(System.Collections.Generic.IEnumerable<EdgeTypeDefinition> myEdgeTypeDefinitions, Library.Transaction.TransactionToken myTransaction, Library.Security.SecurityToken mySecurity, MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateEdgeType(System.Collections.Generic.IEnumerable<EdgeTypeDefinition> myEdgeTypeDefinitions, Library.Transaction.TransactionToken myTransaction, Library.Security.SecurityToken mySecurity, MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region IStorageUsingManager Members

        public void Load(MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        public void Create(MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
