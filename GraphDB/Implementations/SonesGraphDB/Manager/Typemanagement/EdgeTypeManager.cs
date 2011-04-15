using sones.GraphDB.Request;
using sones.GraphDB.TypeSystem;
using sones.Library.Security;
using sones.Library.Transaction;

namespace sones.GraphDB.Manager.TypeManagement
{
    public sealed class EdgeTypeManager : IEdgeTypeManager
    {
        #region IEdgeTypeManager Members

        public IEdgeType GetEdgeType(string myTypeName, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new System.NotImplementedException();
        }

        public IEdgeType GetEdgeType(long myTypeId, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new System.NotImplementedException();
        }

        public bool CanAddEdgeType(EdgeTypeDefinition myEdgeTypeDefinition, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new System.NotImplementedException();
        }

        public IEdgeType AddEdgeType(EdgeTypeDefinition myEdgeTypeDefinition, Library.Transaction.TransactionToken myTransaction, Library.Security.SecurityToken mySecurity)
        {
            throw new System.NotImplementedException();
        }

        public bool CanAddEdgeType(System.Collections.Generic.IEnumerable<EdgeTypeDefinition> myEdgeTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new System.NotImplementedException();
        }

        public IEdgeType AddEdgeType(System.Collections.Generic.IEnumerable<EdgeTypeDefinition> myEdgeTypeDefinitions, Library.Transaction.TransactionToken myTransaction, Library.Security.SecurityToken mySecurity)
        {
            throw new System.NotImplementedException();
        }

        public bool CanRemoveEdgeType(IEdgeType myEdgeType, Library.Transaction.TransactionToken myTransaction, Library.Security.SecurityToken mySecurity)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveEdgeType(IEdgeType myEdgeType, Library.Transaction.TransactionToken myTransaction, Library.Security.SecurityToken mySecurity)
        {
            throw new System.NotImplementedException();
        }

        public bool CanRemoveEdgeType(System.Collections.Generic.IEnumerable<IEdgeType> myEdgeTypes, Library.Transaction.TransactionToken myTransaction, Library.Security.SecurityToken mySecurity)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveEdgeType(System.Collections.Generic.IEnumerable<IEdgeType> myEdgeTypes, Library.Transaction.TransactionToken myTransaction, Library.Security.SecurityToken mySecurity)
        {
            throw new System.NotImplementedException();
        }

        public bool CanUpdateEdgeType(EdgeTypeDefinition myEdgeTypeDefinition, Library.Transaction.TransactionToken myTransaction, Library.Security.SecurityToken mySecurity)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateEdgeType(EdgeTypeDefinition myEdgeTypeDefinition, Library.Transaction.TransactionToken myTransaction, Library.Security.SecurityToken mySecurity)
        {
            throw new System.NotImplementedException();
        }

        public bool CanUpdateEdgeType(System.Collections.Generic.IEnumerable<EdgeTypeDefinition> myEdgeTypeDefinitions, Library.Transaction.TransactionToken myTransaction, Library.Security.SecurityToken mySecurity)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateEdgeType(System.Collections.Generic.IEnumerable<EdgeTypeDefinition> myEdgeTypeDefinitions, Library.Transaction.TransactionToken myTransaction, Library.Security.SecurityToken mySecurity)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region IStorageUsingManager Members

        public void Load(IMetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        public void Create(IMetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
