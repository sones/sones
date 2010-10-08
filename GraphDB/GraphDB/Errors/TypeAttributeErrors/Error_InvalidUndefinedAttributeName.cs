using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.Errors;

namespace sones.GraphDB.Errors
{
    public class Error_InvalidUndefinedAttributeName : GraphDBAttributeError
    {
        public Error_InvalidUndefinedAttributeName()
        {
        }

        public override string ToString()
        {
            return "An undefined attribute with an \".\" is not allowed.";
        }
    }
}
