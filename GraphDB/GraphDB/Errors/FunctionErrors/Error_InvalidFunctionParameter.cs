using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_InvalidFunctionParameter : GraphDBFunctionError
    {
        public String FunctionParameterName { get; private set; }
        public Object FunctionParameterValue { get; private set; }
        public Object Expected { get; private set; }

        public Error_InvalidFunctionParameter(String myFunctionParameterName, Object myExpected, Object myFunctionParameterValue)
        {
            FunctionParameterName = myFunctionParameterName;
            FunctionParameterValue = myFunctionParameterValue;
            Expected = myExpected;
        }

        public override string ToString()
        {
            return String.Format("Invalid parameter value for \"{0}\"! Expected [{1}] \nCurrent [{2}]", FunctionParameterName, Expected, FunctionParameterValue);
        }

    }
}
