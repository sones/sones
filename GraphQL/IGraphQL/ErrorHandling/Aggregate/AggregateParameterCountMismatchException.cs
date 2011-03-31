using System;
using sones.Library.ErrorHandling;

namespace sones.GraphQL.ErrorHandling
{
    /// <summary>
    /// The number of parameters of the function does not match the definition
    /// </summary>
    public sealed class AggregateParameterCountMismatchException : ASonesQLAggregateException
    {

        public Int32 ExpectedParameterCount { get; private set; }
        public Int32 CurrentParameterCount { get; private set; }        
        public String Aggregate { get; private set; }

        /// <summary>
        /// Creates a new AggregateParameterCountMismatchException exception
        /// </summary>
        /// <param name="myAggregate">The used aggregate</param>
        /// <param name="myExpectedParameterCount">The expected count of parameters</param>
        /// <param name="myCurrentParameterCount">The current count of parameters</param>
        public AggregateParameterCountMismatchException(String myAggregate, Int32 myExpectedParameterCount, Int32 myCurrentParameterCount)
        {
            ExpectedParameterCount = myExpectedParameterCount;
            CurrentParameterCount = myCurrentParameterCount;
            Aggregate = myAggregate;
        }

        /// <summary>
        /// Creates a new AggregateParameterCountMismatchException exception
        /// </summary>        
        /// <param name="myExpectedParameterCount">The expected count of parameters</param>
        /// <param name="myCurrentParameterCount">The current count of parameters</param>
        public AggregateParameterCountMismatchException(Int32 myExpectedParameterCount, Int32 myCurrentParameterCount)
        {
            ExpectedParameterCount = myExpectedParameterCount;
            CurrentParameterCount = myCurrentParameterCount;
            Aggregate = null;
        }
        
        public override string ToString()
        {
            if (Aggregate != null)
            {
                return String.Format("The number of parameters [{0}] of the function [{1}]does not match the definition [{2}]", CurrentParameterCount, Aggregate, ExpectedParameterCount);
            }
            else
            {
                return String.Format("The number of parameters [{0}] of the function does not match the definition [{1}]", CurrentParameterCount, ExpectedParameterCount);
            }
        }

        public override ushort ErrorCode
        {
            get { return ErrorCodes.AggregateParameterCountMismatch; }
        }   
    }
}
