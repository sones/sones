using System.Collections.Generic;
using sones.GraphDB.Request;
using sones.GraphDB.TypeSystem;
using sones.Library.Commons.Security;
using sones.Library.Commons.Transaction;

namespace sones.GraphDB.Manager.TypeManagement
{
    public interface IVertexTypeHandler
    {
        /// <summary>
        /// Returns an threadsafe auto-incremented ID.
        /// </summary>
        /// <param name="vertexType">The vertex type for that the ID is needed.</param>
        /// <returns>An ID that is unique for the given vertex type.</returns>
        UniqueID GetUniqueVertexID(IVertexType vertexType);

        /// <summary>
        /// Returns an threadsafe auto-incremented ID.
        /// </summary>
        /// <param name="myVertexTypeID">The ID of the vertex type for that the ID is needed.</param>
        /// <returns>An ID that is unique for the given vertex type.</returns>
        UniqueID GetUniqueVertexID(long myVertexTypeID);

        /// <summary>
        /// Gets a vertex type by id.
        /// </summary>
        /// <param name="myTypeId">The id of the vertex type.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <returns>An instance of IVertexType, that represents the vertex type.</returns>
        IVertexType GetVertexType(long myTypeId, TransactionToken myTransaction, SecurityToken mySecurity);

        /// <summary>
        /// Gets a vertex type by name.
        /// </summary>
        /// <param name="myTypeName">The name of the vertex type.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <returns>An instance of IVertexType, that represents the vertex type.</returns>
        IVertexType GetVertexType(string myTypeName, TransactionToken myTransaction, SecurityToken mySecurity);

        /// <summary>
        /// Gets all vertex types.
        /// </summary>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// <returns>An instance of IVertexType, that represents the vertex type.</returns>
        IEnumerable<IVertexType> GetAllVertexTypes(TransactionToken myTransaction, SecurityToken mySecurity);

        /// <summary>
        /// Adds a bunch of vertex types to the vertex type manager.
        /// </summary>
        /// <param name="myVertexTypeDefinitions">The definitions of the new vertex types.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        IEnumerable<IVertexType> AddVertexTypes(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity);

        /// <summary>
        /// Removes a bunch of vertex types from the vertex type manager.
        /// </summary>
        /// <param name="myVertexTypes">The vertex types that will be removed.</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        /// All types will be removed unless there are no edges that point to at least one of the given types.
        /// If there is such an IncomingEdge, remove the IncomingEdge by altering the type that holds it or remove this type too.
        /// All types are removed simultaneously. This means that edges between the types are not need to be removed before.
        void RemoveVertexTypes(IEnumerable<IVertexType> myVertexTypes, TransactionToken myTransaction, SecurityToken mySecurity);

        /// <summary>
        /// Updates existing vertex types.
        /// </summary>
        /// <param name="myVertexTypeDefinitions">TODO: for update use VertexTypeUpdateDefinition</param>
        /// <param name="myTransaction">A transaction token for this operation.</param>
        /// <param name="mySecurity">A security token for this operation.</param>
        void UpdateVertexType(IEnumerable<VertexTypePredefinition> myVertexTypeDefinitions, TransactionToken myTransaction, SecurityToken mySecurity);

    
    }
}