using System;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// Could not find any objects while updating elements to the list attribute
    /// </summary>
    public sealed class UpdateListVertexAttributeNoElementsException : AGraphQLVertexAttributeException
    {
        public String AttributeName { get; private set; }

        /// <summary>
        /// Creates a new UpdateListVertexAttributeNoElementsException exception
        /// </summary>
        /// <param name="myAttributeName">The name of the attribute</param>
        public UpdateListVertexAttributeNoElementsException(String myAttributeName)
        {
            AttributeName = myAttributeName;
        }

        public override string ToString()
        {
            return String.Format("Could not find any objects while adding or removing elements to the list attribute {0}.", AttributeName);
        }

    }
}
