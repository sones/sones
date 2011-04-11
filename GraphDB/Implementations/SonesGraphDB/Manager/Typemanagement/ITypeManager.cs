using System.Collections.Generic;
using sones.GraphDB.Request;
using sones.GraphDB.TypeSystem;
using sones.Library.Security;
using sones.Library.Transaction;
using sones.GraphDB.Manager.Index;
using sones.Library.VertexStore;

namespace sones.GraphDB.Manager.TypeManagement
{
    /// <summary>
    /// An interface that represents an type manager.
    /// </summary>
    /// The responsibilities of the type manager are creating, removing und retrieving of types.
    /// Each database has one type manager.
    public interface ITypeManager
    {
        /// <summary>
        /// Loads data from the underlying parentVertex store
        /// </summary>
        void Load(MetaManager myMetaManager);

        /// <summary>
        /// Creates the basic parentVertex type definitions.
        /// </summary>
        //TODO: here we get a VertexStore(no security, no transaction) and an IndexManager, so we can create the five base parentVertex types, that are used to store the type manager knowlegde.
        void Create(IIndexManager myIndexMgr, IVertexStore myVertexStore);

        /// <summary>
        /// Adds a given edge type.
        /// </summary>
        /// <param name="myEdgeTypeDefinition">Defines the edge type to be added.</param>
        /// <returns>An instance of IEdgeType, that represents the just now added edge type.</returns>
        IEdgeType AddEdge(EdgeTypeDefinition myEdgeTypeDefinition);

        /// <summary>
        /// Gets a parentVertex type by name.
        /// </summary>
        /// <param name="myTypeName">
        /// The name of the parentVertex type.
        /// </param>
        /// <returns>An instance of IVertexType, that represents the parentVertex type.</returns>
        IVertexType GetVertexType(string myTypeName);

