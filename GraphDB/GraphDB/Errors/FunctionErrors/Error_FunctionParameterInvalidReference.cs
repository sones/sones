using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_FunctionParameterInvalidReference : GraphDBFunctionError
    {
        public String Info { get; private set; }

        public Error_FunctionParameterInvalidReference(String myInfo)
        {
            Info = myInfo;
        }

        public override string ToString()
        {
            return String.Format("An invalid reference for a function parameter: {0}! ", Info);
        }
    }
}
