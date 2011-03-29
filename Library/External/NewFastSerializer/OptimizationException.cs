#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace sones.Library.NewFastSerializer
{

    /// <summary>
    /// Exception thrown when a value being optimized does not meet the required criteria for optimization.
    /// </summary>
    public class OptimizationException : Exception
    {
        public OptimizationException(String message) : base(message) { }
    }

}
