using System;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The parent vertex type of a vertex type does not exist
    /// </summary>
    public sealed class ParentVertexTypeDoesNotExistException : AGraphDBVertexTypeException
    {
        public String ParentType { get; private set; }
        public String Type { get; private set; }

        /// <summary>
        /// Creates a new ParentVertexTypeDoesNotExistException exception
        /// </summary>
        /// <param name="myParentVertexType">The name of the parent type</param>
        /// <param name="myVertexType">The current type</param>
        public ParentVertexTypeDoesNotExistException(String myParentVertexType, String myVertexType)
        {
            ParentType = myParentVertexType;
            Type = myVertexType;
            _msg = String.Format("The parent vertex type {0} of the vertex type {1} does not exist.", ParentType, Type);
        }
 
    }
}
