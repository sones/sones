using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_FunctionParameterTypeMismatch : GraphDBFunctionError
    {
        public Type ExpectedType { get; private set; }
        public Type CurrentType { get; private set; }

        public Error_FunctionParameterTypeMismatch(Type myExpectedType, Type myCurrentType)
        {
            ExpectedType = myExpectedType;
            CurrentType = myCurrentType;
        }

        public override string ToString()
        {
            return String.Format("Function parameter type mismatch! Expected type \"{0}\" dos not match \"{1}\"!", ExpectedType, CurrentType);
        }
    }
}
