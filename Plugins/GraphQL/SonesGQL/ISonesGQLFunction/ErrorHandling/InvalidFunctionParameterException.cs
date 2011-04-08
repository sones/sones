using System;
using sones.Library.ErrorHandling;

namespace sones.Plugins.SonesGQL.Function.ErrorHandling
{
    /// <summary>
    /// The parameter value for this function is invalid
    /// </summary>
    public sealed class InvalidFunctionParameterException : ASonesQLFunctionException
    {
        #region data        

        public String FunctionParameterName { get; private set; }
        public Object FunctionParameterValue { get; private set; }
        public Object Expected { get; private set; }

        #endregion

        #region constructor        

        /// <summary>
        /// Creates a new InvalidFunctionParameterException exception
        /// </summary>
        /// <param name="myFunctionParameterName">The function parameter name</param>
        /// <param name="myExpected">The expected parameter value</param>
        /// <param name="myFunctionParameterValue">The function parameter value</param>
        public InvalidFunctionParameterException(String myFunctionParameterName, Object myExpected, Object myFunctionParameterValue)
        {
            FunctionParameterName = myFunctionParameterName;
            FunctionParameterValue = myFunctionParameterValue;
            Expected = myExpected;
        }

        #endregion

        public override string ToString()
        {
            return String.Format("Invalid parameter value for \"{0}\"! Expected [{1}] \nCurrent [{2}]", FunctionParameterName, Expected, FunctionParameterValue);
        }
        
    }
}

