using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Transaction;
using sones.Library.Commons.Security;
using sones.GraphDB.Request;

namespace sones.GraphDB.Manager.TypeManagement
{
    public class CheckEdgeTypeManager: IEdgeTypeHandler
    {
        #region IEdgeTypeManager Members

        IEdgeType IEdgeTypeHandler.GetEdgeType(long myTypeId, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        IEdgeType IEdgeTypeHandler.GetEdgeType(string myTypeName, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        IEnumerable<IEdgeType> IEdgeTypeHandler.GetAllEdgeTypes(TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        IEdgeType IEdgeTypeHandler.AddEdgeType(IEnumerable<EdgeTypePredefinition> myEdgeTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        void IEdgeTypeHandler.RemoveEdgeTypes(IEnumerable<IEdgeType> myEdgeTypes, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        void IEdgeTypeHandler.UpdateEdgeType(IEnumerable<EdgeTypePredefinition> myEdgeTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IManager Members

        void IManager.Initialize(IMetaManager myMetaManager)
        {
        }

        void IManager.Load(TransactionToken myTransaction, SecurityToken mySecurity)
        {
        }

        #endregion
    }
}
