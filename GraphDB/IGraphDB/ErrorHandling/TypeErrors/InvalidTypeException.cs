using System;

namespace sones.GraphDB.ErrorHandling
{
    public sealed class InvalidTypeException : AGraphDBTypeException
    {
        public String InvalidType { get; private set; }
        public String Info { get; private set; }

        public InvalidTypeException(String myInvalidType, String myInfo)
        {
            Info = myInfo;
            InvalidType = myInvalidType;
            _msg = String.Format("The type {0} is not valid. {1}.", InvalidType, Info);
        }        
    }
}
