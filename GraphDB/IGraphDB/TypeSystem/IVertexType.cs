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
        /// The name of the VertexType
        /// </summary>
        String Name { get; }

        IBehaviour Behaviour { get; }

        String Comment { get; }

        #region inheritance

        /// <summary>
        /// Has this vertex type a parent vertex type?
        /// </summary>
        /// <returns>True or false</returns>
        bool HasParentVertexType();

        /// <summary>
        /// Gets the parent vertex type
        /// </summary>
        /// <returns>The parent vertex type</returns>
        IVertexType GetParentVertexType();

        /// <summary>
        /// Has this vertex type child vertex types?
        /// </summary>
        /// <returns>True or false</returns>
        bool HasChildVertexTypes();

        /// <summary>
        /// Get all child vertex types
        /// </summary>
        /// <returns>An enumerable of child vertex types</returns>
        IEnumerable<IVertexType> GetChildVertexTypes();

        #endregion

        #region Attributes

        IEnumerable<IAttributeDefinition> GetAllAttributeDefinitions();

        #region Properties
        
        /// <summary>
        /// Get all visible incoming edges
        /// </summary>
        /// <returns>An enumerable of incoming edge attributes</returns>
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
        /// Get all visible incoming edges
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

        IEnumerable<IUniqueDefinition> GetAllUniqueDefinitions();

        #endregion

        #region Indices

        IEnumerable<IIndexDefinition> GetAllIndexDefinitions();

        #endregion
    }
}