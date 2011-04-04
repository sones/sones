using System.Collections.Generic;
using sones.GraphDB.TypeSystem;
using sones.GraphDB.Request;
using sones.Library.Transaction;
using sones.Library.LanguageExtensions;
using System;
using sones.Library.Security;
using sones.GraphDB.Manager.Index;
using sones.Library.VertexStore;

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
 *   - load 
 *     - Timo
 *       - will proove if the five main vertex types are available
 *       - will load the five main vetex types
 *       - looks for the maximum vertex type id
 *       
 *   - create
 *     - Timo
 *       - will add the five main vertex types 
 * 
 */



namespace sones.GraphDB.Manager.TypeManagement
{
    /* This class is splitted in three partial classes:
     * - TypeManager.cs declares the public methods for vertex and edge types
     * - EdgeTypeManager.cs declares the private methods for edge types
     * - VertexTypeManager.cs declares the private methods for vertex types
     */

    /// <summary>
    /// A class that represents an type manager.
    /// </summary>
    /// The responsibilities of the type manager are creating, removing und retrieving of types.
    /// Each database has one type manager.
    public sealed partial class TypeManager : ITypeManager
    {
        #region Data

        private long _typeID = Int64.MinValue;

        #endregion

        #region c'tor

        //TODO: here we get a slim version of IGraphDB. So we can ask for types by name and are sure, that the index is used.
        public TypeManager()
        {
            
        }

        #endregion

        #region public methods regarding type manager itself

        /// <summary>
        /// Loads data from the underlying vertex store
        /// </summary>
        public void Load(MetaManager myMetaManager)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Creates the basic vertex type definitions.
        /// </summary>
        //TODO: here we get a VertexStore(no security, no transaction) and an IndexManager, so we can create the five base vertex types, that are used to store the type manager knowlegde.
        public void Create(IIndexManager myIndexMgr, IVertexStore myVertexStore)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region public methods regarding edge type

        #region update edge types

        /// <summary>
        /// Adds a given edge type.
        /// </summary>
        /// <param name="myEdgeTypeDefinition">Defines the edge type to be added.</param>
        /// <returns>An instance of IEdgeType, that represents the just now added edge type.</returns>
        public IEdgeType AddEdge(EdgeTypeDefinition myEdgeTypeDefinition)
        {
            return DoAddEdge(myEdgeTypeDefinition);
        }

        #endregion

        #endregion

        #region public methods regarding vertex type

        #region get vertex types

        /// <summary>
        /// Gets a vertex type by name.
        /// </summary>
        /// <param name="myTypeName">
        /// The name of the vertex type.
        /// </param>
        /// <returns>An instance of IVertexType, that represents the vertex type.</returns>
        public IVertexType GetVertexType(string myTypeName)
        {
            return DoGetVertexType(myTypeName);
        }

        #endregion

        #region update vertex types

        #region Add

        /// <summary>
        /// Checks if the execution of <see cref="AddVertex(VertexTypeDefinition, TransactionToken, SecurityToken, MetaManager)"/> will succeed, if no unexpected error occurs.
        /// </summary>
        /// <param name="myVertexTypeDefinition">The definition of the new type.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The current meta manager.</param>
        /// <returns>
        /// True, if the call of <see cref="AddVertex(VertexTypeDefinition, TransactionToken, SecurityToken, MetaManager)"/> with the given 
        /// <paramref name="myVertexTypeDefinition"/>, <paramref name="myTransaction"/> and <paramref name="mySecurity"/> 
        /// will succeed bar the occurrence of unexpected errors, otherwise false.
        /// </returns>
        public bool CanAddVertex(VertexTypeDefinition myVertexTypeDefinition, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            return DoCanAddVertex(myVertexTypeDefinition.SingleEnumerable(), myTransaction, mySecurity, myMetaManager);
        }

        /// <summary>
        /// Adds a new vertex type to the type manager.
        /// </summary>
        /// <param name="myVertexTypeDefinition">The definition of the new type.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The current meta manager.</param>
        public void AddVertex(VertexTypeDefinition myVertexTypeDefinition, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            DoAddVertex(myVertexTypeDefinition.SingleEnumerable(), myTransaction, mySecurity, myMetaManager);
        }

        /// <summary>
        /// Checks if the execution of <see cref="AddVertex(IEnumerable{VertexTypeDefinition}, TransactionToken, SecurityToken, MetaManager)" /> will succeed, if no unexpected error occurs.
        /// </summary>
        /// <param name="myVertexTypeDefinitions">The definition of the new vertex types.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The current meta manager.</param>
        /// <returns>
        /// True, if the call of <see cref="AddVertex(IEnumerable{VertexTypeDefinition}, TransactionToken, SecurityToken, MetaManager)"/> with the given 
        /// <paramref name="myVertexTypeDefinitions"/>, <paramref name="myTransaction"/> and <paramref name="mySecurity"/> 
        /// will succeed bar the occurrence of unexpected errors, otherwise false.
        /// </returns>
        public bool CanAddVertex(IEnumerable<VertexTypeDefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            return DoCanAddVertex(myVertexTypeDefinitions, myTransaction, mySecurity, myMetaManager);
        }

        /// <summary>
        /// Adds a bunch of new vertex types to the type manager.
        /// </summary>
        /// <param name="myVertexTypeDefinitions">The definition of the new vertex types.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The current meta manager.</param>
        public void AddVertex(IEnumerable<VertexTypeDefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            DoAddVertex(myVertexTypeDefinitions, myTransaction, mySecurity, myMetaManager);
        }

        #endregion

        #region Remove

