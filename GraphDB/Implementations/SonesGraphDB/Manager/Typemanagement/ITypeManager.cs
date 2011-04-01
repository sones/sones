using System.Collections.Generic;
using sones.GraphDB.Request;
using sones.GraphDB.TypeSystem;
using sones.Library.Security;
using sones.Library.Transaction;

namespace sones.GraphDB.Manager.TypeManagement
{
    public interface ITypeManager
    {
        /// <summary>
        /// Loads data from the underlying vertex store
        /// </summary>
        void Load();

        /// <summary>
        /// Creates the basic vertex type definitions.
        /// </summary>
        //TODO: here we get a VertexStore(no security, no transaction) and an IndexManager, so we can create the five base vertex types, that are used to store the type manager knowlegde.
        void Create();

        /// <summary>
        /// Adds a given edge type.
        /// </summary>
        /// <param name="myEdgeTypeDefinition">Defines the edge type to be added.</param>
        /// <returns>An instance of IEdgeType, that represents the just now added edge type.</returns>
        IEdgeType AddEdge(EdgeTypeDefinition myEdgeTypeDefinition);

        /// <summary>
        /// Gets a vertex type by name.
        /// </summary>
        /// <param name="myTypeName">
        /// The name of the vertex type.
        /// </param>
        /// <returns>An instance of IVertexType, that represents the vertex type.</returns>
        IVertexType GetVertexType(string myTypeName);

        /// <summary>
        /// Checks if the execution of <see cref="TypeManager.AddVertex(sones.GraphDB.Request.VertexTypeDefinition,sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)"/> will succeed, if no unexpected error occurs.
        /// </summary>
        /// <param name="myVertexTypeDefinition">The definition of the new type.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurityToken">A security token for this operation.</param>
        /// <param name="myMetaManager">The current meta manager.</param>
        /// <returns>
        /// True, if the call of <see cref="TypeManager.AddVertex(sones.GraphDB.Request.VertexTypeDefinition,sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)"/> with the given 
        /// <paramref name="myVertexTypeDefinition"/>, <paramref name="myTransaction"/> and <paramref name="mySecurityToken"/> 
        /// will succeed bar the occurrence of unexpected errors, otherwise false.
        /// </returns>
        bool CanAddVertex(VertexTypeDefinition myVertexTypeDefinition, TransactionToken myTransaction, SecurityToken mySecurityToken, MetaManager myMetaManager);

        /// <summary>
        /// Adds a new vertex type to the type manager.
        /// </summary>
        /// <param name="myVertexTypeDefinition">The definition of the new type.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurityToken">A security token for this operation.</param>
        /// <param name="myMetaManager">The current meta manager.</param>
        void AddVertex(VertexTypeDefinition myVertexTypeDefinition, TransactionToken myTransaction, SecurityToken mySecurityToken, MetaManager myMetaManager);

        /// <summary>
        /// Checks if the execution of <see cref="AddVertex(IEnumerable<VertexTypeDefinition>, TransactionToken, SecurityToken, MetaManager)">AddVertex</see> will succeed, if no unexpected error occurs.
        /// </summary>
        /// <param name="myVertexTypeDefinitions">The definition of the new vertex types.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurityToken">A security token for this operation.</param>
        /// <param name="myMetaManager">The current meta manager.</param>
        /// <returns>
        /// True, if the call of <see cref="AddVertex(IEnumerable<VertexTypeDefinition>, TransactionToken, SecurityToken, MetaManager)">AddVertex</see> with the given 
        /// <paramref name="myVertexTypeDefinitions"/>, <paramref name="myTransaction"/> and <paramref name="mySecurityToken"/> 
        /// will succeed bar the occurrence of unexpected errors, otherwise false.
        /// </returns>
        bool CanAddVertex(IEnumerable<VertexTypeDefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurityToken, MetaManager myMetaManager);

        /// <summary>
        /// Adds a bunch of new vertex types to the type manager.
        /// </summary>
        /// <param name="myVertexTypeDefinitions">The definition of the new vertex types.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurityToken">A security token for this operation.</param>
        /// <param name="myMetaManager">The current meta manager.</param>
        void AddVertex(IEnumerable<VertexTypeDefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurityToken, MetaManager myMetaManager);

        /// <summary>
        /// Checks if the execution of <see cref="TypeManager.RemoveVertex(sones.GraphDB.TypeSystem.IVertexType,sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)"/> will succeed, if no unexpected error occurs.
        /// </summary>
        /// <param name="myVertexType">The vertex type to be removed.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurityToken">A security token for this operation.</param>
        /// <param name="myMetaManager">The current meta manager.</param>
        /// <returns>
        /// True, if the call of <see cref="TypeManager.RemoveVertex(sones.GraphDB.TypeSystem.IVertexType,sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)"/> with the given 
        /// <paramref name="myVertexType"/>, <paramref name="myTransaction"/> and <paramref name="mySecurityToken"/> 
        /// will succeed bar the occurrence of unexpected errors, otherwise false.
        /// </returns>
        bool CanRemoveVertex(IVertexType myVertexType, TransactionToken myTransaction, SecurityToken mySecurityToken, MetaManager myMetaManager);

