using System;
using sones.Library.ErrorHandling;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// The undefined vertex attribute has a invalid name
    /// </summary>
    public sealed class InvalidUndefinedVertexAttributeNameException : AGraphQLVertexAttributeException
    {
        /// <summary>
        /// Creates a new InvalidUndefinedVertexAttributeNameException exception
        /// </summary>
        public InvalidUndefinedVertexAttributeNameException()
        {
        }

        public override string ToString()
        {
            return "An undefined attribute with an \".\" is not allowed.";
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.InvalidUndefinedVertexAttributeName; }
        }
    }
}
