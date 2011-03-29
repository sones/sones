using System;
using sones.Library.ErrorHandling;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// A base type is not a user defined type 
    /// </summary>
    public sealed class InvalidBaseTypeException : AGraphDBTypeException
    {
        public String BaseTypeName { get; private set; }

        /// <summary>
        /// Creates a new InvalidBaseTypeException exception
        /// </summary>
        /// <param name="myBaseTypeName">The name of the base type</param>
        public InvalidBaseTypeException(String myBaseTypeName)
        {
            BaseTypeName = myBaseTypeName;
        }

        public override string ToString()
        {
            return String.Format("The base type [{0}] must be a user defined type.", BaseTypeName);
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.InvalidBaseType; }
        }
    }
}
