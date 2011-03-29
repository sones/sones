using System;
using sones.Library.ErrorHandling;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The parent type of a type does not exist
    /// </summary>
    public sealed class ParentTypeDoesNotExistException : AGraphDBTypeException
    {
        public String ParentType { get; private set; }
        public String Type { get; private set; }

        /// <summary>
        /// Creates a new ParentTypeDoesNotExistException exception
        /// </summary>
        /// <param name="myParentType">The name of the parent type</param>
        /// <param name="myType">The current type</param>
        public ParentTypeDoesNotExistException(String myParentType, String myType)
        {
            ParentType = myParentType;
            Type = myType;
        }

        public override string ToString()
        {
            return String.Format("The parent type {0} of the type {1} does not exist.", ParentType, Type);
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.ParentTypeDoesNotExist; }
        }
    }
}
