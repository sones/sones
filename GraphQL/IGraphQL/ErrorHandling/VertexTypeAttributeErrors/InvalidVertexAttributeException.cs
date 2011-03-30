using System;
using sones.Library.ErrorHandling;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// The vertex attribute is not valid 
    /// </summary>
    public sealed class InvalidVertexAttributeException : AGraphQLVertexAttributeException
    {
        public String Info { get; private set; }

        /// <summary>
        /// Creates a new InvalidVertexAttributeException exception
        /// </summary>
        /// <param name="myInfo"></param>
        public InvalidVertexAttributeException(String myInfo)
        {
            Info = myInfo;
        }

        public override string ToString()
        {
            return String.Format("The vertex attribute is not valid: {0}", Info);
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.InvalidVertexAttribute; }
        }
    }
}
