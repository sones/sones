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
        /// The ID of the vertex type.
        /// </summary>
        Int64 ID { get; }

        /// <summary>
        /// The name of the vertex type.
        /// </summary>
        /// <remarks>
        /// The name must be unique for all vertex types in one database.
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

        /// <summary>
        /// Defines whether this type can be used as parent type.
        /// </summary>
        /// <value>
        /// If true, this vertex type must not be used as a parent vertex type.
        /// </value>
        Boolean IsSealed { get; }

        #region Inheritance

        /// <summary>
        /// Has this vertex type a parent vertex type?
        /// </summary>
        /// <returns>True, if this vertex type has a parent vertex type, otherwise false.</returns>
        bool HasParentVertexType { get; }

        /// <summary>
        /// Gets the parent vertex type
        /// </summary>
        /// <returns>The parent vertex type</returns>
        IVertexType GetParentVertexType { get; }

        /// <summary>
        /// Has this vertex type child vertex types?
        /// </summary>
        /// <returns>False, if this vertex type has no child vertex type, otherwise true.</returns>
        bool HasChildVertexTypes { get; }

        /// <summary>
        /// Get all child vertex types
        /// </summary>
        /// <returns>An enumerable of child vertex types, never <c>NULL</c>.</returns>
        IEnumerable<IVertexType> GetChildVertexTypes { get; }

        #endregion

        #region Attributes

        /// <summary>
        /// Gets all attributes defined on this vertex type.
        /// </summary>
        /// <param name="myIncludeParents">Include the properties of the parent vertex type(s)</param>
        /// <returns>An enumerable of attribute definitions</returns>
        IEnumerable<IAttributeDefinition> GetAttributeDefinitions(bool myIncludeAncestorDefinitions);

        /// <summary>
        /// Gets a certain attribute definition
        /// </summary>
        /// <param name="myAttributeName">The name of the interesting attribute</param>
        /// <returns>A attribute definition</returns>
        IAttributeDefinition GetAttributeDefinition(String myAttributeName);

        #region Properties

        /// <summary>
        /// Gets all properties defined on this vertex type.
        /// </summary>
        /// <param name="myIncludeParents">Include the properties of the parent vertex type(s)</param>
        /// <returns>An enumerable of property definitions</returns>
        IEnumerable<IPropertyDefinition> GetPropertyDefinitions(bool myIncludeAncestorDefinitions);

        /// <summary>
        /// Gets a certain attribute definition
        /// </summary>
        /// <param name="myPropertyName">The name of the property</param>
        /// <returns>A property definition</returns>
        IPropertyDefinition GetPropertyDefinition(String myPropertyName);

        #endregion

        #region Edges

        #region Incoming

        IIncomingEdgeDefinition GetIncomingEdgeDefinition(String myEdgeName);

        /// <summary>
        /// Has this vertex type any visible incoming edges?
        /// </summary>
        /// <returns>True or false</returns>
        bool HasVisibleIncomingEdges(bool myIncludeAncestorDefinitions);

        /// <summary>
        /// Get all incoming edges
        /// </summary>
        /// <param name="myIncludeParents">Include the properties of the parent vertex type(s)</param>
        /// <returns>An enumerable of incoming edge attributes</returns>
        IEnumerable<IIncomingEdgeDefinition> GetIncomingEdgeDefinitions(bool myIncludeAncestorDefinitions);

        #endregion

        #region Outgoing

        IOutgoingEdgeDefinition GetOutgoingEdgeDefinition(String myEdgeName);

        /// <summary>
        /// Has this vertex type any outgoing edges?
        /// </summary>
        /// <returns>True or false</returns>
        bool HasOutgoingEdges(bool myIncludeAncestorDefinitions);

        /// <summary>
        /// Get all outgoing edges
        /// </summary>
        /// <param name="myIncludeParents">Include the properties of the parent vertex type(s)</param>
        /// <returns>An enumerable of outgoing edge attributes</returns>
        IEnumerable<IOutgoingEdgeDefinition> GetOutgoingEdgeDefinitions(bool myIncludeAncestorDefinitions);

        #endregion

        #endregion

        #endregion

        #region Uniques

        /// <summary>
        /// A set of uniqueness definitions.
        /// </summary>
        /// <returns>An enumerable of uniqueness definitions. Never <c>NULL</c>.</returns>
        IEnumerable<IUniqueDefinition> GetUniqueDefinitions(bool myIncludeAncestorDefinitions);

        #endregion

        #region Indices

        /// <summary>
        /// A set of index definitions.
        /// </summary>
        /// <returns>An enumerable of index definitions. Never <c>NULL</c>.</returns>
        IEnumerable<IIndexDefinition> GetIndexDefinitions(bool myIncludeAncestorDefinitions);

        #endregion

    }
}