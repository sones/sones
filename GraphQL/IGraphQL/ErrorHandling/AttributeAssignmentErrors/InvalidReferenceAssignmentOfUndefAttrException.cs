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
        {
            _msg = "An reference assignment for undefined attributes is not allowed.";
        }

    }
}
