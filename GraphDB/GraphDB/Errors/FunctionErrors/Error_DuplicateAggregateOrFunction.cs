using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_DuplicateAggregateOrFunction : GraphDBFunctionError
    {
        public String FunctionName { get; private set; }
        public Boolean IsFunction { get; private set; }

        public Error_DuplicateAggregateOrFunction(String myFunctionName, Boolean myIsFunction = true)
        {
            FunctionName = myFunctionName;
            IsFunction   = myIsFunction;
        }

        public override string ToString()
        {
            if (IsFunction)
            {
                return String.Format("The function name \"{0}\" is duplicate! The name has to be unique!", FunctionName);
            }
            else
            {
                return String.Format("The aggregate name \"{0}\" is duplicate! The name has to be unique!", FunctionName);
            }
        }
    }
}
