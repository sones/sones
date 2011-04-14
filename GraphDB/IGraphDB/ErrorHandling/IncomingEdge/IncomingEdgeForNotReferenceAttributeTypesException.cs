using System;
using sones.Library.ErrorHandling;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// Defining of incoming IncomingEdge for not referenced attribute is not allowed
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
            _msg = String.Format("You can not define incoming edges for non reference attribute \"{0}\"!", VertexAttributeName);
        } 
    }
}