        /// <summary>
        /// Removes a vertex type from the type manager.
        /// </summary>
        /// <param name="myVertexType">The vertex type that will be removed.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurityToken">A security token for this operation.</param>
        /// <param name="myMetaManager">The current meta manager.</param>
        /// The vertex type will be removed unless there are no edges that point to this type.
        /// If there is such an edge, remove the edge by altering the type that holds it or remove both type simultaneously using <see cref="Add(IEnumerable<IVertexType>, TransactionToken)"/>.
        void RemoveVertex(IVertexType myVertexType, TransactionToken myTransaction, SecurityToken mySecurityToken, MetaManager myMetaManager);

        /// <summary>
        /// Checks if the execution of <see cref="RemoveVertex(IEnumerable<IVertexType>, TransactionToken, SecurityToken, MetaManager)">RemoveVertex</see> will succeed, if no unexpected error occurs.
        /// </summary>
        /// <param name="myVertexTypes">The vertex types to be removed.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurityToken">A security token for this operation.</param>
        /// <param name="myMetaManager">The current meta manager.</param>
        /// <returns>
        /// True, if the call of <see cref="RemoveVertex(IEnumerable<IVertexType>, TransactionToken, SecurityToken, MetaManager)">RemoveVertex</see> with the given 
        /// <paramref name="myVertexTypes"/>, <paramref name="myTransaction"/> and <paramref name="mySecurityToken"/> 
        /// will succeed bar the occurrence of unexpected errors, otherwise false.
        /// </returns>
        bool CanRemoveVertex(IEnumerable<IVertexType> myVertexTypes, TransactionToken myTransaction, SecurityToken mySecurityToken, MetaManager myMetaManager);

        /// <summary>
        /// Removes a bunch of vertex types from the type manager.
        /// </summary>
        /// <param name="myVertexTypes">The vertex types that will be removed.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurityToken">A security token for this operation.</param>
        /// <param name="myMetaManager">The current meta manager.</param>
        /// All types will be removed unless there are no edges that point to at least one of the given types.
        /// If there is such an edge, remove the edge by altering the type that holds it or remove this type too.
        /// All types are removed simultaneously. This means that edges between the types are not need to be removed before.
        void RemoveVertex(IEnumerable<IVertexType> myVertexTypes, TransactionToken myTransaction, SecurityToken mySecurityToken, MetaManager myMetaManager);

        /// <summary>
        /// Checks if the execution of <see cref="TypeManager.UpdateVertex(sones.GraphDB.Request.VertexTypeDefinition,sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)"/> will succeed, if no unexpected error occurs.
        /// </summary>
        /// <param name="myVertexTypeDefinition">TODO: for update use VertexTypeUpdateDefinition</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurityToken">A security token for this operation.</param>
        /// <param name="myMetaManager">The current meta manager.</param>
        /// <returns>
        /// True, if the call of <see cref="TypeManager.UpdateVertex(sones.GraphDB.Request.VertexTypeDefinition,sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)"/> with the given 
        /// <paramref name="myVertexTypeDefinition"/>, <paramref name="myTransaction"/> and <paramref name="mySecurityToken"/> 
        /// will succeed bar the occurrence of unexpected errors, otherwise false.
        /// </returns>
        bool CanUpdateVertex(VertexTypeDefinition myVertexTypeDefinition, TransactionToken myTransaction, SecurityToken mySecurityToken, MetaManager myMetaManager);

        /// <summary>
        /// Updates an existing vertex type.
        /// </summary>
        /// <param name="myVertexTypeDefinition">TODO: for update use VertexTypeUpdateDefinition</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurityToken">A security token for this operation.</param>
        /// <param name="myMetaManager">The current meta manager.</param>
        void UpdateVertex(VertexTypeDefinition myVertexTypeDefinition, TransactionToken myTransaction, SecurityToken mySecurityToken, MetaManager myMetaManager);

        /// <summary>
        /// Checks if the execution of <see cref="UpdateVertex(IEnumerable<VertexTypeDefinition>, TransactionToken, SecurityToken, MetaManager)"/> will succeed, if no unexpected error occurs.
        /// </summary>
        /// <param name="myVertexTypeDefinitions">TODO: for update use VertexTypeUpdateDefinition</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurityToken">A security token for this operation.</param>
        /// <param name="myMetaManager">The current meta manager.</param>
        /// <returns>
        /// True, if the call of <see cref="UpdateVertex(IEnumerable<VertexTypeDefinition>, TransactionToken, SecurityToken, MetaManager)"/> with the given 
        /// <paramref name="myVertexTypeDefinitions"/>, <paramref name="myTransaction"/> and <paramref name="mySecurityToken"/> 
        /// will succeed bar the occurrence of unexpected errors, otherwise false.
        /// </returns>
        bool CanUpdateVertex(IEnumerable<VertexTypeDefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurityToken, MetaManager myMetaManager);

        /// <summary>
        /// Updates existing vertex types.
        /// </summary>
        /// <param name="myVertexTypeDefinition">TODO: for update use VertexTypeUpdateDefinition</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurityToken">A security token for this operation.</param>
        /// <param name="myMetaManager">The current meta manager.</param>
        void UpdateVertex(IEnumerable<VertexTypeDefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurityToken, MetaManager myMetaManager);
    }
}