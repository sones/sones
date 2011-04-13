using System;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// The assignment of the select value is invalid
    /// </summary>
    public sealed class InvalidSelectValueAssignmentException : AGraphQLSelectException
    {
        public String Info { get; private set; }

        /// <summary>
        /// Creates a new InvalidSelectValueAssignmentException exception
        /// </summary>
        /// <param name="myInfo"></param>
        public InvalidSelectValueAssignmentException(String myInfo)
        {
            Info = myInfo;
            _msg = "You can not assign a value to [" + Info + "]";
        }
 
    }
}
