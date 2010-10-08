using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sones.GraphDB.Errors
{
    public class Error_TypeDoesNotExist : GraphDBTypeError
    {
        public String TypeName { get; private set; }
        public Error_TypeDoesNotExist(String myTypeName)
        {
            TypeName = myTypeName;
        }

        public override string ToString()
        {
            return String.Format("The type {0} does not exist.", TypeName);
        }
    }
}
