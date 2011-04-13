using System;

namespace sones.GraphDB.ErrorHandling
{
    public sealed class TypeDoesNotMatchException : AGraphDBTypeException
    {
        public String ExpectedType { get; private set; }
        public String CurrentType { get; private set; }

        public TypeDoesNotMatchException(String myExpectedType, String myCurrentType)
        {
            ExpectedType = myExpectedType;
            CurrentType = myCurrentType;
            _msg = String.Format("The Type {0} does not match the expected Type {1}.", CurrentType, ExpectedType);
        }
    }
}
