using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.TypeSystem
{
    public interface IEdgeType
    {
        /// <summary>
        /// The name of the VertexType
        /// </summary>
        String Name { get; }

        IBehaviour Behaviour { get; }

        String Comment { get; }

        Boolean IsAbstract { get; }

        #region inheritance

        /// <summary>
        /// Has this vertex type a parent vertex type?
        /// </summary>
        /// <returns>True or false</returns>
        bool HasParentEdgeType();

        /// <summary>
        /// Gets the parent vertex type
        /// </summary>
        /// <returns>The parent vertex type</returns>
        IEdgeType GetParentEdgeType();

        /// <summary>
        /// Has this vertex type child vertex types?
        /// </summary>
        /// <returns>True or false</returns>
        bool HasChildEdgeTypes();

        /// <summary>
        /// Get all child vertex types
        /// </summary>
        /// <returns>An enumerable of child vertex types</returns>
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
