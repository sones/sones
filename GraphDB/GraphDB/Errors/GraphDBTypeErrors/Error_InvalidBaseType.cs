using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphDB.TypeManagement.BasicTypes;

namespace sones.GraphDB.Errors
{
    public class Error_InvalidBaseType : GraphDBTypeError
    {
        public String BaseTypeName { get; private set; }
        public Error_InvalidBaseType(String myBaseTypeName)
        {
            BaseTypeName = myBaseTypeName;
        }

        public override string ToString()
        {
            return String.Format("The base type [{0}] must be a user defined type or [{1}].", BaseTypeName, DBReference.Name);
        }    
    }
}
