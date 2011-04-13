using System;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// The IDNode is not valid
    /// </summary>
    public sealed class InvalidIDNodeException : AGraphQLException
    {
        public String Info { get; private set; }

        /// <summary>
        /// Creates a new InvalidIDNodeException exception
        /// </summary>
        /// <param name="myInfo"></param>
        public InvalidIDNodeException(String myInfo)
        {
            Info = myInfo;
            _msg = String.Format("The IDNode is not valid: \"{0}\"", Info);
        }
        
    }
}
