using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Functions;

namespace sones.GraphDB.Errors
{
    public class Error_FunctionParameterCountMismatch : GraphDBFunctionError
    {
        public Int32 ExpectedParameterCount { get; private set; }
        public Int32 CurrentParameterCount { get; private set; }
        public ABaseFunction Function { get; private set; }        

        public Error_FunctionParameterCountMismatch(ABaseFunction myFunction, Int32 myExpectedParameterCount, Int32 myCurrentParameterCount)
        {
            ExpectedParameterCount = myExpectedParameterCount;
            CurrentParameterCount = myCurrentParameterCount;
            Function = myFunction;
        }

        public Error_FunctionParameterCountMismatch(Int32 myExpectedParameterCount, Int32 myCurrentParameterCount)
        {
            ExpectedParameterCount = myExpectedParameterCount;
            CurrentParameterCount = myCurrentParameterCount;
            Function = null;
        }

        public override string ToString()
        {
            if (Function != null)
            {
                return String.Format("The number of parameters [{0}] of the function [{1}]does not match the definition [{2}]", CurrentParameterCount, Function.FunctionName, ExpectedParameterCount);
            }
            else
            {
                return String.Format("The number of parameters [{0}] of the function does not match the definition [{1}]", CurrentParameterCount, ExpectedParameterCount);
            }
        }
    }
}
