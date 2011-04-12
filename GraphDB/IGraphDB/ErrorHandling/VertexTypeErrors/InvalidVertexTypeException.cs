using System;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The vertex type is invalid
    /// </summary>
    public sealed class InvalidVertexTypeException : AGraphDBVertexTypeException
    {
        public String InvalidVertexType { get; private set; }
        public String Info { get; private set; }

        /// <summary>
        /// Creates a new InvalidVertexTypeException exception
        /// </summary>
        /// <param name="myInvalidVertexType">The name of the invalid vertex type</param>
        /// <param name="myInfo"></param>
        public InvalidVertexTypeException(String myInvalidVertexType, String myInfo)
        {
            Info = myInfo;
            InvalidVertexType = myInvalidVertexType;
        }

        public override string ToString()
        {
            return String.Format("The type {0} is not valid. {1}.", InvalidVertexType, Info);
        }

    }
}
