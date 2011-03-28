using System;
using sones.Library.ErrorHandling;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// The desire dump type is not supported
    /// </summary>
    public sealed class InvalidDumpTypeException : AGraphQLDumpException
    {
        public String DumpType { get; private set; }

        /// <summary>
        /// Creates a new InvalidDumpTypeException exception
        /// </summary>
        /// <param name="dumpType"></param>
        public InvalidDumpTypeException(String dumpType)
        {
            DumpType = dumpType;
        }

        public override string ToString()
        {
            return DumpType;
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.InvalidDumpType; }
        }
    }
}
