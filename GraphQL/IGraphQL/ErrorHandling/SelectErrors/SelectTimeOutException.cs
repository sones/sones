using System;
using sones.Library.ErrorHandling;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// The timeout of a query has been reached
    /// </summary>
    public sealed class SelectTimeOutException : AGraphQLSelectException
    {
        public Int64 TimeOut { get; private set; }

        /// <summary>
        /// Creates a new SelectTimeOutException exception
        /// </summary>
        /// <param name="myTimeout">The timeout</param>
        public SelectTimeOutException(Int64 myTimeout)
        {
            TimeOut = myTimeout;
        }

        public override string ToString()
        {
            return String.Format("Aborting query because the timeout of {0}ms has been reached.", TimeOut);
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.SelectTimeOut; }
        }
    }
}
