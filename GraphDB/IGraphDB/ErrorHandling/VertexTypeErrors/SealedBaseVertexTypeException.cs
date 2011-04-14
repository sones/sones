using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The exception that is thrown if a vertex type should derive from a sealed vertex type.
    /// </summary>
    public sealed class SealedBaseVertexTypeException: AGraphDBVertexTypeException
    {
        /// <summary>
        /// The vertex type that causes the error.
        /// </summary>
        public string VertexTypeName { get; private set; }

        /// <summary>
        /// The sealed parent vertex type.
        /// </summary>
        public string ParentVertexTypeName { get; private set; }

        /// <summary>
        /// Creates an instance of SealedBaseVertexTypeException.
        /// </summary>
        /// <param name="myVertexTypeName">
        /// The vertex type that causes the error.
        /// </param>
        /// <param name="myParentVertexTypeName">
        /// The sealed parent vertex type.
        /// </param>
        public SealedBaseVertexTypeException(string myVertexTypeName, string myParentVertexTypeName)
        {
            this.VertexTypeName = myVertexTypeName;
            this.ParentVertexTypeName = myParentVertexTypeName;
            _msg = string.Format("Vertex type {0} can not derive from sealed vertex type {1}.", myVertexTypeName, myParentVertexTypeName);
        }
    }
}
