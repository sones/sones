using System;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// The attribute from a type could not be removed
    /// </summary>
    public sealed class RemoveVertexTypeAttributeException : AGraphQLVertexAttributeException
    {
        public String VertexTypeName { get; private set; }
        public String VertexAttributeName { get; private set; }

        /// <summary>
        /// Creates a new RemoveVertexTypeAttributeException exception
        /// </summary>
        /// <param name="myVertexTypeName">The name of the vertex type</param>
        /// <param name="myVertexAttributeName">The name of the vertex attribute</param>
        public RemoveVertexTypeAttributeException(String myVertexTypeName, String myVertexAttributeName)
        {
            VertexTypeName = myVertexTypeName;
            VertexAttributeName = myVertexAttributeName;
            _msg = String.Format("The attribute " + VertexAttributeName + " from vertex type " + VertexTypeName + " could not be removed.");
        }

    }
}
