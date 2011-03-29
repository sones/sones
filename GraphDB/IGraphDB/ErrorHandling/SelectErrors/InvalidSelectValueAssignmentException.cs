using System;
using sones.Library.ErrorHandling;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The assignment of the select value is invalid
    /// </summary>
    public sealed class InvalidSelectValueAssignmentException : AGraphDBSelectException
    {
        public String Info { get; private set; }

        /// <summary>
        /// Creates a new InvalidSelectValueAssignmentException exception
        /// </summary>
        /// <param name="myInfo"></param>
        public InvalidSelectValueAssignmentException(String myInfo)
        {
            Info = myInfo;
        }

        public override string ToString()
        {
            return "You can not assign a value to [" + Info + "]";
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.InvalidSelectValueAssignment; }
        }
    }
}
