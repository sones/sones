using System;
using System.Collections.Generic;

namespace sones.GraphDB.TypeSystem
{
    /// <summary>
    /// An interface that represents an IncomingEdge type.
    /// </summary>
    public interface IEdgeType: IBaseType
    {

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
