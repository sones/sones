using System;
using sones.Library.ErrorHandling;

namespace sones.Plugins.Index.ErrorHandling
{
    /// <summary>
    /// This class represents an unknown index exception
    /// </summary>
    public sealed class UnknownIndexException : ASonesIndexException
    {
        /// <summary>
        /// The exception that has been thrown
        /// </summary>
        public Exception ThrownException { get; private set; }
        
        #region constructor

        /// <summary>
        /// Creates a new UnknownIndex exception
        /// </summary>
        /// <param name="e"></param>
        public UnknownIndexException(Exception e)
        {
            ThrownException = e;
        }

        #endregion
    }
}