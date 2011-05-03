using System;

namespace sones.Library.ErrorHandling
{
    /// <summary>
    /// This class represents an unknown sones exception
    /// </summary>
    public sealed class UnknownException : ASonesException
    {
        #region constructor

        /// <summary>
        /// Creates a new unknown exception
        /// </summary>
        /// <param name="e">The thrown exception</param>
        public UnknownException(Exception e):base(e)
        {
            _msg = "An unknown exception was thrown. See InnerException for further information.";
        }

        #endregion
    }
}