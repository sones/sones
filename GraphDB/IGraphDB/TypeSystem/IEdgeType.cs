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
        /// Gets the parent vertex type
        /// </summary>
        /// <returns>The parent vertex type</returns>
        IEdgeType GetParentEdgeType { get; }

        /// <summary>
        /// Get all child edge types.
        /// </summary>
        /// <param name="myRecursive">Include all dexcendant.</param>
        /// <param name="myIncludeSelf">If true, this edge type will be included to the result list.</param>
        /// <returns>An enumerable of child vertex types, never <c>NULL</c>.</returns>
        IEnumerable<IEdgeType> GetChildEdgeTypes(bool myRecursive = true, bool myIncludeSelf = false);

        #endregion

    }
}
