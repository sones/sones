using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_FunctionDoesNotExist : GraphDBFunctionError
    {
        public String FunctionName { get; private set; }

        public Error_FunctionDoesNotExist(String myFunctionName)
        {
            FunctionName = myFunctionName;
        }

        public override string ToString()
        {
            return String.Format("The function \"{0}\" does not exist!", FunctionName);
        }
    }
}
