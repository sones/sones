using System;

namespace sones.Plugins.SonesGQL.Function.ErrorHandling
{
    /// <summary>
    /// The number of parameters of the function does not match the definition
    /// </summary>
    public sealed class FunctionParameterCountMismatchException : ASonesQLFunctionException
    {
        #region data        

        public Int32 ExpectedParameterCount { get; private set; }
        public Int32 CurrentParameterCount { get; private set; }
        public String Function { get; private set; }

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new FunctionParameterCountMismatchException exception
        /// </summary>
        /// <param name="myFunction">The current function</param>
        /// <param name="myExpectedParameterCount">The expected count of parameters</param>
        /// <param name="myCurrentParameterCount">The current count of parameters</param>
        public FunctionParameterCountMismatchException(String myFunction, Int32 myExpectedParameterCount, Int32 myCurrentParameterCount)
        {
            ExpectedParameterCount = myExpectedParameterCount;
            CurrentParameterCount = myCurrentParameterCount;
            Function = myFunction;
        }

        /// <summary>
        /// Creates a new FunctionParameterCountMismatchException exception
        /// </summary>
        /// <param name="myExpectedParameterCount">The expected count of parameters</param>
        /// <param name="myCurrentParameterCount">The current count of parameters</param>
        public FunctionParameterCountMismatchException(Int32 myExpectedParameterCount, Int32 myCurrentParameterCount)
        {
            ExpectedParameterCount = myExpectedParameterCount;
            CurrentParameterCount = myCurrentParameterCount;
            Function = null;
        }

        #endregion

        public override string ToString()
        {
            if (Function != null)
            {
                return String.Format("The number of parameters [{0}] of the function [{1}]does not match the definition [{2}]", CurrentParameterCount, Function, ExpectedParameterCount);
            }
            else
            {
                return String.Format("The number of parameters [{0}] of the function does not match the definition [{1}]", CurrentParameterCount, ExpectedParameterCount);
            }
        }
        
    }
}
