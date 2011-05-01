using System.Collections.Generic;
using sones.GraphDB.Request;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;

namespace sones.GraphDB.Manager.TypeManagement
{
    public interface IEdgeTypeManager: IManager
    {
        /// <summary>
        /// Gets an edge type by type id.
        /// </summary>
        /// <param name="myTypeId">The id of the e type.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <returns>An instance of IEdgeType, that represents the e type.</returns>
        IEdgeType GetEdgeType(long myTypeId, TransactionToken myTransaction, SecurityToken mySecurity);

        /// <summary>
        /// Gets an edge type by name.
        /// </summary>
        /// <param name="myTypeName">
        /// The name of the e type.
        /// </param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        /// <returns>An instance of IEdgeType, that represents the e type.</returns>
        IEdgeType GetEdgeType(string myTypeName, TransactionToken myTransaction, SecurityToken mySecurity);


        /// <summary>
        /// Gets a e type by type id.
        /// </summary>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <returns>An instance of IEdgeType, that represents the e type.</returns>
        IEnumerable<IEdgeType> GetAllEdgeTypes(TransactionToken myTransaction, SecurityToken mySecurity);

        /// <summary>
        /// Adds a new e type to the type manager.
        /// </summary>
        /// <param name="myEdgeTypeDefinition">The definition of the new type.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        IEdgeType AddEdgeType(EdgeTypeDefinition myEdgeTypeDefinition, TransactionToken myTransaction, SecurityToken mySecurity);

        /// <summary>
        /// Adds a bunch of new e types to the type manager.
        /// </summary>
        /// <param name="myEdgeTypeDefinitions">The definition of the new e types.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        IEdgeType AddEdgeType(IEnumerable<EdgeTypeDefinition> myEdgeTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity);

        /// <summary>
        /// Removes a e type from the type manager.
        /// </summary>
        /// <param name="myEdgeType">The e type that will be removed.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        /// The e type will be removed unless there are no edges that point to this type.
        /// If there is such an IncomingEdge, remove the IncomingEdge by altering the type that holds it or remove boths type simultaneously using 
        /// <see cref="RemoveEdgeType(System.Collections.Generic.IEnumerable{sones.GraphDB.TypeSystem.IEdgeType},sones.Library.Transaction.TransactionToken,sones.Library.Security.SecurityToken,sones.GraphDB.Manager.MetaManager)" />.
        void RemoveEdgeType(IEdgeType myEdgeType, TransactionToken myTransaction, SecurityToken mySecurity);

        /// <summary>
        /// Removes a bunch of e types from the type manager.
        /// </summary>
        /// <param name="myEdgeTypes">The e types that will be removed.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        /// All types will be removed unless there are no edges that point to at least one of the given types.
        /// If there is such an IncomingEdge, remove the IncomingEdge by altering the type that holds it or remove this type too.
        /// All types are removed simultaneously. This means that edges between the types are not need to be removed before.
        void RemoveEdgeType(IEnumerable<IEdgeType> myEdgeTypes, TransactionToken myTransaction, SecurityToken mySecurity);

        /// <summary>
        /// Updates an existing e type.
        /// </summary>
        /// <param name="myEdgeTypeDefinition">TODO: for update use EdgeTypeUpdateDefinition</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        void UpdateEdgeType(EdgeTypeDefinition myEdgeTypeDefinition, TransactionToken myTransaction, SecurityToken mySecurity);

        /// <summary>
        /// Updates existing e types.
        /// </summary>
        /// <param name="myEdgeTypeDefinitions">TODO: for update use EdgeTypeUpdateDefinition</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <param name="myMetaManager">The meta manager.</param>
        void UpdateEdgeType(IEnumerable<EdgeTypeDefinition> myEdgeTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity);
    }
}