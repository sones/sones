using System;
using sones.Library.ErrorHandling;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// Droping of derived vertex attribute on the child vertex type is not allowed 
    /// </summary>
    public sealed class DropOfDerivedVertexAttributeIsNotAllowedException : AGraphDBVertexAttributeException
    {
        public String VertexAttributeName { get; private set; }
        public String VertexTypeName { get; private set; }

        /// <summary>
        /// Creates a new DropOfDerivedVertexAttributeIsNotAllowedException exception
        /// </summary>
        /// <param name="VertexTypeName">The name of the current vertex type</param>
        /// <param name="myVertexAttributeName">The name of the current vertex attribute</param>
        public DropOfDerivedVertexAttributeIsNotAllowedException(String myVertexTypeName, String myVertexAttributeName)
        {
            VertexTypeName = myVertexTypeName;
            VertexAttributeName = myVertexAttributeName;
            _msg = String.Format("Due to the vertex attribute \"{0}\" is derived from vertex type \"{1}\" you can not drop it!", VertexAttributeName, VertexTypeName);
        }

    }
}
