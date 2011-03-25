using System;
using System.Collections.Generic;

namespace sones.GraphDB.TypeSystem
{
    /// <summary>
    /// An interface that represents an edge type.
    /// </summary>
    public interface IEdgeType
    {
        /// <summary>
        /// The name of the EdgeType.
        /// </summary>
        /// <remarks>
        /// The name must be unique for alle edge types in one database
        /// </remarks>
        String Name { get; }

        /// <summary>
        /// The behaviour for this edge type.
        /// </summary>
        /// <remarks>
        /// If no behaviour is defined, this property is <c>NULL</c>.
        /// </remarks>
        IBehaviour Behaviour { get; }

        /// <summary>
        /// The comment for this edge type.
        /// </summary>
        /// <value>A user defined string, never <c>NULL</c>.</value>
        String Comment { get; }

        /// <summary>
        /// Defines whether this edge type is abstract. 
        /// </summary>
        /// <value>
        /// If true, this edge type can not be used directly on vertex definitions.
        /// </value>
        Boolean IsAbstract { get; }

        #region Inheritance

        /// <summary>
        /// Has this edge type a parent edge type?
        /// </summary>
        /// <returns>True, if this edge type has a parent edge type, otherwise false.</returns>
        bool HasParentEdgeType();

        /// <summary>
        /// Gets the parent vertex type
        /// </summary>
        /// <returns>The parent vertex type</returns>
        IEdgeType GetParentEdgeType();

        /// <summary>
        /// Has this edge type child edge types?
        /// </summary>
        /// <returns>False, if this edge type has no child edge type, otherwise true.</returns>
        bool HasChildEdgeTypes();

        /// <summary>
        /// Get all child edge types
        /// </summary>
        /// <returns>An enumerable of child edge types, never <c>NULL</c>.</returns>
        IEnumerable<IEdgeType> GetChildEdgeTypes();

        #endregion

        #region Properties

        /// <summary>
        /// Get all visible incoming edges
        /// </summary>
        /// <returns>An enumerable of incoming edge attributes</returns>
        IEnumerable<IPropertyDefinition> GetAllProperties();

        #endregion
    }
}
