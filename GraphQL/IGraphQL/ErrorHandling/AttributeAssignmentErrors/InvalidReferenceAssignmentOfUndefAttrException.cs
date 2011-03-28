using System;
using sones.Library.ErrorHandling;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// An reference assignment for undefined attributes is not allowed
    /// </summary>
    public sealed class InvalidReferenceAssignmentOfUndefAttrException : AGraphQLAttributeAssignmentException
    {
        /// <summary>
        /// Creates a new InvalidReferenceAssignmentOfUndefAttrException exception
        /// </summary>
        public InvalidReferenceAssignmentOfUndefAttrException()
        { }

        public override string ToString()
        {
            return "An reference assignment for undefined attributes is not allowed.";   
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.InvalidReferenceAssignmentOfUndefAttr; }
        } 
    }
}
