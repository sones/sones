using System;
using System.Collections.Generic;

namespace sones.GraphDB.TypeSystem
{

    /// <summary>
    /// The interface for all vertex types
    /// </summary>
    public interface IVertexType
    {
        /// <summary>
        /// The name of the vertex type.
        /// </summary>
        /// <remarks>
        /// The name must be unique for alle vertex types in one database
        /// </remarks>
        String Name { get; }

        /// <summary>
        /// The behaviour for this vertex type.
        /// </summary>
        /// <remarks>
        /// If no behaviour is defined, this property is <c>NULL</c>.
        /// </remarks>
        IBehaviour Behaviour { get; }

        /// <summary>
        /// The comment for this vertex type.
        /// </summary>
        /// <value>A user defined string, never <c>NULL</c>.</value>
        String Comment { get; }

        /// <summary>
        /// Defines whether this vertex type is abstract. 
        /// </summary>
        /// <value>
        /// If true, this vertex type can not have vertices.
        /// </value>
        Boolean IsAbstract { get; }

        #region Inheritance

        /// <summary>
        /// Has this vertex type a parent vertex type?
        /// </summary>
        /// <returns>True, if this vertex type has a parent vertex type, otherwise false.</returns>
        bool HasParentVertexType();

        /// <summary>
        /// Gets the parent vertex type
        /// </summary>
        /// <returns>The parent vertex type</returns>
        IVertexType GetParentVertexType();

        /// <summary>
        /// Has this vertex type child vertex types?
        /// </summary>
        /// <returns>False, if this vertex type has no child vertex type, otherwise true.</returns>
        bool HasChildVertexTypes();

        /// <summary>
        /// Get all child vertex types
        /// </summary>
        /// <returns>An enumerable of child vertex types, never <c>NULL</c>.</returns>
        IEnumerable<IVertexType> GetChildVertexTypes();

        #endregion

        #region Attributes

        /// <summary>
        /// Gets all attributes defined on this vertex type.
        /// </summary>
        /// <returns>An enumerable of attribute definitions</returns>
        IEnumerable<IAttributeDefinition> GetAllAttributeDefinitions();

        #region Properties

        /// <summary>
        /// Gets all properties defined on this vertex type.
        /// </summary>
        /// <returns>An enumerable of property definitions</returns>
        IEnumerable<IPropertyDefinition> GetAllProperties();
        
        #endregion

        #region Edges

        #region Incoming

        IIncomingEdgeDefinition GetIncomingEdgeDefinition(String myEdgeName);

        /// <summary>
        /// Has this vertex type any visible incoming edges?
        /// </summary>
        /// <returns>True or false</returns>
        bool HasVisibleIncomingEdges();

        /// <summary>
        /// Get all incoming edges
        /// </summary>
        /// <returns>An enumerable of incoming edge attributes</returns>
        IEnumerable<IIncomingEdgeDefinition> GetAllIncomingEdges();

        #endregion

        #region Outgoing

        IIncomingEdgeDefinition GetOutgoingEdgeDefinition(String myEdgeName);

        /// <summary>
        /// Has this vertex type any outgoing edges?
        /// </summary>
        /// <returns>True or false</returns>
        bool HasOutgoingEdges();

        /// <summary>
        /// Get all outgoing edges
        /// </summary>
        /// <returns>An enumerable of outgoing edge attributes</returns>
        IEnumerable<IOutgoingEdgeDefinition> GetOutgoingEdges();

        #endregion

        #endregion

        #endregion

        #region Uniques

        /// <summary>
        /// A set of uniqueness definitions.
        /// </summary>
        /// <returns>An enumerable of uniqueness definitions. Never <c>NULL</c>.</returns>
        IEnumerable<IUniqueDefinition> GetAllUniqueDefinitions();

        #endregion

        #region Indices

        /// <summary>
        /// A set of index definitions.
        /// </summary>
        /// <returns>An enumerable of index definitions. Never <c>NULL</c>.</returns>
        IEnumerable<IIndexDefinition> GetAllIndexDefinitions();

        #endregion
    }
}