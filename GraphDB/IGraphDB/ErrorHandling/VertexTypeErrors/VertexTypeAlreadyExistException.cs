using System;
using sones.Library.ErrorHandling;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The vertex type already exists
    /// </summary>
    public sealed class VertexTypeAlreadyExistException : AGraphDBVertexTypeException
    {
        public String VertexTypeName { get; private set; }
        
        /// <summary>
        /// Creates a new VertexTypeAlreadyExistException exception
        /// </summary>
        /// <param name="myVertexTypeName">The name of the vertex type</param>
        public VertexTypeAlreadyExistException(String myVertexTypeName)
        {
            VertexTypeName = myVertexTypeName;
        }

        public override string ToString()
        {
            return String.Format("The vertex type {0} already exists", VertexTypeName);
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.VertexTypeAlreadyExist; }
        }
    }
}