        /// <summary>
        /// Checks if the execution of <see cref="AddVertex(sones.GraphDB.Request.VertexTypeDefinitions,sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)"/> will succeed, if no unexpected error occurs.
        /// </summary>
        /// <param name="myVertexTypeDefinition">The definition of the new type.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The myOutgoingEdgeVertex meta manager.</param>
        /// <returns>
        /// True, if the call of <see cref="AddVertex(sones.GraphDB.Request.VertexTypeDefinitions,sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)"/> with the given 
        /// <paramref name="myVertexTypeDefinition"/>, <paramref name="myTransaction"/> and <paramref name="mySecurity"/> 
        /// will succeed bar the occurrence of unexpected errors, otherwise false.
        /// </returns>
        bool CanAddVertex(VertexTypeDefinition myVertexTypeDefinition, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Adds a new parentVertex type to the type manager.
        /// </summary>
        /// <param name="myVertexTypeDefinition">The definition of the new type.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The myOutgoingEdgeVertex meta manager.</param>
        IVertexType AddVertex(VertexTypeDefinition myVertexTypeDefinition, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Checks if the execution of <see cref="AddVertex(System.Collections.Generic.IEnumerable{sones.GraphDB.Request.VertexTypeDefinitions},sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)" /> will succeed, if no unexpected error occurs.
        /// </summary>
        /// <param name="myVertexTypeDefinitions">The definition of the new parentVertex types.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The myOutgoingEdgeVertex meta manager.</param>
        /// <returns>
        /// True, if the call of <see cref="AddVertex(System.Collections.Generic.IEnumerable{sones.GraphDB.Request.VertexTypeDefinitions},sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)"/> with the given 
        /// <paramref name="myVertexTypeDefinitions"/>, <paramref name="myTransaction"/> and <paramref name="mySecurity"/> 
        /// will succeed bar the occurrence of unexpected errors, otherwise false.
        /// </returns>
        bool CanAddVertex(IEnumerable<VertexTypeDefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Adds a bunch of new parentVertex types to the type manager.
        /// </summary>
        /// <param name="myVertexTypeDefinitions">The definition of the new parentVertex types.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The myOutgoingEdgeVertex meta manager.</param>
        IVertexType AddVertex(IEnumerable<VertexTypeDefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Checks if the execution of <see cref="RemoveVertex(sones.GraphDB.TypeSystem.IVertexType,sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)"/> will succeed, if no unexpected error occurs.
        /// </summary>
        /// <param name="myVertexType">The parentVertex type to be removed.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The myOutgoingEdgeVertex meta manager.</param>
        /// <returns>
        /// True, if the call of <see cref="RemoveVertex(sones.GraphDB.TypeSystem.IVertexType,sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)"/> with the given 
        /// <paramref name="myVertexType"/>, <paramref name="myTransaction"/> and <paramref name="mySecurity"/> 
        /// will succeed bar the occurrence of unexpected errors, otherwise false.
        /// </returns>
        bool CanRemoveVertex(IVertexType myVertexType, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Removes a parentVertex type from the type manager.
        /// </summary>
        /// <param name="myVertexType">The parentVertex type that will be removed.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The myOutgoingEdgeVertex meta manager.</param>
        /// The parentVertex type will be removed unless there are no edges that point to this type.
        /// If there is such an edge, remove the edge by altering the type that holds it or remove both type simultaneously using <see cref="Add(IEnumerable<IVertexType>, TransactionToken)"/>.
        void RemoveVertex(IVertexType myVertexType, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Checks if the execution of <see cref="RemoveVertex(System.Collections.Generic.IEnumerable{sones.GraphDB.TypeSystem.IVertexType},sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)"/> will succeed, if no unexpected error occurs.
        /// </summary>
        /// <param name="myVertexTypes">The parentVertex types to be removed.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The myOutgoingEdgeVertex meta manager.</param>
        /// <returns>
        /// True, if the call of <see cref="RemoveVertex(System.Collections.Generic.IEnumerable{sones.GraphDB.TypeSystem.IVertexType},sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)"/> with the given 
        /// <paramref name="myVertexTypes"/>, <paramref name="myTransaction"/> and <paramref name="mySecurity"/> 
        /// will succeed bar the occurrence of unexpected errors, otherwise false.
        /// </returns>
        bool CanRemoveVertex(IEnumerable<IVertexType> myVertexTypes, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Removes a bunch of parentVertex types from the type manager.
        /// </summary>
        /// <param name="myVertexTypes">The parentVertex types that will be removed.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The myOutgoingEdgeVertex meta manager.</param>
        /// All types will be removed unless there are no edges that point to at least one of the given types.
        /// If there is such an edge, remove the edge by altering the type that holds it or remove this type too.
        /// All types are removed simultaneously. This means that edges between the types are not need to be removed before.
        void RemoveVertex(IEnumerable<IVertexType> myVertexTypes, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Checks if the execution of <see cref="UpdateVertex(sones.GraphDB.Request.VertexTypeDefinitions,sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)"/> will succeed, if no unexpected error occurs.
        /// </summary>
        /// <param name="myVertexTypeDefinition">TODO: for update use VertexTypeUpdateDefinition</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The myOutgoingEdgeVertex meta manager.</param>
        /// <returns>
        /// True, if the call of <see cref="UpdateVertex(sones.GraphDB.Request.VertexTypeDefinitions,sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)"/> with the given 
        /// <paramref name="myVertexTypeDefinition"/>, <paramref name="myTransaction"/> and <paramref name="mySecurity"/> 
        /// will succeed bar the occurrence of unexpected errors, otherwise false.
        /// </returns>
        bool CanUpdateVertex(VertexTypeDefinition myVertexTypeDefinition, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Updates an existing parentVertex type.
        /// </summary>
        /// <param name="myVertexTypeDefinition">TODO: for update use VertexTypeUpdateDefinition</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The myOutgoingEdgeVertex meta manager.</param>
        void UpdateVertex(VertexTypeDefinition myVertexTypeDefinition, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Checks if the execution of <see cref="UpdateVertex(System.Collections.Generic.IEnumerable{sones.GraphDB.Request.VertexTypeDefinitions},sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)"/> will succeed, if no unexpected error occurs.
        /// </summary>
        /// <param name="myVertexTypeDefinitions">TODO: for update use VertexTypeUpdateDefinition</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The myOutgoingEdgeVertex meta manager.</param>
        /// <returns>
        /// True, if the call of <see cref="UpdateVertex(System.Collections.Generic.IEnumerable{sones.GraphDB.Request.VertexTypeDefinitions},sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)"/> with the given 
        /// <paramref name="myVertexTypeDefinitions"/>, <paramref name="myTransaction"/> and <paramref name="mySecurity"/> 
        /// will succeed bar the occurrence of unexpected errors, otherwise false.
        /// </returns>
        bool CanUpdateVertex(IEnumerable<VertexTypeDefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Updates existing parentVertex types.
        /// </summary>
        /// <param name="myVertexTypeDefinitions">TODO: for update use VertexTypeUpdateDefinition</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The myOutgoingEdgeVertex meta manager.</param>
        void UpdateVertex(IEnumerable<VertexTypeDefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);
    }
}