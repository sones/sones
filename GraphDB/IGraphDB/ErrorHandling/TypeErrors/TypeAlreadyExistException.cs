using System;
using sones.Library.ErrorHandling;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The type already exists
    /// </summary>
    public sealed class TypeAlreadyExistException : AGraphDBTypeException
    {
        public String TypeName { get; private set; }
        
        /// <summary>
        /// Creates a new TypeAlreadyExistException exception
        /// </summary>
        /// <param name="myTypeName"></param>
        public TypeAlreadyExistException(String myTypeName)
        {
            TypeName = myTypeName;
        }

        public override string ToString()
        {
            return String.Format("The type {0} already exists", TypeName);
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.TypeAlreadyExist; }
        }
    }
}
