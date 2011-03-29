using System;
using sones.Library.ErrorHandling;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The type does not exists
    /// </summary>
    public sealed class TypeDoesNotExistException : AGraphDBTypeException
    {
        public String TypeName { get; private set; }

        /// <summary>
        /// Creates a new TypeDoesNotExistException exception
        /// </summary>
        /// <param name="myTypeName">The name of the type</param>
        public TypeDoesNotExistException(String myTypeName)
        {
            TypeName = myTypeName;
        }

        public override string ToString()
        {
            return String.Format("The type {0} does not exists.", TypeName);
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.TypeDoesNotExist; }
        }
    }
}
