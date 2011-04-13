using System;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// A base vertex type is not a user defined type 
    /// </summary>
    public sealed class InvalidBaseVertexTypeException : AGraphDBVertexTypeException
    {
        public String BaseVertexTypeName { get; private set; }

        /// <summary>
        /// Creates a new InvalidBaseVertexTypeException exception
        /// </summary>
        /// <param name="myBaseVertexTypeName">The name of the base vertex type</param>
        public InvalidBaseVertexTypeException(String myBaseVertexTypeName)
        {
            BaseVertexTypeName = myBaseVertexTypeName;
            _msg = String.Format("The base vertex type [{0}] must be a user defined type.", BaseVertexTypeName);
        }
          
    }
}
