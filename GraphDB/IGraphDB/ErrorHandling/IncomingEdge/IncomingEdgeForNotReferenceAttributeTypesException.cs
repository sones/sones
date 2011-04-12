using System;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// Defining of incoming edge for not referenced attribute is not allowed
    /// </summary>
    public sealed class IncomingEdgeForNotReferenceAttributeTypesException : AGraphDBIncomingEdgeException
    {
        public String VertexAttributeName { get; private set; }

        /// <summary>
        /// Creates a new IncomingEdgeForNotReferenceAttributeTypesException exception
        /// </summary>
        /// <param name="myVertexAttributeName">The name of the vertex attribute</param>
        public IncomingEdgeForNotReferenceAttributeTypesException(String myVertexAttributeName)
        {
            VertexAttributeName = myVertexAttributeName;
        }

        public override string ToString()
        {
            return String.Format("You can not define incoming edges for non reference attribute \"{0}\"!", VertexAttributeName);
        }
        
    }
}
