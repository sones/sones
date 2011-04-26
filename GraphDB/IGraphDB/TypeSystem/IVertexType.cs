using System;
using System.Collections.Generic;

namespace sones.GraphDB.TypeSystem
{

    /// <summary>
    /// The interface for all vertex types
    /// </summary>
    public interface IVertexType: IBaseType
    {

        #region Inheritance


        /// <summary>
        /// Gets the parent vertex type
        /// </summary>
        /// <returns>The parent vertex type</returns>
        IVertexType GetParentVertexType { get; }


        /// <summary>
        /// Get all child vertex types
        /// </summary>
        /// <param name="myRecursive">get child vertex types recursive?</param>
        /// <returns>An enumerable of child vertex types, never <c>NULL</c>.</returns>
        IEnumerable<IVertexType> GetChildVertexTypes(bool myRecursive = true);

        #endregion

        #region Incoming

        /// <summary>
        /// Has this vertex type a certain binary property?
        /// </summary>
        /// <param name="myEdgeName">The name of the binary property.</param>
        /// <returns>True, if a binary property with the given name exists, otherwise false.</returns>
        bool HasBinaryProperty(String myEdgeName);

        /// <summary>
        /// Gets a certain binary property definition.
        /// </summary>
        /// <param name="myAttributeName">The name of the interesting binary property.</param>
        /// <returns>A binary property definition, if existing otherwise <c>NULL</c>.</returns>
        IBinaryPropertyDefinition GetBinaryPropertyDefinition(String myEdgeName);

        /// <summary>
        /// Has this vertex type any binary property.
        /// </summary>
        /// <param name="myIncludeAncestorDefinitions">If true, the ancestor vertex types are included, otherwise false.</param>
        /// <returns>True if a binary property exists, otherwise false.</returns>
        bool HasBinaryProperties(bool myIncludeAncestorDefinitions);

        /// <summary>
        /// Get all binary properties.
        /// </summary>
        /// <param name="myIncludeParents">Include the properties of the parent vertex type(s).</param>
        /// <returns>An enumerable of binary property definitions.</returns>
        IEnumerable<IBinaryPropertyDefinition> GetBinaryProperties(bool myIncludeAncestorDefinitions);

        #endregion


        #region Edges

        #region Incoming

        /// <summary>
        /// Has this vertex type a certain incoming IncomingEdge?
        /// </summary>
        /// <returns>True or false</returns>
        bool HasIncomingEdge(String myEdgeName);

        /// <summary>
        /// Gets a certain incoming IncomingEdge definition
        /// </summary>
        /// <param name="myAttributeName">The name of the interesting incoming IncomingEdge</param>
        /// <returns>An incoming IncomingEdge definition</returns>
        IIncomingEdgeDefinition GetIncomingEdgeDefinition(String myEdgeName);

        /// <summary>
        /// Has this vertex type any visible incoming edges?
        /// </summary>
        /// <returns>True or false</returns>
        bool HasIncomingEdges(bool myIncludeAncestorDefinitions);

        /// <summary>
        /// Get all incoming edges
        /// </summary>
        /// <param name="myIncludeParents">Include the properties of the parent vertex type(s)</param>
        /// <returns>An enumerable of incoming IncomingEdge attributes</returns>
        IEnumerable<IIncomingEdgeDefinition> GetIncomingEdgeDefinitions(bool myIncludeAncestorDefinitions);


        #endregion

        #region Outgoing

        /// <summary>
        /// Has this vertex type a certain outgoing IncomingEdge?
        /// </summary>
        /// <returns>True or false</returns>
        bool HasOutgoingEdge(String myEdgeName);

        /// <summary>
        /// Gets a certain outgoing IncomingEdge definition
        /// </summary>
        /// <param name="myAttributeName">The name of the interesting outgoing IncomingEdge</param>
        /// <returns>An outgoing IncomingEdge definition</returns>
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
        /// <returns>An enumerable of outgoing IncomingEdge attributes</returns>
        IEnumerable<IOutgoingEdgeDefinition> GetOutgoingEdgeDefinitions(bool myIncludeAncestorDefinitions);

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