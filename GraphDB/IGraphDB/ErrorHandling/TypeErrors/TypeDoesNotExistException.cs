using System;

namespace sones.GraphDB.ErrorHandling
{
    public sealed class TypeDoesNotExistException : AGraphDBTypeException
    {
        public String TypeName { get; private set; }
        public TypeDoesNotExistException(String myTypeName)
        {
            TypeName = myTypeName;
            _msg = String.Format("The type {0} does not exist.", TypeName);
        }        
    }
}
