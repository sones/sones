using System;
using sones.Library.ErrorHandling;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The incoming edge destination is invalid
    /// </summary>
    public sealed class IncomingEdgeDestinationIsInvalidException : AGraphDBIncomingEdgeException
    {
        public String VertexAttributeName { get; private set; }
        public String VertexTypeName { get; private set; }

        /// <summary>
        /// Creates a new IncomingEdgeDestinationIsInvalidException exception
        /// </summary>
        /// <param name="myVertexTypeName">The name of the vertex type</param>
        /// <param name="myVertexAttributeName">The name of the vertex attribute </param>
        public IncomingEdgeDestinationIsInvalidException(String myVertexTypeName, String myVertexAttributeName)
        {
            VertexAttributeName = myVertexAttributeName;
            VertexTypeName = myVertexTypeName;
            _msg = String.Format("The incoming edge destination \"{0}\".\"{1}\" is invalid!", VertexTypeName, VertexAttributeName);
        }

    }
}
