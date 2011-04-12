using System.Collections.Generic;
using System.Linq;
using sones.GraphDB.Request;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The exception that is thrown, if a bunch of vertex types are added, but they contains a circle in the derivation hierarchy.
    /// </summary>
    public sealed class CircularTypeHierarchyException: AGraphDBVertexTypeException
    {

        /// <summary>
        /// The list of vertex type names, that contains the circle in derivation list.
        /// </summary>
        public IEnumerable<VertexTypePredefinition> VertexTypeNames { get; private set; }

        /// <summary>
        /// Creates a new instance of CircularTypeHierarchyException.
        /// </summary>
        /// <param name="myVertexTypeNames">The list of vertex type names, that contains the circle in derivation hierarchy.</param>
        public CircularTypeHierarchyException(IEnumerable<VertexTypePredefinition> myVertexTypeNames)
        {
            VertexTypeNames = myVertexTypeNames;
            _msg = string.Format("The following types contains a circle in the derivation hierarchy ({0})", string.Join(",", myVertexTypeNames.Select(t => t.VertexTypeName)));
        }

    }
}
