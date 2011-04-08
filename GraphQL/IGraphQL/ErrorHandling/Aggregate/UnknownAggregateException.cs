using System;
using sones.Library.ErrorHandling;

namespace sones.GraphQL.ErrorHandling
{    
    /// <summary>
    /// This class represents an unknown aggregate exception
    /// </summary>
    public sealed class UnknownAggregateException : ASonesQLAggregateException
    {
        /// <summary>
        /// The exception that has been thrown
        /// </summary>
        public Exception ThrownException { get; private set; }
       
        #region constructor

        /// <summary>
        /// Creates a new UnknownAggregateException exception
        /// </summary>
        /// <param name="e"></param>
        public UnknownAggregateException(Exception e)
        {
            ThrownException = e;
        }

        #endregion
    }
}