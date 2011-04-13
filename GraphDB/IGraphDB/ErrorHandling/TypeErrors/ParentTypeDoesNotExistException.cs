using System;

namespace sones.GraphDB.ErrorHandling
{
    public sealed class ParentTypeDoesNotExistException : AGraphDBTypeException
    {
        public String ParentType { get; private set; }
        public String Type { get; private set; }

        public ParentTypeDoesNotExistException(String myParentType, String myType)
        {
            ParentType = myParentType;
            Type = myType;
            _msg = String.Format("The parent type {0} of the type {1} does not exist.", ParentType, Type);
        }

    }
}
