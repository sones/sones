using System;
using sones.Library.ErrorHandling;

namespace sones.GraphFS.ErrorHandling
{
    /// <summary>
    /// This class represents an unknown graphfs exception
    /// </summary>
    public sealed class UnknownFSException : AGraphFSException
    {
        /// <summary>
        /// The exception that has been thrown
        /// </summary>
        public Exception ThrownException { get; private set; }
        
        #region constructor

        /// <summary>
        /// Creates a new UnknownFS exception
        /// </summary>
        /// <param name="e"></param>
        public UnknownFSException(Exception e)
        {
            ThrownException = e;
        }

        #endregion
    }
}