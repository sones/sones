using System;
using sones.Library.ErrorHandling;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// The dump format is invalid
    /// </summary>
    public sealed class InvalidDumpFormatException : AGraphQLException
    {
        public String DumpFormat { get; private set; }

        /// <summary>
        /// Creates a new InvalidDumpFormatException exception
        /// </summary>
        /// <param name="dumpFormat"></param>
        public InvalidDumpFormatException(String dumpFormat)
        {
            DumpFormat = dumpFormat;
        }

        public override string ToString()
        {
            return DumpFormat;
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.InvalidDumpFormat; }
        } 
    }
}
