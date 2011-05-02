using sones.GraphDB.Request;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.TypeManagement.Base;

namespace sones.GraphDB.Manager.TypeManagement
{
    public sealed class EdgeTypeManager : IManagerOf<IEdgeTypeHandler>
    {
        #region Data

        private CheckEdgeTypeManager _check = new CheckEdgeTypeManager();
        private ExecuteEdgeTypeManager _execute = new ExecuteEdgeTypeManager();

        #endregion



        #region IManagerOf<IEdgeTypeManager> Members

        public IEdgeTypeHandler CheckManager
        {
            get { return _check; }
        }

        public IEdgeTypeHandler ExecuteManager
        {
            get { return _execute; }
        }

        public IEdgeTypeHandler UndoManager
        {
            get { throw new NotImplementedException(); }
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
