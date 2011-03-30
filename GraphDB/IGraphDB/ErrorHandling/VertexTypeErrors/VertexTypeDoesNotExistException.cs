using System;
using sones.Library.ErrorHandling;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The vertex type does not exists
    /// </summary>
    public sealed class VertexTypeDoesNotExistException : AGraphDBVertexTypeException
    {
        public String VertexTypeName { get; private set; }

        /// <summary>
        /// Creates a new VertexTypeDoesNotExistException exception
        /// </summary>
        /// <param name="myVertexTypeName">The name of the vertex type</param>
        public VertexTypeDoesNotExistException(String myVertexTypeName)
        {
            VertexTypeName = myVertexTypeName;
        }

        public override string ToString()
        {
            return String.Format("The vertex type {0} does not exists.", VertexTypeName);
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.VertexTypeDoesNotExist; }
        }
    }
}
