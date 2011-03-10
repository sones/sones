using System;

namespace sones.Library.ErrorHandling
{
    /// <summary>
    /// An abstrac class for all sones exceptions
    /// </summary>
    public abstract class ASonesException : Exception
    {
        /// <summary>
        /// The corresponding error code
        /// </summary>
        public abstract UInt16 ErrorCode { get; }
    }
}