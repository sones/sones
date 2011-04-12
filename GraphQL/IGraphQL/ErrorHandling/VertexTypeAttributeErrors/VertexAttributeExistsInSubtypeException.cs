using System;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// The vertex attribute already exists in subtype
    /// </summary>
    public sealed class VertexAttributeExistsInSubtypeException : AGraphQLVertexAttributeException
    {
        public String AttributeName { get; private set; }
        public String SubtypeName { get; private set; }

        /// <summary>
        /// Creates a new VertexAttributeExistsInSubtypeException exception
        /// </summary>
        /// <param name="myAttributeName">The name of the attribute</param>
        public VertexAttributeExistsInSubtypeException(String myAttributeName)
        {
            AttributeName = myAttributeName;
        }

        /// <summary>
        /// Creates a new VertexAttributeExistsInSubtypeException exception
        /// </summary>
        /// <param name="myAttributeName">The name of the attribute</param>
        /// <param name="mySubtypeName">The name of the subtype </param>
        public VertexAttributeExistsInSubtypeException(String myAttributeName, String mySubtypeName)
        {
            AttributeName = myAttributeName;
            SubtypeName = mySubtypeName;
        }

        public override string ToString()
        {
            return String.Format("The attribute \"{0}\" already exists in subtype \"{1}\"!", AttributeName, SubtypeName);
        }

    }
}
