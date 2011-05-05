using System;
using System.Collections.Generic;
using System.Linq;
using sones.Library.Commons.VertexStore;
using sones.GraphDB.Manager.Index;
using sones.GraphDB.Manager.TypeManagement;
using sones.Library.PropertyHyperGraph;
using sones.GraphDB.Expression;
using sones.Library.Commons.Transaction;
using sones.Library.Commons.Security;
using sones.GraphDB.ErrorHandling.Expression;
using sones.GraphDB.Manager.QueryPlan;
using sones.GraphDB.Expression.Tree;
using sones.Library.Commons.VertexStore.Definitions;
using sones.GraphDB.Request;
using sones.GraphDB.ErrorHandling;
using sones.GraphDB.TypeSystem;
using sones.Plugins.Index.Interfaces;
using sones.Library.LanguageExtensions;
using sones.GraphDB.Expression.Tree.Literals;
using System.Collections;
using sones.GraphDB.Request.Insert;

namespace sones.GraphDB.Manager.Vertex
{

    /// <summary>
    /// This manager is responsible for getting (chosen) vertices from the persistence layer
    /// </summary>
    public sealed class VertexManager: IManagerOf<IVertexHandler>
    {

        #region Data

        private CheckVertexHandler _check;
        private ExecuteVertexHandler _execute;

        #endregion

        public VertexManager(IDManager myIDManager)
        {
            _check = new CheckVertexHandler();
            _execute = new ExecuteVertexHandler(myIDManager);
        }

        #region IManagerOf<IVertexManager> Members

        public IVertexHandler CheckManager
        {
            get { return _check; }
        }

        public IVertexHandler ExecuteManager
        {
            get { return _execute; }
        }

        public IVertexHandler UndoManager
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IManager Members

        void IManager.Initialize(IMetaManager myMetaManager)
        {
            _check.Initialize(myMetaManager);
            _execute.Initialize(myMetaManager);
        }

        void IManager.Load(TransactionToken myTransaction, SecurityToken mySecurity)
        {
            _check.Load(myTransaction, mySecurity);
        }

        #endregion
    }
}
