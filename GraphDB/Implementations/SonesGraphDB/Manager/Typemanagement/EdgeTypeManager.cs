using sones.GraphDB.Request;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using System;

namespace sones.GraphDB.Manager.TypeManagement
{
    public sealed class EdgeTypeManager : IEdgeTypeManager
    {
        #region IEdgeTypeManager Members

        public IEdgeType GetEdgeType(string myTypeName, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        public IEdgeType GetEdgeType(long myTypeId, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        public bool CanAddEdgeType(EdgeTypeDefinition myEdgeTypeDefinition, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        public IEdgeType AddEdgeType(EdgeTypeDefinition myEdgeTypeDefinition, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        public bool CanAddEdgeType(System.Collections.Generic.IEnumerable<EdgeTypeDefinition> myEdgeTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        public IEdgeType AddEdgeType(System.Collections.Generic.IEnumerable<EdgeTypeDefinition> myEdgeTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        public bool CanRemoveEdgeType(IEdgeType myEdgeType, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        public void RemoveEdgeType(IEdgeType myEdgeType, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        public bool CanRemoveEdgeType(System.Collections.Generic.IEnumerable<IEdgeType> myEdgeTypes, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        public void RemoveEdgeType(System.Collections.Generic.IEnumerable<IEdgeType> myEdgeTypes, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        public bool CanUpdateEdgeType(EdgeTypeDefinition myEdgeTypeDefinition, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        public void UpdateEdgeType(EdgeTypeDefinition myEdgeTypeDefinition, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        public bool CanUpdateEdgeType(System.Collections.Generic.IEnumerable<EdgeTypeDefinition> myEdgeTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        public void UpdateEdgeType(System.Collections.Generic.IEnumerable<EdgeTypeDefinition> myEdgeTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IStorageUsingManager Members

        public void Load(IMetaManager myMetaManager)
        {
            throw new NotImplementedException();
        }

        public void Create(IMetaManager myMetaManager)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
