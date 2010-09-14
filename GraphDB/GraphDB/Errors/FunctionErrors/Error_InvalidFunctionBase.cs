using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement;

namespace sones.GraphDB.Errors
{
    public class Error_InvalidFunctionBase : GraphDBFunctionError
    {
        public TypeAttribute TypeAttribute { get; private set; }
        public String FunctionName { get; private set; }

        public Error_InvalidFunctionBase(TypeAttribute myTypeAttribute, String myFunctionName)
        {
            TypeAttribute = myTypeAttribute;
            FunctionName = myFunctionName;
        }

        public override string ToString()
        {
            if (TypeAttribute != null)
            {
                return String.Format("The function {0} is invalid on attribute {1}.", FunctionName, TypeAttribute.Name);
            }
            else
            {
                return String.Format("The function {0} has a invalid working base.", FunctionName);
            }
        }
    }
}
