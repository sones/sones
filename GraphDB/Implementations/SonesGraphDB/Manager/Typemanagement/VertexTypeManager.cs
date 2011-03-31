using System.Collections.Generic;
using sones.GraphDB.Request;
using sones.GraphDB.TypeSystem;
using sones.Library.LanguageExtensions;
using sones.Library.Transaction;
using sones.Library.Security;

namespace sones.GraphDB.Manager.TypeManagement
{
    /* This class is splitted in three partial classes:
     * - TypeManager.csd eclares the public methods for vertex and edge types
     * - EdgeTypeManager.cs declares the private methods for edge types
     * - VertexTypeManager.cs declares the private methods for vertex types
     */
    public sealed partial class TypeManager
    {
        #region VertexTypeManager

        #region Get

        public IVertexType DoGetVertex(string myTypeName)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region Add

        private bool DoCanAddVertex(IEnumerable<VertexTypeDefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurityToken, MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        private void DoAddVertex(IEnumerable<VertexTypeDefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurityToken, MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region Remove

        private bool DoCanRemoveVertex(IEnumerable<IVertexType> myVertexTypes, TransactionToken myTransaction, SecurityToken mySecurityToken, MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        private void DoRemoveVertex(IEnumerable<IVertexType> myVertexTypes, TransactionToken myTransaction, SecurityToken mySecurityToken, MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region Update

        private bool DoCanUpdateVertex(IEnumerable<VertexTypeDefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurityToken, MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        private void DoUpdateVertex(IEnumerable<VertexTypeDefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurityToken, MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #endregion
    }

}
