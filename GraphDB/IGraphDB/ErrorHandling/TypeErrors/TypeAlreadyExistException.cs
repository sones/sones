using System;

namespace sones.GraphDB.ErrorHandling
{
    public sealed class TypeAlreadyExistException : AGraphDBTypeException
    {
        public String TypeName { get; private set; }
        public TypeAlreadyExistException(String myTypeName)
        {
            TypeName = myTypeName;
            _msg = String.Format("The type {0} already exists", TypeName);
        }

    }
}
