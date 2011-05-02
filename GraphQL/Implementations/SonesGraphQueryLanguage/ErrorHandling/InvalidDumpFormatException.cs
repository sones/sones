using System;

namespace sones.GraphQL.ErrorHandling
{
    class InvalidDumpFormatException : AGraphQLException
    {
        #region data

        public String Info { get; private set; }
        public String DumpFormat { get; private set; }

        #endregion

        #region constructor
        
        /// <summary>
        /// Creates a new InvalidDumpFormatException exception
        /// </summary>
        public InvalidDumpFormatException(String myDumpFormat, String myInfo)
        {
            Info = myInfo;
            DumpFormat = myDumpFormat;

            _msg = String.Format("Error the given DumpType: [{0}] is not allowed.\n\n{1}", DumpFormat, myInfo);
        }

        #endregion
    }
}
