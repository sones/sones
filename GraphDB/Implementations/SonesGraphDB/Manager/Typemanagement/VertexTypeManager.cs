using System.Collections.Generic;
using sones.GraphDB.Request;
using sones.GraphDB.TypeSystem;
using sones.Library.LanguageExtensions;
using sones.Library.Transaction;
using sones.Library.Security;
using System;


/*
 * edge cases:
 *   - if someone changes the super type of an vertex or edge type 
 *     - Henning, Timo 
 *       - that this isn't a required feature for version 2.0
 * 
 *   - undoability of the typemanager 
 *     - Henning, Timo 
 *       - the type manager is only responsible for converting type changing request into filesystem requests
 *       - the ability to undo an request should be implemented in the corresponding piplineable request
 * 
 *   - unique attributes
 *     - Henning, Timo
 *       - the type manager creates unique indices on attributes on the type that declares the uniqness attribute and all deriving types
 * 
 *   - load 
 *     - Timo
 *       - will proove if the main vertex types are available
 *       - will load the main vertex types
 *       - looks for the maximum vertex type id
 * 
 *   - create
 *     - Timo
 *       - will add the main vertex types 
 *       
 *   - get vertex type
 *     - if one of the base vertex types is requested, return a predefined result.
 * 
 *   - insert vertex type
 *     - no type can derive from the base types
 */

namespace sones.GraphDB.Manager.TypeManagement
{
    public sealed class VertexTypeManager : IVertexTypeManager
    {
        #region Constants

        public const UInt64 VertexTypeID = UInt64.MinValue;
        public const UInt64 EdgeTypeID = UInt64.MinValue + 1;

        #endregion

        #region IVertexTypeManager Members

        #region Retrieving

        public IVertexType GetVertexType(string myTypeName)
        {
            return DoGetVertexType(myTypeName);
        }

        #endregion

        #region Updates

        #region Add

        public bool CanAddVertexType(VertexTypeDefinition myVertexTypeDefinition, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            return DoCanAddVertex(myVertexTypeDefinition.SingleEnumerable(), myTransaction, mySecurity, myMetaManager);
        }

        public IVertexType AddVertexType(VertexTypeDefinition myVertexTypeDefinition, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            return DoAddVertex(myVertexTypeDefinition.SingleEnumerable(), myTransaction, mySecurity, myMetaManager);
        }

        public bool CanAddVertexType(IEnumerable<VertexTypeDefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            return DoCanAddVertex(myVertexTypeDefinitions, myTransaction, mySecurity, myMetaManager);
        }

        public IVertexType AddVertexType(IEnumerable<VertexTypeDefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            return DoAddVertex(myVertexTypeDefinitions, myTransaction, mySecurity, myMetaManager);
        }

        #endregion

        #region Remove

        public bool CanRemoveVertexType(IVertexType myVertexType, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            return DoCanRemoveVertex(myVertexType.SingleEnumerable(), myTransaction, mySecurity, myMetaManager);
        }

        public void RemoveVertexType(IVertexType myVertexType, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            DoRemoveVertex(myVertexType.SingleEnumerable(), myTransaction, mySecurity, myMetaManager);
        }

        public bool CanRemoveVertexType(IEnumerable<IVertexType> myVertexTypes, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            return DoCanRemoveVertex(myVertexTypes, myTransaction, mySecurity, myMetaManager);
        }

        public void RemoveVertexType(IEnumerable<IVertexType> myVertexTypes, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            DoRemoveVertex(myVertexTypes, myTransaction, mySecurity, myMetaManager);
        }

        #endregion

        #region Update

        public bool CanUpdateVertexType(VertexTypeDefinition myVertexTypeDefinition, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            return DoCanUpdateVertex(myVertexTypeDefinition.SingleEnumerable(), myTransaction, mySecurity, myMetaManager);
        }

        public void UpdateVertexType(VertexTypeDefinition myVertexTypeDefinition, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            DoUpdateVertex(myVertexTypeDefinition.SingleEnumerable(), myTransaction, mySecurity, myMetaManager);
        }

        public bool CanUpdateVertexType(IEnumerable<VertexTypeDefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            return DoCanUpdateVertex(myVertexTypeDefinitions, myTransaction, mySecurity, myMetaManager);
        }

        public void UpdateVertexType(IEnumerable<VertexTypeDefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            DoUpdateVertex(myVertexTypeDefinitions, myTransaction, mySecurity, myMetaManager);
        }

        #endregion

        #endregion
        
        #endregion

        #region IStorageUsingManager Members

        public void Load(MetaManager myMetaManager)
        {
            throw new NotImplementedException();
        }

        public void Create(Index.IIndexManager myIndexMgr, Library.VertexStore.IVertexStore myVertexStore)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region private members

        #region Get

        private IVertexType DoGetVertexType(string myTypeName)
        {
            
            throw new System.NotImplementedException();
        }

        #endregion

        #region Add

        private bool DoCanAddVertex(IEnumerable<VertexTypeDefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            var vstore = myMetaManager.VertexStore;
            var vmgr = myMetaManager.VertexManager;
            var idxmgr = myMetaManager.IndexManager;
            //vmgr.GetVertex();

            throw new NotImplementedException();
        }

        private IVertexType DoAddVertex(IEnumerable<VertexTypeDefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region Remove

        private bool DoCanRemoveVertex(IEnumerable<IVertexType> myVertexTypes, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        private void DoRemoveVertex(IEnumerable<IVertexType> myVertexTypes, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region Update

        private bool DoCanUpdateVertex(IEnumerable<VertexTypeDefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        private void DoUpdateVertex(IEnumerable<VertexTypeDefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #endregion

    }

}
