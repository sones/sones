using System;
using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.ErrorHandling;
using sones.GraphDB.Expression;
using sones.GraphDB.Request;
using sones.GraphDB.TypeManagement;
using sones.GraphDB.TypeManagement.Base;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;
using sones.Library.LanguageExtensions;
using System.Collections;
using sones.GraphDB.Manager.Vertex;
using sones.GraphDB.Manager.Index;
using sones.Library.PropertyHyperGraph;
using sones.Library.Commons.VertexStore.Definitions;
using System.Threading;
using sones.GraphDB.Manager.BaseGraph;
using sones.GraphDB.Index;
using sones.Library.Commons.VertexStore.Definitions.Update;
using sones.GraphDB.Request.CreateVertexTypes;

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
 *   - unique myAttributes
 *     - Henning, Timo
 *       - the type manager creates unique indices on attributes on the type that declares the uniqueness attribute and all deriving types
 * 
 *   - load 
 *     - Timo
 *       - will proove if the main vertex types are available
 *       - will load the main vertex types
 *       - looks for the maximum vertex type id
 * 
 *   - create
 *     - Timo
 *       - not part of the vertex type manager
 *       
 *   - get vertex type
 *     - if one of the base vertex types is requested, return a predefined vertex.
 * 
 *   - insert vertex type
 *     - no type can derive from the base types
 */

namespace sones.GraphDB.Manager.TypeManagement
{
    public sealed class VertexTypeManager : IManagerOf<IVertexTypeManager>
    {
        #region Data

        private CanVertexTypeManager _check = new CanVertexTypeManager();
        private ExecuteVertexTypeManager _execute = new ExecuteVertexTypeManager();

        private IVertexManager _vertexManager;
        private IIndexManager _indexManager;
        private IEdgeTypeManager _edgeManager;

        #endregion

        #region IManagerOf<IVertexTypeManager> Members

        public IVertexTypeManager CheckManager
        {
            get {  return _check; }
        }

        public IVertexTypeManager ExecuteManager
        {
            get { return _execute; }
        }

        public IVertexTypeManager UndoManager
        {
            get { throw new NotImplementedException(); }
        }

        void IManager.Initialize(IMetaManager myMetaManager)
        {
            _execute.Initialize(myMetaManager);
        }

        void IManager.Load(TransactionToken myTransaction, SecurityToken mySecurity)
        {
            _execute.Load(myTransaction, mySecurity);
        }

        #endregion
    }

}
