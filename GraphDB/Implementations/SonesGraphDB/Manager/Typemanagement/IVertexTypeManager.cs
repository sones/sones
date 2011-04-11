using System.Collections.Generic;
using sones.GraphDB.Request;
using sones.GraphDB.TypeSystem;
using sones.Library.Security;
using sones.Library.Transaction;

namespace sones.GraphDB.Manager.TypeManagement
{
    public interface IVertexTypeManager: IStorageUsingManager
    {
        /// <summary>
        /// Gets a vertex type by name.
        /// </summary>
        /// <param name="myTypeName">
        /// The name of the vertex type.
        /// </param>
        /// <returns>An instance of IVertexType, that represents the vertex type.</returns>
        IVertexType GetVertexType(string myTypeName);

        /// <summary>
        /// Checks if the execution of <see cref="AddVertexType(VertexTypeDefinition, TransactionToken, SecurityToken, MetaManager)"/> will succeed, if no unexpected error occurs.
        /// </summary>
        /// <param name="myVertexTypeDefinition">The definition of the new type.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        /// <returns>
        /// True, if the call of <see cref="AddVertexType(VertexTypeDefinition, TransactionToken, SecurityToken, MetaManager)"/> with the given 
        /// <paramref name="myVertexTypeDefinition"/>, <paramref name="myTransaction"/> and <paramref name="mySecurity"/> 
        /// will succeed bar the occurrence of unexpected errors, otherwise false.
        /// </returns>
        bool CanAddVertexType(VertexTypeDefinition myVertexTypeDefinition, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Adds a new vertex type to the type manager.
        /// </summary>
        /// <param name="myVertexTypeDefinition">The definition of the new type.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        IVertexType AddVertexType(VertexTypeDefinition myVertexTypeDefinition, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Checks if the execution of <see cref="AddVertexType(System.Collections.Generic.IEnumerable{sones.GraphDB.Request.VertexTypeDefinition},sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)" /> will succeed, if no unexpected error occurs.
        /// </summary>
        /// <param name="myVertexTypeDefinitions">The definition of the new vertex types.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        /// <returns>
        /// True, if the call of <see cref="AddVertexType(System.Collections.Generic.IEnumerable{sones.GraphDB.Request.VertexTypeDefinition},sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)"/> with the given 
        /// <paramref name="myVertexTypeDefinitions"/>, <paramref name="myTransaction"/> and <paramref name="mySecurity"/> 
        /// will succeed bar the occurrence of unexpected errors, otherwise false.
        /// </returns>
        bool CanAddVertexType(IEnumerable<VertexTypeDefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Adds a bunch of new vertex types to the type manager.
        /// </summary>
        /// <param name="myVertexTypeDefinitions">The definition of the new vertex types.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        IVertexType AddVertexType(IEnumerable<VertexTypeDefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Checks if the execution of <see cref="RemoveVertexType(IVertexType, TransactionToken, SecurityToken, MetaManager)"/> will succeed, if no unexpected error occurs.
        /// </summary>
        /// <param name="myVertexType">The vertex type to be removed.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        /// <returns>
        /// True, if the call of <see cref="RemoveVertexType(IVertexType, TransactionToken, SecurityToken, MetaManager)"/> with the given 
        /// <paramref name="myVertexType"/>, <paramref name="myTransaction"/> and <paramref name="mySecurity"/> 
        /// will succeed bar the occurrence of unexpected errors, otherwise false.
        /// </returns>
        bool CanRemoveVertexType(IVertexType myVertexType, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Removes a vertex type from the type manager.
        /// </summary>
        /// <param name="myVertexType">The vertex type that will be removed.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        /// The vertex type will be removed unless there are no edges that point to this type.
        /// If there is such an edge, remove the edge by altering the type that holds it or remove boths type simultaneously using 
        /// <see cref="RemoveVertexType(System.Collections.Generic.IEnumerable{sones.GraphDB.TypeSystem.IVertexType},sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)" />.
        void RemoveVertexType(IVertexType myVertexType, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Checks if the execution of <see cref="RemoveVertexType(System.Collections.Generic.IEnumerable{sones.GraphDB.TypeSystem.IVertexType},sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)"/> will succeed, if no unexpected error occurs.
        /// </summary>
        /// <param name="myVertexTypes">The vertex types to be removed.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        /// <returns>
        /// True, if the call of <see cref="RemoveVertexType(System.Collections.Generic.IEnumerable{sones.GraphDB.TypeSystem.IVertexType},sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)"/> with the given 
        /// <paramref name="myVertexTypes"/>, <paramref name="myTransaction"/> and <paramref name="mySecurity"/> 
        /// will succeed bar the occurrence of unexpected errors, otherwise false.
        /// </returns>
        bool CanRemoveVertexType(IEnumerable<IVertexType> myVertexTypes, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Removes a bunch of vertex types from the type manager.
        /// </summary>
        /// <param name="myVertexTypes">The vertex types that will be removed.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        /// All types will be removed unless there are no edges that point to at least one of the given types.
        /// If there is such an edge, remove the edge by altering the type that holds it or remove this type too.
        /// All types are removed simultaneously. This means that edges between the types are not need to be removed before.
        void RemoveVertexType(IEnumerable<IVertexType> myVertexTypes, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Checks if the execution of <see cref="UpdateVertexType(VertexTypeDefinition, TransactionToken, SecurityToken, MetaManager)"/> will succeed, if no unexpected error occurs.
        /// </summary>
        /// <param name="myVertexTypeDefinition">TODO: for update use VertexTypeUpdateDefinition</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        /// <returns>
        /// True, if the call of <see cref="UpdateVertexType(VertexTypeDefinition, TransactionToken, SecurityToken, MetaManager)"/> with the given 
        /// <paramref name="myVertexTypeDefinition"/>, <paramref name="myTransaction"/> and <paramref name="mySecurity"/> 
        /// will succeed bar the occurrence of unexpected errors, otherwise false.
        /// </returns>
        bool CanUpdateVertexType(VertexTypeDefinition myVertexTypeDefinition, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Updates an existing vertex type.
        /// </summary>
        /// <param name="myVertexTypeDefinition">TODO: for update use VertexTypeUpdateDefinition</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        void UpdateVertexType(VertexTypeDefinition myVertexTypeDefinition, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Checks if the execution of <see cref="UpdateVertexType(System.Collections.Generic.IEnumerable{sones.GraphDB.Request.VertexTypeDefinition},sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)"/> will succeed, if no unexpected error occurs.
        /// </summary>
        /// <param name="myVertexTypeDefinitions">TODO: for update use VertexTypeUpdateDefinition</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        /// <returns>
        /// True, if the call of <see cref="UpdateVertexType(System.Collections.Generic.IEnumerable{sones.GraphDB.Request.VertexTypeDefinition},sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)"/> with the given 
        /// <paramref name="myVertexTypeDefinitions"/>, <paramref name="myTransaction"/> and <paramref name="mySecurity"/> 
        /// will succeed bar the occurrence of unexpected errors, otherwise false.
        /// </returns>
        bool CanUpdateVertexType(IEnumerable<VertexTypeDefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Updates existing vertex types.
        /// </summary>
        /// <param name="myVertexTypeDefinitions">TODO: for update use VertexTypeUpdateDefinition</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        void UpdateVertexType(IEnumerable<VertexTypeDefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);
    }
}