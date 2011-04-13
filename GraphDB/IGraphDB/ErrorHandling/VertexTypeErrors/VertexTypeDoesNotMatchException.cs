using System;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// A vertex type does not match the expected type
    /// </summary>
    public sealed class VertexTypeDoesNotMatchException : AGraphDBVertexTypeException
    {
        public String ExpectedVertexType { get; private set; }
        public String CurrentVertexType { get; private set; }

        /// <summary>
        /// Creates a new TypeDoesNotMatchException exception
        /// </summary>
        /// <param name="myExpectedVertexType">The expected type</param>
        /// <param name="myCurrentVertexType">The current type</param>
        public VertexTypeDoesNotMatchException(String myExpectedVertexType, String myCurrentVertexType)
        {
            ExpectedVertexType = myExpectedVertexType;
            CurrentVertexType = myCurrentVertexType;
            _msg = String.Format("The Vertex Type {0} does not match the expected Vertex Type {1}.", CurrentVertexType, ExpectedVertexType);
        }
        
    }
}
