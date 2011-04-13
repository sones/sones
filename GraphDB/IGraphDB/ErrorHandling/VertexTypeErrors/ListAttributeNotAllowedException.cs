using System;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The user defined vertex type should not be used with LIST attributes
    /// </summary>
    public sealed class ListAttributeNotAllowedException : AGraphDBVertexTypeException
    {
        public String VertexTypeName { get; private set; }
        
        /// <summary>
        /// Creates a new ListAttributeNotAllowedException exception
        /// </summary>
        /// <param name="myVertexTypeName">The name of the vertex type</param>
        public ListAttributeNotAllowedException(String myVertexTypeName)
        {
            VertexTypeName = myVertexTypeName;
            _msg = String.Format("The user defined vertex type \\{0}\\ should not be used with LIST<> attributes, please use SET<> instead.", VertexTypeName);
        }

    }
}
