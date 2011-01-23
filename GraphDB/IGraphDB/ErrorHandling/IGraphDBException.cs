using System;

namespace sones.GraphDB.ErrorHandling
{
    /// <summary>
    /// The interface for all GraphDB exceptions
    /// </summary>
    public abstract class AGraphDBException : Exception
    {
        /// <summary>
        /// The corresponding error code
        /// </summary>
        public UInt16 ErrorCode;
    }
}
