using System;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// An assignment of a certain reference type with a list is not allowed
    /// </summary>
    public sealed class InvalidAssignOfSetException : AGraphQLAttributeAssignmentException
    {
        public String AttributeName { get; private set; }

        /// <summary>
        /// Creates a new InvalidAssignOfSetException exception
        /// </summary>
        /// <param name="myAttributeName"></param>
        public InvalidAssignOfSetException(String myAttributeName)
        {
            AttributeName = myAttributeName;
            _msg = String.Format("Assignment of the reference type {0} with a list is not allowed. Use SETOF or REF (REFERENCE) instead.", AttributeName);
        }
         
    }
}
