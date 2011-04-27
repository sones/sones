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

        public void CanGetVertices(IExpression iExpression, bool myIsLongRunning, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
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

        public void CanGetVertex(long myVertexTypeID, long myVertexID, string myEdition, TimeSpanDefinition myTimespan, TransactionToken TransactionToken, SecurityToken SecurityToken)
        {
            throw new NotImplementedException();
        }

        public IVertex GetVertex(long myVertexTypeID, long myVertexID, string myEdition, TimeSpanDefinition myTimespan, TransactionToken TransactionToken, SecurityToken SecurityToken)
        {
            throw new NotImplementedException();
        }

        #endregion

        public void CanAddVertex(RequestInsertVertex myInsertDefinition, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            IVertexType vertexType = GetVertexType(myInsertDefinition.VertexTypeName, myTransaction, mySecurity);

            var mandatoryProps = vertexType.GetPropertyDefinitions(true).Where(IsMandatoryProperty).ToArray();


            if (myInsertDefinition.StructuredProperties != null)
            {
                CheckAddStructuredProperties(myInsertDefinition, vertexType);
                CheckMandatoryConstraint(myInsertDefinition, mandatoryProps);
            }
            else
            {
                if (mandatoryProps.Length > 0)
                {
                    throw new MandatoryConstraintViolationException(String.Join(",", mandatoryProps.Select(x => x.Name)));
                }
            }

            if (myInsertDefinition.BinaryProperties != null)
                CheckAddBinaryProperties(myInsertDefinition, vertexType);

        }

        private void CheckMandatoryConstraint(RequestInsertVertex myInsertDefinition, IEnumerable<IPropertyDefinition> myMandatoryProperties)
        {
            foreach (var mand in myMandatoryProperties)
            {
                if (!myInsertDefinition.StructuredProperties.Any(x => mand.Name.Equals(x)))
                {
                    throw new MandatoryConstraintViolationException(mand.Name);
                }
            }
        }

        private static bool IsMandatoryProperty(IPropertyDefinition myPropertyDefinition)
        {
            return myPropertyDefinition.IsMandatory;
        }

        private static void CheckAddBinaryProperties(RequestInsertVertex myInsertDefinition, IVertexType vertexType)
        {
            foreach (var prop in myInsertDefinition.BinaryProperties)
            {
                var propertyDef = vertexType.GetBinaryPropertyDefinition(prop.Key);
                if (propertyDef == null)
                    throw new AttributeDoesNotExistException(prop.Key, myInsertDefinition.VertexTypeName);
            }
        }

        private static void CheckAddStructuredProperties(RequestInsertVertex myInsertDefinition, IVertexType vertexType)
        {
            foreach (var prop in myInsertDefinition.StructuredProperties)
            {
                var propertyDef = vertexType.GetPropertyDefinition(prop.Key);
                if (propertyDef == null)
                    throw new AttributeDoesNotExistException(prop.Key, myInsertDefinition.VertexTypeName);

                //Assign safty should be suffice.
                if (propertyDef.BaseType.IsAssignableFrom(prop.Value.GetType()))
                    throw new PropertyHasWrongTypeException(myInsertDefinition.VertexTypeName, prop.Key, propertyDef.BaseType.Name, prop.Value.GetType().Name);
            }
        }

        private IVertexType GetVertexType(String myVertexTypeName, TransactionToken myTransaction, SecurityToken mySecurity)
        {
            try
            {
                //check if the vertex type exists.
                return _vertexTypeManager.GetVertexType(myVertexTypeName, myTransaction, mySecurity);
            }
            catch (KeyNotFoundException)
            {
                throw new VertexTypeDoesNotExistException(myVertexTypeName);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new EmptyVertexTypeNameException();
            }
        }

        public IVertex AddVertex(RequestInsertVertex myInsertDefinition, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            throw new NotImplementedException();
        }

        public IVertex GetSingleVertex(IExpression myExpression, TransactionToken myTransactionToken, SecurityToken mySecurityToken)
        {
            throw new NotImplementedException();
        }

        public IVertexStore VertexStore
        {
            get { return _vertexStore; }
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
