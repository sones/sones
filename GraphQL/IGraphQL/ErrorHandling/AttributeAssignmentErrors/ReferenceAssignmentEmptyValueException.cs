using System;
using sones.Library.ErrorHandling;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// A single reference attribute does not contain any value
    /// </summary>
    public sealed class ReferenceAssignmentEmptyValueException : AGraphQLAttributeAssignmentException
    {
        public String AttributeName { get; private set; }

        /// <summary>
        /// Creates a new ReferenceAssignmentEmptyValueException exception
        /// </summary>
        /// <param name="myAttributeName">The name of the attribute</param>
        public ReferenceAssignmentEmptyValueException(String myAttributeName)
        {
            AttributeName = myAttributeName;
        }

        public override string ToString()
        {
            return String.Format("The single reference attribute {0} does not contain any value.", AttributeName);
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.ReferenceAssignmentEmptyValue; }
        }  
    }
}
