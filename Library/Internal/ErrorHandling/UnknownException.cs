using System;

namespace sones.ErrorHandling
{
    /// <summary>
    /// This class represents an unknown sones exception
    /// </summary>
    public sealed class UnknownException : ASonesException
    {
        /// <summary>
        /// The exception that has been thrown
        /// </summary>
        public Exception ThrownException { get; private set; }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.UnknownError; }
        }

        #region constructor

        /// <summary>
        /// Creates a new unknown exception
        /// </summary>
        /// <param name="e">The thrown exception</param>
        public UnknownException(Exception e)
        {
            ThrownException = e;
        }

        #endregion
    }
}