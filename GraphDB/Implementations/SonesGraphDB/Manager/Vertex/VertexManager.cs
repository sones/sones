using System;
using System.Collections.Generic;
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

namespace sones.GraphDB.Manager.Vertex
{

    /// <summary>
    /// This manager is responsible for getting (chosen) vertices from the persistence layer
    /// </summary>
    public sealed class VertexManager : IVertexManager
    {
        #region data

        /// <summary>
        /// Needed for getting vertices from the persistence layer
        /// </summary>
        private IVertexStore _vertexStore;

        /// <summary>
        /// Needed for index interaction
        /// </summary>
        private IIndexManager _indexManager;

        /// <summary>
        /// Needed for VertexType interaction
        /// </summary>
        private IVertexTypeManager _vertexTypeManager;

        /// <summary>
        /// Needed for transforming an expression into a query plan
        /// </summary>
        private IQueryPlanManager _queryPlanManager;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new vertex manager
        /// </summary>
        public VertexManager()
        {
        }

        #endregion

        #region IVertexManager Members

        #region GetVertices

        public IEnumerable<IVertex> GetVertices(IExpression myExpression, bool myIsLongrunning, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            var queryPlan = _queryPlanManager.CreateQueryPlan(myExpression, myIsLongrunning, myTransactionToken, mySecurityToken);

            return queryPlan.Execute();
        }

        public void CanGetVertices(IExpression iExpression, bool p, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            
        }

        #endregion

        #region GetVertex

        public void CanGetVertex(string myVertexTypeName, long myVertexID, string myEdition, TimeSpanDefinition myTimespan, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            throw new NotImplementedException();
        }

        public IVertex GetVertex(string myVertexTypeName, long myVertexID, string myEdition, TimeSpanDefinition myTimespan, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        public IVertex AddVertex(VertexAddDefinition myVertexDefinition, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            throw new NotImplementedException();
        }

        public IVertex GetSingleVertex(IExpression myExpression, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region public methods

        /// <summary>
        /// Sets the vertex store
        /// </summary>
        /// <param name="myVertexStore">The vertex store that should be used within the vertex manager</param>
        public void SetVertexStore(IVertexStore myVertexStore)
        {
            _vertexStore = myVertexStore;
        }

        /// <summary>
        /// Sets the vertex type manager
        /// </summary>
        /// <param name="myVertexTypeManager">The vertex type manager that should be used within the vertex manager</param>
        public void SetVertexTypeManager(IVertexTypeManager myVertexTypeManager)
        {
            _vertexTypeManager = myVertexTypeManager;
        }

        /// <summary>
        /// Sets the index manager
        /// </summary>
        /// <param name="myIndexManager">The index manager that should be used within the vertex manager</param>
        public void SetIndexManager(IIndexManager myIndexManager)
        {
            _indexManager = myIndexManager;
        }
        
        /// <summary>
        /// Sets the query plan manager
        /// </summary>
        /// <param name="myQueryplanManager">The query plan manager that should be used within the vertex manager</param>
        public void SetQueryPlanManager(IQueryPlanManager myQueryplanManager)
        {
            _queryPlanManager = myQueryplanManager;
        }

        #endregion
    }
}
