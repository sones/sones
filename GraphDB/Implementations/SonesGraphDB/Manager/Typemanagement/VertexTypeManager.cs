using System.Collections.Generic;
using sones.GraphDB.Request;
using sones.GraphDB.TypeSystem;
using sones.Library.LanguageExtensions;
using sones.Library.Transaction;
using sones.Library.Security;
using System;

namespace sones.GraphDB.Manager.TypeManagement
{
    /* This class is splitted in three partial classes:
     * - TypeManager.csd eclares the public methods for vertex and edge types
     * - EdgeTypeManager.cs declares the private methods for edge types
     * - VertexTypeManager.cs declares the private methods for vertex types
     */
    public sealed partial class TypeManager : ITypeManager
    {
        public static UInt64 VertexTypeID = UInt64.MinValue;
        public static UInt64 EdgeTypeID   = UInt64.MinValue + 1;


        #region VertexTypeManager

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

        private void DoAddVertex(IEnumerable<VertexTypeDefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
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