        /// <summary>
        /// Checks if the execution of <see cref="RemoveVertex(IVertexType, TransactionToken, SecurityToken, MetaManager)"/> will succeed, if no unexpected error occurs.
        /// </summary>
        /// <param name="myVertexType">The vertex type to be removed.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The current meta manager.</param>
        /// <returns>
        /// True, if the call of <see cref="RemoveVertex(IVertexType, TransactionToken, SecurityToken, MetaManager)"/> with the given 
        /// <paramref name="myVertexType"/>, <paramref name="myTransaction"/> and <paramref name="mySecurity"/> 
        /// will succeed bar the occurrence of unexpected errors, otherwise false.
        /// </returns>
        public bool CanRemoveVertex(IVertexType myVertexType, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            return DoCanRemoveVertex(myVertexType.SingleEnumerable(), myTransaction, mySecurity, myMetaManager);
        }

        /// <summary>
        /// Removes a vertex type from the type manager.
        /// </summary>
        /// <param name="myVertexType">The vertex type that will be removed.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The current meta manager.</param>
        /// The vertex type will be removed unless there are no edges that point to this type.
        /// If there is such an edge, remove the edge by altering the type that holds it or remove both type simultaneously using <see cref="Add(IEnumerable<IVertexType>, TransactionToken)"/>.
        public void RemoveVertex(IVertexType myVertexType, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            DoRemoveVertex(myVertexType.SingleEnumerable(), myTransaction, mySecurity, myMetaManager);
        }

        /// <summary>
        /// Checks if the execution of <see cref="RemoveVertex(IEnumerable{IVertexType}, TransactionToken, SecurityToken, MetaManager)"/> will succeed, if no unexpected error occurs.
        /// </summary>
        /// <param name="myVertexTypes">The vertex types to be removed.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The current meta manager.</param>
        /// <returns>
        /// True, if the call of <see cref="RemoveVertex(IEnumerable{IVertexType}, TransactionToken, SecurityToken, MetaManager)"/> with the given 
        /// <paramref name="myVertexTypes"/>, <paramref name="myTransaction"/> and <paramref name="mySecurity"/> 
        /// will succeed bar the occurrence of unexpected errors, otherwise false.
        /// </returns>
        public bool CanRemoveVertex(IEnumerable<IVertexType> myVertexTypes, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            return DoCanRemoveVertex(myVertexTypes, myTransaction, mySecurity, myMetaManager);
        }

        /// <summary>
        /// Removes a bunch of vertex types from the type manager.
        /// </summary>
        /// <param name="myVertexTypes">The vertex types that will be removed.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The current meta manager.</param>
        /// All types will be removed unless there are no edges that point to at least one of the given types.
        /// If there is such an edge, remove the edge by altering the type that holds it or remove this type too.
        /// All types are removed simultaneously. This means that edges between the types are not need to be removed before.
        public void RemoveVertex(IEnumerable<IVertexType> myVertexTypes, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            DoRemoveVertex(myVertexTypes, myTransaction, mySecurity, myMetaManager);
        }

        #endregion

        #region Update

        /// <summary>
        /// Checks if the execution of <see cref="UpdateVertex(VertexTypeDefinition, TransactionToken, SecurityToken, MetaManager)"/> will succeed, if no unexpected error occurs.
        /// </summary>
        /// <param name="myVertexTypeDefinition">TODO: for update use VertexTypeUpdateDefinition</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The current meta manager.</param>
        /// <returns>
        /// True, if the call of <see cref="UpdateVertex(VertexTypeDefinition, TransactionToken, SecurityToken, MetaManager)"/> with the given 
        /// <paramref name="myVertexTypeDefinition"/>, <paramref name="myTransaction"/> and <paramref name="mySecurity"/> 
        /// will succeed bar the occurrence of unexpected errors, otherwise false.
        /// </returns>
        public bool CanUpdateVertex(VertexTypeDefinition myVertexTypeDefinition, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            return DoCanUpdateVertex(myVertexTypeDefinition.SingleEnumerable(), myTransaction, mySecurity, myMetaManager);
        }

        /// <summary>
        /// Updates an existing vertex type.
        /// </summary>
        /// <param name="myVertexTypeDefinition">TODO: for update use VertexTypeUpdateDefinition</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The current meta manager.</param>
        public void UpdateVertex(VertexTypeDefinition myVertexTypeDefinition, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            DoUpdateVertex(myVertexTypeDefinition.SingleEnumerable(), myTransaction, mySecurity, myMetaManager);
        }

        /// <summary>
        /// Checks if the execution of <see cref="UpdateVertex(IEnumerable{VertexTypeDefinition}, TransactionToken, SecurityToken, MetaManager)"/> will succeed, if no unexpected error occurs.
        /// </summary>
        /// <param name="myVertexTypeDefinitions">TODO: for update use VertexTypeUpdateDefinition</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The current meta manager.</param>
        /// <returns>
        /// True, if the call of <see cref="UpdateVertex(IEnumerable{VertexTypeDefinition}, TransactionToken, SecurityToken, MetaManager)"/> with the given 
        /// <paramref name="myVertexTypeDefinitions"/>, <paramref name="myTransaction"/> and <paramref name="mySecurity"/> 
        /// will succeed bar the occurrence of unexpected errors, otherwise false.
        /// </returns>
        public bool CanUpdateVertex(IEnumerable<VertexTypeDefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            return DoCanUpdateVertex(myVertexTypeDefinitions, myTransaction, mySecurity, myMetaManager);
        }

        /// <summary>
        /// Updates existing vertex types.
        /// </summary>
        /// <param name="myVertexTypeDefinitions">TODO: for update use VertexTypeUpdateDefinition</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The current meta manager.</param>
        public void UpdateVertex(IEnumerable<VertexTypeDefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager)
        {
            DoUpdateVertex(myVertexTypeDefinitions, myTransaction, mySecurity, myMetaManager);
        }

        #endregion

        #endregion

        #endregion
    }
}
