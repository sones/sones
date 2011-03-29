using System;
using sones.Library.ErrorHandling;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// A Type does not match the expected Type
    /// </summary>
    public sealed class TypeDoesNotMatchException : AGraphDBTypeException
    {
        public String ExpectedType { get; private set; }
        public String CurrentType { get; private set; }

        /// <summary>
        /// Creates a new TypeDoesNotMatchException exception
        /// </summary>
        /// <param name="myExpectedType">The expected type</param>
        /// <param name="myCurrentType">The current type</param>
        public TypeDoesNotMatchException(String myExpectedType, String myCurrentType)
        {
            ExpectedType = myExpectedType;
            CurrentType = myCurrentType;
        }

        public override string ToString()
        {
            return String.Format("The Type {0} does not match the expected Type {1}.", CurrentType, ExpectedType);
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.TypeDoesNotMatch; }
        }
    }
}
