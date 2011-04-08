using System;

namespace sones.Library.ErrorHandling
{
    /// <summary>
    /// A abstract class for all sones exceptions
    /// </summary>
    public abstract class ASonesException : Exception
    {
        /// <summary>
        /// The message which is associated to this Exception
        /// </summary>
        protected String _msg;

        /// <summary>
        /// The error code which is associated to this Exception
        /// </summary>
        protected UInt16 _errorCode;

        /// <summary>
        /// The corresponding error code
        /// </summary>
        public abstract UInt16 ErrorCode { get; }              

        /// <summary>
        /// The message property
        /// </summary>
        public override String Message
        {
            get { return _msg; }
        }

        /// <summary>
        /// The String of the error code
        /// </summary>
        /// <returns>The error message</returns>
        public override string ToString()
        {
            return _msg;
        }
    }
}