using System;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class InvalidInRangeIntervalException : AGraphQLOperatorException
    {
        public Int32 Expected { get; private set; }
        public Int32 Current { get; private set; }

        /// <summary>
        /// Creates a new InvalidInRangeIntervalException exception
        /// </summary>
        /// <param name="expected">The expected value</param>
        /// <param name="current">The current value</param>
        public InvalidInRangeIntervalException(Int32 expected, Int32 current)
        {
            Expected = expected;
            Current = current;
            _msg = String.Format("Expected: \"{0}\" Current: \"{1}\"", Expected, Current);
        }
    }
}
