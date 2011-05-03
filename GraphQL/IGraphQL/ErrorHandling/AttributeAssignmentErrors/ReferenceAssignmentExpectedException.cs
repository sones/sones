using System;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// A attribute expects a Reference assignment
    /// </summary>
    public sealed class ReferenceAssignmentExpectedException : AGraphQLAttributeAssignmentException
    {
        /// <summary>
        /// Creates a new ReferenceAssignmentExpectedException exception
        /// </summary>
        public ReferenceAssignmentExpectedException(String myInfo)
        {
            _msg = myInfo;
        }
          
    }
}
