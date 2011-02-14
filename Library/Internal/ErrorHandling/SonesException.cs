using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.ErrorHandling
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
