using System;
using sones.Library.ErrorHandling;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The type is invalid
    /// </summary>
    public sealed class InvalidTypeException : AGraphDBTypeException
    {
        public String InvalidType { get; private set; }
        public String Info { get; private set; }

        /// <summary>
        /// Creates a new InvalidTypeException exception
        /// </summary>
        /// <param name="myInvalidType">The name of the invalid type</param>
        /// <param name="myInfo"></param>
        public InvalidTypeException(String myInvalidType, String myInfo)
        {
            Info = myInfo;
            InvalidType = myInvalidType;
        }

        public override string ToString()
        {
            return String.Format("The type {0} is not valid. {1}.", InvalidType, Info);
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.InvalidType; }
        }
    }
}
