using System.Collections.Generic;
using sones.GraphDB.Request;
using sones.GraphDB.TypeSystem;
using sones.Library.Security;
using sones.Library.Transaction;

namespace sones.GraphDB.Manager.TypeManagement
{
    public interface IEdgeTypeManager: IStorageUsingManager
    {
        /// <summary>
        /// Gets a e type by name.
        /// </summary>
        /// <param name="myTypeName">
        /// The name of the e type.
        /// </param>
        /// <returns>An instance of IEdgeType, that represents the e type.</returns>
        IEdgeType GetEdgeType(string myTypeName);

        /// <summary>
        /// Checks if the execution of <see cref="AddEdge(sones.GraphDB.Request.EdgeTypeDefinition,sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)"/> will succeed, if no unexpected error occurs.
        /// </summary>
        /// <param name="myEdgeTypeDefinition">The definition of the new type.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        /// <returns>
        /// True, if the call of <see cref="AddEdge(sones.GraphDB.Request.EdgeTypeDefinition,sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)"/> with the given 
        /// <paramref name="myEdgeTypeDefinition"/>, <paramref name="myTransaction"/> and <paramref name="mySecurity"/> 
        /// will succeed bar the occurrence of unexpected errors, otherwise false.
        /// </returns>
        bool CanAddEdge(EdgeTypeDefinition myEdgeTypeDefinition, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Adds a new e type to the type manager.
        /// </summary>
        /// <param name="myEdgeTypeDefinition">The definition of the new type.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        IEdgeType AddEdge(EdgeTypeDefinition myEdgeTypeDefinition, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Checks if the execution of <see cref="AddEdge(System.Collections.Generic.IEnumerable{sones.GraphDB.Request.EdgeTypeDefinition},sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)" /> will succeed, if no unexpected error occurs.
        /// </summary>
        /// <param name="myEdgeTypeDefinitions">The definition of the new e types.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        /// <returns>
        /// True, if the call of <see cref="AddEdge(System.Collections.Generic.IEnumerable{sones.GraphDB.Request.EdgeTypeDefinition},sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)"/> with the given 
        /// <paramref name="myEdgeTypeDefinitions"/>, <paramref name="myTransaction"/> and <paramref name="mySecurity"/> 
        /// will succeed bar the occurrence of unexpected errors, otherwise false.
        /// </returns>
        bool CanAddEdge(IEnumerable<EdgeTypeDefinition> myEdgeTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Adds a bunch of new e types to the type manager.
        /// </summary>
        /// <param name="myEdgeTypeDefinitions">The definition of the new e types.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        IEdgeType AddEdge(IEnumerable<EdgeTypeDefinition> myEdgeTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Checks if the execution of <see cref="RemoveEdge(sones.GraphDB.TypeSystem.IEdgeType,sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)"/> will succeed, if no unexpected error occurs.
        /// </summary>
        /// <param name="myEdgeType">The e type to be removed.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        /// <returns>
        /// True, if the call of <see cref="RemoveEdge(sones.GraphDB.TypeSystem.IEdgeType,sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)"/> with the given 
        /// <paramref name="myEdgeType"/>, <paramref name="myTransaction"/> and <paramref name="mySecurity"/> 
        /// will succeed bar the occurrence of unexpected errors, otherwise false.
        /// </returns>
        bool CanRemoveEdge(IEdgeType myEdgeType, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Removes a e type from the type manager.
        /// </summary>
        /// <param name="myEdgeType">The e type that will be removed.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        /// The e type will be removed unless there are no edges that point to this type.
        /// If there is such an edge, remove the edge by altering the type that holds it or remove boths type simultaneously using 
        /// <see cref="RemoveEdge(System.Collections.Generic.IEnumerable{sones.GraphDB.TypeSystem.IEdgeType}, sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)" />.
        void RemoveEdge(IEdgeType myEdgeType, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Checks if the execution of <see cref="RemoveEdge(System.Collections.Generic.IEnumerable{sones.GraphDB.TypeSystem.IEdgeType},sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)"/> will succeed, if no unexpected error occurs.
        /// </summary>
        /// <param name="myEdgeTypes">The e types to be removed.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        /// <returns>
        /// True, if the call of <see cref="RemoveEdge(System.Collections.Generic.IEnumerable{sones.GraphDB.TypeSystem.IEdgeType},sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)"/> with the given 
        /// <paramref name="myEdgeTypes"/>, <paramref name="myTransaction"/> and <paramref name="mySecurity"/> 
        /// will succeed bar the occurrence of unexpected errors, otherwise false.
        /// </returns>
        bool CanRemoveEdge(IEnumerable<IEdgeType> myEdgeTypes, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Removes a bunch of e types from the type manager.
        /// </summary>
        /// <param name="myEdgeTypes">The e types that will be removed.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        /// All types will be removed unless there are no edges that point to at least one of the given types.
        /// If there is such an edge, remove the edge by altering the type that holds it or remove this type too.
        /// All types are removed simultaneously. This means that edges between the types are not need to be removed before.
        void RemoveEdge(IEnumerable<IEdgeType> myEdgeTypes, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Checks if the execution of <see cref="UpdateEdge(sones.GraphDB.Request.EdgeTypeDefinition,sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)"/> will succeed, if no unexpected error occurs.
        /// </summary>
        /// <param name="myEdgeTypeDefinition">TODO: for update use EdgeTypeUpdateDefinition</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        /// <returns>
        /// True, if the call of <see cref="UpdateEdge(sones.GraphDB.Request.EdgeTypeDefinition,sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)"/> with the given 
        /// <paramref name="myEdgeTypeDefinition"/>, <paramref name="myTransaction"/> and <paramref name="mySecurity"/> 
        /// will succeed bar the occurrence of unexpected errors, otherwise false.
        /// </returns>
        bool CanUpdateEdge(EdgeTypeDefinition myEdgeTypeDefinition, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Updates an existing e type.
        /// </summary>
        /// <param name="myEdgeTypeDefinition">TODO: for update use EdgeTypeUpdateDefinition</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        void UpdateEdge(EdgeTypeDefinition myEdgeTypeDefinition, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Checks if the execution of <see cref="UpdateEdge(System.Collections.Generic.IEnumerable{sones.GraphDB.Request.EdgeTypeDefinition},sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)"/> will succeed, if no unexpected error occurs.
        /// </summary>
        /// <param name="myEdgeTypeDefinitions">TODO: for update use EdgeTypeUpdateDefinition</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        /// <returns>
        /// True, if the call of <see cref="UpdateEdge(System.Collections.Generic.IEnumerable{sones.GraphDB.Request.EdgeTypeDefinition},sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)"/> with the given 
        /// <paramref name="myEdgeTypeDefinitions"/>, <paramref name="myTransaction"/> and <paramref name="mySecurity"/> 
        /// will succeed bar the occurrence of unexpected errors, otherwise false.
        /// </returns>
        bool CanUpdateEdge(IEnumerable<EdgeTypeDefinition> myEdgeTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);

        /// <summary>
        /// Updates existing e types.
        /// </summary>
        /// <param name="myEdgeTypeDefinitions">TODO: for update use EdgeTypeUpdateDefinition</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        void UpdateEdge(IEnumerable<EdgeTypeDefinition> myEdgeTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity, MetaManager myMetaManager);
    }
}