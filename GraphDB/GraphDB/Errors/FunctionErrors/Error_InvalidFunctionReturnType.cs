using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement;
using sones.Lib;

namespace sones.GraphDB.Errors
{
    public class Error_InvalidFunctionReturnType : GraphDBFunctionError
    {

        public String FunctionName { get; private set; }
        public Type TypeOfFunctionReturn { get; private set; }
        public Type[] ValidTypes { get; private set; }

        public Error_InvalidFunctionReturnType(String myFunctionName, Type myTypeOfFunctionReturn, params Type[] myValidTypes)
        {
            FunctionName = myFunctionName;
            TypeOfFunctionReturn = myTypeOfFunctionReturn;
            ValidTypes = myValidTypes;
        }

        public override string ToString()
        {
            if (ValidTypes.IsNullOrEmpty())
            {
                return String.Format("The return type [{0}] of function [{1}] is not valid.", TypeOfFunctionReturn, FunctionName);
            }
            else
            {
                return String.Format("The return type [{0}] of function [{1}] is not valid. Please choose one of: {2}", TypeOfFunctionReturn, FunctionName, ValidTypes.ToAggregatedString(t => t.Name));
            }
        }
    }
}
