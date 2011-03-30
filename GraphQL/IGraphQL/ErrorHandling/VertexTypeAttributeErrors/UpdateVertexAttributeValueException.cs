using System;
using sones.Library.ErrorHandling;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// Could not update a value for a vertex attribute
    /// </summary>
    public sealed class UpdateVertexAttributeValueException : AGraphQLVertexAttributeException
    {
        public String VertexAttributeName { get; private set; }

        /// <summary>
        /// Creates a new UpdateVertexAttributeValueException exception
        /// </summary>
        /// <param name="myVertexAttributeName">The name of the vertex attribute</param>
        public UpdateVertexAttributeValueException(String myVertexAttributeName)
        {
            VertexAttributeName = myVertexAttributeName;
        }

        public override string ToString()
        {
            return String.Format("Could not update value for vertex attribute \"{0}\".", VertexAttributeName);
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.UpdateVertexAttributeValue; }
        }
    }
}
