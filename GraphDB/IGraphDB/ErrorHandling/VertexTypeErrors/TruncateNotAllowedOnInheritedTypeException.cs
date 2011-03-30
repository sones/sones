using System;
using sones.Library.ErrorHandling;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// Truncate on an inherited vertex type is not allowed
    /// </summary>
    public sealed class TruncateNotAllowedOnInheritedTypeException : AGraphDBVertexTypeException
    {
        public String VertexTypeName { get; private set; }

        /// <summary>
        /// Creates a new TruncateNotAllowedOnInheritedTypeException exception
        /// </summary>
        /// <param name="myVertexTypeName"></param>
        public TruncateNotAllowedOnInheritedTypeException(String myVertexTypeName)
        {
            VertexTypeName = myVertexTypeName;
        }

        public override string ToString()
        {
            return String.Format("Truncate on the inherited vertex type '{0}' is not allowed!", VertexTypeName);
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.TruncateNotAllowedOnInheritedType; }
        }
    }
}
