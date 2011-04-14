using System;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class InvalidInRangeOperationException : AGraphQLOperatorException
    {
        public String Info { get; private set; }

        /// <summary>
        /// Creates a new InvalidInRangeIntervalException exception
        /// </summary>
        /// <param name="myInfo"></param>
        public InvalidInRangeOperationException(String myInfo)
        {
            Info = myInfo;
            _msg = Info;
        }
    }
}
