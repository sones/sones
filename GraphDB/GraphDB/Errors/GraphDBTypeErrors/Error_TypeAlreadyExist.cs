using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_TypeAlreadyExist : GraphDBTypeError
    {
        public String TypeName { get; private set; }
        public Error_TypeAlreadyExist(String myTypeName)
        {
            TypeName = myTypeName;
        }

        public override string ToString()
        {
            return String.Format("The type {0} already exists", TypeName);
        }
    }
}
