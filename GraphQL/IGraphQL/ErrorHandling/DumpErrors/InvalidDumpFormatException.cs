using System;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// The dump format is invalid
    /// </summary>
    public sealed class InvalidDumpFormatException : AGraphQLDumpException
    {
        public String DumpFormat { get; private set; }

        /// <summary>
        /// Creates a new InvalidDumpFormatException exception
        /// </summary>
        /// <param name="dumpFormat"></param>
        public InvalidDumpFormatException(String dumpFormat)
        {
            DumpFormat = dumpFormat;
            _msg = DumpFormat;
        }      
    }
}
