using System;

namespace sones.Library.ErrorHandling
{
    /// <summary>
    /// A abstract class for all sones exceptions
    /// </summary>
    public abstract class ASonesException : Exception
    {
        protected ASonesException(Exception innerException = null) : base(String.Empty, innerException) { }

        /// <summary>
        /// The message which is associated to this Exception
        /// </summary>
        protected String _msg;                         

        /// <summary>
        /// The message property
        /// </summary>
        public override String Message
        {
            get { return _msg; }
        }

        /// <summary>
        /// The error message
        /// </summary>
        /// <returns>The error message</returns>
        public override string ToString()
        {
            return _msg;
        }
    }
}