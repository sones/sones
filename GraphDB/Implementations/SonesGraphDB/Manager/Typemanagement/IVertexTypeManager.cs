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
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        /// <returns>An instance of IVertexType, that represents the vertex type.</returns>
        IVertexType GetVertexType(string myTypeName, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Checks if the execution of <see cref="AddVertexType(VertexTypePredefinition, TransactionToken, SecurityToken, MetaManager)"/> will succeed, if no unexpected error occurs.
        /// </summary>
        /// <param name="myVertexTypePredefinition">The definition of the new type.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        /// <returns>
        /// True, if the call of <see cref="AddVertexType(VertexTypePredefinition, TransactionToken, SecurityToken, MetaManager)"/> with the given 
        /// <paramref name="myVertexTypePredefinition"/>, <paramref name="myTransaction"/> and <paramref name="mySecurity"/> 
        /// will succeed bar the occurrence of unexpected errors, otherwise false.
        /// </returns>
        bool CanAddVertexType(VertexTypePredefinition myVertexTypePredefinition, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Adds a new vertex type to the type manager.
        /// </summary>
        /// <param name="myVertexTypePredefinition">The definition of the new type.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        IVertexType AddVertexType(VertexTypePredefinition myVertexTypePredefinition, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Checks if the execution of <see cref="AddVertexType(System.Collections.Generic.IEnumerable{VertexTypePredefinition},sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)" /> will succeed, if no unexpected error occurs.
        /// </summary>
        /// <param name="myVertexTypeDefinitions">The definition of the new vertex types.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        /// <returns>
        /// True, if the call of <see cref="AddVertexType(System.Collections.Generic.IEnumerable{VertexTypePredefinition},sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)"/> with the given 
        /// <paramref name="myVertexTypeDefinitions"/>, <paramref name="myTransaction"/> and <paramref name="mySecurity"/> 
        /// will succeed bar the occurrence of unexpected errors, otherwise false.
        /// </returns>
        bool CanAddVertexType(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Adds a bunch of new vertex types to the type manager.
        /// </summary>
        /// <param name="myVertexTypeDefinitions">The definition of the new vertex types.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        IVertexType AddVertexType(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

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
        /// Checks if the execution of <see cref="UpdateVertexType(VertexTypePredefinition, TransactionToken, SecurityToken, MetaManager)"/> will succeed, if no unexpected error occurs.
        /// </summary>
        /// <param name="myVertexTypePredefinition">TODO: for update use VertexTypeUpdateDefinition</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        /// <returns>
        /// True, if the call of <see cref="UpdateVertexType(VertexTypePredefinition, TransactionToken, SecurityToken, MetaManager)"/> with the given 
        /// <paramref name="myVertexTypePredefinition"/>, <paramref name="myTransaction"/> and <paramref name="mySecurity"/> 
        /// will succeed bar the occurrence of unexpected errors, otherwise false.
        /// </returns>
        bool CanUpdateVertexType(VertexTypePredefinition myVertexTypePredefinition, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Updates an existing vertex type.
        /// </summary>
        /// <param name="myVertexTypePredefinition">TODO: for update use VertexTypeUpdateDefinition</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        void UpdateVertexType(VertexTypePredefinition myVertexTypePredefinition, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Checks if the execution of <see cref="UpdateVertexType(System.Collections.Generic.IEnumerable{VertexTypePredefinition},sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)"/> will succeed, if no unexpected error occurs.
        /// </summary>
        /// <param name="myVertexTypeDefinitions">TODO: for update use VertexTypeUpdateDefinition</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        /// <returns>
        /// True, if the call of <see cref="UpdateVertexType(System.Collections.Generic.IEnumerable{VertexTypePredefinition},sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)"/> with the given 
        /// <paramref name="myVertexTypeDefinitions"/>, <paramref name="myTransaction"/> and <paramref name="mySecurity"/> 
        /// will succeed bar the occurrence of unexpected errors, otherwise false.
        /// </returns>
        bool CanUpdateVertexType(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Updates existing vertex types.
        /// </summary>
        /// <param name="myVertexTypeDefinitions">TODO: for update use VertexTypeUpdateDefinition</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        void UpdateVertexType(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);
    }
}