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
            _msg = "An undefined attribute with an \".\" is not allowed.";
        }
 
    }
}
