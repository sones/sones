using System;
using sones.Library.ErrorHandling;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// This class represents an unknown graphdb exception
    /// </summary>
    public sealed class UnknownDBException : AGraphDBException
    {
        /// <summary>
        /// The exception that has been thrown
        /// </summary>
        public Exception ThrownException { get; private set; }
                
        #region constructor

        public UnknownDBException(Exception e)
        {
            ThrownException = e;
        }

        #endregion
    }
}