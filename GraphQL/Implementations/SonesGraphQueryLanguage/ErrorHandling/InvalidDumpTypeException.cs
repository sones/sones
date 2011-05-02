using System;

namespace sones.GraphQL.ErrorHandling
{
    public sealed class InvalidDumpTypeException : AGraphQLException
    {
        #region data

        public String Info { get; private set; }
        public String DumpType { get; private set; }

        #endregion

        #region constructor
        
        /// <summary>
        /// Creates a new InvalidDumpTypeException exception
        /// </summary>
        public InvalidDumpTypeException(String myDumpType, String myInfo)
        {
            Info = myInfo;
            DumpType = myDumpType;
            
            _msg = String.Format("Error the given DumpType: [{0}] is not allowed.\n\n{1}", DumpType, myInfo);
        }

        #endregion
    }
}
