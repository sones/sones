using System;
using System.Collections.Generic;

namespace sones.GraphDB.TypeSystem
{
    /// <summary>
    /// An interface that represents an IncomingEdge type.
    /// </summary>
    public interface IEdgeType
    {
        /// <summary>
        /// The name of the EdgeType.
        /// </summary>
        /// <remarks>
        /// The name must be unique for alle IncomingEdge types in one database
        /// </remarks>
        String Name { get; }

        /// <summary>
        /// The behaviour for this IncomingEdge type.
        /// </summary>
        /// <remarks>
        /// If no behaviour is defined, this property is <c>NULL</c>.
        /// </remarks>
        IBehaviour Behaviour { get; }

        /// <summary>
        /// The comment for this IncomingEdge type.
        /// </summary>
        /// <value>A user defined string, never <c>NULL</c>.</value>
        String Comment { get; }

        /// <summary>
        /// Defines whether this IncomingEdge type is abstract. 
        /// </summary>
        /// <value>
        /// If true, this IncomingEdge type can not be used directly on vertex definitions.
        /// </value>
        Boolean IsAbstract { get; }

        /// <summary>
        /// Defines whether this type can be used as parent type.
        /// </summary>
        /// <value>
        /// If true, this IncomingEdge type must not be used as a parent IncomingEdge type.
        /// </value>
        Boolean IsSealed { get; }

        #region Inheritance

        /// <summary>
        /// Has this IncomingEdge type a parent IncomingEdge type?
        /// </summary>
        /// <returns>True, if this IncomingEdge type has a parent IncomingEdge type, otherwise false.</returns>
        bool HasParentEdgeType { get; }

        /// <summary>
        /// Gets the parent vertex type
        /// </summary>
        /// <returns>The parent vertex type</returns>
        IEdgeType GetParentEdgeType { get; }

        /// <summary>
        /// Has this IncomingEdge type child IncomingEdge types?
        /// </summary>
        /// <returns>False, if this IncomingEdge type has no child IncomingEdge type, otherwise true.</returns>
        bool HasChildEdgeTypes { get; }

        /// <summary>
        /// Get all child IncomingEdge types
        /// </summary>
        /// <returns>An enumerable of child IncomingEdge types, never <c>NULL</c>.</returns>
        IEnumerable<IEdgeType> GetChildEdgeTypes { get; }

        #endregion

        #region Properties

        /// <summary>
        /// Get all visible incoming edges
        /// </summary>
        /// <returns>An enumerable of incoming IncomingEdge attributes</returns>
        IEnumerable<IPropertyDefinition> GetProperties { get; }

        #endregion
    }
}
